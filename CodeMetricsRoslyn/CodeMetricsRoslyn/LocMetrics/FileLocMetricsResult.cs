namespace CodeMetricsRoslyn.LocMetrics
{
    public class FileLocMetricsResult
    {
        public int BlankCount { get; set; }
        public int PhysicalCount { get; set; }
        public int SourceCount => PhysicalCount - BlankCount;
        public int LogicalCount { get; set; }
        public int CommentedCount { get; set; }
        public int CyclomaticComplexity { get; set; }
    }
}
