using System.Threading.Tasks;
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;
using TestFramework.Tests.Tests;
using Xunit;

namespace TestFramework.Tests.Integration
{
    public class ModbusTests
    {
        private readonly MockLogger _logger;
        private readonly ProtocolTest _test;

        public ModbusTests()
        {
            _logger = new MockLogger();
            _test = new ProtocolTest(_logger, "modbus", "localhost", 502);
        }

        [Fact]
        public async Task WhenModbusErrorOccurs_ErrorIsReported()
        {
            // Act
            var result = await _test.RunAsync();

            // Assert
            Assert.False(result);
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Protocol test failed"));
        }
    }
} 