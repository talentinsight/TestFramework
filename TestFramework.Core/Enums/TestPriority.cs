namespace TestFramework.Core.Enums
{
    /// <summary>
    /// Represents the priority level of a test
    /// </summary>
    public enum TestPriority
    {
        /// <summary>
        /// Critical priority - must be run first
        /// </summary>
        Critical = 0,

        /// <summary>
        /// High priority - should be run early
        /// </summary>
        High = 1,

        /// <summary>
        /// Medium priority - normal execution order
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Low priority - can be run last
        /// </summary>
        Low = 3
    }
} 