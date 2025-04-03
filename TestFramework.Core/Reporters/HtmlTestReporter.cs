using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestFramework.Core.Models;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Utils;

namespace TestFramework.Core.Reporters
{
    /// <summary>
    /// Test reporter that generates HTML reports
    /// </summary>
    public class HtmlTestReporter : ITestReporter
    {
        private readonly StringBuilder _reportBuilder;
        private readonly string _title;
        private readonly DateTime _startTime;

        /// <summary>
        /// Initializes a new instance of the HtmlTestReporter class
        /// </summary>
        /// <param name="title">Title of the report</param>
        public HtmlTestReporter(string title = "Test Execution Report")
        {
            _reportBuilder = new StringBuilder();
            _title = title;
            _startTime = DateTime.Now;
        }

        /// <inheritdoc />
        public void ReportTestResult(TestResult result)
        {
            _reportBuilder.AppendLine("<div class=\"test-result\">");
            _reportBuilder.AppendLine($"<h3>{result.TestName}</h3>");
            _reportBuilder.AppendLine("<table>");
            _reportBuilder.AppendLine($"<tr><td>Status:</td><td class=\"status-{result.Status.ToString().ToLower()}\">{result.Status}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Category:</td><td>{result.Category}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Priority:</td><td>{result.Priority}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Duration:</td><td>{result.ExecutionTimeMs} ms</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Start Time:</td><td>{result.StartTime}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>End Time:</td><td>{result.EndTime}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Message:</td><td>{result.Message}</td></tr>");
            
            if (result.Exception != null)
            {
                _reportBuilder.AppendLine($"<tr><td>Exception:</td><td class=\"error\">{result.Exception}</td></tr>");
            }
            
            _reportBuilder.AppendLine("</table>");
            _reportBuilder.AppendLine("</div>");
        }

        /// <inheritdoc />
        public void ReportTestResults(IEnumerable<TestResult> results)
        {
            var resultsList = results.ToList();
            
            _reportBuilder.AppendLine("<!DOCTYPE html>");
            _reportBuilder.AppendLine("<html>");
            _reportBuilder.AppendLine("<head>");
            _reportBuilder.AppendLine($"<title>{_title}</title>");
            _reportBuilder.AppendLine("<style>");
            _reportBuilder.AppendLine(@"
                body { font-family: Arial, sans-serif; margin: 20px; }
                .test-result { border: 1px solid #ddd; padding: 10px; margin-bottom: 10px; }
                .status-passed { color: green; }
                .status-failed { color: red; }
                .status-skipped { color: orange; }
                .error { color: red; }
                table { border-collapse: collapse; width: 100%; }
                td { padding: 8px; border: 1px solid #ddd; }
                .summary { margin-bottom: 20px; }
                .metrics { margin-top: 20px; }
            ");
            _reportBuilder.AppendLine("</style>");
            _reportBuilder.AppendLine("</head>");
            _reportBuilder.AppendLine("<body>");
            
            _reportBuilder.AppendLine($"<h1>{_title}</h1>");
            _reportBuilder.AppendLine($"<p>Generated on: {DateTime.Now}</p>");
            
            ReportTestSummary(resultsList);
            
            _reportBuilder.AppendLine("<h2>Test Results</h2>");
            
            foreach (var result in resultsList)
            {
                ReportTestResult(result);
            }
            
            _reportBuilder.AppendLine("</body>");
            _reportBuilder.AppendLine("</html>");
        }

        /// <inheritdoc />
        public void ReportTestSummary(IEnumerable<TestResult> results)
        {
            var resultsList = results.ToList();
            
            _reportBuilder.AppendLine("<div class=\"summary\">");
            _reportBuilder.AppendLine("<h2>Summary</h2>");
            
            var totalTests = resultsList.Count;
            var passedTests = resultsList.Count(r => r.Status == TestStatus.Passed);
            var failedTests = resultsList.Count(r => r.Status == TestStatus.Failed);
            var skippedTests = resultsList.Count(r => r.Status == TestStatus.Skipped);
            
            _reportBuilder.AppendLine("<table>");
            _reportBuilder.AppendLine($"<tr><td>Total Tests:</td><td>{totalTests}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Passed:</td><td class=\"status-passed\">{passedTests}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Failed:</td><td class=\"status-failed\">{failedTests}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Skipped:</td><td class=\"status-skipped\">{skippedTests}</td></tr>");
            _reportBuilder.AppendLine($"<tr><td>Pass Rate:</td><td>{(double)passedTests / totalTests * 100:F2}%</td></tr>");
            _reportBuilder.AppendLine("</table>");
            
            // Report metrics by category
            _reportBuilder.AppendLine("<div class=\"metrics\">");
            _reportBuilder.AppendLine("<h3>Metrics by Category</h3>");
            _reportBuilder.AppendLine("<table>");
            _reportBuilder.AppendLine("<tr><th>Category</th><th>Total</th><th>Passed</th><th>Failed</th><th>Pass Rate</th></tr>");
            
            foreach (var category in Enum.GetValues<TestCategory>())
            {
                var categoryResults = TestFilter.FilterByCategory(resultsList, category).ToList();
                if (categoryResults.Any())
                {
                    var categoryPassed = categoryResults.Count(r => r.Status == TestStatus.Passed);
                    _reportBuilder.AppendLine($"<tr><td>{category}</td><td>{categoryResults.Count}</td><td>{categoryPassed}</td><td>{categoryResults.Count - categoryPassed}</td><td>{(double)categoryPassed / categoryResults.Count * 100:F2}%</td></tr>");
                }
            }
            
            _reportBuilder.AppendLine("</table>");
            _reportBuilder.AppendLine("</div>");
            
            _reportBuilder.AppendLine("</div>");
        }

        /// <inheritdoc />
        public void SaveReport(string filePath)
        {
            File.WriteAllText(filePath, _reportBuilder.ToString());
        }
    }
} 