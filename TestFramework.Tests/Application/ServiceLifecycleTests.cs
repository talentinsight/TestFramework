using NUnit.Framework;
using System.Linq;
using TestFramework.Core;
using TestFramework.Core.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Models;
using TestFramework.Core.Application;
using Xunit;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ServiceLifecycleTests
    {
        [Test]
        public void WhenStartingValidService_ShouldStart()
        {
            // Arrange
            var test = new ServiceStartTest("Database");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenStartingInvalidService_ShouldFail()
        {
            // Arrange
            var test = new ServiceStartTest("NonExistentService");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Does.Contain("not found"));
        }
        
        [Test]
        public void WhenStoppingService_ShouldStop()
        {
            // Arrange
            var test = new ServiceStopTest("WebServer");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenCheckingServiceStatus_ShouldReportCorrectly()
        {
            // Arrange
            var test = new ServiceStatusTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
    }
    
    // Service start test
    public class ServiceStartTest : CppApplicationTest
    {
        private readonly string _serviceName;
        
        public ServiceStartTest(string serviceName)
        {
            _serviceName = serviceName;
        }
        
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            if (!Application.StartService(_serviceName))
            {
                throw new System.Exception(Application.GetLastError() ?? $"Failed to start service '{_serviceName}'");
            }
            
            AssertTrue(Application.IsServiceRunning(_serviceName), 
                $"Service '{_serviceName}' should be running after start");
        }
    }
    
    // Service stop test
    public class ServiceStopTest : CppApplicationTest
    {
        private readonly string _serviceName;
        
        public ServiceStopTest(string serviceName)
        {
            _serviceName = serviceName;
        }
        
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // First start the service
            if (!Application.StartService(_serviceName))
            {
                throw new System.Exception($"Failed to start service '{_serviceName}'");
            }
            
            // Now stop it
            if (!Application.StopService(_serviceName))
            {
                throw new System.Exception($"Failed to stop service '{_serviceName}'");
            }
            
            AssertFalse(Application.IsServiceRunning(_serviceName), 
                $"Service '{_serviceName}' should not be running after stop");
        }
    }
    
    // Service status test
    public class ServiceStatusTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // Get all available services
            var services = Application.GetAvailableServices();
            AssertTrue(services.Count > 0, "Application should have some services available");
            
            // Start one service
            string serviceToStart = services.First();
            if (!Application.StartService(serviceToStart))
            {
                throw new System.Exception($"Failed to start service '{serviceToStart}'");
            }
            
            // Check if the service is running
            AssertTrue(Application.IsServiceRunning(serviceToStart), 
                $"Service '{serviceToStart}' should be running after start");
                
            // Check if other services are not running
            foreach (var service in services.Where(s => s != serviceToStart))
            {
                AssertFalse(Application.IsServiceRunning(service), 
                    $"Service '{service}' should not be running");
            }
        }
    }
} 