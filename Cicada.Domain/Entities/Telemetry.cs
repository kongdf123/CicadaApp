using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Entities
{
    public class Telemetry
    {
        public string DeviceId { get; set; }
        public string Tag { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
