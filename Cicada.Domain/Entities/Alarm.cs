using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Entities
{
    public class Alarm
    {
        public string DeviceId { get; set; }
        public string Tag { get; set; }
        public double Value { get; set; }
        public string Level { get; set; } // Info / Warning / Critical
        public DateTime Timestamp { get; set; }
    }
}
