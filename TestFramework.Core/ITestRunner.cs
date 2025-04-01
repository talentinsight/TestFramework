using System;
using System.Collections.Generic;

namespace TestFramework.Core
{
    /// <summary>
    /// Represents a test result with status and message
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Gets or sets the test status
        /// </summary>
        public TestStatus Status { get; set; }

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
    }

    /// <summary>
    /// Enum representing test execution status
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// Test passed successfully
        /// </summary>
        Passed,

        /// <summary>
        /// Test failed
        /// </summary>
        Failed,

        /// <summary>
        /// Test was skipped
        /// </summary>
        Skipped,

        /// <summary>
        /// Test is not applicable in current environment
        /// </summary>
        NotApplicable
    }

    /// <summary>
    /// Interface for test runners that execute tests against C++ applications
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Initializes the test runner
        /// </summary>
        /// <param name="config">Configuration data</param>
        void Initialize(Dictionary<string, string> config);

        /// <summary>
        /// Runs a specific test by name
        /// </summary>
        /// <param name="testName">Name of the test to run</param>
        /// <returns>Test result</returns>
        TestResult RunTest(string testName);

        /// <summary>
        /// Runs all available tests
        /// </summary>
        /// <returns>Collection of test results</returns>
        IEnumerable<TestResult> RunAllTests();

        /// <summary>
        /// Cleans up resources after test execution
        /// </summary>
        void Cleanup();
    }
} 