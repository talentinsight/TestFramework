using System;
using TestFramework.Core.Tests;

namespace TestFramework.Core.Models
{
    /// <summary>
    /// Represents the result of a test execution
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Gets the test type
        /// </summary>
        public TestType TestType { get; }

        /// <summary>
        /// Gets the test name
        /// </summary>
        public string TestName { get; }

        /// <summary>
        /// Gets the start time of the test
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Gets the end time of the test
        /// </summary>
        public DateTime EndTime { get; }

        /// <summary>
        /// Gets the duration of the test
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets whether the test passed
        /// </summary>
        public bool Passed { get; }

        /// <summary>
        /// Gets the error message if the test failed
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Gets or sets the category of the test
        /// </summary>
        public TestCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the priority of the test
        /// </summary>
        public TestPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the status of the test
        /// </summary>
        public TestStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the test was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the execution time in milliseconds
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        /// <summary>
        /// Gets or sets the stack trace if the test failed
        /// </summary>
        public string? StackTrace { get; }

        /// <summary>
        /// Gets or sets the screenshot path if the test failed
        /// </summary>
        public string? Screenshot { get; set; }

        /// <summary>
        /// Gets or sets the test message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets any exception that occurred during test execution
        /// </summary>
        public Exception? Exception { get; set; }

        public TestResult(TestType testType, string testName, DateTime startTime, DateTime endTime, bool passed, string? errorMessage = null, string? stackTrace = null)
        {
            TestType = testType;
            TestName = testName;
            StartTime = startTime;
            EndTime = endTime;
            Passed = passed;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
        }
    }
} 