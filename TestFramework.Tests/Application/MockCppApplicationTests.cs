using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Logger;
using TestFramework.Tests.Application;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class MockCppApplicationTests
    {
        private MockCppApplication _application;
        private MockLogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication(logger: _logger);
        }

        [Test]
        public async Task InitializeAsync_WhenSuccessful_ReturnsTrueAndSetsInitialized()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsInitialized, Is.True);
            Assert.That(_logger.Logs, Has.Some.Contains("Application initialized"));
        }

        [Test]
        public async Task InitializeAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Initialization failed"));
        }

        [Test]
        public void InitializeAsync_WhenThrowExceptionOnNextCall_ThrowsException()
        {
            // Arrange
            _application.ThrowExceptionOnNextCall = true;

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _application.InitializeAsync());
        }

        [Test]
        public async Task StartAsync_WhenNotInitialized_ReturnsFalse()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not initialized"));
        }

        [Test]
        public async Task StartAsync_WhenInitialized_ReturnsTrueAndSetsRunning()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
            Assert.That(_logger.Logs, Has.Some.Contains("Application started"));
        }

        [Test]
        public async Task StartAsync_WhenAlreadyRunning_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application already running"));
        }

        [Test]
        public async Task StopAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not running"));
        }

        [Test]
        public async Task StopAsync_WhenRunning_ReturnsTrueAndSetsNotRunning()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.False);
            Assert.That(_logger.Logs, Has.Some.Contains("Application stopped"));
        }

        [Test]
        public async Task StopAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Operation failed"));
        }

        [Test]
        public void StopAsync_WhenThrowExceptionOnNextCall_ThrowsException()
        {
            // Arrange
            _application.InitializeAsync().Wait();
            _application.StartAsync().Wait();
            _application.ThrowExceptionOnNextCall = true;

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _application.StopAsync());
        }

        [Test]
        public async Task RestartAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not running"));
        }

        [Test]
        public async Task RestartAsync_WhenRunning_ReturnsTrueAndKeepsRunning()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
            Assert.That(_logger.Logs, Has.Some.Contains("Application restarted"));
        }

        [Test]
        public async Task GetStatusAsync_WhenNotRunning_ReturnsStopped()
        {
            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.That(status, Is.EqualTo("Stopped"));
        }

        [Test]
        public async Task GetStatusAsync_WhenRunning_ReturnsRunning()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.That(status, Is.EqualTo("Running"));
        }

        [Test]
        public async Task GetStatusAsync_WhenFailNextCall_ReturnsNull()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.That(status, Is.Null);
            Assert.That(_application.LastError, Is.EqualTo("Operation failed"));
        }

        [Test]
        public async Task SendCommandAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not running"));
        }

        [Test]
        public async Task SendCommandAsync_WhenRunning_ReturnsTrueAndLogsCommand()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_logger.Logs, Has.Some.Contains("Command sent: test"));
        }

        [Test]
        public async Task SendCommandAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Operation failed"));
        }

        [Test]
        public async Task GetResponseAsync_WhenNotRunning_ReturnsNull()
        {
            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.That(response, Is.Null);
            Assert.That(_application.LastError, Is.EqualTo("Application not running"));
        }

        [Test]
        public async Task GetResponseAsync_WhenRunning_ReturnsOK()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.That(response, Is.EqualTo("OK"));
        }

        [Test]
        public async Task GetResponseAsync_WhenFailNextCall_ReturnsNull()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.That(response, Is.Null);
            Assert.That(_application.LastError, Is.EqualTo("Operation failed"));
        }

        [Test]
        public void SetModbusResponse_SetsResponse()
        {
            // Arrange
            var response = new byte[] { 1, 2, 3, 4 };

            // Act
            _application.SetModbusResponse(response);

            // Assert
            // Note: We can't directly verify the response was set as it's private
            // This test is mainly for coverage
        }

        [Test]
        public void Dispose_SetsNotRunningAndNotInitialized()
        {
            // Arrange
            _application.InitializeAsync().Wait();
            _application.StartAsync().Wait();

            // Act
            _application.Dispose();

            // Assert
            Assert.That(_application.IsRunning, Is.False);
            Assert.That(_application.IsInitialized, Is.False);
            Assert.That(_logger.Logs, Has.Some.Contains("Application disposed"));
        }

        [Test]
        public async Task MultipleOperations_WhenInErrorState_HandlesErrorsCorrectly()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act & Assert
            var initResult = await _application.InitializeAsync();
            Assert.That(initResult, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Initialization failed"));

            var startResult = await _application.StartAsync();
            Assert.That(startResult, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not initialized"));

            var status = await _application.GetStatusAsync();
            Assert.That(status, Is.EqualTo("Stopped"));
        }

        [Test]
        public async Task Dispose_WhenNotInitialized_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _application.Dispose());
        }

        [Test]
        public async Task Dispose_WhenDisposed_CanBeDisposedAgain()
        {
            // Arrange
            _application.InitializeAsync().Wait();
            _application.StartAsync().Wait();
            _application.Dispose();

            // Act & Assert
            Assert.DoesNotThrow(() => _application.Dispose());
        }
    }
} 