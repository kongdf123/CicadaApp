using Cicada.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Models
{
    public class AlarmEventBus
    {
        public event Action<AlarmEvent>? OnAlarm;

        public void Publish(AlarmEvent alarm)
        {
            OnAlarm?.Invoke(alarm);
        }
    }
}
