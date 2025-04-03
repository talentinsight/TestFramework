using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents a load test
    /// </summary>
    public class LoadTest : BaseTest
    {
        private readonly Func<Task<bool>> _testAction;
        private readonly Func<Task>? _setupAction;
        private readonly Func<Task>? _cleanupAction;
        private readonly int _concurrentUsers;
        private readonly TimeSpan _duration;
        private readonly TimeSpan _rampUpTime;
        private readonly int _maxErrorRate;

        /// <summary>
        /// Initializes a new instance of the LoadTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="testAction">Test action to execute</param>
        /// <param name="concurrentUsers">Number of concurrent users</param>
        /// <param name="duration">Test duration</param>
        /// <param name="rampUpTime">Time to ramp up to full load</param>
        /// <param name="maxErrorRate">Maximum acceptable error rate in percentage</param>
        /// <param name="priority">Test priority</param>
        /// <param name="setupAction">Setup action to execute before the test</param>
        /// <param name="cleanupAction">Cleanup action to execute after the test</param>
        public LoadTest(
            string name,
            string description,
            Func<Task<bool>> testAction,
            int concurrentUsers = 100,
            TimeSpan? duration = null,
            TimeSpan? rampUpTime = null,
            int maxErrorRate = 5,
            TestPriority priority = TestPriority.Medium,
            Func<Task>? setupAction = null,
            Func<Task>? cleanupAction = null)
            : base(name, description, TestCategory.Load, priority)
        {
            _testAction = testAction ?? throw new ArgumentNullException(nameof(testAction));
            _setupAction = setupAction;
            _cleanupAction = cleanupAction;
            _concurrentUsers = concurrentUsers;
            _duration = duration ?? TimeSpan.FromMinutes(5);
            _rampUpTime = rampUpTime ?? TimeSpan.FromSeconds(30);
            _maxErrorRate = maxErrorRate;
        }

        /// <inheritdoc />
        public override async Task<TestResult> ExecuteAsync()
        {
            try
            {
                var results = new ConcurrentBag<(bool Success, long ExecutionTime)>();
                var errors = new ConcurrentBag<Exception>();
                var startTime = DateTime.Now;
                var stopwatch = Stopwatch.StartNew();

                // Calculate how many users to add per second during ramp-up
                var usersPerSecond = _concurrentUsers / _rampUpTime.TotalSeconds;
                var currentUsers = 0;

                while (stopwatch.Elapsed < _duration)
                {
                    // During ramp-up, gradually increase the number of users
                    if (stopwatch.Elapsed < _rampUpTime)
                    {
                        currentUsers = (int)(usersPerSecond * stopwatch.Elapsed.TotalSeconds);
                        currentUsers = Math.Min(currentUsers, _concurrentUsers);
                    }
                    else
                    {
                        currentUsers = _concurrentUsers;
                    }

                    var tasks = new Task[currentUsers];
                    for (int i = 0; i < currentUsers; i++)
                    {
                        tasks[i] = Task.Run(async () =>
                        {
                            try
                            {
                                var iterationWatch = Stopwatch.StartNew();
                                var success = await _testAction();
                                iterationWatch.Stop();
                                results.Add((success, iterationWatch.ElapsedMilliseconds));
                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex);
                            }
                        });
                    }

                    await Task.WhenAll(tasks);
                    await Task.Delay(1000); // Wait 1 second before next batch
                }

                stopwatch.Stop();

                var totalRequests = results.Count;
                var successfulRequests = results.Count(r => r.Success);
                var failedRequests = totalRequests - successfulRequests;
                var errorRate = (double)failedRequests / totalRequests * 100;
                var averageResponseTime = results.Average(r => r.ExecutionTime);
                var maxResponseTime = results.Max(r => r.ExecutionTime);
                var p95ResponseTime = CalculatePercentile(results.Select(r => r.ExecutionTime).ToList(), 95);

                var message = $@"Load test results:
Total requests: {totalRequests}
Successful requests: {successfulRequests}
Failed requests: {failedRequests}
Error rate: {errorRate:F2}%
Average response time: {averageResponseTime:F2}ms
Max response time: {maxResponseTime}ms
95th percentile response time: {p95ResponseTime:F2}ms
Total errors: {errors.Count}
Test duration: {stopwatch.Elapsed.TotalSeconds:F2} seconds";

                if (errors.Any())
                {
                    message += "\n\nError samples:";
                    foreach (var error in errors.Take(5))
                    {
                        message += $"\n- {error.Message}";
                    }
                }

                var status = errorRate <= _maxErrorRate ? TestStatus.Passed : TestStatus.Failed;
                return CreateResult(
                    status,
                    message,
                    executionTimeMs: (long)stopwatch.Elapsed.TotalMilliseconds
                );
            }
            catch (Exception ex)
            {
                return CreateResult(
                    TestStatus.Failed,
                    "Load test failed with exception",
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

        private static double CalculatePercentile(System.Collections.Generic.List<long> values, int percentile)
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