using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TestFramework.Core
{
    /// <summary>
    /// Singleton Configuration Reader class responsible for reading test configurations
    /// </summary>
    public class ConfigReader
    {
        private static ConfigReader? _instance;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;

        // Private constructor for singleton pattern
        private ConfigReader()
        {
            // Build configuration from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        /// <summary>
        /// Gets the singleton instance of ConfigReader
        /// </summary>
        public static ConfigReader Instance
        {
            get
            {
                // Double-check locking pattern
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigReader();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value</returns>
        public string? GetValue(string key)
        {
            return _configuration[key];
        }

        /// <summary>
        /// Gets a strongly typed configuration section
        /// </summary>
        /// <typeparam name="T">Type to bind configuration to</typeparam>
        /// <param name="sectionName">Configuration section name</param>
        /// <returns>Configuration object</returns>
        public T GetSection<T>(string sectionName) where T : class, new()
        {
            var section = _configuration.GetSection(sectionName);
            var config = new T();
            // Extension method to bind configuration section to object
            Microsoft.Extensions.Configuration.ConfigurationBinder.Bind(section, config);
            return config;
        }
    }
} 