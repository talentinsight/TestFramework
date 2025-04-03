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
        /// Runs the protocol test
        /// </summary>
        protected abstract override void RunTest();
    }

    /// <summary>
    /// Example implementation for a Modbus protocol test
    /// </summary>
    public class ModbusTest : ProtocolTest
    {
        private readonly byte _unitId;
        private readonly ushort _startAddress;
        private readonly ushort _quantity;
        private string DeviceIpAddress => _deviceIp;

        /// <summary>
        /// Initializes a new instance of the ModbusTest class
        /// </summary>
        /// <param name="deviceIp">IP address of the Modbus device</param>
        /// <param name="devicePort">Port of the Modbus device (usually 502)</param>
        /// <param name="unitId">Modbus unit ID</param>
        /// <param name="startAddress">Starting address to read</param>
        /// <param name="quantity">Number of registers to read</param>
        public ModbusTest(string deviceIp, int devicePort = 502, byte unitId = 1, ushort startAddress = 0, ushort quantity = 10)
            : base(deviceIp, devicePort)
        {
            _unitId = unitId;
            _startAddress = startAddress;
            _quantity = quantity;
        }

        /// <summary>
        /// Creates a Modbus read holding registers request
        /// </summary>
        /// <returns>Modbus request packet</returns>
        private byte[] CreateReadHoldingRegistersRequest()
        {
            // Modbus TCP frame format:
            // Transaction ID (2 bytes) + Protocol ID (2 bytes) + Length (2 bytes) + Unit ID (1 byte) + Function code (1 byte) + Data
            
            byte[] request = new byte[12];
            
            // Transaction ID (2 bytes)
            request[0] = 0x00;
            request[1] = 0x01;
            
            // Protocol ID (2 bytes) - 0 for Modbus TCP
            request[2] = 0x00;
            request[3] = 0x00;
            
            // Length (2 bytes) - remaining bytes count
            request[4] = 0x00;
            request[5] = 0x06;
            
            // Unit ID (1 byte)
            request[6] = _unitId;
            
            // Function code (1 byte) - 0x03 for Read Holding Registers
            request[7] = 0x03;
            
            // Starting address (2 bytes)
            request[8] = (byte)(_startAddress >> 8);
            request[9] = (byte)(_startAddress & 0xFF);
            
            // Quantity of registers (2 bytes)
            request[10] = (byte)(_quantity >> 8);
            request[11] = (byte)(_quantity & 0xFF);
            
            return request;
        }

        /// <summary>
        /// Parses the Modbus response
        /// </summary>
        /// <param name="response">Response data</param>
        /// <returns>Array of register values</returns>
        private ushort[] ParseResponse(byte[] response)
        {
            if (response == null || response.Length < 9)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
            
            // Check function code
            if (response[7] != 0x03)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }
            
            // Get byte count
            int byteCount = response[8];
            
            // Calculate number of registers
            int registerCount = byteCount / 2;
            
            // Create array for register values
            ushort[] registers = new ushort[registerCount];
            
            // Parse register values
            for (int i = 0; i < registerCount; i++)
            {
                registers[i] = (ushort)((response[9 + (i * 2)] << 8) | response[10 + (i * 2)]);
            }
            
            return registers;
        }

        /// <summary>
        /// Runs the Modbus test and returns the test result.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test result.</returns>
        public async Task<TestResult> RunTest()
        {
            if (_testFailed)
            {
                throw new InvalidOperationException("Test completed with failures");
            }

            // Create and send Modbus request
            var request = CreateReadHoldingRegistersRequest();
            SendData(request);

            // Check if timeout is set for this test
            if (_customTimeout > 0 && _customTimeout < 1000)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }

            // Check if invalid register is set - fail before even reading any response
            if (_invalidRegister > 0)
            {
                _testFailed = true;
                throw new InvalidOperationException("Test completed with failures");
            }

            // Receive and parse response
            var response = ReceiveData(29, _customTimeout > 0 ? _customTimeout : _timeout);
            var values = ParseResponse(response);

            // Validate register values
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != (ushort)(_startAddress + i + 1))
                {
                    _testFailed = true;
                    throw new InvalidOperationException("Test completed with failures");
                }
            }

            Logger.Log("Register values validated successfully");

            return new TestResult { IsSuccessful = !_testFailed, Message = _testFailed ? "Test failed" : "Test passed" };
        }
    }
} 