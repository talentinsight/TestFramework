using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Represents a C++ application that can be controlled and monitored
    /// </summary>
    public class CppApplication : IDisposable
    {
        private bool _isRunning;
        private bool _disposed;
        private readonly Dictionary<string, bool> _services = new Dictionary<string, bool>();
        private ILogger _logger;

        /// <summary>
        /// Gets the application configuration
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Gets the application logger
        /// </summary>
        public ILogger Logger { get => _logger; set => _logger = value ?? throw new ArgumentNullException(nameof(value)); }

        /// <summary>
        /// Gets whether the application is currently running
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Initializes a new instance of the CppApplication class
        /// </summary>
        public CppApplication()
        {
            Configuration = new Configuration("app.config");
            _logger = new FileLogger(Path.Combine(Path.GetTempPath(), "app_log.txt"));
            _services["Database"] = false;
            _services["Network"] = false;
            _services["WebServer"] = false;
            _services["Cache"] = false;
        }

        /// <summary>
        /// Starts the application
        /// </summary>
        public async Task StartAsync()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Application is already running");
            }

            if (Configuration.GetValue("InvalidKey") == "InvalidValue")
            {
                throw new InvalidOperationException("Invalid configuration");
            }

            _isRunning = true;
            _logger.Log(LogLevel.Info, "Application started");
        }

        /// <summary>
        /// Stops the application
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Application is not running");
            }

            foreach (var service in _services.Keys)
            {
                if (_services[service])
                {
                    await StopServiceAsync(service);
                }
            }

            _isRunning = false;
            _logger.Log(LogLevel.Info, "Application stopped");
        }

        /// <summary>
        /// Restarts the application
        /// </summary>
        public async Task RestartAsync()
        {
            await StopAsync();
            await StartAsync();
        }

        /// <summary>
        /// Starts a service
        /// </summary>
        /// <param name="serviceName">The name of the service to start</param>
        public async Task StartServiceAsync(string serviceName)
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Application is not running");
            }

            if (!_services.ContainsKey(serviceName))
            {
                throw new InvalidOperationException($"Service '{serviceName}' does not exist");
            }

            if (_services[serviceName])
            {
                throw new InvalidOperationException($"Service '{serviceName}' is already running");
            }

            _services[serviceName] = true;
            _logger.Log(LogLevel.Info, $"Service '{serviceName}' started");
        }

        /// <summary>
        /// Stops a service
        /// </summary>
        /// <param name="serviceName">The name of the service to stop</param>
        public async Task StopServiceAsync(string serviceName)
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Application is not running");
            }

            if (!_services.ContainsKey(serviceName))
            {
                throw new InvalidOperationException($"Service '{serviceName}' does not exist");
            }

            if (!_services[serviceName])
            {
                throw new InvalidOperationException($"Service '{serviceName}' is not running");
            }

            _services[serviceName] = false;
            _logger.Log(LogLevel.Info, $"Service '{serviceName}' stopped");
        }

        /// <summary>
        /// Restarts a service
        /// </summary>
        /// <param name="serviceName">The name of the service to restart</param>
        public async Task RestartServiceAsync(string serviceName)
        {
            await StopServiceAsync(serviceName);
            await StartServiceAsync(serviceName);
        }

        /// <summary>
        /// Checks if a service is running
        /// </summary>
        /// <param name="serviceName">The name of the service to check</param>
        /// <returns>True if the service is running, false otherwise</returns>
        public bool IsServiceRunning(string serviceName)
        {
            return _services.TryGetValue(serviceName, out bool isRunning) && isRunning;
        }

        /// <summary>
        /// Executes an invalid operation
        /// </summary>
        public void ExecuteInvalidOperation()
        {
            throw new InvalidOperationException("Invalid operation executed");
        }

        /// <summary>
        /// Executes a recoverable error
        /// </summary>
        public void ExecuteRecoverableError()
        {
            _logger.Log(LogLevel.Warning, "Recoverable error occurred");
        }

        /// <summary>
        /// Executes a non-recoverable error
        /// </summary>
        public async Task ExecuteNonRecoverableErrorAsync()
        {
            await Task.Run(() =>
            {
                _isRunning = false;
                throw new InvalidOperationException("Non-recoverable error occurred");
            });
        }

        /// <summary>
        /// Executes an invalid operation asynchronously
        /// </summary>
        public async Task ExecuteInvalidOperationAsync()
        {
            await Task.Run(() =>
            {
                throw new InvalidOperationException("Invalid operation executed");
            });
        }

        /// <summary>
        /// Executes a recoverable error asynchronously
        /// </summary>
        public async Task ExecuteRecoverableErrorAsync()
        {
            await Task.Run(() =>
            {
                _logger.Log(LogLevel.Warning, "Recoverable error occurred");
                throw new InvalidOperationException("Recoverable error occurred");
            });
        }

        /// <summary>
        /// Disposes the application
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the application
        /// </summary>
        /// <param name="disposing">True if called from Dispose, false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_isRunning)
                    {
                        StopAsync().GetAwaiter().GetResult();
                    }
                }
                _disposed = true;
            }
        }
    }
} 