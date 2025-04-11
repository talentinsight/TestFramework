namespace TestFramework.Core.Models
{
    /// <summary>
    /// Enum representing test execution status
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// Test passed successfully
        /// </summary>
        Passed,

        /// <summary>
        /// Test failed
        /// </summary>
        Failed,

        /// <summary>
        /// Test was skipped
        /// </summary>
        Skipped,

        /// <summary>
        /// Test is not applicable in current environment
        /// </summary>
        NotApplicable
    }
} 