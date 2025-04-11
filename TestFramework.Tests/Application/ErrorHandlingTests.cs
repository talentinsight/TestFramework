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
        }
    }
} 