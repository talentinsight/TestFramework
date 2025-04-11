using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TestFramework.Core.Models;
using TestFramework.Core.Interfaces;

namespace TestFramework.Core.Reporters
{
    /// <summary>
    /// Test reporter that generates XML reports
    /// </summary>
    public class XmlTestReporter : ITestReporter
    {
        private readonly List<XElement> _results;
        private readonly string _title;
        private readonly DateTime _startTime;

        /// <summary>
        /// Initializes a new instance of the XmlTestReporter class
        /// </summary>
        /// <param name="title">Title of the report</param>
        public XmlTestReporter(string title = "Test Execution Report")
        {
            _results = new List<XElement>();
            _title = title;
            _startTime = DateTime.Now;
        }

        /// <inheritdoc />
        public void ReportTestResult(TestResult result)
        {
            _results.Add(CreateTestResultElement(result));
        }

        /// <inheritdoc />
        public void ReportTestResults(IEnumerable<TestResult> results)
        {
            _results.AddRange(results.Select(CreateTestResultElement));
        }

        /// <inheritdoc />
        public void ReportTestSummary(IEnumerable<TestResult> results)
        {
            var metrics = new TestMetrics(results, _startTime, DateTime.Now);
            
            var summaryElement = new XElement("TestSummary",
                new XElement("Title", _title),
                new XElement("GeneratedAt", DateTime.Now),
                new XElement("TotalTests", metrics.TotalTests),
                new XElement("PassedTests", metrics.PassedTests),
                new XElement("FailedTests", metrics.FailedTests),
                new XElement("SkippedTests", metrics.SkippedTests),
                new XElement("PassRate", $"{metrics.PassRate:F2}%"),
                new XElement("TotalDuration", metrics.TotalDuration),
                new XElement("AverageExecutionTimeMs", metrics.AverageExecutionTimeMs),
                new XElement("SlowestTestTimeMs", metrics.SlowestTestTimeMs),
                new XElement("FastestTestTimeMs", metrics.FastestTestTimeMs),
                CreateMetricsByCategoryElement(metrics.MetricsByCategory),
                CreateMetricsByPriorityElement(metrics.MetricsByPriority)
            );

            _results.Clear();
            _results.Add(summaryElement);
        }

        /// <inheritdoc />
        public void SaveReport(string filePath)
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("TestReport",
                    new XAttribute("generated", DateTime.Now),
                    _results
                )
            );

            doc.Save(filePath);
        }

        private XElement CreateTestResultElement(TestResult result)
        {
            var element = new XElement("TestResult",
                new XElement("TestName", result.TestName),
                new XElement("Status", result.Status),
                new XElement("Category", result.Category),
                new XElement("Priority", result.Priority),
                new XElement("Message", result.Message),
                new XElement("ExecutionTimeMs", result.ExecutionTimeMs),
                new XElement("StartTime", result.StartTime),
                new XElement("EndTime", result.EndTime)
            );

            if (result.Exception != null)
            {
                element.Add(new XElement("Exception", result.Exception.ToString()));
            }

            return element;
        }

        private XElement CreateMetricsByCategoryElement(IDictionary<TestCategory, CategoryMetrics> metrics)
        {
            return new XElement("MetricsByCategory",
                metrics.Select(kvp => new XElement("Category",
                    new XAttribute("name", kvp.Key),
                    new XElement("TotalTests", kvp.Value.TotalTests),
                    new XElement("PassedTests", kvp.Value.PassedTests),
                    new XElement("FailedTests", kvp.Value.FailedTests),
                    new XElement("SkippedTests", kvp.Value.SkippedTests),
                    new XElement("PassRate", $"{kvp.Value.PassRate:F2}%"),
                    new XElement("TotalExecutionTimeMs", kvp.Value.TotalExecutionTimeMs),
                    new XElement("AverageExecutionTimeMs", kvp.Value.AverageExecutionTimeMs)
                ))
            );
        }

        private XElement CreateMetricsByPriorityElement(IDictionary<TestPriority, PriorityMetrics> metrics)
        {
            return new XElement("MetricsByPriority",
                metrics.Select(kvp => new XElement("Priority",
                    new XAttribute("level", kvp.Key),
                    new XElement("TotalTests", kvp.Value.TotalTests),
                    new XElement("PassedTests", kvp.Value.PassedTests),
                    new XElement("FailedTests", kvp.Value.FailedTests),
                    new XElement("SkippedTests", kvp.Value.SkippedTests),
                    new XElement("PassRate", $"{kvp.Value.PassRate:F2}%"),
                    new XElement("TotalExecutionTimeMs", kvp.Value.TotalExecutionTimeMs),
                    new XElement("AverageExecutionTimeMs", kvp.Value.AverageExecutionTimeMs)
                ))
            );
        }
    }
} 