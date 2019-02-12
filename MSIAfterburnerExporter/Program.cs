using System;
using Prometheus;
using Prometheus.Advanced;

namespace MSIAfterburnerExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultCollectorRegistry.Instance.RegisterOnDemandCollectors(new MSICollector());
            var server = new KestrelMetricServer(1234);
            server.Start();
            Console.ReadKey();
            server.Stop();
        }
    }
}
