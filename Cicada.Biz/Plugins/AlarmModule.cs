using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class AlarmModule : IPipelineModule
    {
        private readonly AlarmService _alarm;

        public AlarmModule(AlarmService alarm)
        {
            _alarm = alarm;
        }

        public Task ProcessAsync(Telemetry data)
        {
            _alarm.Evaluate(data);
            return Task.CompletedTask;
        }
    }
}
