using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeMetricsRoslyn.SyntaxWalkers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeMetricsRoslyn.LocMetrics
{
    public static class LocMetricsCalculator
    {
        private static SyntaxNode _root;
        private static List<string> _lines;

        public static FolderLocMetricsResult Calculate(string directoryPath)
        {
            var csFiles = Directory
                .EnumerateFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            var fileMetrics = csFiles.Select(GetFileMetricsResult).ToList();

            return new FolderLocMetricsResult
            {
                BlankCount = fileMetrics.Select(metric => metric.BlankCount).Sum(),
                PhysicalCount = fileMetrics.Select(metric => metric.PhysicalCount).Sum(),
                LogicalCount = fileMetrics.Select(metric => metric.LogicalCount).Sum(),
                CommentedCount = fileMetrics.Select(metric => metric.CommentedCount).Sum(),
                CyclomaticComplexity = fileMetrics.Select(metric => metric.CyclomaticComplexity).Sum()
            };
        }

        private static FileLocMetricsResult GetFileMetricsResult(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);

            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            _lines = File.ReadAllLines(filePath).ToList();
            _root = syntaxTree.GetRoot();

            var blankLinesNumbers = GetBlankLines();
            var (commentedLines, commentedRanges) = GetCommentedResult();

            blankLinesNumbers.RemoveAll(
                blankLineNumber => commentedRanges.Any(range => range.startLine <= blankLineNumber
                                                                && range.endLine >= blankLineNumber));

            return new FileLocMetricsResult
            {
                PhysicalCount = _lines.Count,
                CommentedCount = commentedLines,
                BlankCount = blankLinesNumbers.Count,
                LogicalCount = GetLogicalLinesCount(),
                CyclomaticComplexity = GetCyclomaticComplexity()
            };
        }

        private static (int commentedLinesCount, List<(int startLine, int endLine)>) GetCommentedResult()
        {
            var commentWalker = new CommentsWalker();

            commentWalker.Visit(_root);

            return (commentWalker.CommentedLinesCount, commentWalker.CommentedRanges);
        }

        private static List<int> GetBlankLines()
            => _lines
            .Where(string.IsNullOrWhiteSpace)
            .Select((line, index) => index)
            .ToList();

        private static int GetCyclomaticComplexity()
        {
            var cyclomaticComplexityWalker = new CyclomaticComplexityWalker();

            cyclomaticComplexityWalker.Visit(_root);

            return cyclomaticComplexityWalker.CyclomaticComplexity;
        }

        private static int GetLogicalLinesCount()
        {
            var statements = new[] {
                SyntaxKind.IfKeyword, SyntaxKind.ElseKeyword, SyntaxKind.ElifKeyword, SyntaxKind.ConditionalExpression,
                SyntaxKind.NullableKeyword,
                SyntaxKind.ForEachKeyword, SyntaxKind.WhileKeyword, SyntaxKind.DoKeyword, SyntaxKind.ForKeyword,
                SyntaxKind.ReturnKeyword, SyntaxKind.BreakKeyword, SyntaxKind.ContinueKeyword, SyntaxKind.ThrowKeyword,
                SyntaxKind.SwitchKeyword, SyntaxKind.CaseKeyword,
                SyntaxKind.UsingKeyword,
                SyntaxKind.ExpressionStatement, SyntaxKind.LocalDeclarationStatement, SyntaxKind.EmptyStatement,
                SyntaxKind.TryKeyword, SyntaxKind.CatchKeyword, SyntaxKind.FinallyKeyword};

            return _root.DescendantNodesAndTokens().Count(n => statements.Contains(n.Kind()));
        }
    }
}
