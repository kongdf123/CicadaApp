using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class PluginLoader
    {
        public List<IModulePlugin> Load(string path)
        {
            var plugins = new List<IModulePlugin>();

            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(file);

                var types = assembly.GetTypes()
                    .Where(t => typeof(IModulePlugin).IsAssignableFrom(t) && !t.IsInterface);

                foreach (var type in types)
                {
                    var plugin = (IModulePlugin)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }
            }

            return plugins;
        }
    }
}
