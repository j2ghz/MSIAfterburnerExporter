using System;
using System.Collections.Generic;
using System.Linq;
using MSI.Afterburner;
using Prometheus;
using Prometheus.Advanced;

namespace MSIAfterburnerExporter
{
    public class MSICollector : IOnDemandCollector
    {
        private readonly HardwareMonitor hwm = new HardwareMonitor();
        private readonly Dictionary<int, Gauge> gauges = new Dictionary<int, Gauge>();

        public void RegisterMetrics(ICollectorRegistry registry)
        {
            hwm.Connect();
            var m = Metrics.WithCustomRegistry(registry);

            for (var i = 0; i < hwm.Entries.Length; i++)
            {
                var entry = hwm.Entries[i];
                gauges.Add(i,
                    m.CreateGauge(
                        entry.SrcName.Replace(" ", "_").Replace("%","").Replace(".",""),
                        entry.LocalizedSrcName,
                        "max",
                        "min"));
            }
        }

        public void UpdateMetrics()
        {
            Console.Write($"[{DateTime.Now}] Reloading ");
            var before = DateTime.Now;
            hwm.ReloadAll();
            Console.Write($"{(DateTime.Now-before).TotalMilliseconds}ms Updating ");
            before = DateTime.Now;
            foreach (var kvp in gauges)
            {
                var entry = hwm.Entries[kvp.Key];
                kvp.Value.Labels(entry.MaxLimit.ToString(), entry.MinLimit.ToString()).Set(entry.Data);
            }
            Console.WriteLine($"{(DateTime.Now - before).TotalMilliseconds}ms Done");
        }
    }
}