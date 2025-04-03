using System;
using System.Diagnostics;
using TestFramework.Core.Logger;
using TestFramework.Core.Models;

namespace TestFramework.Core
{
    /// <summary>
    /// Base class for all test cases providing common functionality
    /// </summary>
    public abstract class TestBase
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the TestBase class
        /// </summary>
        /// <param name="loggerType">Type of logger to use</param>
        protected TestBase(LoggerType loggerType = LoggerType.Console)
        {
            _logger = LoggerFactory.CreateLogger(loggerType);
        }

        /// <summary>
        /// Gets the logger instance
        /// </summary>
        protected ILogger Logger => _logger;

        /// <summary>
        /// Executes the test and returns the result
        /// </summary>
        /// <returns>Test result</returns>
        public TestResult Execute()
        {
            var testName = GetType().Name;
            _logger.Log($"Starting test: {testName}");
            
            var result = new TestResult
            {
                Status = TestStatus.Failed,
                Message = "Test failed to complete"
            };

            try
            {
                _stopwatch.Start();
                
                // Run setup
                Setup();
                
                // Run test implementation
                _logger.Log("Executing test logic...");
                RunTest();
                
                // If we get here without exceptions, test passed
                result.Status = TestStatus.Passed;
                result.Message = "Test completed successfully";
            }
            catch (Exception ex)
            {
                _logger.Log($"Exception occurred: {ex.Message}");
                result.Status = TestStatus.Failed;
                result.Message = ex.Message;
                result.Exception = ex;
            }
            finally
            {
                try
                {
                    // Always run teardown
                    TearDown();
                }
                catch (Exception ex)
                {
                    _logger.Log($"Exception in teardown: {ex.Message}");
                }
                
                _stopwatch.Stop();
                result.ExecutionTimeMs = _stopwatch.ElapsedMilliseconds;
                
                _logger.Log($"Test {testName} completed with status: {result.Status}");
                _logger.Log($"Execution time: {result.ExecutionTimeMs}ms");
            }

            return result;
        }

        /// <summary>
        /// Setup method called before test execution
        /// </summary>
        protected virtual void Setup()
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Abstract method containing the test implementation
        /// </summary>
        protected abstract void RunTest();

        /// <summary>
        /// Teardown method called after test execution
        /// </summary>
        protected virtual void TearDown()
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Asserts that a condition is true
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="message">Message to include if assertion fails</param>
        protected void Assert(bool condition, string message = "Assertion failed")
        {
            if (!condition)
            {
                throw new AssertionException(message);
            }
        }

        /// <summary>
        /// Asserts that a condition is true
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="message">Message to include if assertion fails</param>
        protected void AssertTrue(bool condition, string message = "Expected condition to be true")
        {
            Assert(condition, message);
        }

        /// <summary>
        /// Asserts that a condition is false
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="message">Message to include if assertion fails</param>
        protected void AssertFalse(bool condition, string message = "Expected condition to be false")
        {
            Assert(!condition, message);
        }

        /// <summary>
        /// Asserts that two values are equal
        /// </summary>
        /// <param name="expected">Expected value</param>
        /// <param name="actual">Actual value</param>
        /// <param name="message">Message to include if assertion fails</param>
        protected void AssertEqual<T>(T expected, T actual, string? message = null)
        {
            if (!Equals(expected, actual))
            {
                throw new AssertionException(
                    message ?? $"Expected '{expected}' but found '{actual}'");
            }
        }
    }

    /// <summary>
    /// Exception thrown when an assertion fails
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AssertionException class
        /// </summary>
        /// <param name="message">Exception message</param>
        public AssertionException(string message) : base(message)
        {
        }
    }
} 