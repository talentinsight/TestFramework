using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Recovery
{
    /// <summary>
    /// Recovery action for restarting applications
    /// </summary>
    public class ApplicationRestartAction : IRecoveryAction
    {
        private readonly ILogger _logger;
        private readonly string _applicationName;

        /// <summary>
        /// Initializes a new instance of the ApplicationRestartAction class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="applicationName">The name of the application to restart</param>
        public ApplicationRestartAction(ILogger logger, string applicationName)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        }

        /// <summary>
        /// Executes the application restart action
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            try
            {
                // Simulate application restart
                await Task.Delay(1000); // Simulate restart delay
                
                _logger.Log($"Successfully restarted application: {_applicationName}", LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to restart application {_applicationName}: {ex.Message}", LogLevel.Error);
                throw;
            }
        }
    }
} 