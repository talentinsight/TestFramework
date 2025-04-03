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
        /// <param name="isSuccess">Test success status</param>
        /// <param name="errorMessage">Test error message</param>
        /// <param name="stackTrace">Test stack trace</param>
        /// <param name="screenshot">Test screenshot</param>
        /// <returns>Test result</returns>
        protected TestResult CreateResult(bool isSuccess, string errorMessage = "", string stackTrace = "", string screenshot = "")
        {
            var result = new TestResult
            {
                TestName = Name,
                Category = Category,
                Priority = Priority,
                IsSuccess = isSuccess,
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