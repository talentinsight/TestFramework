using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFramework.Core.Configuration
{
    /// <summary>
    /// Configuration provider that uses environment variables
    /// </summary>
    public class EnvironmentConfigurationProvider : IConfigurationProvider
    {
        private readonly string _prefix;
        private Dictionary<string, string> _cache;

        /// <summary>
        /// Initializes a new instance of the EnvironmentConfigurationProvider class
        /// </summary>
        /// <param name="prefix">Prefix for environment variables (e.g., "TEST_")</param>
        public EnvironmentConfigurationProvider(string prefix = "TEST_")
        {
            _prefix = prefix;
            _cache = new Dictionary<string, string>();
            Reload();
        }

        /// <inheritdoc />
        public string? GetValue(string key)
        {
            var envKey = NormalizeKey(key);
            return _cache.TryGetValue(envKey, out var value) ? value : null;
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetSection(string sectionName)
        {
            var sectionPrefix = NormalizeKey(sectionName);
            return _cache
                .Where(kvp => kvp.Key.StartsWith(sectionPrefix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    kvp => kvp.Key.Substring(sectionPrefix.Length),
                    kvp => kvp.Value
                );
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAll()
        {
            return new Dictionary<string, string>(_cache);
        }

        /// <inheritdoc />
        public void SetValue(string key, string value)
        {
            var envKey = NormalizeKey(key);
            Environment.SetEnvironmentVariable(envKey, value);
            _cache[envKey] = value;
        }

        /// <inheritdoc />
        public void SetValues(IDictionary<string, string> values)
        {
            foreach (var kvp in values)
            {
                SetValue(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc />
        public void Reload()
        {
            _cache.Clear();
            var envVars = Environment.GetEnvironmentVariables();
            
            foreach (string key in envVars.Keys)
            {
                if (key.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var value = envVars[key]?.ToString();
                    if (value != null)
                    {
                        _cache[key] = value;
                    }
                }
            }
        }

        private string NormalizeKey(string key)
        {
            return $"{_prefix}{key.ToUpperInvariant()}";
        }
    }
} 