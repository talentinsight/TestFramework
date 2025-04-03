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
        public string TestName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the test status
        /// </summary>
        public TestStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the test category
        /// </summary>
        public TestCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the test priority
        /// </summary>
        public TestPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the result message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the execution time in milliseconds
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        /// <summary>
        /// Gets or sets any exception that occurred during test execution
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the start time of the test
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the test
        /// </summary>
        public DateTime EndTime { get; set; }
    }
} 