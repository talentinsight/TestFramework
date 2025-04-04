using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Logger;
using TestFramework.Core.Models;
using TestFramework.Core.Tests;
using TestFramework.Tests.Tests;

namespace TestFramework.Tests.Runners
{
    public class TestRunner : ITestRunner
    {
        private readonly ILogger _logger;

        public TestRunner(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<TestResult> RunTestAsync(ITest test)
        {
            if (test == null)
            {
                _logger.Log("Test is null", LogLevel.Error);
                return new TestResult(TestType.Unit, "Unknown", DateTime.Now, DateTime.Now, false, "Test is null");
            }

            var startTime = DateTime.Now;
            try
            {
                _logger.Log("Initializing test...", LogLevel.Info);
                if (!await test.InitializeAsync())
                {
                    var endTime = DateTime.Now;
                    _logger.Log("Test initialization failed", LogLevel.Error);
                    return new TestResult(test.TestType, test.Name, startTime, endTime, false, "Test initialization failed");
                }

                _logger.Log("Running test...", LogLevel.Info);
                if (!await test.RunAsync())
                {
                    var endTime = DateTime.Now;
                    _logger.Log("Test execution failed", LogLevel.Error);
                    return new TestResult(test.TestType, test.Name, startTime, endTime, false, "Test execution failed");
                }

                _logger.Log("Stopping test...", LogLevel.Info);
                if (!await test.StopAsync())
                {
                    var endTime = DateTime.Now;
                    _logger.Log("Test stop failed", LogLevel.Error);
                    return new TestResult(test.TestType, test.Name, startTime, endTime, false, "Test stop failed");
                }

                var successEndTime = DateTime.Now;
                _logger.Log("Test completed successfully", LogLevel.Info);
                return new TestResult(test.TestType, test.Name, startTime, successEndTime, true);
            }
            catch (Exception ex)
            {
                var endTime = DateTime.Now;
                _logger.Log($"Test failed with error: {ex.Message}", LogLevel.Error);
                return new TestResult(test.TestType, test.Name, startTime, endTime, false, ex.Message);
            }
        }

        public async Task<IEnumerable<TestResult>> RunTestsAsync(params ITest[] tests)
        {
            if (tests == null || tests.Length == 0)
            {
                _logger.Log("No tests to run", LogLevel.Error);
                return new[] { new TestResult(TestType.Unit, "No Tests", DateTime.Now, DateTime.Now, false, "No tests to run") };
            }

            var results = new List<TestResult>();
            foreach (var test in tests)
            {
                results.Add(await RunTestAsync(test));
            }

            return results;
        }
    }
} 