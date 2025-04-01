using NUnit.Framework;
using System.Linq;
using TestFramework.Core;
using TestFramework.Core.Application;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class LoggingTests
    {
        [Test]
        public void WhenSettingValidLogLevel_ShouldSucceed()
        {
            // Arrange
            var test = new LogLevelTest("INFO");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenSettingInvalidLogLevel_ShouldFail()
        {
            // Arrange
            var test = new LogLevelTest("INVALID_LEVEL");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Does.Contain("Invalid log level"));
        }
        
        [Test]
        public void WhenGettingLogLevel_ShouldReturnCorrectValue()
        {
            // Arrange
            var test = new GetLogLevelTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenGettingRecentLogs_ShouldReturnLogs()
        {
            // Arrange
            var test = new RecentLogsTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenClearingLogs_ShouldClearAll()
        {
            // Arrange
            var test = new ClearLogsTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
    }
    
    // Log level test
    public class LogLevelTest : CppApplicationTest
    {
        private readonly string _logLevel;
        
        public LogLevelTest(string logLevel)
        {
            _logLevel = logLevel;
        }
        
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            if (!Application.SetLogLevel(_logLevel))
            {
                throw new System.Exception(Application.GetLastError() ?? $"Failed to set log level to {_logLevel}");
            }
                
            string currentLevel = Application.GetLogLevel();
            AssertEqual(_logLevel, currentLevel, 
                $"Log level was not set correctly. Expected {_logLevel}, got {currentLevel}");
        }
    }
    
    // Get log level test
    public class GetLogLevelTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Set a specific log level
            AssertTrue(Application.SetLogLevel("DEBUG"), 
                "Failed to set log level to DEBUG");
                
            // Get and verify the log level
            string logLevel = Application.GetLogLevel();
            AssertEqual("DEBUG", logLevel, 
                $"Log level was not returned correctly. Expected DEBUG, got {logLevel}");
        }
    }
    
    // Recent logs test
    public class RecentLogsTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Generate some logs by performing operations
            Application.LoadConfiguration("config.json");
            Application.ValidateConfiguration();
            Application.StartService("Database");
            
            // Get recent logs
            var logs = Application.GetRecentLogs(10);
            
            AssertTrue(logs.Count > 0, "Should have received at least one log entry");
            AssertTrue(logs.Any(log => log.Contains("Configuration loaded")), 
                "Logs should contain entry about configuration loading");
            AssertTrue(logs.Any(log => log.Contains("Service 'Database' started")), 
                "Logs should contain entry about starting the Database service");
        }
    }
    
    // Clear logs test
    public class ClearLogsTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Generate some logs
            Application.LoadConfiguration("config.json");
            Application.ValidateConfiguration();
            
            // Verify logs exist
            var logsBefore = Application.GetRecentLogs(10);
            AssertTrue(logsBefore.Count > 0, "Should have logs before clearing");
            
            // Clear logs
            AssertTrue(Application.ClearLogs(), "Failed to clear logs");
            
            // Verify logs are cleared (only 1 log entry should remain - the "logs cleared" entry)
            var logsAfter = Application.GetRecentLogs(10);
            AssertEqual(1, logsAfter.Count, "Should have exactly one log entry after clearing");
            AssertTrue(logsAfter[0].Contains("Logs cleared"), 
                "The remaining log entry should be about logs being cleared");
        }
    }
} 