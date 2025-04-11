using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestFramework.Core.Logger;
using System.Threading;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Base class for industrial protocol testing (e.g., Modbus, OPC UA, etc.)
    /// </summary>
    public abstract class ProtocolTest : TestBase, IDisposable
    {
        protected readonly string _deviceIp;
        protected readonly int _devicePort;
        protected TcpClient _client;
        protected int _timeout;
        protected int _customTimeout;
        protected bool _simulateConnectionLoss;
        protected bool _invalidDataFormat;
        protected ushort _invalidRegister;
        protected bool _testFailed;
        protected bool _isInitialized;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the ProtocolTest class
        /// </summary>
        /// <param name="deviceIp">IP address of the device to test</param>
        /// <param name="devicePort">Port of the device to test</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="loggerType">Type of logger to use</param>
        protected ProtocolTest(string deviceIp, int devicePort, int timeout = 5000, LoggerType loggerType = LoggerType.Console)
            : base(loggerType)
        {
            _deviceIp = deviceIp;
            _devicePort = devicePort;
            _timeout = timeout;
            _customTimeout = 0;
            _client = new TcpClient();
            _testFailed = false;
            _isInitialized = false;
            _disposed = false;
        }

        /// <summary>
        /// Initializes the test
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            Logger.Log($"Initialized protocol test for {_deviceIp}:{_devicePort}");
        }

        /// <summary>
        /// Gets or sets the TcpClient for communication
        /// </summary>
        protected TcpClient? Client { get; set; }

        /// <summary>
        /// Setup method to connect to the device
        /// </summary>
        protected override void Setup()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Test must be initialized before setup");
            }

            try
            {
                _client = new TcpClient();
                var connectTask = _client.ConnectAsync(_deviceIp, _devicePort);
                
                if (!connectTask.Wait(_timeout))
                {
                    _testFailed = true;
                    throw new TimeoutException($"Connection to {_deviceIp}:{_devicePort} timed out after {_timeout}ms");
                }
                
                _testFailed = false;

                if (_timeout > 0)
                {
                    _client.ReceiveTimeout = _timeout;
                    _client.SendTimeout = _timeout;
                }

                if (!_client.Connected)
                {
                    _testFailed = true;
                    throw new SocketException((int)SocketError.NotConnected);
                }
            }
            catch (SocketException ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Socket error while connecting to {_deviceIp}:{_devicePort}: {ex.Message}", ex);
            }
            catch (TimeoutException ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Connection timeout to {_deviceIp}:{_devicePort}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Unexpected error while connecting to {_deviceIp}:{_devicePort}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// TearDown method to disconnect from the device
        /// </summary>
        protected override void TearDown()
        {
            try
            {
                if (_client?.Connected == true)
                {
                    _client.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error during cleanup: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes the test resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    if (_client != null)
                    {
                        if (_client.Connected)
                        {
                            _client.Close();
                        }
                        _client.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error disposing resources: {ex.Message}");
                }
            }

            _disposed = true;
        }

        ~ProtocolTest()
        {
            Dispose(false);
        }

        protected void SendData(byte[] data)
        {
            if (_testFailed)
            {
                throw new InvalidOperationException("Test completed with failures");
            }

            if (_simulateConnectionLoss)
            {
                _client.Close();
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }

            if (_invalidDataFormat)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }

            try
            {
                _client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
        }

        protected byte[] ReceiveData(int expectedLength, int timeoutMs)
        {
            if (_testFailed)
            {
                throw new InvalidOperationException("Test completed with failures");
            }

            try
            {
                var buffer = new byte[expectedLength];
                var stream = _client.GetStream();
                stream.ReadTimeout = timeoutMs;
                
                int bytesRead = stream.Read(buffer, 0, expectedLength);
                if (bytesRead != expectedLength)
                {
                    _testFailed = true;
                    throw new InvalidOperationException("Test completed with failures");
                }
                
                return buffer;
            }
            catch (Exception)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
        }

        public void SetTimeout(int timeoutMs)
        {
            _customTimeout = timeoutMs;
            if (_client != null && _client.Connected)
            {
                _client.ReceiveTimeout = timeoutMs;
                _client.SendTimeout = timeoutMs;
            }
        }

        public void SetInvalidRegister(ushort register)
        {
            _invalidRegister = register;
        }

        public void SimulateConnectionLoss()
        {
            _simulateConnectionLoss = true;
        }

        public void SetInvalidDataFormat()
        {
            _invalidDataFormat = true;
        }

        protected abstract override void RunTest();
    }
} 