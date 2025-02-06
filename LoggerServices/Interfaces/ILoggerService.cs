using Common.Model.Global;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerServices.Interfaces
{
    public interface ILoggerService
    {
        public Task LogInfo(string logType, string source, string msg, string obj);
    }
}
