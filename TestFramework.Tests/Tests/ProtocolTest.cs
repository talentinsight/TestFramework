using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Tests
{
    public class ProtocolTest : BaseTest
    {
        private readonly string _protocol;
        private readonly string _host;
        private readonly int _port;

        public ProtocolTest(ILogger logger, string protocol = "tcp", string host = "localhost", int port = 502) 
            : base(logger)
        {
            _protocol = protocol;
            _host = host;
            _port = port;
        }

        public override async Task<bool> RunAsync()
        {
            if (!await base.RunAsync())
            {
                return false;
            }

            try
            {
                _logger.Log($"Running protocol test using {_protocol}://{_host}:{_port}", LogLevel.Info);
                await Task.Delay(200); // Simulate protocol communication
                _logger.Log("Protocol test completed", LogLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Protocol test failed: {ex.Message}";
                _logger.Log(_lastError, LogLevel.Error);
                _isInErrorState = true;
                return false;
            }
        }
    }
} 