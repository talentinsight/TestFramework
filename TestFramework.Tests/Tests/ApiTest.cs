using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class ApiTest : BaseTest
    {
        private readonly string _endpoint;

        public ApiTest(ILogger logger, string endpoint = "http://localhost:8080") : base(logger)
        {
            _endpoint = endpoint;
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log($"Running API test against endpoint: {_endpoint}", LogLevel.Info);
                await Task.Delay(150); // Simulate API call
                _logger.Log("API test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"API test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 