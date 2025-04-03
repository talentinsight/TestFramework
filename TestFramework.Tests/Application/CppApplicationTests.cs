using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class CppApplicationTests
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
        public async Task WhenApplicationIsInitialized_StateIsCorrect()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsInitialized, Is.True);
        }

        [Test]
        public async Task WhenApplicationIsStarted_StateIsCorrect()
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
        public async Task WhenApplicationIsStopped_StateIsCorrect()
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
        public async Task WhenApplicationIsRestarted_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.IsRunning, Is.True);
        }
    }
} 