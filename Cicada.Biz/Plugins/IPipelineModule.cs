using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public interface IPipelineModule
    {
        Task ProcessAsync(Telemetry data, CancellationToken token = default);
    }
}
