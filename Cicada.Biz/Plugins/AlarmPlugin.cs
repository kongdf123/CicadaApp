using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class AlarmPlugin : IModulePlugin
    {
        public string Name => "Alarm Plugin";

        public Task InitializeAsync()
        {
            Console.WriteLine("Alarm plugin initialized");
            return Task.CompletedTask;
        }

        public Task ExecuteAsync(Telemetry data)
        {
            if (data.Value > 80)
            {
                Console.WriteLine($"ALARM: {data.Tag} = {data.Value}");
            }

            return Task.CompletedTask;
        }
    }
}
