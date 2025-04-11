using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class UiTest : BaseTest
    {
        private readonly string _browserType;

        public UiTest(ILogger logger, string browserType = "chrome") : base(logger)
        {
            _browserType = browserType;
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log($"Running UI test using {_browserType} browser", LogLevel.Info);
                await Task.Delay(300); // Simulate browser interaction
                _logger.Log("UI test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"UI test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 