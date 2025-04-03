using System;
using System.Threading.Tasks;
using TestFramework.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Base class for all test types
    /// </summary>
    public abstract class BaseTest : ITest
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public TestCategory Category { get; }

        /// <inheritdoc />
        public TestPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the BaseTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="category">Test category</param>
        /// <param name="priority">Test priority</param>
        protected BaseTest(string name, string description, TestCategory category, TestPriority priority)
        {
            Name = name;
            Description = description;
            Category = category;
            Priority = priority;
        }

        /// <inheritdoc />
        public abstract Task<TestFramework.Core.Models.TestResult> ExecuteAsync();

        /// <inheritdoc />
        public virtual Task SetupAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task CleanupAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a test result
        /// </summary>
        /// <param name="status">Test status</param>
        /// <param name="message">Test message</param>
        /// <param name="exception">Test exception</param>
        /// <param name="executionTimeMs">Test execution time in milliseconds</param>
        /// <param name="screenshot">Test screenshot</param>
        /// <returns>Test result</returns>
        protected TestFramework.Core.Models.TestResult CreateResult(TestStatus status, string message = "", Exception? exception = null, long executionTimeMs = 0, string screenshot = "")
        {
            return new TestFramework.Core.Models.TestResult
            {
                TestName = Name,
                Category = Category,
                Priority = Priority,
                Status = status,
                IsSuccess = status == TestStatus.Passed,
                Message = message,
                ErrorMessage = exception?.Message ?? "",
                ExecutionTimeMs = executionTimeMs,
                Exception = exception,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                StackTrace = exception?.StackTrace ?? "",
                Screenshot = screenshot
            };
        }

        /// <summary>
        /// Creates a test result (legacy method)
        /// </summary>
        /// <param name="isSuccess">Test success status</param>
        /// <param name="errorMessage">Test error message</param>
        /// <param name="stackTrace">Test stack trace</param>
        /// <param name="screenshot">Test screenshot</param>
        /// <returns>Test result</returns>
        protected TestFramework.Core.Models.TestResult CreateResult(bool isSuccess, string errorMessage = "", string stackTrace = "", string screenshot = "")
        {
            var result = new TestFramework.Core.Models.TestResult
            {
                TestName = Name,
                Category = Category,
                Priority = Priority,
                Status = isSuccess ? TestStatus.Passed : TestStatus.Failed,
                IsSuccess = isSuccess,
                Message = isSuccess ? "Test passed successfully" : "Test failed",
                ErrorMessage = errorMessage,
                StackTrace = stackTrace,
                Screenshot = screenshot,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };

            return result;
        }
    }
} 