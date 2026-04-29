using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class TelemetryProducer
    {
        //private readonly TelemetryChannel _channel;

        //public TelemetryProducer(TelemetryChannel channel)
        //{
        //    _channel = channel;
        //}

        //public async Task StartAsync(CancellationToken token)
        //{
        //    var random = new Random();

        //    while (!token.IsCancellationRequested)
        //    {
        //        var data = new Telemetry
        //        {
        //            DeviceId = "Device1",
        //            Tag = "Temperature",
        //            Value = random.NextDouble() * 100,
        //            Timestamp = DateTime.UtcNow
        //        };

        //        await _channel.Writer.WriteAsync(data, token);

        //        await Task.Delay(500, token); // 模拟设备频率
        //    }
        //}
    }
}
