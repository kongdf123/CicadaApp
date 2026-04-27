using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz
{
    public interface IAppLogger
    {
        void Info(string message);
        void Error(Exception ex, string message);
    }
}
