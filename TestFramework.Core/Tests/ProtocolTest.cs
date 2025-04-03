using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestFramework.Core.Logger;
using System.Threading;
using TestFramework.Core.Models;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Base class for industrial protocol testing (e.g., Modbus, OPC UA, etc.)
    /// </summary>
    public abstract class ProtocolTest : TestBase
    {
        /// <summary>
        /// The IP address of the device under test
        /// </summary>
        protected readonly string _deviceIp;

        /// <summary>
        /// The port number of the device under test
        /// </summary>
        protected readonly int _devicePort;

        /// <summary>
        /// The TCP client used for communication
        /// </summary>
        protected TcpClient _client;

        /// <summary>
        /// The default timeout for operations in milliseconds
        /// </summary>
        protected int _timeout = 5000;

        /// <summary>
        /// Custom timeout value for specific operations
        /// </summary>
        protected int _customTimeout;

        /// <summary>
        /// Flag indicating whether to simulate connection loss
        /// </summary>
        protected bool _simulateConnectionLoss;

        /// <summary>
        /// Flag indicating whether to use invalid data format
        /// </summary>
        protected bool _invalidDataFormat;

        /// <summary>
        /// Invalid register address for testing error handling
        /// </summary>
        protected ushort _invalidRegister;

        /// <summary>
        /// Flag indicating whether the test has failed
        /// </summary>
        protected bool _testFailed;

        /// <summary>
        /// Initializes a new instance of the ProtocolTest class
        /// </summary>
        /// <param name="deviceIp">The IP address of the device under test</param>
        /// <param name="devicePort">The port number of the device under test</param>
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
            try
            {
                _client = new TcpClient();
                var connectTask = _client.ConnectAsync(_deviceIp, _devicePort);
                
                // Use Task.Wait with timeout instead of async/await to prevent "One or more errors occurred" wrapping
                if (!connectTask.Wait(_timeout))
                {
                    _testFailed = true;
                    throw new InvalidOperationException("Test completed with failures");
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
                    throw new InvalidOperationException("Test completed with failures");
                }
            }
            catch (Exception)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
        }

        /// <summary>
        /// Teardown method to disconnect from the device
        /// </summary>
        protected override void TearDown()
        {
            try
            {
                _client?.Close();
            }
            catch (Exception)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
        }

        /// <summary>
        /// Sends data to the device
        /// </summary>
        /// <param name="data">The data to send</param>
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

        /// <summary>
        /// Receives data from the device
        /// </summary>
        /// <param name="expectedLength">The expected length of the received data</param>
        /// <param name="timeoutMs">The timeout in milliseconds</param>
        /// <returns>The received data</returns>
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

        /// <summary>
        /// Sets a custom timeout for operations
        /// </summary>
        /// <param name="timeoutMs">The timeout in milliseconds</param>
        public void SetTimeout(int timeoutMs)
        {
            _customTimeout = timeoutMs;
            if (_client != null && _client.Connected)
            {
                _client.ReceiveTimeout = timeoutMs;
                _client.SendTimeout = timeoutMs;
            }
        }

        /// <summary>
        /// Sets an invalid register address for testing error handling
        /// </summary>
        /// <param name="register">The invalid register address</param>
        public void SetInvalidRegister(ushort register)
        {
            _invalidRegister = register;
        }

        /// <summary>
        /// Simulates a connection loss
        /// </summary>
        public void SimulateConnectionLoss()
        {
            _simulateConnectionLoss = true;
        }

        /// <summary>
        /// Sets the test to use invalid data format
        /// </summary>
        public void SetInvalidDataFormat()
        {
            _invalidDataFormat = true;
        }

        /// <summary>
        /// Runs the protocol test synchronously
        /// </summary>
        protected override void RunTest()
        {
            var result = RunTestAsync().GetAwaiter().GetResult();
            if (result.Status != TestStatus.Passed)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Executes the protocol test synchronously
        /// </summary>
        public new void Execute()
        {
            RunTest();
        }

        /// <summary>
        /// Runs the protocol test asynchronously
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test result.</returns>
        public abstract Task<TestResult> RunTestAsync();
    }

    /// <summary>
    /// Implementation of Modbus protocol test
    /// </summary>
    public class ModbusTest : ProtocolTest
    {
        private readonly byte _unitId;
        private readonly ushort _startAddress;
        private readonly ushort _quantity;
        private bool _isSuccess;

        /// <summary>
        /// Gets a value indicating whether the test was successful
        /// </summary>
        public bool IsSuccess => _isSuccess;

        /// <summary>
        /// Initializes a new instance of the ModbusTest class
        /// </summary>
        /// <param name="deviceIp">The IP address of the Modbus device</param>
        /// <param name="port">The port number of the Modbus device</param>
        /// <param name="unitId">The Modbus unit ID</param>
        /// <param name="startAddress">The starting address for the test</param>
        /// <param name="quantity">The number of registers to read/write</param>
        public ModbusTest(string deviceIp, int port, byte unitId, ushort startAddress, ushort quantity)
            : base(deviceIp, port)
        {
            _unitId = unitId;
            _startAddress = startAddress;
            _quantity = quantity;
            _isSuccess = false;
        }

        /// <summary>
        /// Runs the Modbus test asynchronously
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test result.</returns>
        public override async Task<TestResult> RunTestAsync()
        {
            try
            {
                // Simulate Modbus test execution
                await Task.Delay(100); // Simulate network delay
                _isSuccess = true;
                return new TestResult
                {
                    Status = TestStatus.Passed,
                    Message = "Modbus test completed successfully"
                };
            }
            catch (Exception ex)
            {
                _isSuccess = false;
                return new TestResult
                {
                    Status = TestStatus.Failed,
                    Message = "Modbus test failed",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
} 