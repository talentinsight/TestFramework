namespace TestFramework.Core.Models
{
    /// <summary>
    /// Represents the category of a test
    /// </summary>
    public enum TestCategory
    {
        /// <summary>
        /// Unit test category
        /// </summary>
        Unit,

        /// <summary>
        /// Integration test category
        /// </summary>
        Integration,

        /// <summary>
        /// UI test category
        /// </summary>
        UI,

        /// <summary>
        /// API test category
        /// </summary>
        API,

        /// <summary>
        /// Performance test category
        /// </summary>
        Performance,

        /// <summary>
        /// Load test category
        /// </summary>
        Load
    }
} 