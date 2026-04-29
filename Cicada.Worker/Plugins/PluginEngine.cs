using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class PluginEngine
    {
        private readonly List<IModulePlugin> _plugins;

        public PluginEngine(List<IModulePlugin> plugins)
        {
            _plugins = plugins;
        }

        public async Task ExecuteAsync(Telemetry data)
        {
            foreach (var plugin in _plugins)
            {
                await plugin.ExecuteAsync(data);
            }
        }
    }
}
