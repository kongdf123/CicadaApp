using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public interface IModulePlugin
    {
        string Name { get; }
        Task InitializeAsync();
        Task ExecuteAsync(Telemetry data);
    }
}
