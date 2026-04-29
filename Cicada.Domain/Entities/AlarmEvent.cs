using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Entities
{
    public class AlarmRule
    {
        public string Tag { get; set; } = default!;
        public double WarningThreshold { get; set; }
        public double CriticalThreshold { get; set; }

        // 冷却时间（秒）
        public int CooldownSeconds { get; set; } = 10;
    }

    public class AlarmState
    {
        public AlarmLevel CurrentLevel { get; set; } = AlarmLevel.None;
        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;
    }

    public enum AlarmLevel
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Critical = 3
    }

    public enum AlarmStatus
    {
        Active,
        Acknowledged,
        Recovered
    }

    public class AlarmEvent
    {
        public string DeviceId { get; set; } = default!;
        public string Tag { get; set; } = default!;
        public double Value { get; set; }

        public AlarmLevel Level { get; set; }
        public AlarmStatus Status { get; set; }

        public DateTime Timestamp { get; set; }

        public string Key => $"{DeviceId}:{Tag}";
    }
}
