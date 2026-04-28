using Cicada.UI.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Infra
{
    public class DeviceManager
    {
        private readonly List<DeviceConfig> _devices;

        public DeviceManager(IOptions<List<DeviceConfig>> options)
        {
            _devices = options.Value;
        }
    }
}
