using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;

namespace TestFramework.Tests.Application
{
    /// <summary>
    /// Mock implementation of a C++ application for testing purposes
    /// </summary>
    public class MockCppApplication : IDisposable, ICppApplication
    {
        private readonly MockLogger _logger;
        private bool _isInitialized;
        private bool _isRunning;
        private string _lastError = string.Empty;
        private TestFramework.Core.Logger.LogLevel _currentLogLevel = TestFramework.Core.Logger.LogLevel.Info;
        private bool _throwExceptionOnNextCall;
        private bool _failNextCall;
        private string? _lastResponse;
        private readonly List<string> _logs = new();
        private readonly string _version;
        private readonly Dictionary<string, string> _configuration;
        private readonly Dictionary<string, bool> _services;
        private string _logLevel;
        private readonly List<string> _errorHistory;
        private byte[]? _modbusResponse;
        private bool _isInErrorState;

        public bool IsInitialized => _isInitialized;
        public bool IsRunning => _isRunning;
        public string LastError => _lastError;
        public bool ThrowExceptionOnNextCall
        {
            get => _throwExceptionOnNextCall;
            set => _throwExceptionOnNextCall = value;
        }
        public bool FailNextCall
        {
            get => _failNextCall;
            set => _failNextCall = value;
        }
        public bool IsInErrorState => _isInErrorState;
        public TestFramework.Core.Logger.LogLevel CurrentLogLevel => _currentLogLevel;

        public MockCppApplication(MockLogger logger, string version = "1.0.0")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _version = version;
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
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                _lastError = "Test exception";
                _logger.Log(_lastError, LogLevel.Error);
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                _lastError = "Configuration load failed";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            _configuration["configPath"] = configPath;
            _logger.Log($"Configuration loaded from {configPath}", LogLevel.Info);
            return true;
        }

        public bool ValidateConfiguration()
        {
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Configuration validation failed");
                return false;
            }

            if (_isRunning)
            {
                SetError("Cannot validate configuration while application is running");
                return false;
            }

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
                return false;
            }

            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                _lastError = "Test exception";
                _logger.Log(_lastError, LogLevel.Error);
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                _lastError = $"Failed to start service {serviceName}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
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

        public bool StopService(string serviceName)
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                _lastError = "Test exception";
                _logger.Log(_lastError, LogLevel.Error);
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                _lastError = $"Failed to stop service {serviceName}";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
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
            if (Enum.TryParse<LogLevel>(level, true, out var logLevel))
            {
                _currentLogLevel = logLevel;
                _logger.SetLogLevel(logLevel);
                _logger.Log($"Log level set to {level}", LogLevel.Info);
                return true;
            }
            else
            {
                SetError($"Invalid log level: {level}");
                return false;
            }
        }

        public string GetLogLevel()
        {
            return _currentLogLevel.ToString().ToUpper();
        }

        public bool ClearLogs()
        {
            _logs.Clear();
            LogToLogger("Logs cleared", LogLevel.Info);
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
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Operation failed");
                return false;
            }

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

        private void SetError(string message)
        {
            _lastError = message;
            _isInErrorState = true;
            _logger.Log(message, LogLevel.Error);
            _errorHistory.Add(message);
        }

        private void ClearError()
        {
            _lastError = string.Empty;
            _isInErrorState = false;
            _errorHistory.Clear();
        }

        public async Task<bool> InitializeAsync()
        {
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Initialization failed");
                return false;
            }

            if (_isInitialized)
            {
                SetError("Application already initialized");
                return false;
            }

            _isInitialized = true;
            _isInErrorState = false;
            _lastError = string.Empty;
            _logger.Log("Application initialized", LogLevel.Info);
            return true;
        }

        public async Task<bool> StartAsync()
        {
            if (!_isInitialized)
            {
                SetError("Application not initialized");
                return false;
            }

            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Start failed");
                return false;
            }

            if (_isRunning)
            {
                SetError("Application already running");
                return false;
            }

            _isRunning = true;
            _isInErrorState = false;
            _lastError = string.Empty;
            _logger.Log("Application started", LogLevel.Info);
            return true;
        }

        public async Task<bool> StopAsync()
        {
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

            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Operation failed");
                return false;
            }

            _isRunning = false;
            _isInErrorState = false;
            _lastError = string.Empty;
            _logger.Log("Application stopped", LogLevel.Info);
            return true;
        }

        public async Task<bool> RestartAsync()
        {
            if (!_isInitialized)
            {
                SetError("Application not initialized");
                return false;
            }

            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Restart failed");
                return false;
            }

            var startResult = await StartAsync();
            if (!startResult)
            {
                return false;
            }

            _isInErrorState = false;
            _lastError = string.Empty;
            _logger.Log("Application restarted", LogLevel.Info);
            return true;
        }

        public string GetStatus()
        {
            if (!_isRunning)
            {
                return "Stopped";
            }
            return "Running";
        }

        public async Task<string?> GetStatusAsync()
        {
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                SetError("Test exception");
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                SetError("Operation failed");
                return null;
            }

            return GetStatus();
        }

        public async Task<bool> SendCommandAsync(string command)
        {
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                _lastError = "Test exception";
                LogToLogger(_lastError, LogLevel.Error);
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                _lastError = "Operation failed";
                LogToLogger(_lastError, LogLevel.Error);
                return false;
            }

            if (!_isRunning)
            {
                _lastError = "Application not running";
                LogToLogger(_lastError, LogLevel.Error);
                return false;
            }

            LogToLogger($"Command sent: {command}", LogLevel.Info);
            _lastResponse = $"Response to: {command}";
            return true;
        }

        public Task<string?> GetResponseAsync()
        {
            if (_throwExceptionOnNextCall)
            {
                _throwExceptionOnNextCall = false;
                _lastError = "Test exception";
                _logger.Log(_lastError, LogLevel.Error);
                throw new Exception(_lastError);
            }

            if (_failNextCall)
            {
                _failNextCall = false;
                _lastError = "Operation failed";
                _logger.Log(_lastError, LogLevel.Error);
                return Task.FromResult<string?>(null);
            }

            if (!_isRunning)
            {
                _lastError = "Application not running";
                _logger.Log(_lastError, LogLevel.Error);
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>("OK");
        }

        public void SetModbusResponse(byte[] response)
        {
            _modbusResponse = response;
            _lastResponse = Convert.ToBase64String(response);

            // Check if this is a Modbus exception response (function code with high bit set)
            if (response.Length >= 2 && (response[1] & 0x80) != 0)
            {
                _isInErrorState = true;
                _lastError = $"Modbus exception: {response[2]}";
                _logger.Log(_lastError, LogLevel.Error);
            }
        }

        public void Dispose()
        {
            if (_isRunning)
            {
                _logger.Log("Application stopped", LogLevel.Info);
            }
            _isRunning = false;
            _isInitialized = false;
        }

        public string? GetResponse()
        {
            return _lastResponse;
        }

        private void LogToLogger(string message, TestFramework.Core.Logger.LogLevel level)
        {
            _logger.Log(message, level);
            if (level == TestFramework.Core.Logger.LogLevel.Error)
            {
                _isInErrorState = true;
                _errorHistory.Add(message);
            }
        }
    }
} 