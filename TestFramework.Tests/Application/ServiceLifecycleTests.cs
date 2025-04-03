using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ServiceLifecycleTests
    {
        private MockCppApplication _application;
        private MockLogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication();
        }

        [Test]
        public async Task WhenServiceIsStarted_ServiceIsRunning()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
        }

        [Test]
        public async Task WhenServiceIsStopped_ServiceIsNotRunning()
        {
            // Arrange
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.False);
        }

        [Test]
        public async Task WhenServiceIsRestarted_ServiceIsRunning()
        {
            // Arrange
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
        }

        [Test]
        public async Task WhenServiceIsAlreadyRunning_StartReturnsFalse()
        {
            // Arrange
            await _application.StartAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application already running"));
        }

        [Test]
        public async Task WhenServiceIsNotRunning_StopReturnsFalse()
        {
            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Application not running"));
        }
    }
} 