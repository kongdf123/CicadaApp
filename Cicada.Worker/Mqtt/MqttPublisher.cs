using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Worker.Mqtt
{
    public class MqttPublisher
    {
        private IMqttClient _client;

        public async Task ConnectAsync()
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            await _client.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .Build());
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await _client.PublishAsync(message);
        }
    }
}
