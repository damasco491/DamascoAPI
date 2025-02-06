using Common.Model.Global;
using Common.Model.Global.Location;
using Common.Model.Global.Merchants;
using Common.Model.Global.Roles;
using Common.Model.Global.Users;
using LoggerServices.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Constants.StoredProcedures;
namespace LoggerServices.Implementations
{
    public class LoggerService : ILoggerService
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public LoggerService(IMediator mediator,IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
            Log.Logger = new LoggerConfiguration().CreateLogger();
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();
        }
        public async Task LogInfo(string logType,string source, string msg, string obj)
        {
            LogContext.PushProperty("LogType", logType);
            LogContext.PushProperty("Source", source);
            LogContext.PushProperty("DataFromWeb", obj);
            Log.Information(msg);
            Log.CloseAndFlush();

        }

    }
}
