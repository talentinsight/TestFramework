using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Tests;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class CppApplicationTest : TestBase
    {
        private ICppApplication _application;

        [SetUp]
        protected override void Setup()
        {
            base.Setup();
            _application = new MockCppApplication(Logger);
        }

        [Test]
        public async Task Initialize_WhenSuccessful_ReturnsTrue()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsInitialized, Is.True);
        }

        [Test]
        public async Task Start_WhenInitialized_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
        }

        [Test]
        public async Task Stop_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.False);
        }

        [Test]
        public async Task Restart_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
        }

        [Test]
        public async Task GetStatus_WhenRunning_ReturnsRunning()
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
        public async Task SendCommand_WhenRunning_ReturnsTrue()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetResponse_WhenRunning_ReturnsResponse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.SendCommandAsync("test");

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        [TearDown]
        protected override void TearDown()
        {
            _application?.Dispose();
            base.TearDown();
        }
    }
} 