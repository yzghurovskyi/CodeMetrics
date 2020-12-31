using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMetricsRoslyn.OOPMetrics
{
    public class OOPMetricsCalculator
    {
        private static SemanticModel _model;
        private static List<TypeDeclarationSyntax> _typeDeclarations;

        public static void Calculate(string directoryPath)
        {
            InitSemanticModel(directoryPath);

            var types = new Dictionary<string, FileOOPMetricResult>();

            _typeDeclarations.ForEach(type => types.Add(type.Identifier.ValueText, new FileOOPMetricResult()));

            foreach (var typeDeclaration in _typeDeclarations)
            {
                var typeName = typeDeclaration.Identifier.ValueText;
                var typeInfo = _model.GetDeclaredSymbol(typeDeclaration);
                var members = typeInfo.GetMembers().ToList();

                var methods = members.Where(member => member.Kind == SymbolKind.Method || member.Kind == SymbolKind.Property).ToList();

                var attributes = members.Where(member => member.Kind == SymbolKind.Field).ToList();

                types[typeName].VisibleMethods = methods.Count(IsPublic);
                types[typeName].HiddenMethods = methods.Count(IsHidden);

                types[typeName].VisibleAttributes = attributes.Count(IsPublic);
                types[typeName].HiddenAttributes = attributes.Count(IsHidden);

                var baseType = typeInfo.BaseType;

                if (baseType is null)
                    continue;

                var baseTypeName = baseType.Name;

                if (types.ContainsKey(baseTypeName))
                    types[baseTypeName].NumberOfChildren++;

                var dip = 0;

                while (baseType != null && baseType.Name != "Object")
                {
                    dip++;
                    baseType = baseType.BaseType;
                }

                types[typeName].InheritanceDepth = dip;
            }


            foreach (var (typeName, oopMetric) in types)
            {
                Console.WriteLine($"DIP for {typeName}: {oopMetric.InheritanceDepth}");
                Console.WriteLine($"NOC for {typeName}: {oopMetric.NumberOfChildren}");
            }
                

            var allHiddenMethodsCount = types.Sum(type => type.Value.HiddenMethods);
            var allVisibleMethodsCount = types.Sum(type => type.Value.VisibleMethods);
            var allHiddenAttributesCount = types.Sum(type => type.Value.HiddenAttributes);
            var allVisibleAttributesCount = types.Sum(type => type.Value.VisibleAttributes);

            Console.WriteLine("-----------------------MOOD--------------------");
            Console.WriteLine($"MHF: {Math.Round((double) allHiddenMethodsCount / (allVisibleMethodsCount + allHiddenMethodsCount), 5)}");
            Console.WriteLine($"AHF: {Math.Round((double) allHiddenAttributesCount / (allVisibleAttributesCount + allHiddenAttributesCount), 5)}");
        }

        private static void InitSemanticModel(string directoryPath)
        {
            var csFiles = Directory
                .EnumerateFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            var assemblyContent = string.Join(Environment.NewLine, csFiles.Select(File.ReadAllText));

            var tree = CSharpSyntaxTree.ParseText(assemblyContent);
            var root = tree.GetRoot();

            var compilation = CSharpCompilation.Create("AssemblyName")
                .AddReferences(
                    MetadataReference.CreateFromFile(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);
            _model = compilation.GetSemanticModel(tree);

            _typeDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();
        }

        private static bool IsPublic(ISymbol member) => member.DeclaredAccessibility == Accessibility.Public;

        private static bool IsHidden(ISymbol member) => !IsPublic(member);
    }
}
