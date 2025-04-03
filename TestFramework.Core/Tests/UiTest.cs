using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TestFramework.Core.Models;
using OpenQA.Selenium.Support.Extensions;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents a UI test
    /// </summary>
    public class UiTest : BaseTest
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl;
        private readonly Func<IWebDriver, Task<bool>> _testAction;
        private readonly Func<IWebDriver, Task>? _setupAction;
        private readonly Func<IWebDriver, Task>? _cleanupAction;
        private readonly TimeSpan _timeout;
        private readonly TimeSpan _pollingInterval;

        /// <summary>
        /// Initializes a new instance of the UiTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="driver">Web driver instance</param>
        /// <param name="baseUrl">Base URL for the UI</param>
        /// <param name="testAction">Test action to execute</param>
        /// <param name="timeout">Test timeout</param>
        /// <param name="pollingInterval">Polling interval for wait conditions</param>
        /// <param name="priority">Test priority</param>
        /// <param name="setupAction">Setup action to execute before the test</param>
        /// <param name="cleanupAction">Cleanup action to execute after the test</param>
        public UiTest(
            string name,
            string description,
            IWebDriver driver,
            string baseUrl,
            Func<IWebDriver, Task<bool>> testAction,
            TimeSpan? timeout = null,
            TimeSpan? pollingInterval = null,
            TestPriority priority = TestPriority.Medium,
            Func<IWebDriver, Task>? setupAction = null,
            Func<IWebDriver, Task>? cleanupAction = null)
            : base(name, description, TestCategory.UI, priority)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _baseUrl = baseUrl.TrimEnd('/');
            _testAction = testAction ?? throw new ArgumentNullException(nameof(testAction));
            _setupAction = setupAction;
            _cleanupAction = cleanupAction;
            _timeout = timeout ?? TimeSpan.FromMinutes(2);
            _pollingInterval = pollingInterval ?? TimeSpan.FromSeconds(1);
        }

        /// <inheritdoc />
        public override async Task<TestFramework.Core.Models.TestResult> ExecuteAsync()
        {
            try
            {
                _driver.Manage().Timeouts().PageLoad = _timeout;
                _driver.Manage().Timeouts().ImplicitWait = _timeout;

                var wait = new WebDriverWait(_driver, _timeout)
                {
                    PollingInterval = _pollingInterval
                };

                var startTime = DateTime.Now;
                var success = await _testAction(_driver);
                var endTime = DateTime.Now;
                var executionTimeMs = (long)(endTime - startTime).TotalMilliseconds;

                var errorMessage = success ? string.Empty : $@"UI test failed:
URL: {_driver.Url}
Title: {_driver.Title}
Browser: {_driver.GetType().Name}";

                string screenshot = string.Empty;
                if (!success)
                {
                    try
                    {
                        var screenshotBytes = _driver.TakeScreenshot().AsByteArray;
                        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var screenshotPath = $"screenshots/{Name}_{timestamp}.png";
                        System.IO.Directory.CreateDirectory("screenshots");
                        System.IO.File.WriteAllBytes(screenshotPath, screenshotBytes);
                        screenshot = screenshotPath;
                    }
                    catch (Exception ex)
                    {
                        errorMessage += $"\nFailed to capture screenshot: {ex.Message}";
                    }
                }

                return CreateResult(
                    success ? TestStatus.Passed : TestStatus.Failed,
                    success ? "Test passed successfully" : errorMessage,
                    executionTimeMs: executionTimeMs,
                    screenshot: screenshot
                );
            }
            catch (Exception ex)
            {
                return CreateResult(
                    TestStatus.Failed,
                    $"Test failed with exception: {ex.Message}",
                    ex
                );
            }
        }

        /// <inheritdoc />
        public override async Task SetupAsync()
        {
            if (_setupAction != null)
            {
                await _setupAction(_driver);
            }
        }

        /// <inheritdoc />
        public override async Task CleanupAsync()
        {
            if (_cleanupAction != null)
            {
                await _cleanupAction(_driver);
            }
        }

        /// <summary>
        /// Waits for an element to be present
        /// </summary>
        /// <param name="by">Element locator</param>
        /// <returns>Web element</returns>
        protected IWebElement WaitForElement(By by)
        {
            var wait = new WebDriverWait(_driver, _timeout)
            {
                PollingInterval = _pollingInterval
            };
            return wait.Until(d => d.FindElement(by));
        }

        /// <summary>
        /// Waits for an element to be clickable
        /// </summary>
        /// <param name="by">Element locator</param>
        /// <returns>Web element</returns>
        protected IWebElement WaitForElementToBeClickable(By by)
        {
            var wait = new WebDriverWait(_driver, _timeout)
            {
                PollingInterval = _pollingInterval
            };
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
        }

        /// <summary>
        /// Waits for an element to be visible
        /// </summary>
        /// <param name="by">Element locator</param>
        /// <returns>Web element</returns>
        protected IWebElement WaitForElementToBeVisible(By by)
        {
            var wait = new WebDriverWait(_driver, _timeout)
            {
                PollingInterval = _pollingInterval
            };
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }
    }
} 