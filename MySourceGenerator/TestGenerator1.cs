//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Text;
//using System;
//using System.Text;

//namespace MySourceGenerator
//{
//    [Generator]
//    public class TestGenerator1 : ISourceGenerator
//    {
//        public void Execute(GeneratorExecutionContext context)
//        {
//            const string source = @"
//namespace EmployeeGenerator
//{
//    public class Employee
//    {
//        public string FirstName => ""Mandana"";
//        public string LastName => ""Tahmuresi"";
//    }
//}";
//            const string desiredFileName = "Employee.cs";

//            SourceText sourceText = SourceText.From(source, Encoding.UTF8);

//            // Add the "generated" source to the compilation
//            context.AddSource(desiredFileName, sourceText);
//        }

//        public void Initialize(GeneratorInitializationContext context)
//        {
//        }
//    }
//}
