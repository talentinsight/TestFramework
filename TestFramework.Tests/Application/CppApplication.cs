using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    public class CppApplication : IDisposable, ICppApplication
    {
        private readonly ILogger _logger;
        private readonly string _executablePath;
        private bool _isInitialized;
        private bool _isRunning;
        private bool _isDisposed;
        private bool _isInErrorState;
        private readonly List<string> _logs;
        private readonly List<string> _errorHistory;
        private ApplicationConfiguration _configuration;
        private LogLevel _currentLogLevel;
        private string _lastError;

        public string LastError { get; private set; }
        public bool IsRunning => _isRunning;
        public bool IsInitialized => _isInitialized;
        public bool IsInErrorState => _isInErrorState;
        public ApplicationConfiguration Configuration => _configuration;

        public CppApplication(ILogger logger, string executablePath)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _executablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
            _configuration = new ApplicationConfiguration();
            _logs = new List<string>();
            _errorHistory = new List<string>();
            _currentLogLevel = LogLevel.Info;
            _lastError = string.Empty;
            LastError = string.Empty;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            if (_isRunning)
            {
                StopAsync().GetAwaiter().GetResult();
            }

            _isDisposed = true;
        }

        private void SetError(string message)
        {
            LastError = message;
            _isInErrorState = true;
            LogMessage(message, LogLevel.Error);
        }

        private void LogMessage(string message, LogLevel level)
        {
            if (level < _currentLogLevel)
                return;

            var logMessage = $"[{level.ToString().ToUpper()}] {message}";
            _logger.Log(logMessage, level);
            _logs.Add(logMessage);
            if (level == LogLevel.Error)
            {
                _lastError = message;
                _errorHistory.Add(message);
            }
        }

        private void ClearError()
        {
            LastError = string.Empty;
            _lastError = string.Empty;
            _isInErrorState = false;
            _errorHistory.Clear();
        }

        public async Task<bool> InitializeAsync()
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (_isInitialized)
            {
                SetError("Application already initialized");
                return false;
            }

            _isInitialized = true;
            ClearError();
            LogMessage("Application initialized", LogLevel.Info);
            return true;
        }

        public async Task<bool> StartAsync()
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isInitialized)
            {
                SetError("Application not initialized");
                return false;
            }

            if (_isRunning)
            {
                SetError("Application already running");
                return false;
            }

            _isRunning = true;
            ClearError();
            LogMessage("Application started", LogLevel.Info);
            return true;
        }

        public async Task<bool> StopAsync()
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isRunning)
            {
                SetError("Application not running");
                return false;
            }

            if (!_isInitialized)
            {
                SetError("Application not initialized");
                return false;
            }

            _isRunning = false;
            ClearError();
            LogMessage("Application stopped", LogLevel.Info);
            return true;
        }

        public async Task<bool> RestartAsync()
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isInitialized)
            {
                SetError("Application not initialized");
                return false;
            }

            var startResult = await StartAsync();
            if (!startResult)
            {
                return false;
            }

            LogMessage("Application restarted", LogLevel.Info);
            return true;
        }

        public async Task<string?> GetStatusAsync()
        {
            if (_isDisposed)
                return "Disposed";
            if (_isRunning)
                return "Running";
            return "Stopped";
        }

        public async Task<bool> SendCommandAsync(string command)
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isRunning)
            {
                SetError("Application is not running");
                return false;
            }

            if (string.IsNullOrEmpty(command))
            {
                SetError("Command cannot be empty");
                return false;
            }

            _logger.Log($"Sending command: {command}", LogLevel.Debug);
            return true;
        }

        public async Task<string?> GetResponseAsync()
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return null;
            }

            if (!_isRunning)
            {
                SetError("Application is not running");
                return null;
            }

            return "Command processed successfully";
        }

        public async Task<bool> SetConfigurationAsync(ApplicationConfiguration configuration)
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (_isRunning)
            {
                SetError("Cannot change configuration while running");
                return false;
            }

            if (!ValidateConfiguration(configuration))
            {
                SetError("Invalid configuration");
                return false;
            }

            _configuration = configuration;
            _logger.Log("Configuration updated successfully", LogLevel.Info);
            return true;
        }

        private bool ValidateConfiguration(ApplicationConfiguration configuration)
        {
            if (configuration == null)
                return false;

            if (configuration.Port <= 0 || configuration.Port > 65535)
                return false;

            if (string.IsNullOrEmpty(configuration.Host))
                return false;

            if (configuration.Timeout <= 0)
                return false;

            return true;
        }

        public string GetApplicationVersion()
        {
            return "1.0.0";
        }

        public bool LoadConfiguration(string configPath)
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            _logger.Log($"Loading configuration from {configPath}", LogLevel.Info);
            return true;
        }

        public bool ValidateConfiguration()
        {
            return ValidateConfiguration(_configuration);
        }

        public string GetConfigurationValue(string key)
        {
            return string.Empty;
        }

        public Dictionary<string, string> GetAllConfigurationValues()
        {
            return new Dictionary<string, string>();
        }

        public bool StartService(string serviceName)
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isRunning)
            {
                SetError("Application is not running");
                return false;
            }

            _logger.Log($"Starting service: {serviceName}", LogLevel.Info);
            return true;
        }

        public bool StopService(string serviceName)
        {
            if (_isDisposed)
            {
                SetError("Application is disposed");
                return false;
            }

            if (!_isRunning)
            {
                SetError("Application is not running");
                return false;
            }

            _logger.Log($"Stopping service: {serviceName}", LogLevel.Info);
            return true;
        }

        public bool IsServiceRunning(string serviceName)
        {
            return _isRunning;
        }

        public List<string> GetAvailableServices()
        {
            return new List<string> { "Database", "WebServer", "MessageQueue" };
        }

        public bool SetLogLevel(string level)
        {
            if (Enum.TryParse<LogLevel>(level, true, out var logLevel))
            {
                _currentLogLevel = logLevel;
                _logger.SetLogLevel(logLevel);
                return true;
            }
            return false;
        }

        public string GetLogLevel()
        {
            return _currentLogLevel.ToString().ToUpper();
        }

        public List<string> GetRecentLogs(int count)
        {
            if (count <= 0)
                return new List<string>();

            return _logs.Count <= count ? new List<string>(_logs) : _logs.GetRange(_logs.Count - count, count);
        }

        public bool ClearLogs()
        {
            _logs.Clear();
            return true;
        }

        public bool ClearErrorState()
        {
            ClearError();
            return true;
        }

        public List<string> GetErrorHistory()
        {
            return new List<string>(_errorHistory);
        }

        public async Task<bool> StartServiceAsync(string serviceName)
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                return false;
            }

            if (_isRunning)
            {
                _lastError = "Application already running";
                return false;
            }

            try
            {
                _isRunning = true;
                LogMessage($"Application service {serviceName} started", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to start service {serviceName}: {ex.Message}";
                _isRunning = false;
                return false;
            }
        }

        public async Task<bool> StopServiceAsync(string serviceName)
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                return false;
            }

            try
            {
                _isRunning = false;
                LogMessage($"Application service {serviceName} stopped", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to stop service {serviceName}: {ex.Message}";
                return false;
            }
        }
    }
} 