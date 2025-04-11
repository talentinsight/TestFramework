using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TestFramework.Core;
using TestFramework.Core.Application;

namespace TestFramework.Tests.Application
{
    [TestClass]
    public class CppApplicationTests
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
        public async Task StartApplication_ValidConfiguration_StartsSuccessfully()
        {
            // Arrange
            _app.Configuration.SetValue("LogLevel", "INFO");

            // Act
            await _app.StartAsync();

            // Assert
            Assert.IsTrue(_app.IsRunning);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task StartApplication_InvalidConfiguration_ThrowsException()
        {
            // Arrange
            _app.Configuration.SetValue("InvalidKey", "InvalidValue");

            // Act
            await _app.StartAsync();
        }

        [TestMethod]
        public async Task StopApplication_RunningApplication_StopsSuccessfully()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.StopAsync();

            // Assert
            Assert.IsFalse(_app.IsRunning);
        }

        [TestMethod]
        public async Task RestartApplication_RunningApplication_RestartsSuccessfully()
        {
            // Arrange
            await _app.StartAsync();

            // Act
            await _app.RestartAsync();

            // Assert
            Assert.IsTrue(_app.IsRunning);
        }
    }

    // Specific test implementation for version checking
    public class VersionCheckTest : CppApplicationTest
    {
        private readonly string _expectedVersion;

        public VersionCheckTest(string expectedVersion)
        {
            _expectedVersion = expectedVersion;
        }

        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication(_expectedVersion);
        }

        protected override void RunTest()
        {
            var version = Application.GetApplicationVersion();
            AssertEqual(_expectedVersion, version, "Application version mismatch");
        }
    }

    // Specific test implementation for error handling
    public class ErrorHandlingTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }

        protected override void RunTest()
        {
            var mockApp = (MockCppApplication)Application;
            mockApp.SimulateError();
        }
    }
} 