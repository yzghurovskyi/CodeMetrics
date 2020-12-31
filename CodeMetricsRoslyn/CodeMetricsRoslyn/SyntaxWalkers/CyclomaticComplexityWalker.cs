using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeMetricsRoslyn.SyntaxWalkers
{
    public class CyclomaticComplexityWalker : CSharpSyntaxWalker
    {
        private readonly Dictionary<string, int> _cyclomaticCounter = new Dictionary<string, int>();
        public int CyclomaticComplexity => _cyclomaticCounter.Select(pair => pair.Value + 1).Sum();
        public CyclomaticComplexityWalker() : base(SyntaxWalkerDepth.Token) { }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            AddConditionNode(node);

            base.VisitIfStatement(node);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            AddConditionNode(node);

            base.VisitDoStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            AddConditionNode(node);

            base.VisitForStatement(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            AddConditionNode(node);

            base.VisitWhileStatement(node);
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            AddConditionNode(node);

            base.VisitCaseSwitchLabel(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            AddDeclarationNode(node);

            base.VisitMethodDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            AddDeclarationNode(node);

            base.VisitPropertyDeclaration(node);
        }

        private void AddConditionNode(SyntaxNode node)
        {
            var typeDeclaration = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();

            var firstToken = typeDeclaration?.Identifier.ValueText;
            
            var methodDeclaration = node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            var propertyDeclaration = node.Ancestors().OfType<PropertyDeclarationSyntax>().FirstOrDefault();

            var secondToken = string.Empty;

            if (methodDeclaration != null)
                secondToken = methodDeclaration.Identifier.ValueText;
            else if (propertyDeclaration != null)
                secondToken = propertyDeclaration.Identifier.ValueText;

            var token = $"{firstToken}.{secondToken}";

            if (_cyclomaticCounter.ContainsKey(token))
                _cyclomaticCounter[token] += 1;
            else
                _cyclomaticCounter[token] = 0;
        }

        private void AddDeclarationNode(SyntaxNode node)
        {
            var typeDeclaration = node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();

            string name;
            switch (node)
            {
                case MethodDeclarationSyntax method:
                    name = method.Identifier.ValueText;
                    break;
                case PropertyDeclarationSyntax property:
                    name = property.Identifier.ValueText;
                    break;
                default:
                    throw new ArgumentException(
                        "node is not a recognized DeclarationType",
                        nameof(node));
            }

            var token = $"{typeDeclaration?.Identifier.ValueText}.{name}";

            if (!_cyclomaticCounter.ContainsKey(token))
                _cyclomaticCounter[token] = 0;
        }
    }
}
