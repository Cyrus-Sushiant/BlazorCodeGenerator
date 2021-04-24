//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Text;
//using System;
//using System.Diagnostics;
//using System.Text;
//using System.IO;

//namespace MySourceGenerator
//{
//    [Generator]
//    public class TestGenerator2 : ISourceGenerator
//    {
//        public void Execute(GeneratorExecutionContext context)
//        {
//            const string source = @"
//namespace EmployeeGenerator
//{
//    public class Employee2
//    {
//        public string FirstName => ""Mandana"";
//        public string LastName => ""Cyrus"";
//    }
//}";
//            const string desiredFileName = "Employee2.cs";

//            SourceText sourceText = SourceText.From(source, Encoding.UTF8);

//            // Add the "generated" source to the compilation
//            context.AddSource(desiredFileName, sourceText);
//        }

//        public void Initialize(GeneratorInitializationContext context)
//        {
////#if DEBUG
////            if (!Debugger.IsAttached)
////            {
////                Debugger.Launch();
////            }
////#endif 
//        }
//    }
//}
