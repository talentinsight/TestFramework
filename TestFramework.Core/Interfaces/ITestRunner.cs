using System.Collections.Generic;
using TestFramework.Core.Models;

namespace TestFramework.Core.Interfaces
{
    /// <summary>
    /// Interface for test runners that execute tests
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Runs a specific test by name
        /// </summary>
        /// <param name="testName">Name of the test to run</param>
        /// <returns>Test result</returns>
        TestFramework.Core.Models.TestResult RunTest(string testName);

        /// <summary>
        /// Runs all available tests
        /// </summary>
        /// <returns>Collection of test results</returns>
        IEnumerable<TestFramework.Core.Models.TestResult> RunAllTests();
    }
} 