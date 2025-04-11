using Microsoft.Extensions.DependencyInjection;
using TestFramework.Core.Application;
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
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTestFramework(this IServiceCollection services)
        {
            // Register core services
            services.AddSingleton<ICppApplication, CppApplication>();
            
            // Logger should be registered by the consuming application
            // Core project only defines the interface

            return services;
        }
    }
} 