using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace LoggingSourceGenerator
{
    internal static class SourceGeneratorValidator
    {
        public static bool ClassIsPartial(TypeDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        }
    }
}
