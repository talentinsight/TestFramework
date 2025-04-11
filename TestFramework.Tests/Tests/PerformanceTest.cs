using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class PerformanceTest : BaseTest
    {
        private readonly int _durationInSeconds;

        public PerformanceTest(ILogger logger, int durationInSeconds = 10) : base(logger)
        {
            _durationInSeconds = durationInSeconds;
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log($"Running performance test for {_durationInSeconds} seconds", LogLevel.Info);
                await Task.Delay(_durationInSeconds * 1000); // Convert to milliseconds
                _logger.Log("Performance test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Performance test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 