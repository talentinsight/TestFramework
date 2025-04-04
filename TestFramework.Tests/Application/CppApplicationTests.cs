using System;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class CppApplicationTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly ICppApplication _application;

        public CppApplicationTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        public void Dispose()
        {
            _application?.Dispose();
        }

        #region Initialization Tests

        [Fact]
        public async Task Initialize_WhenNotInitialized_ReturnsTrue()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.False(_application.IsRunning);
        }

        [Fact]
        public async Task Initialize_WhenAlreadyInitialized_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application already initialized", _application.LastError);
        }

        #endregion

        #region Start Tests

        [Fact]
        public async Task Start_WhenInitialized_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.True(_application.IsRunning);
        }

        [Fact]
        public async Task Start_WhenNotInitialized_ReturnsFalse()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application not initialized", _application.LastError);
        }

        [Fact]
        public async Task Start_WhenAlreadyRunning_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application already running", _application.LastError);
        }

        #endregion

        #region Stop Tests

        [Fact]
        public async Task Stop_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.False(_application.IsRunning);
        }

        [Fact]
        public async Task Stop_WhenNotRunning_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application not running", _application.LastError);
        }

        #endregion

        #region Restart Tests

        [Fact]
        public async Task Restart_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.True(_application.IsRunning);
        }

        [Fact]
        public async Task Restart_WhenStopped_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.True(_application.IsRunning);
        }

        #endregion

        #region Status and Command Tests

        [Fact]
        public async Task GetStatus_WhenRunning_ReturnsRunning()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.Equal("Running", status);
        }

        [Fact]
        public async Task SendCommand_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetResponse_WhenCommandSent_ReturnsResponse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.SendCommandAsync("test");

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.NotNull(response);
        }

        #endregion
    }
} 