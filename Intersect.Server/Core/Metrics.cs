using App.Metrics;
using App.Metrics.Histogram;
using App.Metrics.ReservoirSampling.SlidingWindow;
using Intersect.Server.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Core
{
    public static class Metrics
    {
        private static readonly IMetricsRoot _metrics = AppMetrics.CreateDefaultBuilder().SampleWith.SlidingWindow(10000).Build();

        //How late (in ms) we are sending a map to the thread pool for an update
        private static readonly HistogramOptions _mapQueueLateness = new HistogramOptions()
        {
            Name = "Map Queue Update Offset"
        };

        //How long (in ms) it is taking for a map update to start once queued up
        private static readonly HistogramOptions _mapUpdateStartTime = new HistogramOptions()
        {
            Name = "Map Update Start Time"
        };

        //How long (in ms) it is taking to update a map
        private static readonly HistogramOptions _mapUpdateTime = new HistogramOptions()
        {
            Name = "Map Update Time"
        };


        public static void RecordMapQueueUpdateLateness(long ms)
        {
            _metrics.Measure.Histogram.Update(_mapQueueLateness, ms);
        }

        public static void RecordMapUpdateStartTime(long ms)
        {
            _metrics.Measure.Histogram.Update(_mapUpdateStartTime, ms);
        }

        public static void RecordMapUpdateTime(long ms)
        {
            _metrics.Measure.Histogram.Update(_mapUpdateTime, ms);
        }

        public static void LogInfo()
        {
            Console.WriteLine();
            Console.WriteLine("CPS: " + Globals.Cps);
            var snapshot = _metrics.Snapshot.Get();

            foreach (var context in snapshot.Contexts)
            {
                foreach (var hist in context.Histograms)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{hist.Name.Split('|')[0]}:");
                    Console.WriteLine($"Mn: {hist.Value.Min}");
                    Console.WriteLine($"Max: {hist.Value.Max}");
                    Console.WriteLine($"Median: {hist.Value.Median}");
                    Console.WriteLine($"Mean: {hist.Value.Mean}");
                }
            }

        }
    }
}
