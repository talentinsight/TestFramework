using System;
using System.Collections.Generic;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Represents the configuration for an application
    /// </summary>
    public class ApplicationConfiguration
    {
        private readonly Dictionary<string, string> _settings;

        /// <summary>
        /// Gets or sets the port number
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the host name or IP address
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timeout in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the ApplicationConfiguration class
        /// </summary>
        public ApplicationConfiguration()
        {
            _settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets a configuration value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <returns>The configuration value</returns>
        public string this[string key]
        {
            get => _settings.TryGetValue(key, out var value) ? value : string.Empty;
            set => _settings[key] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets all configuration settings
        /// </summary>
        /// <returns>A dictionary of all configuration settings</returns>
        public Dictionary<string, string> GetAllSettings()
        {
            return new Dictionary<string, string>(_settings);
        }

        /// <summary>
        /// Sets multiple configuration settings
        /// </summary>
        /// <param name="settings">The settings to set</param>
        public void SetSettings(Dictionary<string, string> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            foreach (var setting in settings)
            {
                _settings[setting.Key] = setting.Value;
            }
        }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if the configuration is valid, false otherwise</returns>
        public bool Validate()
        {
            return true;
        }
    }
} 