using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class PipelineEngine
    {
        private readonly IEnumerable<IPipelineModule> _modules;

        public PipelineEngine(IEnumerable<IPipelineModule> modules)
        {
            _modules = modules;
        }

        public async Task ExecuteAsync(Telemetry data)
        {
            foreach (var module in _modules)
            {
                await module.ProcessAsync(data);
            }
        }
    }
}
