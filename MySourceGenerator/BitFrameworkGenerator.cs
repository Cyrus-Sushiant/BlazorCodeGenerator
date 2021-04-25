﻿using Microsoft.CodeAnalysis;
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
            if (context.SyntaxContextReceiver is not BitSyntaxReceiver receiver)
                return;

            foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> group in receiver.Properties.GroupBy(f => f.ContainingType))
            {
                string classSource = GeneratePartialClassToOverrideSetParametersAsync(group.Key, group.ToList(), context);
                context.AddSource($"{group.Key.Name}_SetParametersAsync.AutoGenerated.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BitSyntaxReceiver());
        }

        string GeneratePartialClassToOverrideSetParametersAsync(INamedTypeSymbol classSymbol, List<IPropertySymbol> properties, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            StringBuilder source = new StringBuilder($@"
using Microsoft.AspNetCore.Components;
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
                GenerateParameterReaderCode(source, propertySymbol);
            }

            source.AppendLine("                }");
            source.AppendLine("            }");

            source.AppendLine("            return base.SetParametersAsync(parameters);");
            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            return source.ToString();
        }

        private void GenerateParameterReaderCode(StringBuilder source, IPropertySymbol propertySymbol)
        {
            source.AppendLine($"                    case nameof({propertySymbol.Name}):");
            source.AppendLine($"                       {propertySymbol.Name} = ({propertySymbol.Type.ToDisplayString()})parameter.Value;");
            source.AppendLine("                       break;");
        }
    }
}
