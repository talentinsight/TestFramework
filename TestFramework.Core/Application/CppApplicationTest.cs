using System;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Base class for all C++ application tests
    /// </summary>
    public abstract class CppApplicationTest : TestBase
    {
        /// <summary>
        /// Gets the C++ application instance being tested
        /// </summary>
        protected ICppApplication Application { get; private set; }

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
} 