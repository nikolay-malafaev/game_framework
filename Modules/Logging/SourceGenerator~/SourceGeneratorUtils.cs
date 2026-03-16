using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
internal static class SourceGeneratorUtils
{
    public static string[] LoadTemplateLines(string resourceName, Assembly assembly)
    {
        var stream = assembly.GetManifestResourceStream(resourceName);
        if(stream == null)
            throw new InvalidOperationException($"Resource '{resourceName}' not found. Have: {string.Join(", ", assembly.GetManifestResourceNames())}");

        using (var reader = new StreamReader(stream))
        {
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines.ToArray();
        }
    }
    public static void ReportWarning(GeneratorExecutionContext context, Location location, string id, string message)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                id,
                "Loggable Generator",
                message,
                "LoggableGenerator",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location);

        context.ReportDiagnostic(diagnostic);
    }

    public static void ReportError(GeneratorExecutionContext context, string id, string title, string message)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                id,
                title,
                message,
                "LoggableGenerator",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            Location.None);

        context.ReportDiagnostic(diagnostic);
    }

    public static string GetAccessibilityString(INamedTypeSymbol classSymbol)
    {
        switch (classSymbol.DeclaredAccessibility)
        {
            case Accessibility.Public:
                return "public";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.Internal:
                return "internal";
            case Accessibility.ProtectedOrInternal:
                return "protected internal";
            case Accessibility.ProtectedAndInternal:
                return "private protected";
            default:
                return "private";
        }
    }

    public static string CSharpStringLiteral(string s)
        => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

    public static void EmitTypeHeader(StringBuilder stringBuilder, INamedTypeSymbol classSimbol, int tabs, bool addGeneratedAttributes)
    {
        var accessibility = GetAccessibilityString(classSimbol);
        var modifiers = GetModifiers(classSimbol);
        var typeKeyword = GetTypeKeyword(classSimbol);
        var typeParams = GetTypeParameterList(classSimbol);
        var whereClauses = BuildWhereClauses(classSimbol);

        if (addGeneratedAttributes)
        {
            stringBuilder.AppendLineWithTab("[global::System.CodeDom.Compiler.GeneratedCode(\"LoggingSourceGenerator\", \"1.0.0\")]", tabs);
            stringBuilder.AppendLineWithTab("[global::System.Diagnostics.DebuggerNonUserCode]", tabs);
            stringBuilder.AppendLineWithTab("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]", tabs);
        }

        stringBuilder.AppendLineWithTab($"{accessibility}{modifiers} partial {typeKeyword} {classSimbol.Name}{typeParams}", tabs);
        foreach (var clause in whereClauses)
            stringBuilder.AppendLineWithTab(clause, tabs);
    }

    private static string GetModifiers(INamedTypeSymbol s)
    {
        if (s.IsStatic) return " static";
        var m = "";
        if (s.TypeKind == TypeKind.Struct && s.IsReadOnly) m += " readonly";
        if (s.IsAbstract && !s.IsSealed && s.TypeKind != TypeKind.Struct) m += " abstract";
        if (s.IsSealed && !s.IsAbstract && s.TypeKind != TypeKind.Struct) m += " sealed";
        return m;
    }

    private static string GetTypeKeyword(INamedTypeSymbol symbol)
    {
        switch(symbol.TypeKind)
        {
            case TypeKind.Struct:
                return "struct";
            case TypeKind.Interface:
                return "interface";
            default:
                return "class";
        }
    }

    private static string GetTypeParameterList(INamedTypeSymbol s)
    {
        if (s.TypeParameters.Length == 0) return "";
        return "<" + string.Join(", ", s.TypeParameters.Select(tp => tp.Name)) + ">";
    }

    public static string GetFriendlyTypeName(INamedTypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsGenericType)
            return typeSymbol.Name;

        var baseName = typeSymbol.Name;
        var genericArgs = string.Join(", ", typeSymbol.TypeArguments.Select(t => t.Name));
        return $"{baseName}<{genericArgs}>";
    }

    public static string GetFileNameFromType(INamedTypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsGenericType)
            return typeSymbol.Name;

        var baseName = typeSymbol.ToDisplayString()
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace(",", "_")
            .Replace(" ", "");
            
        var genericArgs = string.Join(".", typeSymbol.TypeArguments.Select(t => t.Name));
        return $"{baseName}.Generic.{genericArgs}";
    }

    private static IEnumerable<string> BuildWhereClauses(INamedTypeSymbol s)
    {
        foreach (var tp in s.TypeParameters)
        {
            var parts = new List<string>();

            if (tp.HasNotNullConstraint) parts.Add("notnull");
            if (tp.HasReferenceTypeConstraint) parts.Add("class");
            if (tp.HasUnmanagedTypeConstraint) parts.Add("unmanaged");
            if (tp.HasValueTypeConstraint) parts.Add("struct");

            foreach (var ct in tp.ConstraintTypes)
                parts.Add(ct.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            if (tp.HasConstructorConstraint) parts.Add("new()");

            if (parts.Count > 0)
                yield return $"where {tp.Name} : {string.Join(", ", parts)}";
        }
    }

    public static AttributeData GetAttributeData(INamedTypeSymbol classSymbol, string AttributeFullName)
    {
        return classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AttributeFullName);
    }
}
