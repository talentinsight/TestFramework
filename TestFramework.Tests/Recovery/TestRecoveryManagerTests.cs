using System;
using System.Threading.Tasks;
using TestFramework.Core.Recovery;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Recovery
{
    public class TestRecoveryManagerTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly TestRecoveryManager _recoveryManager;

        public TestRecoveryManagerTests()
        {
            _logger = new MockLogger();
            _recoveryManager = new TestRecoveryManager(_logger);
        }

        public void Dispose()
        {
            _recoveryManager.Dispose();
        }

        [Fact]
        public async Task WhenRecoveryIsNeeded_RecoveryIsExecuted()
        {
            // Arrange
            var action = new MockRecoveryAction("test1", true);
            _recoveryManager.AddRecoveryAction(action);

            // Act
            var result = await _recoveryManager.RecoverAsync();

            // Assert
            Assert.True(result);
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Recovery executed"));
        }

        [Fact]
        public async Task WhenRecoveryFails_ReturnsFalse()
        {
            // Arrange
            var action = new MockRecoveryAction("test1", false);
            _recoveryManager.AddRecoveryAction(action);

            // Act
            var result = await _recoveryManager.RecoverAsync();

            // Assert
            Assert.False(result);
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Recovery failed"));
        }

        [Fact]
        public async Task WhenMultipleRecoveriesAreNeeded_AllAreExecuted()
        {
            // Arrange
            var action1 = new MockRecoveryAction("test1", true);
            var action2 = new MockRecoveryAction("test2", true);
            _recoveryManager.AddRecoveryAction(action1);
            _recoveryManager.AddRecoveryAction(action2);

            // Act
            var result = await _recoveryManager.RecoverAsync();

            // Assert
            Assert.True(result);
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Recovery executed"));
        }
    }

    public class MockRecoveryAction : IRecoveryAction
    {
        private readonly string _id;
        private readonly bool _shouldSucceed;

        public MockRecoveryAction(string id, bool shouldSucceed)
        {
            _id = id;
            _shouldSucceed = shouldSucceed;
        }

        public async Task ExecuteAsync()
        {
            await Task.Delay(100); // Simulate recovery action
            if (!_shouldSucceed)
            {
                throw new Exception($"Recovery failed for test {_id}");
            }
        }
    }
} 