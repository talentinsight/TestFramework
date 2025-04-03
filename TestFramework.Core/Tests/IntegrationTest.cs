using System;
using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents an integration test
    /// </summary>
    public class IntegrationTest : BaseTest
    {
        private readonly Func<Task<bool>> _testAction;
        private readonly Func<Task>? _setupAction;
        private readonly Func<Task>? _cleanupAction;
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Initializes a new instance of the IntegrationTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="testAction">Test action to execute</param>
        /// <param name="priority">Test priority</param>
        /// <param name="timeout">Test timeout</param>
        /// <param name="setupAction">Setup action to execute before the test</param>
        /// <param name="cleanupAction">Cleanup action to execute after the test</param>
        public IntegrationTest(
            string name,
            string description,
            Func<Task<bool>> testAction,
            TestPriority priority = TestPriority.Medium,
            TimeSpan? timeout = null,
            Func<Task>? setupAction = null,
            Func<Task>? cleanupAction = null)
            : base(name, description, TestCategory.Integration, priority)
        {
            _testAction = testAction ?? throw new ArgumentNullException(nameof(testAction));
            _setupAction = setupAction;
            _cleanupAction = cleanupAction;
            _timeout = timeout ?? TimeSpan.FromMinutes(5);
        }

        /// <inheritdoc />
        public override async Task<TestResult> ExecuteAsync()
        {
            try
            {
                var startTime = DateTime.Now;
                
                using var cts = new System.Threading.CancellationTokenSource(_timeout);
                var testTask = _testAction();
                
                bool success;
                try
                {
                    success = await testTask.WaitAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return CreateResult(
                        TestStatus.Failed,
                        $"Test timed out after {_timeout.TotalSeconds} seconds",
                        executionTimeMs: (long)_timeout.TotalMilliseconds
                    );
                }

                var executionTime = (long)(DateTime.Now - startTime).TotalMilliseconds;

                return CreateResult(
                    success ? TestStatus.Passed : TestStatus.Failed,
                    success ? "Test passed successfully" : "Test failed",
                    executionTimeMs: executionTime
                );
            }
            catch (Exception ex)
            {
                return CreateResult(
                    TestStatus.Failed,
                    "Test failed with exception",
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
    }
} 