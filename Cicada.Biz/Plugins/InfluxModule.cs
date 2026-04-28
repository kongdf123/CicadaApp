using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class InfluxModule : IPipelineModule
    {
        private readonly InfluxService _influx;

        public InfluxModule(InfluxService influx)
        {
            _influx = influx;
        }

        public Task ProcessAsync(Telemetry data)
        {
            return _influx.WriteAsync(data);
        }
    }
}
