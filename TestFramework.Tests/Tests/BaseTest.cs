using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public abstract class BaseTest
    {
        protected readonly ILogger _logger;
        protected bool _isInitialized;
        protected bool _isRunning;
        protected string _lastError;
        protected bool _isInErrorState;

        protected BaseTest(ILogger logger)
        {
            _logger = logger;
            _isInitialized = false;
            _isRunning = false;
            _lastError = string.Empty;
            _isInErrorState = false;
        }

        public virtual async Task<bool> InitializeAsync()
        {
            if (_isInitialized)
            {
                _lastError = "Test already initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                await Task.Delay(100);
                _isInitialized = true;
                _logger.Log("Test initialized", LogLevel.Info);
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

        public virtual async Task<bool> RunAsync()
        {
            if (!_isInitialized)
            {
                _lastError = "Test not initialized";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            if (_isRunning)
            {
                _lastError = "Test already running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                _isRunning = true;
                _logger.Log("Test started", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Run failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }

        public virtual async Task<bool> StopAsync()
        {
            if (!_isRunning)
            {
                _lastError = "Test not running";
                _logger.Log(_lastError, LogLevel.Error);
                return false;
            }

            try
            {
                _isRunning = false;
                _logger.Log("Test stopped", LogLevel.Info);
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

        public virtual void Dispose()
        {
            if (_isRunning)
            {
                StopAsync().Wait();
            }
        }
    }
} 