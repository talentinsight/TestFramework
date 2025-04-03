using System;
using TestFramework.Core.Models;

namespace TestFramework.Core.Models
{
    /// <summary>
    /// Represents a test result with status and message
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Gets or sets the test name
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// Gets or sets the test category
        /// </summary>
        public TestCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the test priority
        /// </summary>
        public TestPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets whether the test is successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message
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
        /// Gets or sets the stack trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the screenshot
        /// </summary>
        public string Screenshot { get; set; }

        public TestResult()
        {
            TestName = string.Empty;
            ErrorMessage = string.Empty;
            StackTrace = string.Empty;
            Screenshot = string.Empty;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
        }
    }
} 