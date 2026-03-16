using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoggingSourceGenerator
{
    [Generator]
    public class LoggingSourceGenerator : ISourceGenerator
    {
        private const string ATTRIBUTE_FULL_NAME = "GameFramework.Logging.LoggableAttribute";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AttributesSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                if (!(context.SyntaxReceiver is AttributesSyntaxReceiver receiver))
                    return;

                var compilation = context.Compilation;

                foreach (var candidateClass in receiver.CandidateClasses)
                {
                    ProcessClass(context, compilation, candidateClass);
                }
            }
            catch (Exception ex)
            {
                SourceGeneratorUtils.ReportError(context, "GEN001", "Unexpected error in source generator", ex.ToString());
            }
        }

        private void ProcessClass(GeneratorExecutionContext context, Compilation compilation, TypeDeclarationSyntax classDeclaration)
        {
            try
            {
                var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(classDeclaration);
                var location = classDeclaration.GetLocation();

                if (classSymbol == null)
                    return;

                var loggableAttribute = SourceGeneratorUtils.GetAttributeData(classSymbol, ATTRIBUTE_FULL_NAME);
                if (loggableAttribute == null)
                    return;

                if(!SourceGeneratorValidator.ClassIsPartial(classDeclaration))
                {
                    SourceGeneratorUtils.ReportWarning(context, location, "LOG001",
                        $"Class '{classSymbol.Name}' with [Loggable] attribute must be declared as 'partial'. Add 'partial' modifier to enable logging generation.");
                    return;
                }

                if (classSymbol.IsStatic)
                {
                    SourceGeneratorUtils.ReportWarning(context, location, "LOG002",
                        $"Static class '{classSymbol.Name}' cannot use [Loggable] attribute. Remove attribute or make class non-static.");
                    return;
                }

                string tag = GetTagFromAttribute(loggableAttribute, classSymbol);

                string source = GenerateSource(classSymbol, tag);

                if (!string.IsNullOrWhiteSpace(source))
                {
                    var fileName = $"{SourceGeneratorUtils.GetFileNameFromType(classSymbol)}.Logging.g.cs";
                    context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
                }
            }
            catch (Exception ex)
            {
                var location = classDeclaration.GetLocation();
                SourceGeneratorUtils.ReportWarning(context, location, "GEN002",
                    $"Failed to generate logging methods for class '{classDeclaration.Identifier.Text}': {ex.Message}");
            }
        }

        private string GetTagFromAttribute(AttributeData attribute, INamedTypeSymbol classSymbol)
        {
            string tag = null;

            // try get tag from constructor
            if (attribute.ConstructorArguments.Length > 0)
            {
                var arg = attribute.ConstructorArguments[0];
                if (arg.Value is string stringValue)
                {
                    tag = stringValue;
                }
            }

            return tag;
        }

        private string GenerateSource(INamedTypeSymbol classSymbol, string tag)
        {
            bool hasNamespace = !classSymbol.ContainingNamespace.IsGlobalNamespace;
            int countTabs = 0;
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendDefaultGeneratedLines();

            if (hasNamespace)
            {
                stringBuilder.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                stringBuilder.AppendLine("{");
                ++countTabs;
            }

            {
                SourceGeneratorUtils.EmitTypeHeader(stringBuilder, classSymbol, countTabs, true);
                stringBuilder.AppendLineWithTab("{", countTabs);
                ++countTabs;
            }

            bool hasExplicitTag = !string.IsNullOrWhiteSpace(tag);

            if (hasExplicitTag)
            {
                var safeTag = SourceGeneratorUtils.CSharpStringLiteral(tag);
                stringBuilder.AppendLineWithTab($"private string LoggingTag => {safeTag};", countTabs);
                stringBuilder.AppendTabs(countTabs);
            }
            else
            {
                stringBuilder.AppendLineWithTab("private string LoggingTag", countTabs);
                stringBuilder.AppendLineWithTab("{", countTabs);
                stringBuilder.AppendLineWithTab("    get", countTabs);
                stringBuilder.AppendLineWithTab("    {", countTabs);
                stringBuilder.AppendLineWithTab("        var type = this.GetType();", countTabs);
                stringBuilder.AppendLineWithTab("        if (type.IsGenericType)", countTabs);
                stringBuilder.AppendLineWithTab("        {", countTabs);
                stringBuilder.AppendLineWithTab("            return type.Name.Split('`')[0];", countTabs);
                stringBuilder.AppendLineWithTab("        }", countTabs);
                stringBuilder.AppendLineWithTab("        return type.Name;", countTabs);
                stringBuilder.AppendLineWithTab("    }", countTabs);
                stringBuilder.AppendLineWithTab("}", countTabs);
                stringBuilder.AppendTabs(countTabs);
            }

            {
                stringBuilder.AppendResources("LoggingMethods", typeof(LoggingSourceGenerator).Assembly, countTabs);
            }

            // end bracket for class
            {
                --countTabs;
                stringBuilder.AppendLineWithTab("}", countTabs);
            }

            // end bracket for namespace
            if (hasNamespace)
            {
                stringBuilder.AppendLine("}");
            }

            return stringBuilder.ToString();
        }
    }
}
