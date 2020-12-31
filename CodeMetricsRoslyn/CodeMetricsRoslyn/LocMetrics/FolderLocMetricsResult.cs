using System;

namespace CodeMetricsRoslyn.LocMetrics
{
    public class FolderLocMetricsResult : FileLocMetricsResult
    {
        public double CommentingLevel => Math.Round(CommentedCount / (double) PhysicalCount, 5);

        public override string ToString()
        {
            return $"Physical LOC: {PhysicalCount}{Environment.NewLine}" +
                   $"Source LOC: {SourceCount}{Environment.NewLine}" +
                   $"Blank LOC: {BlankCount}{Environment.NewLine}" +
                   $"Logic LOC: {LogicalCount}{Environment.NewLine}" +
                   $"Commented LOC: {CommentedCount}{Environment.NewLine}" +
                   $"Commenting level: {CommentingLevel} %{Environment.NewLine}" +
                   $"Cyclomatic complexity: {CyclomaticComplexity}{Environment.NewLine}";
        }
    }
}
