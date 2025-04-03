using System;
using System.Net.Http;
using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Represents an API test
    /// </summary>
    public class ApiTest : BaseTest
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _endpoint;
        private readonly HttpMethod _method;
        private readonly string? _requestBody;
        private readonly Func<HttpResponseMessage, Task<bool>>? _validationFunc;
        private readonly int _expectedStatusCode;
        private readonly TimeSpan _timeout;

        /// <summary>
        /// Initializes a new instance of the ApiTest class
        /// </summary>
        /// <param name="name">Test name</param>
        /// <param name="description">Test description</param>
        /// <param name="baseUrl">Base URL for the API</param>
        /// <param name="endpoint">API endpoint to test</param>
        /// <param name="method">HTTP method to use</param>
        /// <param name="requestBody">Request body (for POST/PUT/PATCH)</param>
        /// <param name="validationFunc">Custom validation function</param>
        /// <param name="expectedStatusCode">Expected HTTP status code</param>
        /// <param name="timeout">Request timeout</param>
        /// <param name="priority">Test priority</param>
        public ApiTest(
            string name,
            string description,
            string baseUrl,
            string endpoint,
            HttpMethod method = null!,
            string? requestBody = null,
            Func<HttpResponseMessage, Task<bool>>? validationFunc = null,
            int expectedStatusCode = 200,
            TimeSpan? timeout = null,
            TestPriority priority = TestPriority.Medium)
            : base(name, description, TestCategory.API, priority)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/');
            _endpoint = endpoint.TrimStart('/');
            _method = method ?? HttpMethod.Get;
            _requestBody = requestBody;
            _validationFunc = validationFunc;
            _expectedStatusCode = expectedStatusCode;
            _timeout = timeout ?? TimeSpan.FromSeconds(30);
        }

        /// <inheritdoc />
        public override async Task<TestResult> ExecuteAsync()
        {
            try
            {
                _httpClient.Timeout = _timeout;
                var url = $"{_baseUrl}/{_endpoint}";
                var request = new HttpRequestMessage(_method, url);

                if (!string.IsNullOrEmpty(_requestBody))
                {
                    request.Content = new StringContent(_requestBody, System.Text.Encoding.UTF8, "application/json");
                }

                var startTime = DateTime.Now;
                var response = await _httpClient.SendAsync(request);
                var executionTime = (long)(DateTime.Now - startTime).TotalMilliseconds;

                var statusCodeValid = (int)response.StatusCode == _expectedStatusCode;
                var customValidationPassed = true;

                if (_validationFunc != null)
                {
                    customValidationPassed = await _validationFunc(response);
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var message = $@"API test results:
URL: {url}
Method: {_method}
Status Code: {(int)response.StatusCode} {response.StatusCode}
Response Time: {executionTime}ms
Response Body: {responseBody}";

                if (!statusCodeValid)
                {
                    message = $"Expected status code {_expectedStatusCode} but got {(int)response.StatusCode}\n" + message;
                }

                if (!customValidationPassed)
                {
                    message = "Custom validation failed\n" + message;
                }

                return CreateResult(
                    statusCodeValid && customValidationPassed ? TestStatus.Passed : TestStatus.Failed,
                    message,
                    executionTimeMs: executionTime
                );
            }
            catch (Exception ex)
            {
                return CreateResult(
                    TestStatus.Failed,
                    "API test failed with exception",
                    ex
                );
            }
        }

        /// <inheritdoc />
        public override Task SetupAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task CleanupAsync()
        {
            _httpClient.Dispose();
            return Task.CompletedTask;
        }
    }
} 