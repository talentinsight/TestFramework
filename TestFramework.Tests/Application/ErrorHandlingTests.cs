using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ErrorHandlingTests
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
        public async Task WhenApplicationThrowsException_ErrorIsLogged()
        {
            // Arrange
            _application.ThrowExceptionOnNextCall = true;

            // Act
            try
            {
                await _application.StartAsync();
            }
            catch (Exception)
            {
                // Expected exception
            }

            // Assert
            Assert.That(_logger.ErrorMessages, Has.Count.EqualTo(1));
            Assert.That(_logger.ErrorMessages[0], Contains.Substring("Test exception"));
        }

        [Test]
        public async Task WhenApplicationFails_LastErrorIsSet()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_application.LastError, Is.EqualTo("Operation failed"));
        }

        [Test]
        public async Task WhenApplicationRecovers_ErrorStateIsCleared()
        {
            // Arrange
            _application.FailNextCall = true;
            await _application.StartAsync(); // This will fail

            // Act
            _application.FailNextCall = false;
            var result = await _application.StartAsync();

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_application.LastError, Is.Empty);
        }

        [Test]
        public async Task WhenMultipleErrorsOccur_AllErrorsAreLogged()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            await _application.StartAsync(); // First error
            await _application.StopAsync();  // Second error
            await _application.StartAsync(); // Third error

            // Assert
            Assert.That(_logger.ErrorMessages, Has.Count.EqualTo(3));
        }
    }
} 