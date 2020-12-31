using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace CodeMetricsRoslyn.SyntaxWalkers
{
    public class CommentsWalker : CSharpSyntaxWalker
    {
        public List<(int startLine, int endLine)> CommentedRanges { get; } 
            = new List<(int startLine, int stopLine)>();

        public int CommentedLinesCount => CommentedRanges
            .Select(range => range.endLine - range.startLine + 1)
            .Sum();

        public CommentsWalker() : base(SyntaxWalkerDepth.StructuredTrivia){}

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.SingleLineCommentTrivia 
                || trivia.Kind() == SyntaxKind.MultiLineCommentTrivia)
            {
                var lineSpan = trivia.GetLocation().GetLineSpan();

                var startLine = lineSpan.StartLinePosition.Line;
                var endLine = lineSpan.EndLinePosition.Line;

                CommentedRanges.Add((startLine, endLine));
            }

            base.VisitTrivia(trivia);
        }
    }
}
