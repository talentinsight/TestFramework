using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Configuration
{
    public class ConfigurationManager
    {
        private readonly ILogger _logger;
        private object? _currentConfig;

        public ConfigurationManager(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<T> LoadConfigurationAsync<T>(string configPath) where T : class
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Configuration file not found: {configPath}");
                }

                var json = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<T>(json);

                if (config == null)
                {
                    throw new InvalidOperationException("Failed to deserialize configuration");
                }

                _currentConfig = config;
                _logger.Log($"Configuration loaded from {configPath}", LogLevel.Info);

                return config;
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading configuration: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public async Task SaveConfigurationAsync<T>(string configPath, T config) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(configPath, json);
                _currentConfig = config;
                _logger.Log($"Configuration saved to {configPath}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.Log($"Error saving configuration: {ex.Message}", LogLevel.Error);
                throw;
            }
        }

        public T GetConfiguration<T>() where T : class
        {
            if (_currentConfig == null)
            {
                throw new InvalidOperationException("Configuration not loaded");
            }

            if (_currentConfig is T config)
            {
                return config;
            }

            throw new InvalidOperationException($"Configuration is not of type {typeof(T).Name}");
        }
    }
} 