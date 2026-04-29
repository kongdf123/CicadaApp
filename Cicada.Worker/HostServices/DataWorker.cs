using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using Cicada.Worker.Mqtt;
using Cicada.Worker.Opc;
using Serilog;

namespace Cicada.Worker.Workers
{
    public class DataWorker : BackgroundService
    {
        private readonly TelemetryConsumer _consumer;
        private readonly TelemetryChannel _channel;
        private readonly OpcUaClientService _opc;
        private readonly MqttTelemetryService _mqttSubscriber;
        private readonly MqttPublisher _mqttPublisher;

        public DataWorker(
            TelemetryConsumer consumer,
            TelemetryChannel channel,
            OpcUaClientService opc,
            MqttTelemetryService mqttSubscriber,
            MqttPublisher mqttPublisher)
        {
            _consumer = consumer;
            _channel = channel;
            _opc = opc;
            _mqttSubscriber = mqttSubscriber;
            _mqttPublisher = mqttPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("🚀 DataWorker started");

            // =========================
            // 1. Start Consumer (Pipeline)
            // =========================
            _ = Task.Run(() => _consumer.StartAsync(stoppingToken), stoppingToken);

            // =========================
            // 2. Start MQTT Subscriber
            // =========================
            _ = Task.Run(() => _mqttSubscriber.StartAsync(stoppingToken), stoppingToken);

            // =========================
            // 3. Connect MQTT Publisher (for OPC bridge)
            // =========================
            await _mqttPublisher.ConnectAsync();

            // =========================
            // 4. Connect OPC UA
            // =========================
            //await _opc.ConnectAsync("opc.tcp://localhost:4840");
            await _opc.ConnectAsync("opc.tcp://127.0.0.1:62541/Quickstarts/ReferenceServer");

            // =========================
            // 5. Subscribe OPC Nodes → Publish to MQTT
            // =========================
            //_opc.SubscribeNode("ns=2;s=Temperature", async value =>
            //{
            //    var topic = "cicada/device1/temperature";

            //    await _mqttPublisher.PublishAsync(topic, value.ToString("F2"));

            //    Log.Information($"OPC → MQTT | {topic} = {value:F2}");
            //});

            //_opc.SubscribeNode("ns=2;s=Pressure", async value =>
            //{
            //    var topic = "cicada/device1/pressure";

            //    await _mqttPublisher.PublishAsync(topic, value.ToString("F2"));

            //    Log.Information($"OPC → MQTT | {topic} = {value:F2}");
            //});

            _opc.SubscribeNode("ns=7;s=Alarms.Start", v =>
            {
                Console.WriteLine($"Value: {v}");
            });

            // =========================
            // 6. Keep Alive Loop
            // =========================
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Log.Debug("Service heartbeat...");

                    var value = await _opc.ReadNodeAsync("ns=7;s=Alarms.Start");

                    if (value.HasValue)
                    {
                        await _channel.Writer.WriteAsync(new Telemetry
                        {
                            DeviceId = "OPC",
                            Tag = "Double",
                            Value = value.Value,
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // normal shutdown
            }

            Log.Information("🛑 DataWorker stopped");
        }
    }
}
