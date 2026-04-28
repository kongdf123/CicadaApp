using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Domain.Entities
{
    public class DeviceStatus
    {
        public string DeviceId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
