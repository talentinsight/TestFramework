using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class UnitTest : BaseTest
    {
        public UnitTest(ILogger logger) : base(logger)
        {
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log("Running unit test", LogLevel.Info);
                await Task.Delay(100); // Simulate test execution
                _logger.Log("Unit test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Unit test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 