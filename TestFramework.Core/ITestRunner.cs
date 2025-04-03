using System;
using System.Collections.Generic;
using TestFramework.Core.Models;

namespace TestFramework.Core
{
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