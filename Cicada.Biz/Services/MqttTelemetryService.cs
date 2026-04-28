using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cicada.Domain.Entities;
using MQTTnet;
using MQTTnet.Client;

namespace Cicada.Biz.Services
{
    public class MqttTelemetryService
    {
        private readonly TelemetryChannel _channel;

        public MqttTelemetryService(TelemetryChannel channel)
        {
            _channel = channel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            client.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                // 👉 假设 JSON: { deviceId, tag, value }
                var parts = payload.Split(',');

                var data = new Telemetry
                {
                    DeviceId = parts[0],
                    Tag = parts[1],
                    Value = double.Parse(parts[2]),
                    Timestamp = DateTime.UtcNow
                };

                await _channel.Writer.WriteAsync(data, cancellationToken);
            };

            await client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .Build(), cancellationToken);

            await client.SubscribeAsync("cicada/telemetry");
        }
    }
}
