using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Application;

namespace TestFramework.Tests.Application
{
    /// <summary>
    /// Mock implementation of a C++ application for testing purposes
    /// </summary>
    public class MockCppApplication : ICppApplication
    {
        private bool _isRunning;
        private readonly string _version;
        private readonly Dictionary<string, string> _configuration;
        private readonly Dictionary<string, bool> _services;
        private string _logLevel;
        private readonly List<string> _logs;
        private readonly List<string> _errorHistory;
        private string _lastError = string.Empty;
        private bool _isInErrorState;
        private bool _isInitialized;
        
        public MockCppApplication(string version = "1.0.0")
        {
            _version = version;
            _configuration = new Dictionary<string, string>();
            _services = new Dictionary<string, bool>();
            _logs = new List<string>();
            _errorHistory = new List<string>();
            _logLevel = "INFO";
            _isInErrorState = false;
            
            // Initialize some default services
            _services["Database"] = false;
            _services["WebServer"] = false;
            _services["MessageQueue"] = false;
        }

        public bool Initialize()
        {
            _isRunning = true;
            _logs.Add($"[{DateTime.Now}] [INFO] Application initialized");
            return true;
        }

        public void Shutdown()
        {
            _isRunning = false;
            _logs.Add($"[{DateTime.Now}] [INFO] Application shut down");
        }

        public bool IsRunning => _isRunning;

        public string GetApplicationVersion()
        {
            return _version;
        }

        public bool LoadConfiguration(string configPath)
        {
            // Simulate loading configuration
            _configuration.Clear();
            _configuration["LogLevel"] = "INFO";
            _configuration["MaxConnections"] = "100";
            _configuration["Timeout"] = "30";
            _logs.Add($"[{DateTime.Now}] [INFO] Configuration loaded from {configPath}");
            return true;
        }

        public bool ValidateConfiguration()
        {
            // Simulate configuration validation
            bool isValid = _configuration.Count > 0 &&
                   _configuration.ContainsKey("LogLevel") &&
                   _configuration.ContainsKey("MaxConnections") &&
                   _configuration.ContainsKey("Timeout");
            
            _logs.Add($"[{DateTime.Now}] [INFO] Configuration validation: {(isValid ? "passed" : "failed")}");
            return isValid;
        }

        public string GetConfigurationValue(string key)
        {
            return _configuration.TryGetValue(key, out string value) ? value : null;
        }

        public Dictionary<string, string> GetAllConfigurationValues()
        {
            return new Dictionary<string, string>(_configuration);
        }

        // Service Management Implementation
        
        public bool StartService(string serviceName)
        {
            if (!_services.ContainsKey(serviceName))
            {
                _lastError = $"Service '{serviceName}' not found";
                _errorHistory.Add(_lastError);
                _isInErrorState = true;
                _logs.Add($"[{DateTime.Now}] [ERROR] {_lastError}");
                return false;
            }
            
            _services[serviceName] = true;
            _logs.Add($"[{DateTime.Now}] [INFO] Service '{serviceName}' started");
            return true;
        }

        public bool StopService(string serviceName)
        {
            if (!_services.ContainsKey(serviceName))
            {
                _lastError = $"Service '{serviceName}' not found";
                _errorHistory.Add(_lastError);
                _isInErrorState = true;
                _logs.Add($"[{DateTime.Now}] [ERROR] {_lastError}");
                return false;
            }
            
            _services[serviceName] = false;
            _logs.Add($"[{DateTime.Now}] [INFO] Service '{serviceName}' stopped");
            return true;
        }

        public bool IsServiceRunning(string serviceName)
        {
            if (!_services.ContainsKey(serviceName))
            {
                _lastError = $"Service '{serviceName}' not found";
                _errorHistory.Add(_lastError);
                _isInErrorState = true;
                _logs.Add($"[{DateTime.Now}] [ERROR] {_lastError}");
                return false;
            }
            
            return _services[serviceName];
        }

        public List<string> GetAvailableServices()
        {
            return new List<string>(_services.Keys);
        }
        
        // Log Management Implementation
        
        public bool SetLogLevel(string level)
        {
            string[] validLevels = { "DEBUG", "INFO", "WARNING", "ERROR" };
            
            if (Array.IndexOf(validLevels, level) < 0)
            {
                _lastError = $"Invalid log level: {level}";
                _errorHistory.Add(_lastError);
                _isInErrorState = true;
                _logs.Add($"[{DateTime.Now}] [ERROR] {_lastError}");
                return false;
            }
            
            _logLevel = level;
            _logs.Add($"[{DateTime.Now}] [INFO] Log level set to {level}");
            return true;
        }

        public string GetLogLevel()
        {
            return _logLevel;
        }

        public List<string> GetRecentLogs(int count)
        {
            if (count <= 0 || count > _logs.Count)
            {
                count = _logs.Count;
            }
            
            return _logs.GetRange(Math.Max(0, _logs.Count - count), Math.Min(count, _logs.Count));
        }

        public bool ClearLogs()
        {
            _logs.Clear();
            _logs.Add($"[{DateTime.Now}] [INFO] Logs cleared");
            return true;
        }
        
        // Error State Management Implementation
        
        public string GetLastError()
        {
            return _lastError;
        }

        public bool ClearErrorState()
        {
            _lastError = null;
            _isInErrorState = false;
            _logs.Add($"[{DateTime.Now}] [INFO] Error state cleared");
            return true;
        }

        public bool IsInErrorState => _isInErrorState;

        public List<string> GetErrorHistory()
        {
            return new List<string>(_errorHistory);
        }

        // Mock methods for testing specific functionality
        public void SimulateError()
        {
            _lastError = "Simulated application error";
            _errorHistory.Add(_lastError);
            _isInErrorState = true;
            _logs.Add($"[{DateTime.Now}] [ERROR] {_lastError}");
            throw new Exception(_lastError);
        }

        public Task<bool> InitializeAsync()
        {
            _isInitialized = true;
            return Task.FromResult(true);
        }

        public Task<bool> StartAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Application not initialized";
                return Task.FromResult(false);
            }

            _isRunning = true;
            return Task.FromResult(true);
        }

        public Task<bool> StopAsync()
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                return Task.FromResult(false);
            }

            _isRunning = false;
            return Task.FromResult(true);
        }

        public Task<bool> RestartAsync()
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                return Task.FromResult(false);
            }

            _isRunning = false;
            _isRunning = true;
            return Task.FromResult(true);
        }

        public Task<string?> GetStatusAsync()
        {
            if (!_isRunning)
            {
                return Task.FromResult<string?>("Stopped");
            }

            return Task.FromResult<string?>("Running");
        }

        public Task<bool> SendCommandAsync(string command)
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<string?> GetResponseAsync()
        {
            if (!_isRunning)
            {
                _lastError = "Application not running";
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>("OK");
        }

        public void Dispose()
        {
            _isRunning = false;
            _isInitialized = false;
        }
    }
} 