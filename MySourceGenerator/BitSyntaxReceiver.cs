using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MySourceGenerator
{
    public class BitSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IPropertySymbol> Properties { get; } = new List<IPropertySymbol>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax
                    && propertyDeclarationSyntax.AttributeLists.Count > 0)
            {

                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)propertyDeclarationSyntax.Parent;

                if (!classDeclarationSyntax.Modifiers.Any(k => k.IsKind(SyntaxKind.PartialKeyword)))
                    return;

                IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax);

                if (propertySymbol.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "Microsoft.AspNetCore.Components.ParameterAttribute"))
                {
                    Properties.Add(propertySymbol);
                }
            }
        }
    }
}
