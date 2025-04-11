using System;
using System.Collections.Generic;
using System.IO;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Manages application configuration settings
    /// </summary>
    public class Configuration
    {
        private readonly string _filePath;
        private readonly Dictionary<string, string> _settings;

        /// <summary>
        /// Initializes a new instance of the Configuration class
        /// </summary>
        /// <param name="filePath">Path to the configuration file</param>
        public Configuration(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Loads configuration settings from the file
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Configuration file not found: {_filePath}");
            }

            _settings.Clear();
            // Read settings in JSON format
            string content = File.ReadAllText(_filePath);
            // Simple JSON parsing for {"key": "value"} format
            if (content.StartsWith("{") && content.EndsWith("}"))
            {
                content = content.Substring(1, content.Length - 2).Trim();
                string[] entries = content.Split(',');
                foreach (string entry in entries)
                {
                    string trimmedEntry = entry.Trim();
                    int colonIndex = trimmedEntry.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string key = trimmedEntry.Substring(0, colonIndex).Trim();
                        string value = trimmedEntry.Substring(colonIndex + 1).Trim();
                        // Remove quotes from key and value
                        key = key.Trim('"');
                        value = value.Trim('"');
                        _settings[key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Saves configuration settings to the file
        /// </summary>
        public void Save()
        {
            string directory = Path.GetDirectoryName(_filePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write settings in JSON format
            using (var writer = new StreamWriter(_filePath))
            {
                writer.WriteLine("{");
                int count = 0;
                foreach (var setting in _settings)
                {
                    count++;
                    writer.Write($"  \"{setting.Key}\": \"{setting.Value}\"");
                    if (count < _settings.Count)
                    {
                        writer.WriteLine(",");
                    }
                    else
                    {
                        writer.WriteLine();
                    }
                }
                writer.WriteLine("}");
            }
        }

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <returns>The configuration value, or null if not found</returns>
        public string? GetValue(string key)
        {
            return _settings.TryGetValue(key, out string? value) ? value : null;
        }

        /// <summary>
        /// Sets a configuration value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The configuration value</param>
        public void SetValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _settings[key] = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
} 