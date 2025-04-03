using System;
using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents a unit test
    /// </summary>
    public class UnitTest : BaseTest
    {
        private readonly Func<Task<bool>> _testAction;
        private readonly Func<Task>? _setupAction;
        private readonly Func<Task>? _cleanupAction;

        /// <summary>
        /// Initializes a new instance of the UnitTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="testAction">Test action to execute</param>
        /// <param name="priority">Test priority</param>
        /// <param name="setupAction">Setup action to execute before the test</param>
        /// <param name="cleanupAction">Cleanup action to execute after the test</param>
        public UnitTest(
            string name,
            string description,
            Func<Task<bool>> testAction,
            TestPriority priority = TestPriority.Medium,
            Func<Task>? setupAction = null,
            Func<Task>? cleanupAction = null)
            : base(name, description, TestCategory.Unit, priority)
        {
            _testAction = testAction ?? throw new ArgumentNullException(nameof(testAction));
            _setupAction = setupAction;
            _cleanupAction = cleanupAction;
        }

        /// <inheritdoc />
        public override async Task<TestResult> ExecuteAsync()
        {
            try
            {
                var startTime = DateTime.Now;
                var success = await _testAction();
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