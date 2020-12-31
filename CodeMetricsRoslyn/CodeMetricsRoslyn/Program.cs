using System;
using CodeMetricsRoslyn.LocMetrics;
using CodeMetricsRoslyn.OOPMetrics;

namespace CodeMetricsRoslyn
{
    class Program
    {
        static void Main(string[] args)
        {
            const string oopMetricDirectoryPath = @"D:\Projects\CodeMetricsRoslyn\CodeMetricsRoslyn\TestOOP";
            const string locDemoMetricDirectoryPath = @"D:\Projects\CodeMetricsRoslyn\CodeMetricsRoslyn\TestLoc";

            var metricResult = LocMetricsCalculator.Calculate(locDemoMetricDirectoryPath);
            Console.WriteLine(metricResult);

            OOPMetricsCalculator.Calculate(oopMetricDirectoryPath);
        }
    }
}
