using System;
using System.Collections.Generic;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Interface for interacting with C++ applications under test
    /// </summary>
    public interface ICppApplication
    {
        /// <summary>
        /// Initializes the C++ application
        /// </summary>
        /// <returns>True if initialization successful, false otherwise</returns>
        bool Initialize();

        /// <summary>
        /// Shuts down the C++ application
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Gets whether the application is currently running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the version of the C++ application
        /// </summary>
        /// <returns>Version string of the application</returns>
        string GetApplicationVersion();

        /// <summary>
        /// Loads configuration from the specified file
        /// </summary>
        /// <param name="configPath">Path to the configuration file</param>
        /// <returns>True if configuration loaded successfully, false otherwise</returns>
        bool LoadConfiguration(string configPath);

        /// <summary>
        /// Validates the current configuration
        /// </summary>
        /// <returns>True if configuration is valid, false otherwise</returns>
        bool ValidateConfiguration();

        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value or null if not found</returns>
        string GetConfigurationValue(string key);

        /// <summary>
        /// Gets all configuration values
        /// </summary>
        /// <returns>Dictionary of configuration key-value pairs</returns>
        Dictionary<string, string> GetAllConfigurationValues();

        // Service Management

        /// <summary>
        /// Starts a specific service within the application
        /// </summary>
        /// <param name="serviceName">The name of the service to start</param>
        /// <returns>True if service started successfully, false otherwise</returns>
        bool StartService(string serviceName);

        /// <summary>
        /// Stops a specific service within the application
        /// </summary>
        /// <param name="serviceName">The name of the service to stop</param>
        /// <returns>True if service stopped successfully, false otherwise</returns>
        bool StopService(string serviceName);

        /// <summary>
        /// Gets the status of a specific service
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>True if service is running, false otherwise</returns>
        bool IsServiceRunning(string serviceName);

        /// <summary>
        /// Gets a list of all available services
        /// </summary>
        /// <returns>List of service names</returns>
        List<string> GetAvailableServices();

        // Log Management

        /// <summary>
        /// Sets the logging level for the application
        /// </summary>
        /// <param name="level">Log level (e.g., DEBUG, INFO, WARNING, ERROR)</param>
        /// <returns>True if log level was set successfully, false otherwise</returns>
        bool SetLogLevel(string level);

        /// <summary>
        /// Gets the current logging level
        /// </summary>
        /// <returns>Current log level</returns>
        string GetLogLevel();

        /// <summary>
        /// Retrieves a specified number of recent log entries
        /// </summary>
        /// <param name="count">Maximum number of entries to retrieve</param>
        /// <returns>List of log entries</returns>
        List<string> GetRecentLogs(int count);

        /// <summary>
        /// Clears all log entries
        /// </summary>
        /// <returns>True if logs were cleared successfully, false otherwise</returns>
        bool ClearLogs();

        // Error State Management

        /// <summary>
        /// Gets the last error that occurred in the application
        /// </summary>
        /// <returns>Error message or null if no error</returns>
        string GetLastError();

        /// <summary>
        /// Clears the error state of the application
        /// </summary>
        /// <returns>True if error state was cleared successfully, false otherwise</returns>
        bool ClearErrorState();

        /// <summary>
        /// Gets whether the application is in an error state
        /// </summary>
        bool IsInErrorState { get; }

        /// <summary>
        /// Gets the list of all errors since initialization
        /// </summary>
        /// <returns>List of error messages</returns>
        List<string> GetErrorHistory();
    }
} 