using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class TelemetryUiDispatcher
    {
        private readonly TelemetryChannel _channel;

        public event Action<Telemetry>? OnData;

        public TelemetryUiDispatcher(TelemetryChannel channel)
        {
            _channel = channel;
        }

        public void Publish(Telemetry data)
        {
            OnData?.Invoke(data);
        }

        public async Task StartAsync(CancellationToken token)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(token))
            {
                OnData?.Invoke(item);
            }
        }
    }
}
