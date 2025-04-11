namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Represents the severity level of a log message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
<<<<<<< HEAD
=======
        /// Trace level for detailed diagnostic information
        /// </summary>
        Trace,

        /// <summary>
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
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
<<<<<<< HEAD
        Error
=======
        Error,

        /// <summary>
        /// Fatal level for very severe error events that will presumably lead the application to abort
        /// </summary>
        Fatal,

        /// <summary>
        /// Critical level for very severe error events that will presumably lead the application to abort
        /// </summary>
        Critical
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
    }
}

/* 
 * Defines the LogLevel enum
 * Defines Info, Warning, Error enum values
 */     