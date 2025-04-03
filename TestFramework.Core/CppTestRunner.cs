using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Logger;
using TestFramework.Core.Utils;
using TestFramework.Core.Models;

namespace TestFramework.Core
{
    /// <summary>
    /// Test runner for C++ applications
    /// </summary>
    public class CppTestRunner : ITestRunner, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ProcessHelper _processHelper;
        private string? _executablePath;
        private Dictionary<string, string> _testParameters;
        private List<string> _testNames;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the CppTestRunner class
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="processHelper">Process helper instance</param>
        public CppTestRunner(ILogger logger, ProcessHelper processHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processHelper = processHelper ?? throw new ArgumentNullException(nameof(processHelper));
            _testNames = new List<string>();
            _testParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes the test runner with configuration
        /// </summary>
        /// <param name="config">Configuration dictionary</param>
        public void Initialize(Dictionary<string, string> config)
        {
            _testParameters = config ?? new Dictionary<string, string>();

            if (!_testParameters.TryGetValue("ExecutablePath", out _executablePath))
            {
                throw new InvalidOperationException("ExecutablePath must be specified in configuration");
            }

            if (!File.Exists(_executablePath))
            {
                throw new FileNotFoundException($"Executable not found at path: {_executablePath}");
            }

            // Get list of available tests by running with --list-tests
            if (_processHelper.Start(_executablePath, "--list-tests"))
            {
                _processHelper.WaitForExit();
                _testNames = _processHelper.OutputLines
                    .Where(line => !string.IsNullOrWhiteSpace(line) && line.StartsWith("Test"))
                    .ToList();
            }
            else
            {
                throw new InvalidOperationException("Failed to start the C++ test executable");
            }

            _isInitialized = true;
            _logger.Log($"CppTestRunner initialized with executable: {_executablePath}");
            _logger.Log($"Available tests: {string.Join(", ", _testNames)}");
        }

        /// <summary>
        /// Runs a specific test by name
        /// </summary>
        /// <param name="testName">Name of the test to run</param>
        /// <returns>Test result</returns>
        public TestResult RunTest(string testName)
        {
            EnsureInitialized();

            if (!_testNames.Contains(testName))
            {
                return new TestResult
                {
                    Status = TestStatus.NotApplicable,
                    Message = $"Test '{testName}' not found in available tests"
                };
            }

            _logger.Log($"Running test: {testName}");

            // Start the process with the specific test
            if (!_processHelper.Start(_executablePath!, $"--run-test {testName}"))
            {
                return new TestResult
                {
                    Status = TestStatus.Failed,
                    Message = "Failed to start test process"
                };
            }

            // Wait for the process to complete
            bool completed = _processHelper.WaitForExit(30000); // 30 second timeout

            var result = new TestResult();

            if (!completed)
            {
                _processHelper.Kill();
                result.Status = TestStatus.Failed;
                result.Message = "Test timed out after 30 seconds";
                return result;
            }

            // Check exit code
            if (_processHelper.ExitCode == 0)
            {
                result.Status = TestStatus.Passed;
                result.Message = "Test passed successfully";
            }
            else
            {
                result.Status = TestStatus.Failed;
                result.Message = $"Test failed with exit code {_processHelper.ExitCode}";
            }

            // Include relevant output
            result.Message += $"\nOutput: {_processHelper.StandardOutput}";
            if (!string.IsNullOrEmpty(_processHelper.StandardError))
            {
                result.Message += $"\nErrors: {_processHelper.StandardError}";
            }

            return result;
        }

        /// <summary>
        /// Runs all available tests
        /// </summary>
        /// <returns>Collection of test results</returns>
        public IEnumerable<TestResult> RunAllTests()
        {
            EnsureInitialized();

            var results = new List<TestResult>();

            foreach (var testName in _testNames)
            {
                results.Add(RunTest(testName));
            }

            return results;
        }

        /// <summary>
        /// Cleans up resources after test execution
        /// </summary>
        public void Cleanup()
        {
            _processHelper.Dispose();
        }

        /// <summary>
        /// Disposes the test runner resources
        /// </summary>
        public void Dispose()
        {
            Cleanup();
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("CppTestRunner must be initialized before running tests");
            }
        }
    }
} 