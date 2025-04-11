<<<<<<< HEAD
using System;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class ServiceLifecycleTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;

        public ServiceLifecycleTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        public void Dispose()
        {
            _application.Dispose();
        }

        [Fact]
        public async Task WhenServiceStarts_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceStops_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.True(result);
            Assert.False(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceRestarts_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceFails_ErrorStateIsSet()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.True(_application.IsInErrorState);
            Assert.Equal("Application not initialized", _application.LastError);
=======
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
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
        }
    }
} 