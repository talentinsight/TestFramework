using System.Collections.Generic;
using TestFramework.Core.Models;

namespace TestFramework.Core.Interfaces
{
    /// <summary>
    /// Interface for test reporters that generate test execution reports
    /// </summary>
    public interface ITestReporter
    {
        /// <summary>
        /// Generates a report for a single test result
        /// </summary>
        /// <param name="result">Test result to report</param>
        void ReportTestResult(TestResult result);

        /// <summary>
        /// Generates a report for multiple test results
        /// </summary>
        /// <param name="results">Collection of test results to report</param>
        void ReportTestResults(IEnumerable<TestResult> results);

        /// <summary>
        /// Generates a summary report for test execution
        /// </summary>
        /// <param name="results">Collection of test results</param>
        void ReportTestSummary(IEnumerable<TestResult> results);

        /// <summary>
        /// Saves the report to a file
        /// </summary>
        /// <param name="filePath">Path where to save the report</param>
        void SaveReport(string filePath);
    }
} 