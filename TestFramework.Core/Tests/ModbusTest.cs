using System;
using System.Net.Sockets;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Tests
{
    public class ModbusTest : ProtocolTest
    {
        private readonly byte _unitId;
        private readonly ushort _startAddress;
        private readonly ushort _quantity;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the ModbusTest class
        /// </summary>
        /// <param name="deviceIp">IP address of the Modbus device</param>
        /// <param name="devicePort">Port of the Modbus device</param>
        /// <param name="unitId">Unit ID of the Modbus device</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="loggerType">Type of logger to use</param>
        public ModbusTest(string deviceIp, int devicePort, byte unitId = 1, int timeout = 5000, LoggerType loggerType = LoggerType.Console)
            : base(deviceIp, devicePort, timeout, loggerType)
        {
            _unitId = unitId;
            _startAddress = 0;
            _quantity = 10;
            _disposed = false;
        }

        /// <summary>
        /// Reads a holding register
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Register value</returns>
        public ushort ReadHoldingRegister(ushort address)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModbusTest));

            if (_testFailed)
                throw new InvalidOperationException("Test completed with failures");

            try
            {
                var request = CreateReadHoldingRegistersRequest(address, 1);
                SendData(request);
                var response = ReceiveData(9, _timeout);
                var values = ParseResponse(response);
                return values[0];
            }
            catch (Exception ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Error reading register {address}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Writes a holding register
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Register value</param>
        public void WriteHoldingRegister(ushort address, ushort value)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModbusTest));

            if (_testFailed)
                throw new InvalidOperationException("Test completed with failures");

            try
            {
                var request = CreateWriteSingleRegisterRequest(address, value);
                SendData(request);
                var response = ReceiveData(12, _timeout);
                ValidateWriteResponse(response, address, value);
            }
            catch (Exception ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Error writing register {address}: {ex.Message}", ex);
            }
        }

        private byte[] CreateReadHoldingRegistersRequest(ushort startAddress, ushort quantity)
        {
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
            request[8] = (byte)(startAddress >> 8);
            request[9] = (byte)(startAddress & 0xFF);
            
            // Quantity of registers (2 bytes)
            request[10] = (byte)(quantity >> 8);
            request[11] = (byte)(quantity & 0xFF);
            
            return request;
        }

        private byte[] CreateWriteSingleRegisterRequest(ushort address, ushort value)
        {
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
            
            // Function code (1 byte) - 0x06 for Write Single Register
            request[7] = 0x06;
            
            // Register address (2 bytes)
            request[8] = (byte)(address >> 8);
            request[9] = (byte)(address & 0xFF);
            
            // Register value (2 bytes)
            request[10] = (byte)(value >> 8);
            request[11] = (byte)(value & 0xFF);
            
            return request;
        }

        private ushort[] ParseResponse(byte[] response)
        {
            if (response == null || response.Length < 9)
            {
                throw new InvalidOperationException("Invalid response length");
            }
            
            // Check function code
            if (response[7] != 0x03)
            {
                throw new InvalidOperationException($"Invalid function code: {response[7]}");
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

        private void ValidateWriteResponse(byte[] response, ushort address, ushort value)
        {
            if (response == null || response.Length != 12)
            {
                throw new InvalidOperationException("Invalid response length");
            }
            
            // Check function code
            if (response[7] != 0x06)
            {
                throw new InvalidOperationException($"Invalid function code: {response[7]}");
            }
            
            // Check address
            ushort responseAddress = (ushort)((response[8] << 8) | response[9]);
            if (responseAddress != address)
            {
                throw new InvalidOperationException($"Address mismatch: expected {address}, got {responseAddress}");
            }
            
            // Check value
            ushort responseValue = (ushort)((response[10] << 8) | response[11]);
            if (responseValue != value)
            {
                throw new InvalidOperationException($"Value mismatch: expected {value}, got {responseValue}");
            }
        }

        protected override void RunTest()
        {
            if (_testFailed)
            {
                throw new InvalidOperationException("Test completed with failures");
            }

            try
            {
                var request = CreateReadHoldingRegistersRequest(_startAddress, _quantity);
                SendData(request);
                var response = ReceiveData(9 + (_quantity * 2), _timeout);
                var values = ParseResponse(response);

                // Validate register values
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] != (ushort)(_startAddress + i + 1))
                    {
                        _testFailed = true;
                        throw new InvalidOperationException($"Value mismatch at address {_startAddress + i}: expected {_startAddress + i + 1}, got {values[i]}");
                    }
                }

                Logger.Log("Register values validated successfully");
            }
            catch (Exception ex)
            {
                _testFailed = true;
                throw new InvalidOperationException($"Test failed: {ex.Message}", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            base.Dispose(disposing);
            _disposed = true;
        }
    }
} 