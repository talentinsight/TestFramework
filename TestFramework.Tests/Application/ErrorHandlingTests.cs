<<<<<<< HEAD
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TestFramework.Core;

namespace TestFramework.Tests.Application
{
    [TestClass]
    public class ErrorHandlingTests
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
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task HandleError_InvalidOperation_ThrowsException()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.ExecuteInvalidOperationAsync();
        }

        [TestMethod]
        public async Task HandleError_RecoverableError_RecoversSuccessfully()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            try
            {
                await _app.ExecuteRecoverableErrorAsync();
            }
            catch (Exception)
            {
                // Expected exception
            }

            // Assert
            Assert.IsTrue(_app.IsRunning);
        }

        [TestMethod]
        public async Task HandleError_NonRecoverableError_StopsApplication()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            try
            {
                await _app.ExecuteNonRecoverableErrorAsync();
            }
            catch (Exception)
            {
                // Expected exception
            }

            // Assert
            Assert.IsFalse(_app.IsRunning);
        }

        [TestMethod]
        public async Task HandleError_MultipleErrors_HandlesAllErrors()
        {
            // Arrange
            await _app.StartAsync();
            int errorCount = 0;

            // Act
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await _app.ExecuteRecoverableErrorAsync();
                }
                catch (Exception)
                {
                    errorCount++;
                }
            }

            // Assert
            Assert.AreEqual(5, errorCount);
            Assert.IsTrue(_app.IsRunning);
=======
using System;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class ErrorHandlingTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;

        public ErrorHandlingTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        public void Dispose()
        {
            _application.Dispose();
        }

        [Fact]
        public async Task WhenErrorOccurs_ErrorStateIsSet()
        {
            // Act
            await _application.StartAsync();

            // Assert
            Assert.True(_application.IsInErrorState);
            Assert.Equal("Application not initialized", _application.LastError);
        }

        [Fact]
        public async Task WhenErrorClears_ErrorStateIsReset()
        {
            // Arrange
            await _application.StartAsync();
            Assert.True(_application.IsInErrorState);

            // Act
            await _application.InitializeAsync();

            // Assert
            Assert.False(_application.IsInErrorState);
            Assert.Empty(_application.LastError);
        }

        [Fact]
        public async Task WhenMultipleErrorsOccur_LastErrorIsUpdated()
        {
            // Arrange
            await _application.StartAsync();
            Assert.Equal("Application not initialized", _application.LastError);

            // Act
            await _application.StopAsync();

            // Assert
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task WhenErrorOccurs_LogsError()
        {
            // Act
            await _application.StartAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Application not initialized"));
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
        }
    }
} 