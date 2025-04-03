using System;

namespace TestFramework.Core.Models
{
    /// <summary>
    /// Represents the result of a test execution
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Gets or sets the name of the test
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the category of the test
        /// </summary>
        public TestCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the priority of the test
        /// </summary>
        public TestPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the test was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if the test failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the start time of the test
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the test
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets the duration of the test
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets or sets the stack trace if the test failed
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the screenshot path if the test failed
        /// </summary>
        public string? Screenshot { get; set; }

        /// <summary>
        /// Gets or sets the test message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the TestResult class
        /// </summary>
        public TestResult()
        {
            TestName = string.Empty;
            Category = TestCategory.Unit;
            Priority = TestPriority.Medium;
            IsSuccess = false;
            ErrorMessage = string.Empty;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            StackTrace = string.Empty;
            Screenshot = null;
            Message = null;
        }
    }
} 