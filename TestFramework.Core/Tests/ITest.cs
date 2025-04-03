using System.Threading.Tasks;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Base interface for all test types
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Gets the name of the test
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the test
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the category of the test
        /// </summary>
        TestCategory Category { get; }

        /// <summary>
        /// Gets the priority of the test
        /// </summary>
        TestPriority Priority { get; }

        /// <summary>
        /// Executes the test
        /// </summary>
        /// <returns>Test result</returns>
        Task<TestResult> ExecuteAsync();

        /// <summary>
        /// Sets up the test environment
        /// </summary>
        Task SetupAsync();

        /// <summary>
        /// Cleans up the test environment
        /// </summary>
        Task CleanupAsync();
    }
} 