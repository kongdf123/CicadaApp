using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Interfaces
{
    public interface ITelemetryRepository
    {
        Task SaveAsync(Telemetry data);
    }
}
