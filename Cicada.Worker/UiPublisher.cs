using Cicada.Biz;
using Cicada.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Worker
{
    public class UiPublisher
    {
        private readonly IHubContext<TelemetryHub> _hub;

        public UiPublisher(IHubContext<TelemetryHub> hub)
        {
            _hub = hub;
        }

        public async Task Push(Telemetry data)
        {
            await _hub.Clients.All.SendAsync("telemetry", data);
        }
    }
}
