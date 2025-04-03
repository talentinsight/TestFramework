using System.Collections.Generic;

namespace TestFramework.Core.Interfaces
{
    /// <summary>
    /// Interface for test configuration
    /// </summary>
    public interface ITestConfiguration
    {
        /// <summary>
        /// Gets the configuration dictionary
        /// </summary>
        Dictionary<string, string> Configuration { get; }

        /// <summary>
        /// Initializes the configuration
        /// </summary>
        /// <param name="config">Configuration data</param>
        void Initialize(Dictionary<string, string> config);
    }
} 