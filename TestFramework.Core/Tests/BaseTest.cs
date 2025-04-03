using System;
using System.Threading.Tasks;
using TestFramework.Core.Models;

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
        public abstract Task<TestResult> ExecuteAsync();

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
        /// <returns>Test result</returns>
        protected TestResult CreateResult(TestStatus status, string message = "", Exception? exception = null, long executionTimeMs = 0)
        {
            return new TestResult
            {
                TestName = Name,
                Status = status,
                Category = Category,
                Priority = Priority,
                Message = message,
                Exception = exception,
                ExecutionTimeMs = executionTimeMs,
                StartTime = DateTime.Now.AddMilliseconds(-executionTimeMs),
                EndTime = DateTime.Now
            };
        }
    }
} 