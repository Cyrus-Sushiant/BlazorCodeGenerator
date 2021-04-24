using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySourceGenerator
{
    [Generator]
    public class BitFrameworkGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (context.SyntaxContextReceiver is not BitSyntaxReceiver receiver)
                return;

            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> group in receiver.Properties.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), context);
                context.AddSource($"{group.Key.Name}_BitFramework.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BitSyntaxReceiver());
            //Debugger.Launch();
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<IPropertySymbol> properties, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System;

namespace {namespaceName}
{{
    public partial class {classSymbol.Name}  
    {{
        public override Task SetParametersAsync(ParameterView parameters) 
        {{
            foreach (ParameterValue parameter in parameters)
            {{
                switch (parameter.Name)
                {{
");

            // create cases for each property 
            foreach (IPropertySymbol propertySymbol in properties)
            {
                ProcessField(source, propertySymbol);
            }
            source.AppendLine("                }");
            source.AppendLine("            }");

            source.AppendLine("            return base.SetParametersAsync(parameters);");
            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            return source.ToString();
        }

        private void ProcessField(StringBuilder source, IPropertySymbol propertySymbol)
        {
            source.AppendLine($"                    case nameof({propertySymbol.Name}):");
            source.AppendLine($"                       {propertySymbol.Name} = ({propertySymbol.Type.ToDisplayString()})parameter.Value;");
            source.AppendLine("                       break;");
        }
    }



    public class BitSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IPropertySymbol> Properties { get; } = new List<IPropertySymbol>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
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
