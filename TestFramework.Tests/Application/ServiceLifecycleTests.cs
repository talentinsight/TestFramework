using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TestFramework.Core;

namespace TestFramework.Tests.Application
{
    [TestClass]
    public class ServiceLifecycleTests
    {
        private CppApplication _app;

        [TestInitialize]
        public void Setup()
        {
            _app = new CppApplication();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _app?.Dispose();
        }

        [TestMethod]
        public async Task StartService_ValidService_StartsSuccessfully()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.StartServiceAsync("Database");

            // Assert
            Assert.IsTrue(_app.IsServiceRunning("Database"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task StartService_InvalidService_ThrowsException()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.StartServiceAsync("InvalidService");
        }

        [TestMethod]
        public async Task StopService_RunningService_StopsSuccessfully()
        {
            // Arrange
            await _app.StartAsync();
            await _app.StartServiceAsync("Database");

            // Act
            await _app.StopServiceAsync("Database");

            // Assert
            Assert.IsFalse(_app.IsServiceRunning("Database"));
        }

        [TestMethod]
        public async Task RestartService_RunningService_RestartsSuccessfully()
        {
            // Arrange
            await _app.StartAsync();
            await _app.StartServiceAsync("Database");

            // Act
            await _app.RestartServiceAsync("Database");

            // Assert
            Assert.IsTrue(_app.IsServiceRunning("Database"));
        }

        [TestMethod]
        public async Task MultipleServices_CanRunConcurrently()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.StartServiceAsync("Database");
            await _app.StartServiceAsync("WebServer");
            await _app.StartServiceAsync("Cache");

            // Assert
            Assert.IsTrue(_app.IsServiceRunning("Database"));
            Assert.IsTrue(_app.IsServiceRunning("WebServer"));
            Assert.IsTrue(_app.IsServiceRunning("Cache"));
        }
    }
} 