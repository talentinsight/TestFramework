using System.Collections.Generic;

namespace TestFramework.Core.Configuration
{
    /// <summary>
    /// Interface for configuration providers
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value if found, null otherwise</returns>
        string? GetValue(string key);

        /// <summary>
        /// Gets a configuration section
        /// </summary>
        /// <param name="sectionName">Section name</param>
        /// <returns>Configuration section if found, empty dictionary otherwise</returns>
        IDictionary<string, string> GetSection(string sectionName);

        /// <summary>
        /// Gets all configuration values
        /// </summary>
        /// <returns>Dictionary of all configuration values</returns>
        IDictionary<string, string> GetAll();

        /// <summary>
        /// Sets a configuration value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Configuration value</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Sets multiple configuration values
        /// </summary>
        /// <param name="values">Dictionary of configuration values</param>
        void SetValues(IDictionary<string, string> values);

        /// <summary>
        /// Reloads the configuration from the source
        /// </summary>
        void Reload();
    }
} 