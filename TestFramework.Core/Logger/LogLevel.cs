namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Represents the severity level of a log message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug level for detailed diagnostic information
        /// </summary>
        Debug,

        /// <summary>
        /// Information level for general operational messages
        /// </summary>
        Info,

        /// <summary>
        /// Warning level for potentially harmful situations
        /// </summary>
        Warning,

        /// <summary>
        /// Error level for error events that might still allow the application to continue running
        /// </summary>
        Error,

        /// <summary>
        /// Fatal level for very severe error events that will presumably lead the application to abort
        /// </summary>
        Fatal
    }
}

/* 
 * Defines the LogLevel enum
 * Defines Info, Warning, Error enum values
 */     