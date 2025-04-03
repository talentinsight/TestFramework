namespace TestFramework.Core.Enums
{
    /// <summary>
    /// Represents different categories of tests
    /// </summary>
    public enum TestCategory
    {
        /// <summary>
        /// Unit tests that verify individual components
        /// </summary>
        Unit,

        /// <summary>
        /// Integration tests that verify component interactions
        /// </summary>
        Integration,

        /// <summary>
        /// Performance tests that measure system performance
        /// </summary>
        Performance,

        /// <summary>
        /// Load tests that verify system behavior under load
        /// </summary>
        Load,

        /// <summary>
        /// Security tests that verify system security
        /// </summary>
        Security,

        /// <summary>
        /// UI tests that verify user interface functionality
        /// </summary>
        UI,

        /// <summary>
        /// API tests that verify API functionality
        /// </summary>
        API,

        /// <summary>
        /// Database tests that verify database operations
        /// </summary>
        Database,

        /// <summary>
        /// Other category of tests
        /// </summary>
        Other
    }
} 