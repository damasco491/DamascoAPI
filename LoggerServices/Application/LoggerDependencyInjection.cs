using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerServices.Application
{
    public static class LoggerDependencyInjection
    {
        public static IServiceCollection AddLoggerApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoggerDependencyInjection).Assembly));
            return services;
        }
    }
}
