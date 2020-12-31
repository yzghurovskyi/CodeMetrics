namespace CodeMetricsRoslyn.OOPMetrics
{
    public class FileOOPMetricResult
    {
        public int NumberOfChildren { get; set; }
        public int InheritanceDepth { get; set; }
        public int VisibleMethods { get; set; }
        public int HiddenMethods { get; set; }
        public int VisibleAttributes { get; set; }
        public int HiddenAttributes { get; set; }
    }
}
