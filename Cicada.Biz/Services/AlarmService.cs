using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class AlarmService
    {
        public event Action<Alarm>? OnAlarm;

        public void Evaluate(Telemetry data)
        {
            if (data.Tag == "Temperature")
            {
                if (data.Value > 80)
                {
                    OnAlarm?.Invoke(new Alarm
                    {
                        DeviceId = data.DeviceId,
                        Tag = data.Tag,
                        Value = data.Value,
                        Level = "Warning",
                        Timestamp = DateTime.Now
                    });
                }

                if (data.Value > 100)
                {
                    OnAlarm?.Invoke(new Alarm
                    {
                        DeviceId = data.DeviceId,
                        Tag = data.Tag,
                        Value = data.Value,
                        Level = "Critical",
                        Timestamp = DateTime.Now
                    });
                }
            }
        }
    }
}
