using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Configuration
{
    /// <summary>
    /// Manages configuration loading, saving, and retrieval operations.
    /// </summary>
    public class ConfigurationManager
    {
        private readonly ILogger _logger;
        private object _currentConfiguration;

        /// <summary>
        /// Initializes a new instance of the ConfigurationManager class.
        /// </summary>
        /// <param name="logger">The logger instance to use for logging operations.</param>
        public ConfigurationManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Loads configuration from a file asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of configuration to load.</typeparam>
        /// <param name="filePath">The path to the configuration file.</param>
        /// <returns>The loaded configuration object.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the configuration file is not found.</exception>
        /// <exception cref="JsonException">Thrown when the configuration file contains invalid JSON.</exception>
        public async Task<T> LoadConfigurationAsync<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Configuration file not found: {filePath}");
                }

                var json = await File.ReadAllTextAsync(filePath);
                var config = JsonSerializer.Deserialize<T>(json);
                _currentConfiguration = config;
                _logger.Log($"Configuration loaded from {filePath}", LogLevel.Info);
                return config;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading configuration: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Saves configuration to a file asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of configuration to save.</typeparam>
        /// <param name="filePath">The path to save the configuration file.</param>
        /// <param name="config">The configuration object to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveConfigurationAsync<T>(string filePath, T config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
                _currentConfiguration = config;
                _logger.Log($"Configuration saved to {filePath}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error saving configuration: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the current configuration of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of configuration to retrieve.</typeparam>
        /// <returns>The current configuration object.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no configuration has been loaded.</exception>
        public T GetConfiguration<T>()
        {
            if (_currentConfiguration == null)
            {
                throw new InvalidOperationException("No configuration has been loaded.");
            }

            return (T)_currentConfiguration;
        }
    }
} 