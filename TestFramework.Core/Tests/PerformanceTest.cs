using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents a performance test
    /// </summary>
    public class PerformanceTest : BaseTest
    {
        private readonly Func<Task> _testAction;
        private readonly Func<Task>? _setupAction;
        private readonly Func<Task>? _cleanupAction;
        private readonly int _iterations;
        private readonly TimeSpan _maxExecutionTime;
        private readonly TimeSpan _warmupDuration;

        /// <summary>
        /// Initializes a new instance of the PerformanceTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="testAction">Test action to execute</param>
        /// <param name="iterations">Number of test iterations</param>
        /// <param name="maxExecutionTime">Maximum execution time per iteration</param>
        /// <param name="warmupDuration">Warmup duration before actual test</param>
        /// <param name="priority">Test priority</param>
        /// <param name="setupAction">Setup action to execute before the test</param>
        /// <param name="cleanupAction">Cleanup action to execute after the test</param>
        public PerformanceTest(
            string name,
            string description,
            Func<Task> testAction,
            int iterations = 1000,
            TimeSpan? maxExecutionTime = null,
            TimeSpan? warmupDuration = null,
            TestPriority priority = TestPriority.Medium,
            Func<Task>? setupAction = null,
            Func<Task>? cleanupAction = null)
            : base(name, description, TestCategory.Performance, priority)
        {
            _testAction = testAction ?? throw new ArgumentNullException(nameof(testAction));
            _setupAction = setupAction;
            _cleanupAction = cleanupAction;
            _iterations = iterations;
            _maxExecutionTime = maxExecutionTime ?? TimeSpan.FromSeconds(1);
            _warmupDuration = warmupDuration ?? TimeSpan.FromSeconds(2);
        }

        /// <inheritdoc />
        public override async Task<TestResult> ExecuteAsync()
        {
            try
            {
                // Warmup
                var warmupWatch = Stopwatch.StartNew();
                while (warmupWatch.Elapsed < _warmupDuration)
                {
                    await _testAction();
                }
                warmupWatch.Stop();

                // Actual test
                var executionTimes = new List<long>();
                var startTime = DateTime.Now;

                for (int i = 0; i < _iterations; i++)
                {
                    var iterationWatch = Stopwatch.StartNew();
                    await _testAction();
                    iterationWatch.Stop();
                    executionTimes.Add(iterationWatch.ElapsedMilliseconds);

                    if (iterationWatch.Elapsed > _maxExecutionTime)
                    {
                        return CreateResult(
                            TestStatus.Failed,
                            $"Iteration {i + 1} exceeded maximum execution time of {_maxExecutionTime.TotalMilliseconds}ms (took {iterationWatch.ElapsedMilliseconds}ms)",
                            executionTimeMs: (long)(DateTime.Now - startTime).TotalMilliseconds
                        );
                    }
                }

                var totalTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                var averageTime = executionTimes.Average();
                var minTime = executionTimes.Min();
                var maxTime = executionTimes.Max();
                var p95Time = CalculatePercentile(executionTimes, 95);
                var p99Time = CalculatePercentile(executionTimes, 99);

                var message = $@"Performance test results:
Average execution time: {averageTime:F2}ms
Min execution time: {minTime}ms
Max execution time: {maxTime}ms
95th percentile: {p95Time:F2}ms
99th percentile: {p99Time:F2}ms
Total iterations: {_iterations}
Total time: {totalTime}ms";

                return CreateResult(
                    TestStatus.Passed,
                    message,
                    executionTimeMs: totalTime
                );
            }
            catch (Exception ex)
            {
                return CreateResult(
                    TestStatus.Failed,
                    "Performance test failed with exception",
                    ex
                );
            }
        }

        /// <inheritdoc />
        public override async Task SetupAsync()
        {
            if (_setupAction != null)
            {
                await _setupAction();
            }
        }

        /// <inheritdoc />
        public override async Task CleanupAsync()
        {
            if (_cleanupAction != null)
            {
                await _cleanupAction();
            }
        }

        private static double CalculatePercentile(List<long> values, int percentile)
        {
            var sorted = values.OrderBy(x => x).ToList();
            var index = (percentile / 100.0) * (sorted.Count - 1);
            var lower = (int)Math.Floor(index);
            var upper = (int)Math.Ceiling(index);

            if (lower == upper)
            {
                return sorted[lower];
            }

            var weight = index - lower;
            return (1 - weight) * sorted[lower] + weight * sorted[upper];
        }
    }
} 