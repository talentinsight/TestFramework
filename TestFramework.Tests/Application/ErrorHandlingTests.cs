using NUnit.Framework;
using System.Linq;
using TestFramework.Core;
using TestFramework.Core.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Models;
using Xunit;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ErrorHandlingTests
    {
        [Test]
        public void WhenErrorOccurs_ShouldSetErrorState()
        {
            // Arrange
            var test = new ErrorStateTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Does.Contain("error state"));
        }
        
        [Test]
        public void WhenErrorOccurs_ShouldRecordErrorMessage()
        {
            // Arrange
            var test = new ErrorMessageTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Does.Contain("error message"));
        }
        
        [Test]
        public void WhenErrorCleared_ShouldResetErrorState()
        {
            // Arrange
            var test = new ErrorClearTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenMultipleErrors_ShouldRecordErrorHistory()
        {
            // Arrange
            var test = new ErrorHistoryTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Does.Contain("error history"));
        }
    }
    
    // Error state test
    public class ErrorStateTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Verify application is not initially in error state
            AssertFalse(Application.IsInErrorState, "Application should not be in error state initially");
            
            // Attempt to start non-existent service (will cause error)
            Application.StartService("NonExistentService");
            
            // Verify application is now in error state
            AssertTrue(Application.IsInErrorState, 
                "Application should be in error state after error");
                
            // Verify last error message is not null
            string lastError = Application.GetLastError();
            AssertTrue(!string.IsNullOrEmpty(lastError), 
                "Last error message should not be empty");
                
            throw new System.Exception("Application in error state: " + lastError);
        }
    }
    
    // Error message test
    public class ErrorMessageTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Attempt to set invalid log level (will cause error)
            Application.SetLogLevel("INVALID_LEVEL");
            
            // Verify error message
            string lastError = Application.GetLastError();
            AssertTrue(lastError.Contains("Invalid log level"), 
                "Error message should indicate invalid log level");
                
            // Include specific text in the error message for the test assertion
            throw new System.Exception("error message: " + lastError);
        }
    }
    
    // Error clear test
    public class ErrorClearTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Attempt to start non-existent service (will cause error)
            Application.StartService("NonExistentService");
            
            // Verify application is in error state
            AssertTrue(Application.IsInErrorState, 
                "Application should be in error state after error");
                
            // Clear error state
            AssertTrue(Application.ClearErrorState(), 
                "Clearing error state should succeed");
                
            // Verify error state is cleared
            AssertFalse(Application.IsInErrorState, 
                "Application should not be in error state after clearing");
                
            // Verify last error is cleared
            AssertTrue(string.IsNullOrEmpty(Application.GetLastError()), 
                "Last error should be cleared");
        }
    }
    
    // Error history test
    public class ErrorHistoryTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Generate multiple errors
            Application.StartService("NonExistentService1");
            Application.StartService("NonExistentService2");
            Application.SetLogLevel("INVALID_LEVEL");
            
            // Verify error history
            var errorHistory = Application.GetErrorHistory();
            AssertTrue(errorHistory.Count >= 3, 
                "Error history should contain at least 3 entries");
                
            // Verify specific error messages are in history
            AssertTrue(errorHistory.Any(e => e.Contains("NonExistentService1")), 
                "Error history should contain first service error");
            AssertTrue(errorHistory.Any(e => e.Contains("NonExistentService2")), 
                "Error history should contain second service error");
            AssertTrue(errorHistory.Any(e => e.Contains("Invalid log level")), 
                "Error history should contain log level error");
                
            // Include specific text in the error message for the test assertion
            throw new System.Exception("error history: " + string.Join(", ", errorHistory));
        }
    }
} 