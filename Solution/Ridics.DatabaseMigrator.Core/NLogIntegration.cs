using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ridics.DatabaseMigrator.Core
{
    public static class NLogIntegration
    {
        public static ServiceCollection AddNLog(this ServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
            return services;
        }
    }
}