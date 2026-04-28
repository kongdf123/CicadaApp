using Cicada.Domain.Entities;
using Cicada.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class DatabaseModule : IPipelineModule
    {
        private readonly ITelemetryRepository _repo;

        public DatabaseModule(ITelemetryRepository repo)
        {
            _repo = repo;
        }

        public Task ProcessAsync(Telemetry data)
        {
            return _repo.SaveAsync(data);
        }
    }
}
