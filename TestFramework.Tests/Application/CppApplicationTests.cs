using NUnit.Framework;
using TestFramework.Core;
using TestFramework.Core.Application;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class CppApplicationTests : CppApplicationTest
    {
        private const string TestVersion = "2.0.0";

        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication(TestVersion);
        }

        protected override void RunTest()
        {
            // Basic test implementation - will be overridden by specific test methods
        }

        [Test]
        public void WhenApplicationInitialized_ShouldReturnCorrectVersion()
        {
            // Arrange
            var test = new VersionCheckTest(TestVersion);
            
            // Act
            var result = test.Execute();
            
            // Assert
            NUnit.Framework.Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public void WhenApplicationErrors_ShouldFailGracefully()
        {
            // Arrange
            var test = new ErrorHandlingTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            NUnit.Framework.Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
            NUnit.Framework.Assert.That(result.Message, Does.Contain("Simulated application error"));
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