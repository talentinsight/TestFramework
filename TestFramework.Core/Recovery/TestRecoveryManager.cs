using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Recovery
{
    /// <summary>
    /// Manages test recovery operations
    /// </summary>
    public class TestRecoveryManager : IDisposable
    {
        private readonly ILogger _logger;
        private readonly List<IRecoveryAction> _recoveryActions;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the TestRecoveryManager class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public TestRecoveryManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recoveryActions = new List<IRecoveryAction>();
        }

        /// <summary>
        /// Adds a recovery action to the manager
        /// </summary>
        /// <param name="action">The recovery action to add</param>
        public void AddRecoveryAction(IRecoveryAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _recoveryActions.Add(action);
            _logger.Log($"Added recovery action: {action.GetType().Name}", LogLevel.Info);
        }

        /// <summary>
        /// Executes all recovery actions
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteRecoveryAsync()
        {
            _logger.Log("Starting test recovery...", LogLevel.Info);

            foreach (var action in _recoveryActions)
            {
                try
                {
                    await action.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.Log($"Failed to execute recovery action {action.GetType().Name}: {ex.Message}", LogLevel.Error);
                    // Continue with other actions even if one fails
                }
            }

            _logger.Log("Test recovery completed", LogLevel.Info);
        }

        /// <summary>
        /// Executes all recovery actions
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<bool> RecoverAsync()
        {
            try
            {
                await ExecuteRecoveryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Recovery failed: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Clears all recovery actions
        /// </summary>
        public void ClearRecoveryActions()
        {
            _recoveryActions.Clear();
            _logger.Log("Cleared all recovery actions", LogLevel.Info);
        }

        /// <summary>
        /// Disposes the TestRecoveryManager
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the TestRecoveryManager
        /// </summary>
        /// <param name="disposing">True if called from Dispose, false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ClearRecoveryActions();
            }

            _disposed = true;
        }
    }
} 