using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class AnomalyModule : IPipelineModule
    {
        private readonly List<double> _buffer = new();

        public Task ProcessAsync(Telemetry data, CancellationToken token = default)
        {
            _buffer.Add(data.Value);

            if (_buffer.Count < 10)
                return Task.CompletedTask;

            var avg = _buffer.Average();
            var std = Math.Sqrt(_buffer.Average(v => Math.Pow(v - avg, 2)));

            if (Math.Abs(data.Value - avg) > 3 * std)
            {
                Console.WriteLine($"Anomaly detected: {data.Value}");
            }

            if (_buffer.Count > 100)
                _buffer.RemoveAt(0);

            return Task.CompletedTask;
        }
    }
}
