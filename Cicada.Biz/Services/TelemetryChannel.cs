using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class TelemetryChannel
    {
        private readonly Channel<Telemetry> _channel; // High performance, async, thread safety channel for telemetry data

        public TelemetryChannel()
        {
            _channel = Channel.CreateUnbounded<Telemetry>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            }); 
        }

        public ChannelWriter<Telemetry> Writer => _channel.Writer;
        public ChannelReader<Telemetry> Reader => _channel.Reader;
    }
}
