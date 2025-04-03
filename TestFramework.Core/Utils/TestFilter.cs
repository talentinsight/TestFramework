using System.Collections.Generic;
using System.Linq;
using TestFramework.Core.Models;

namespace TestFramework.Core.Utils
{
    /// <summary>
    /// Utility class for filtering and grouping test results
    /// </summary>
    public static class TestFilter
    {
        /// <summary>
        /// Filters test results by category
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <param name="category">Category to filter by</param>
        /// <returns>Filtered test results</returns>
        public static IEnumerable<TestResult> FilterByCategory(IEnumerable<TestResult> results, TestCategory category)
        {
            return results.Where(r => r.Category == category);
        }

        /// <summary>
        /// Filters test results by priority
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <param name="priority">Priority to filter by</param>
        /// <returns>Filtered test results</returns>
        public static IEnumerable<TestResult> FilterByPriority(IEnumerable<TestResult> results, TestPriority priority)
        {
            return results.Where(r => r.Priority == priority);
        }

        /// <summary>
        /// Filters test results by status
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <param name="status">Status to filter by</param>
        /// <returns>Filtered test results</returns>
        public static IEnumerable<TestResult> FilterByStatus(IEnumerable<TestResult> results, TestStatus status)
        {
            return results.Where(r => r.Status == status);
        }

        /// <summary>
        /// Groups test results by category
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <returns>Grouped test results</returns>
        public static IDictionary<TestCategory, IEnumerable<TestResult>> GroupByCategory(IEnumerable<TestResult> results)
        {
            return results.GroupBy(r => r.Category)
                         .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        /// <summary>
        /// Groups test results by priority
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <returns>Grouped test results</returns>
        public static IDictionary<TestPriority, IEnumerable<TestResult>> GroupByPriority(IEnumerable<TestResult> results)
        {
            return results.GroupBy(r => r.Priority)
                         .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        /// <summary>
        /// Groups test results by status
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <returns>Grouped test results</returns>
        public static IDictionary<TestStatus, IEnumerable<TestResult>> GroupByStatus(IEnumerable<TestResult> results)
        {
            return results.GroupBy(r => r.Status)
                         .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        /// <summary>
        /// Orders test results by priority
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <returns>Ordered test results</returns>
        public static IEnumerable<TestResult> OrderByPriority(IEnumerable<TestResult> results)
        {
            return results.OrderBy(r => r.Priority);
        }

        /// <summary>
        /// Orders test results by execution time
        /// </summary>
        /// <param name="results">Collection of test results</param>
        /// <returns>Ordered test results</returns>
        public static IEnumerable<TestResult> OrderByExecutionTime(IEnumerable<TestResult> results)
        {
            return results.OrderByDescending(r => r.ExecutionTimeMs);
        }
    }
} 