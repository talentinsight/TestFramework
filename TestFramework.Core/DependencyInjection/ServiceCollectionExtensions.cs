using Microsoft.Extensions.DependencyInjection;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Logger;
using TestFramework.Core.Utils;

namespace TestFramework.Core.DependencyInjection
{
    /// <summary>
    /// Extension methods for IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds test framework services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="loggerType">Type of logger to use</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTestFramework(this IServiceCollection services, LoggerType loggerType = LoggerType.Console)
        {
            // Register core services
            services.AddSingleton<ILogger>(_ => LoggerFactory.CreateLogger(loggerType));
            services.AddSingleton<ProcessHelper>();
            
            // Register test runner
            services.AddScoped<ITestRunner, CppTestRunner>();
            
            return services;
        }
    }
} 