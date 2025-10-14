using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LoggingSourceGenerator
{
    internal class AttributesSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateClasses { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclaration &&
                (typeDeclaration is ClassDeclarationSyntax || typeDeclaration is StructDeclarationSyntax))
            {
                if (typeDeclaration.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(typeDeclaration);
                }
            }
        }
    }
}
