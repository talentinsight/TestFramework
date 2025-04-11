using System.Threading.Tasks;

namespace TestFramework.Core.Recovery
{
    /// <summary>
    /// Defines the interface for test recovery actions
    /// </summary>
    public interface IRecoveryAction
    {
        /// <summary>
        /// Executes the recovery action asynchronously
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteAsync();
    }
} 