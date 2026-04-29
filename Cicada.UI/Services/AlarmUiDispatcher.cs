using Cicada.Domain.Entities;
using Cicada.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.UI.Services
{
    public class AlarmUiDispatcher
    {
        private readonly AlarmEventBus _bus;

        public event Action<AlarmEvent>? OnAlarm;

        public AlarmUiDispatcher(AlarmEventBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _bus.OnAlarm += alarm =>
            {
                OnAlarm?.Invoke(alarm);
            };
        }
    }
}
