using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class LoadTest : BaseTest
    {
        private readonly int _iterations;

        public LoadTest(ILogger logger, int iterations = 1000) : base(logger)
        {
            _iterations = iterations;
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log($"Running load test with {_iterations} iterations", LogLevel.Info);
                
                for (int i = 0; i < _iterations; i++)
                {
                    if (i % 100 == 0)
                    {
                        _logger.Log($"Completed {i} iterations", LogLevel.Info);
                    }
                    await Task.Delay(10); // Simulate work
                }

                _logger.Log("Load test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Load test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 