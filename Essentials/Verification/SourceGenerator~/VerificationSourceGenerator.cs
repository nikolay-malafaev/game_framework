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
    public class VerificationSourceGenerator : ISourceGenerator
    {
        private const string ATTRIBUTE_FULL_NAME = "GameFramework.Verification.VerifiableAttribute";

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
                    SourceGeneratorUtils.ReportWarning(context, location, "VER001",
                        $"Class '{classSymbol.Name}' with [Verifiable] attribute must be declared as 'partial'. Add 'partial' modifier to enable verifiable generation.");
                    return;
                }

                if (classSymbol.IsStatic)
                {
                    SourceGeneratorUtils.ReportWarning(context, location, "VER002",
                        $"Static class '{classSymbol.Name}' cannot use [Verifiable] attribute. Remove attribute or make class non-static.");
                    return;
                }

                string source = GenerateSource(classSymbol);

                if (!string.IsNullOrWhiteSpace(source))
                {
                    var fileName = $"{SourceGeneratorUtils.GetFileNameFromType(classSymbol)}.Verification.g.cs";
                    context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
                }
            }
            catch (Exception ex)
            {
                var location = classDeclaration.GetLocation();
                SourceGeneratorUtils.ReportWarning(context, location, "GEN002",
                    $"Failed to generate verification methods for class '{classDeclaration.Identifier.Text}': {ex.Message}");
            }
        }

        private string GenerateSource(INamedTypeSymbol classSymbol)
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
                SourceGeneratorUtils.EmitTypeHeader(stringBuilder, classSymbol, countTabs, false);
                stringBuilder.AppendLineWithTab("{", countTabs);
                ++countTabs;
            }

            {
                stringBuilder.AppendResources("VerificationMethods", typeof(VerificationSourceGenerator).Assembly, countTabs);
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
