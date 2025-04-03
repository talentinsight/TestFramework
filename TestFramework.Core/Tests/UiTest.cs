using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TestFramework.Core.Models;

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
        public override async Task<TestResult> ExecuteAsync()
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
                var executionTime = (long)(DateTime.Now - startTime).TotalMilliseconds;

                var message = $@"UI test results:
URL: {_driver.Url}
Title: {_driver.Title}
Browser: {_driver.GetType().Name}
Test Duration: {executionTime}ms";

                if (!success)
                {
                    // Take screenshot on failure
                    if (_driver is ITakesScreenshot screenshotDriver)
                    {
                        var screenshot = screenshotDriver.GetScreenshot();
                        var screenshotPath = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                        screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                        message += $"\nScreenshot saved to: {screenshotPath}";
                    }
                }

                return CreateResult(
                    success ? TestStatus.Passed : TestStatus.Failed,
                    message,
                    executionTimeMs: executionTime
                );
            }
            catch (Exception ex)
            {
                // Take screenshot on exception
                if (_driver is ITakesScreenshot screenshotDriver)
                {
                    var screenshot = screenshotDriver.GetScreenshot();
                    var screenshotPath = $"error_screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
                }

                return CreateResult(
                    TestStatus.Failed,
                    "UI test failed with exception",
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