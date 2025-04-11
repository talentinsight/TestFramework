using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestFramework.Core.Models;
using TestFramework.Core.Interfaces;

namespace TestFramework.Core.Reporters
{
    /// <summary>
    /// Test reporter that generates JSON reports
    /// </summary>
    public class JsonTestReporter : ITestReporter
    {
        private readonly List<TestResultDto> _results;
        private readonly string _title;
        private readonly DateTime _startTime;

        /// <summary>
        /// Initializes a new instance of the JsonTestReporter class
        /// </summary>
        /// <param name="title">Title of the report</param>
        public JsonTestReporter(string title = "Test Execution Report")
        {
            _results = new List<TestResultDto>();
            _title = title;
            _startTime = DateTime.Now;
        }

        /// <inheritdoc />
        public void ReportTestResult(TestResult result)
        {
            _results.Add(new TestResultDto(result));
        }

        /// <inheritdoc />
        public void ReportTestResults(IEnumerable<TestResult> results)
        {
            _results.AddRange(results.Select(r => new TestResultDto(r)));
        }

        /// <inheritdoc />
        public void ReportTestSummary(IEnumerable<TestResult> results)
        {
            var metrics = new TestMetrics(results, _startTime, DateTime.Now);
            var summary = new TestSummaryDto
            {
                Title = _title,
                GeneratedAt = DateTime.Now,
                TotalTests = metrics.TotalTests,
                PassedTests = metrics.PassedTests,
                FailedTests = metrics.FailedTests,
                SkippedTests = metrics.SkippedTests,
                PassRate = metrics.PassRate,
                TotalDuration = metrics.TotalDuration,
                AverageExecutionTimeMs = metrics.AverageExecutionTimeMs,
                SlowestTestTimeMs = metrics.SlowestTestTimeMs,
                FastestTestTimeMs = metrics.FastestTestTimeMs,
                MetricsByCategory = metrics.MetricsByCategory.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => new CategoryMetricsDto(kvp.Value)
                ),
                MetricsByPriority = metrics.MetricsByPriority.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => new PriorityMetricsDto(kvp.Value)
                )
            };

            _results.Clear();
            _results.Add(new TestResultDto { Summary = summary });
        }

        /// <inheritdoc />
        public void SaveReport(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(_results, options);
            File.WriteAllText(filePath, json);
        }

        private class TestResultDto
        {
            public string? TestName { get; set; }
            public string? Status { get; set; }
            public string? Category { get; set; }
            public string? Priority { get; set; }
            public string? Message { get; set; }
            public long ExecutionTimeMs { get; set; }
            public string? Exception { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public TestSummaryDto? Summary { get; set; }

            public TestResultDto() { }

            public TestResultDto(TestResult result)
            {
                TestName = result.TestName;
                Status = result.Status.ToString();
                Category = result.Category.ToString();
                Priority = result.Priority.ToString();
                Message = result.Message;
                ExecutionTimeMs = result.ExecutionTimeMs;
                Exception = result.Exception?.ToString();
                StartTime = result.StartTime;
                EndTime = result.EndTime;
            }
        }

        private class TestSummaryDto
        {
            public string? Title { get; set; }
            public DateTime GeneratedAt { get; set; }
            public int TotalTests { get; set; }
            public int PassedTests { get; set; }
            public int FailedTests { get; set; }
            public int SkippedTests { get; set; }
            public double PassRate { get; set; }
            public TimeSpan TotalDuration { get; set; }
            public double AverageExecutionTimeMs { get; set; }
            public long SlowestTestTimeMs { get; set; }
            public long FastestTestTimeMs { get; set; }
            public Dictionary<string, CategoryMetricsDto>? MetricsByCategory { get; set; }
            public Dictionary<string, PriorityMetricsDto>? MetricsByPriority { get; set; }
        }

        private class CategoryMetricsDto
        {
            public int TotalTests { get; set; }
            public int PassedTests { get; set; }
            public int FailedTests { get; set; }
            public int SkippedTests { get; set; }
            public double PassRate { get; set; }
            public long TotalExecutionTimeMs { get; set; }
            public double AverageExecutionTimeMs { get; set; }

            public CategoryMetricsDto(CategoryMetrics metrics)
            {
                TotalTests = metrics.TotalTests;
                PassedTests = metrics.PassedTests;
                FailedTests = metrics.FailedTests;
                SkippedTests = metrics.SkippedTests;
                PassRate = metrics.PassRate;
                TotalExecutionTimeMs = metrics.TotalExecutionTimeMs;
                AverageExecutionTimeMs = metrics.AverageExecutionTimeMs;
            }
        }

        private class PriorityMetricsDto
        {
            public int TotalTests { get; set; }
            public int PassedTests { get; set; }
            public int FailedTests { get; set; }
            public int SkippedTests { get; set; }
            public double PassRate { get; set; }
            public long TotalExecutionTimeMs { get; set; }
            public double AverageExecutionTimeMs { get; set; }

            public PriorityMetricsDto(PriorityMetrics metrics)
            {
                TotalTests = metrics.TotalTests;
                PassedTests = metrics.PassedTests;
                FailedTests = metrics.FailedTests;
                SkippedTests = metrics.SkippedTests;
                PassRate = metrics.PassRate;
                TotalExecutionTimeMs = metrics.TotalExecutionTimeMs;
                AverageExecutionTimeMs = metrics.AverageExecutionTimeMs;
            }
        }
    }
} 