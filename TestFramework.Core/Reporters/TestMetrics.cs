using System;
using System.Collections.Generic;
using System.Linq;
using TestFramework.Core.Models;

namespace TestFramework.Core.Reporters
{
    /// <summary>
    /// Collects and analyzes test execution metrics
    /// </summary>
    public class TestMetrics
    {
        private readonly List<TestResult> _results;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;

        /// <summary>
        /// Initializes a new instance of the TestMetrics class
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <param name="startTime">Test execution start time</param>
        /// <param name="endTime">Test execution end time</param>
        public TestMetrics(IEnumerable<TestResult> results, DateTime startTime, DateTime endTime)
        {
            _results = results.ToList();
            _startTime = startTime;
            _endTime = endTime;
        }

        /// <summary>
        /// Gets the total number of tests
        /// </summary>
        public int TotalTests => _results.Count;

        /// <summary>
        /// Gets the number of passed tests
        /// </summary>
        public int PassedTests => _results.Count(r => r.Status == TestStatus.Passed);

        /// <summary>
        /// Gets the number of failed tests
        /// </summary>
        public int FailedTests => _results.Count(r => r.Status == TestStatus.Failed);

        /// <summary>
        /// Gets the number of skipped tests
        /// </summary>
        public int SkippedTests => _results.Count(r => r.Status == TestStatus.Skipped);

        /// <summary>
        /// Gets the test pass rate
        /// </summary>
        public double PassRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;

        /// <summary>
        /// Gets the total execution time in milliseconds
        /// </summary>
        public long TotalExecutionTimeMs => _results.Sum(r => r.ExecutionTimeMs);

        /// <summary>
        /// Gets the average execution time in milliseconds
        /// </summary>
        public double AverageExecutionTimeMs => TotalTests > 0 ? (double)TotalExecutionTimeMs / TotalTests : 0;

        /// <summary>
        /// Gets the slowest test execution time in milliseconds
        /// </summary>
        public long SlowestTestTimeMs => _results.Max(r => r.ExecutionTimeMs);

        /// <summary>
        /// Gets the fastest test execution time in milliseconds
        /// </summary>
        public long FastestTestTimeMs => _results.Min(r => r.ExecutionTimeMs);

        /// <summary>
        /// Gets the test metrics by category
        /// </summary>
        public IDictionary<TestCategory, CategoryMetrics> MetricsByCategory
        {
            get
            {
                var metrics = new Dictionary<TestCategory, CategoryMetrics>();
                
                foreach (var category in Enum.GetValues<TestCategory>())
                {
                    var categoryResults = _results.Where(r => r.Category == category).ToList();
                    if (categoryResults.Any())
                    {
                        metrics[category] = new CategoryMetrics(categoryResults);
                    }
                }
                
                return metrics;
            }
        }

        /// <summary>
        /// Gets the test metrics by priority
        /// </summary>
        public IDictionary<TestPriority, PriorityMetrics> MetricsByPriority
        {
            get
            {
                var metrics = new Dictionary<TestPriority, PriorityMetrics>();
                
                foreach (var priority in Enum.GetValues<TestPriority>())
                {
                    var priorityResults = _results.Where(r => r.Priority == priority).ToList();
                    if (priorityResults.Any())
                    {
                        metrics[priority] = new PriorityMetrics(priorityResults);
                    }
                }
                
                return metrics;
            }
        }

        /// <summary>
        /// Gets the overall test execution duration
        /// </summary>
        public TimeSpan TotalDuration => _endTime - _startTime;

        /// <summary>
        /// Gets the tests with the longest execution time
        /// </summary>
        /// <param name="count">Number of tests to return</param>
        /// <returns>Collection of slowest tests</returns>
        public IEnumerable<TestResult> GetSlowestTests(int count = 5)
        {
            return _results.OrderByDescending(r => r.ExecutionTimeMs).Take(count);
        }

        /// <summary>
        /// Gets the tests with the shortest execution time
        /// </summary>
        /// <param name="count">Number of tests to return</param>
        /// <returns>Collection of fastest tests</returns>
        public IEnumerable<TestResult> GetFastestTests(int count = 5)
        {
            return _results.OrderBy(r => r.ExecutionTimeMs).Take(count);
        }

        /// <summary>
        /// Gets the failed tests
        /// </summary>
        /// <returns>Collection of failed tests</returns>
        public IEnumerable<TestResult> GetFailedTests()
        {
            return _results.Where(r => r.Status == TestStatus.Failed);
        }
    }

    /// <summary>
    /// Represents metrics for a test category
    /// </summary>
    public class CategoryMetrics
    {
        private readonly List<TestResult> _results;

        /// <summary>
        /// Initializes a new instance of the CategoryMetrics class
        /// </summary>
        /// <param name="results">Collection of test results for the category</param>
        public CategoryMetrics(List<TestResult> results)
        {
            _results = results;
        }

        /// <summary>
        /// Gets the total number of tests in the category
        /// </summary>
        public int TotalTests => _results.Count;

        /// <summary>
        /// Gets the number of passed tests in the category
        /// </summary>
        public int PassedTests => _results.Count(r => r.Status == TestStatus.Passed);

        /// <summary>
        /// Gets the number of failed tests in the category
        /// </summary>
        public int FailedTests => _results.Count(r => r.Status == TestStatus.Failed);

        /// <summary>
        /// Gets the number of skipped tests in the category
        /// </summary>
        public int SkippedTests => _results.Count(r => r.Status == TestStatus.Skipped);

        /// <summary>
        /// Gets the pass rate for the category
        /// </summary>
        public double PassRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;

        /// <summary>
        /// Gets the total execution time in milliseconds for the category
        /// </summary>
        public long TotalExecutionTimeMs => _results.Sum(r => r.ExecutionTimeMs);

        /// <summary>
        /// Gets the average execution time in milliseconds for the category
        /// </summary>
        public double AverageExecutionTimeMs => TotalTests > 0 ? (double)TotalExecutionTimeMs / TotalTests : 0;
    }

    /// <summary>
    /// Represents metrics for a test priority
    /// </summary>
    public class PriorityMetrics
    {
        private readonly List<TestResult> _results;

        /// <summary>
        /// Initializes a new instance of the PriorityMetrics class
        /// </summary>
        /// <param name="results">Collection of test results for the priority</param>
        public PriorityMetrics(List<TestResult> results)
        {
            _results = results;
        }

        /// <summary>
        /// Gets the total number of tests with the priority
        /// </summary>
        public int TotalTests => _results.Count;

        /// <summary>
        /// Gets the number of passed tests with the priority
        /// </summary>
        public int PassedTests => _results.Count(r => r.Status == TestStatus.Passed);

        /// <summary>
        /// Gets the number of failed tests with the priority
        /// </summary>
        public int FailedTests => _results.Count(r => r.Status == TestStatus.Failed);

        /// <summary>
        /// Gets the number of skipped tests with the priority
        /// </summary>
        public int SkippedTests => _results.Count(r => r.Status == TestStatus.Skipped);

        /// <summary>
        /// Gets the pass rate for the priority
        /// </summary>
        public double PassRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;

        /// <summary>
        /// Gets the total execution time in milliseconds for the priority
        /// </summary>
        public long TotalExecutionTimeMs => _results.Sum(r => r.ExecutionTimeMs);

        /// <summary>
        /// Gets the average execution time in milliseconds for the priority
        /// </summary>
        public double AverageExecutionTimeMs => TotalTests > 0 ? (double)TotalExecutionTimeMs / TotalTests : 0;
    }
} 