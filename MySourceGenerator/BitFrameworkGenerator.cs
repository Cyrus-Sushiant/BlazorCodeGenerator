using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
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

            foreach (IGrouping<ISymbol, IPropertySymbol> group in receiver.Properties.GroupBy(f => f.ContainingType, SymbolEqualityComparer.Default))
            {
                var properties = group.Select(p => new BitProperty(p, false)).ToList();
                string classSource = GeneratePartialClassToOverrideSetParametersAsync(group.Key as INamedTypeSymbol, properties, context);
                context.AddSource($"{group.Key.Name}_SetParametersAsync.AutoGenerated.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BitSyntaxReceiver());

            //Debugger.Launch();
        }

        string GeneratePartialClassToOverrideSetParametersAsync(INamedTypeSymbol classSymbol, List<BitProperty> properties, GeneratorExecutionContext context)
        {
            SetTwoWayBoundProperties(properties);
            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            StringBuilder source = new StringBuilder($@"
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System;

namespace {namespaceName}
{{
    public partial class {GetName(classSymbol)}  
    {{
        public override Task SetParametersAsync(ParameterView parameters) 
        {{

");

            foreach (var property in properties.Where(p => p.IsTwoWayBoundProperty))
            {
                source.AppendLine($"            {property.PropertySymbol.Name}HasBeenSet = false;");
            }

            source.AppendLine(@"            foreach (ParameterValue parameter in parameters)
            {
                switch (parameter.Name)
                {");

            // create cases for each property 
            foreach (var property in properties)
            {
                GenerateParameterReaderCode(source, property);
            }

            source.AppendLine("                }");
            source.AppendLine("            }");

            if (classSymbol.BaseType.ToDisplayString() == "Microsoft.AspNetCore.Components.ComponentBase")
            {
                source.AppendLine("            return base.SetParametersAsync(ParameterView.Empty);");
            }
            else
            {
                source.AppendLine("            return base.SetParametersAsync(parameters);");
            }
            source.AppendLine("        }");
            source.AppendLine("    }");
            source.AppendLine("}");

            return source.ToString();
        }

        private void SetTwoWayBoundProperties(List<BitProperty> properties)
        {
            foreach (var item in properties)
            {
                var propName = $"{item.PropertySymbol.Name}Changed";
                var propType = $"Microsoft.AspNetCore.Components.EventCallback<{item.PropertySymbol.Type.ToDisplayString()}>";
                item.IsTwoWayBoundProperty = properties.Any(p => p.PropertySymbol.Name == propName && p.PropertySymbol.Type.ToDisplayString() == propType);
            }
        }

        private void GenerateParameterReaderCode(StringBuilder source, BitProperty bitProperty)
        {
            source.AppendLine($"                    case nameof({bitProperty.PropertySymbol.Name}):");

            if (bitProperty.IsTwoWayBoundProperty)
                source.AppendLine($"                       {bitProperty.PropertySymbol.Name}HasBeenSet = true;");

            source.AppendLine($"                       {bitProperty.PropertySymbol.Name} = ({bitProperty.PropertySymbol.Type.ToDisplayString()})parameter.Value;");
            source.AppendLine("                       break;");
        }

        public string GetName(INamedTypeSymbol type)
        {
            StringBuilder stringBuilder = new StringBuilder(type.Name);

            if (type.IsGenericType)
            {
                stringBuilder.Append("<");
                stringBuilder.Append(string.Join(", ", type.TypeArguments.Select(s => s.Name)));
                stringBuilder.Append(">");
            }

            return stringBuilder.ToString();
        }
    }
}
