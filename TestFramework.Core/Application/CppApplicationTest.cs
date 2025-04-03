using System;
using System.Threading.Tasks;
using TestFramework.Core.Models;
using TestFramework.Core.Utils;
using TestFramework.Core.Logger;
using NUnit.Framework;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Base class for all C++ application tests
    /// </summary>
    public abstract class CppApplicationTest : TestBase
    {
        /// <summary>
        /// Gets or sets the application instance
        /// </summary>
        public ICppApplication? Application { get; set; }

        /// <summary>
        /// Initializes a new instance of the CppApplicationTest class
        /// </summary>
        /// <param name="loggerType">Type of logger to use</param>
        protected CppApplicationTest(LoggerType loggerType = LoggerType.Console) 
            : base(loggerType)
        {
        }

        /// <summary>
        /// Sets up the test by initializing the C++ application
        /// </summary>
        protected override void Setup()
        {
            base.Setup();
            
            Logger.Log("Initializing C++ application...");
            Application = CreateApplication();
            
            if (!Application.Initialize())
            {
                throw new ApplicationException("Failed to initialize C++ application");
            }
            
            Logger.Log($"C++ application initialized successfully. Version: {Application.GetApplicationVersion()}");
        }

        /// <summary>
        /// Tears down the test by shutting down the C++ application
        /// </summary>
        protected override void TearDown()
        {
            if (Application != null)
            {
                Logger.Log("Shutting down C++ application...");
                Application.Shutdown();
                Logger.Log("C++ application shut down successfully");
            }
            
            base.TearDown();
        }

        /// <summary>
        /// Creates an instance of the C++ application to test
        /// </summary>
        /// <returns>Instance of ICppApplication</returns>
        protected abstract ICppApplication CreateApplication();
    }

    [TestFixture]
    public class CppApplicationTest
    {
        private ICppApplication _application;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new ConsoleLogger();
            _application = new CppApplication(_logger);
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
        public void TearDown()
        {
            _application.Dispose();
        }
    }
} 