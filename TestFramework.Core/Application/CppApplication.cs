using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILogger = TestFramework.Core.Logger.ILogger;
using LogLevel = TestFramework.Core.Logger.LogLevel;
using Microsoft.Extensions.Logging;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Base implementation of a C++ application
    /// </summary>
    public class CppApplication : IDisposable, ICppApplication
    {
        private readonly ILogger _logger;
        private bool _isInitialized;
        private bool _isRunning;
        private string _lastError = string.Empty;
        private readonly List<string> _logs = new();
        private readonly string _version;
        private readonly Dictionary<string, string> _configuration;
        private readonly Dictionary<string, bool> _services;
        private string _logLevel;
        private readonly List<string> _errorHistory;
        private bool _isInErrorState;
        private readonly string _applicationPath;
        private readonly string? _workingDirectory;
        private readonly string? _arguments;
        private ApplicationConfiguration? _appConfig;

        public bool IsInitialized => _isInitialized;
        public bool IsRunning => _isRunning;
        public string LastError => _lastError;
        public bool IsInErrorState => _isInErrorState;
        public ApplicationConfiguration? Configuration => _appConfig;

        public CppApplication(ILogger logger, string applicationPath, string? workingDirectory = null, string? arguments = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationPath = applicationPath ?? throw new ArgumentNullException(nameof(applicationPath));
            _workingDirectory = workingDirectory ?? System.IO.Path.GetDirectoryName(applicationPath);
            _arguments = arguments;
            _version = "1.0.0";
            _configuration = new Dictionary<string, string>();
            _services = new Dictionary<string, bool>();
            _errorHistory = new List<string>();
            _logLevel = "INFO";
            _isInErrorState = false;
            
            // Initialize some default services
            _services["Database"] = false;
            _services["WebServer"] = false;
            _services["MessageQueue"] = false;

            _isInitialized = false;
            _isRunning = false;
        }

        public string GetApplicationVersion()
        {
            return _version;
        }

        public bool LoadConfiguration(string configPath)
        {
            _configuration["configPath"] = configPath;
            _logger.Log($"Configuration loaded from {configPath}", LogLevel.Info);
            return true;
        }

        public bool ValidateConfiguration()
        {
            return true;
        }

        public string GetConfigurationValue(string key)
        {
            return _configuration.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public Dictionary<string, string> GetAllConfigurationValues()
        {
            return new Dictionary<string, string>(_configuration);
        }

        public bool StartService(string serviceName)
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                throw new InvalidOperationException(_lastError);
            }

            if (!_services.ContainsKey(serviceName))
            {
                _lastError = $"Service {serviceName} not found";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (_services[serviceName])
            {
                _lastError = $"Service {serviceName} is already running";
                _logger.Log(_lastError, LogLevel.Warning);
                return false;
            }

            _services[serviceName] = true;
            _logger.Log($"Service {serviceName} started", LogLevel.Info);
            return true;
        }

        public async Task<bool> StartServiceAsync(string serviceName)
        {
            try
            {
                await Task.Delay(100); // Simulate async operation
                return StartService(serviceName);
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to start service {serviceName}: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }
        }

        public bool StopService(string serviceName)
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                throw new InvalidOperationException(_lastError);
            }

            if (!_services.ContainsKey(serviceName))
            {
                _lastError = $"Service {serviceName} not found";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (!_services[serviceName])
            {
                _lastError = $"Service {serviceName} is already stopped";
                _logger.Log(_lastError, LogLevel.Warning);
                return false;
            }

            _services[serviceName] = false;
            _logger.Log($"Service {serviceName} stopped", LogLevel.Info);
            return true;
        }

        public async Task<bool> StopServiceAsync(string serviceName)
        {
            try
            {
                await Task.Delay(100); // Simulate async operation
                return StopService(serviceName);
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to stop service {serviceName}: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }
        }

        public bool IsServiceRunning(string serviceName)
        {
            return _services.TryGetValue(serviceName, out var isRunning) && isRunning;
        }

        public List<string> GetAvailableServices()
        {
            return new List<string>(_services.Keys);
        }

        public bool SetLogLevel(string level)
        {
            var validLevels = new[] { "DEBUG", "INFO", "WARNING", "ERROR" };
            if (!validLevels.Contains(level))
            {
                _lastError = $"Invalid log level: {level}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            _logLevel = level;
            var logLevel = level switch
            {
                "DEBUG" => LogLevel.Debug,
                "INFO" => LogLevel.Info,
                "WARNING" => LogLevel.Warning,
                "ERROR" => LogLevel.Error,
                _ => LogLevel.Info
            };
            _logger.SetLogLevel(logLevel);
            _logger.Log($"Log level set to {level}", LogLevel.Info);
            return true;
        }

        public string GetLogLevel()
        {
            return _logLevel;
        }

        public bool ClearLogs()
        {
            _logs.Clear();
            _logger.Log("Logs cleared", LogLevel.Info);
            return true;
        }

        public List<string> GetRecentLogs(int count)
        {
            if (count <= 0)
            {
                return new List<string>();
            }

            var startIndex = Math.Max(0, _logs.Count - count);
            return _logs.GetRange(startIndex, Math.Min(count, _logs.Count - startIndex));
        }

        public bool ClearErrorState()
        {
            _isInErrorState = false;
            _lastError = string.Empty;
            _errorHistory.Clear();
            _logger.Log("Error state cleared", LogLevel.Info);
            return true;
        }

        public List<string> GetErrorHistory()
        {
            return new List<string>(_errorHistory);
        }

        public async Task<bool> InitializeAsync()
        {
            if (_isInitialized)
            {
                _lastError = "Application already initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                await Task.Delay(100); // Simulate initialization
                _isInitialized = true;
                _logger.Log("Application initialized", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Initialization failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }

        public async Task<bool> StartAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (_isRunning)
            {
                _lastError = "Application already running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                await Task.Delay(100); // Simulate startup
                _isRunning = true;
                _logger.Log("Application started", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Start failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }

        public async Task<bool> StopAsync()
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                await Task.Delay(100); // Simulate shutdown
                _isRunning = false;
                _logger.Log("Application stopped", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Stop failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }

        public async Task<bool> RestartAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                if (_isRunning)
                {
                    await StopAsync();
                }
                await StartAsync();
                _logger.Log("Application restarted", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Restart failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }

        public async Task<string?> GetStatusAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return null;
            }

            try
            {
                var status = _isRunning ? "Running" : "Stopped";
                foreach (var service in _services)
                {
                    status += $", {service.Key}: {(service.Value ? "Running" : "Stopped")}";
                }
                return status;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to get status: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return null;
            }
        }

        public async Task<bool> SendCommandAsync(string command)
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (!_isRunning)
            {
                _lastError = "Application not running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                _logger.Log($"Sending command: {command}", LogLevel.Info);
                await Task.Delay(100); // Simulate command processing
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to send command: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }
        }

        public async Task<string?> GetResponseAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return null;
            }

            if (!_isRunning)
            {
                _lastError = "Application not running";
                _logger.Log(_lastError, LogLevel.Error);
                return null;
            }

            try
            {
                await Task.Delay(100); // Simulate response delay
                return "Command processed successfully";
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to get response: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return null;
            }
        }

        public void Dispose()
        {
            if (_isRunning)
            {
                StopAsync().Wait();
            }
            _isInitialized = false;
            _logger.Log("Application disposed", LogLevel.Info);
        }

        private void LogToLogger(string message, LogLevel level)
        {
            _logger.Log(message, level);
            _logs.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
            if (level == LogLevel.Error)
            {
                _errorHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }
        }

        private bool ShouldLog(string level)
        {
            var levels = new[] { "DEBUG", "INFO", "WARNING", "ERROR" };
            var currentLevelIndex = Array.IndexOf(levels, _logLevel);
            var messageLevelIndex = Array.IndexOf(levels, level);
            return messageLevelIndex >= currentLevelIndex;
        }

        public async Task<bool> SetConfigurationAsync(ApplicationConfiguration config)
        {
            if (config == null)
            {
                _lastError = "Configuration cannot be null";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (_isRunning)
            {
                _lastError = "Cannot change configuration while application is running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                _appConfig = config;
                _logger.Log("Configuration updated", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to set configuration: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }
        }
    }
} 