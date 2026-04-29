using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class UiModule : IPipelineModule
    {
        private readonly TelemetryUiDispatcher _dispatcher;

        public UiModule(TelemetryUiDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task ProcessAsync(Telemetry data, CancellationToken token = default)
        {
            _dispatcher.Publish(data);
            return Task.CompletedTask;
        }
    }
}
