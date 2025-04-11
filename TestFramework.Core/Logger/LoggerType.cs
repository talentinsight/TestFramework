namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Represents the type of logger to use
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// Console logger that writes to standard output
        /// </summary>
        Console,

        /// <summary>
        /// File logger that writes to a file
        /// </summary>
        File,

        /// <summary>
        /// Mock logger for testing purposes
        /// </summary>
        Mock
    }
} 