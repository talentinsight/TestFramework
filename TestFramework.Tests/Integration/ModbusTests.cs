<<<<<<< HEAD
using System.Threading.Tasks;
=======
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework.Core.Application;
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;
using TestFramework.Tests.Tests;
using Xunit;

namespace TestFramework.Tests.Integration
{
<<<<<<< HEAD
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
=======
    [TestClass]
    public class ModbusTests
    {
        private MockModbusSlave _slave = null!;
        private MockModbusMaster _master = null!;
        private const string TestHost = "127.0.0.1";
        private const int TestPort = 502;

        [TestInitialize]
        public void Setup()
        {
            _slave = new MockModbusSlave();
            _master = new MockModbusMaster();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _slave?.Dispose();
            _master?.Dispose();
        }

        [TestMethod]
        public async Task ReadHoldingRegisters_ValidAddress_ReturnsCorrectValues()
        {
            // Arrange
            ushort startingAddress = 0;
            ushort quantity = 2;
            var expectedValues = new ushort[] { 100, 200 };

            // Act
            var values = await _master.ReadHoldingRegistersAsync(startingAddress, quantity);

            // Assert
            CollectionAssert.AreEqual(expectedValues, values);
        }

        [TestMethod]
        public async Task WriteSingleRegister_ValidAddressAndValue_UpdatesRegister()
        {
            // Arrange
            ushort address = 0;
            ushort value = 100;

            // Act
            await _master.WriteSingleRegisterAsync(address, value);

            // Assert
            var values = await _master.ReadHoldingRegistersAsync(address, 1);
            Assert.AreEqual(value, values[0]);
        }

        [TestMethod]
        public async Task WriteMultipleRegisters_ValidAddressAndValues_UpdatesRegisters()
        {
            // Arrange
            ushort startingAddress = 0;
            var values = new ushort[] { 100, 200, 300 };

            // Act
            await _master.WriteMultipleRegistersAsync(startingAddress, values);

            // Assert
            var readValues = await _master.ReadHoldingRegistersAsync(startingAddress, (ushort)values.Length);
            CollectionAssert.AreEqual(values, readValues);
        }

        [TestMethod]
        public async Task ReadInputRegisters_ValidAddress_ReturnsCorrectValues()
        {
            // Arrange
            ushort startAddress = 0;
            ushort numberOfRegisters = 5;
            ushort[] expectedValues = new ushort[] { 100, 200, 300, 400, 500 };

            // Act
            ushort[] result = await _master.ReadInputRegistersAsync(startAddress, numberOfRegisters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfRegisters, result.Length);
            CollectionAssert.AreEqual(expectedValues, result);
        }

        [TestMethod]
        public async Task ReadCoils_ValidAddress_ReturnsCorrectValues()
        {
            // Arrange
            ushort startAddress = 0;
            ushort numberOfCoils = 8;
            bool[] expectedValues = new bool[] { true, false, true, false, true, false, true, false };
            
            // Act
            bool[] result = await _master.ReadCoilsAsync(startAddress, numberOfCoils);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfCoils, result.Length);
            CollectionAssert.AreEqual(expectedValues, result);
        }

        [TestMethod]
        public async Task WriteSingleCoil_ValidAddressAndValue_UpdatesCoil()
        {
            // Arrange
            ushort address = 0;
            bool value = true;
            
            // Act
            await _master.WriteSingleCoilAsync(address, value);
            
            // Assert
            bool[] result = await _master.ReadCoilsAsync(address, 1);
            Assert.AreEqual(value, result[0]);
        }

        [TestMethod]
        public async Task WriteMultipleCoils_ValidAddressAndValues_UpdatesCoils()
        {
            // Arrange
            ushort startAddress = 0;
            bool[] values = new bool[] { true, false, true, false, true };
            
            // Act
            await _master.WriteMultipleCoilsAsync(startAddress, values);
            
            // Assert
            bool[] result = await _master.ReadCoilsAsync(startAddress, (ushort)values.Length);
            CollectionAssert.AreEqual(values, result);
        }

        [TestMethod]
        public async Task ReadDiscreteInputs_ValidAddress_ReturnsCorrectValues()
        {
            // Arrange
            ushort startAddress = 0;
            ushort numberOfInputs = 8;
            bool[] expectedValues = new bool[] { false, true, false, true, false, true, false, true };
            
            // Act
            bool[] result = await _master.ReadDiscreteInputsAsync(startAddress, numberOfInputs);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfInputs, result.Length);
            CollectionAssert.AreEqual(expectedValues, result);
        }

        [TestMethod]
        public async Task ReadWriteMultipleRegisters_ValidAddressesAndValues_UpdatesAndReturnsCorrectValues()
        {
            // Arrange
            ushort readStartAddress = 0;
            ushort writeStartAddress = 10;
            ushort numberOfRegisters = 5;
            ushort[] writeValues = new ushort[] { 100, 200, 300, 400, 500 };
            
            // Act
            ushort[] result = await _master.ReadWriteMultipleRegistersAsync(
                readStartAddress,
                numberOfRegisters,
                writeStartAddress,
                writeValues);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfRegisters, result.Length);
        }

        [TestMethod]
        public async Task MaskWriteRegister_ValidAddressAndValues_UpdatesRegister()
        {
            // Arrange
            ushort address = 0;
            ushort andMask = 0x00FF;
            ushort orMask = 0xFF00;
            
            // Act
            await _master.MaskWriteRegisterAsync(address, andMask, orMask);
            
            // Assert
            ushort[] result = await _master.ReadHoldingRegistersAsync(address, 1);
            Assert.AreEqual((ushort)((result[0] & andMask) | orMask), result[0]);
        }

        [TestMethod]
        public async Task ReadFifoQueue_ValidAddress_ReturnsCorrectValues()
        {
            // Arrange
            ushort address = 0;
            ushort[] expectedValues = new ushort[] { 1, 2, 3, 4, 5 };
            
            // Act
            ushort[] result = await _master.ReadFifoQueueAsync(address);
            
            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedValues, result);
        }

        [TestMethod]
        public async Task ReadFileRecord_ValidRequest_ReturnsCorrectValues()
        {
            // Arrange
            byte fileNumber = 1;
            ushort recordNumber = 0;
            ushort length = 5;
            
            // Act
            byte[] result = await _master.ReadFileRecordAsync(fileNumber, recordNumber, length);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(length * 2, result.Length);
        }

        [TestMethod]
        public async Task WriteFileRecord_ValidRequest_UpdatesFile()
        {
            // Arrange
            byte fileNumber = 1;
            ushort recordNumber = 0;
            byte[] data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            
            // Act
            await _master.WriteFileRecordAsync(fileNumber, recordNumber, data);

            // Assert
            var readData = await _master.ReadFileRecordAsync(fileNumber, recordNumber, (ushort)(data.Length / 2));
            CollectionAssert.AreEqual(data, readData);
        }

        [TestMethod]
        public async Task ReadExceptionStatus_ReturnsCorrectValue()
        {
            // Act
            byte result = await _master.ReadExceptionStatusAsync();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetCommEventCounter_ReturnsCorrectValues()
        {
            // Act
            (ushort status, ushort eventCount) = await _master.GetCommEventCounterAsync();

            // Assert
            Assert.IsNotNull(status);
            Assert.IsNotNull(eventCount);
        }

        [TestMethod]
        public async Task GetCommEventLog_ReturnsCorrectValues()
        {
            // Act
            (ushort status, ushort eventCount, ushort messageCount, byte[] events) = await _master.GetCommEventLogAsync();

            // Assert
            Assert.IsNotNull(status);
            Assert.IsNotNull(eventCount);
            Assert.IsNotNull(messageCount);
            Assert.IsNotNull(events);
        }

        [TestMethod]
        public async Task ReportServerId_ReturnsCorrectValue()
        {
            // Act
            byte[] result = await _master.ReportServerIdAsync();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ReadDeviceIdentification_ReturnsCorrectValues()
        {
            // Act
            (byte objectId, byte[] objectValue) = await _master.ReadDeviceIdentificationAsync();
            
            // Assert
            Assert.IsNotNull(objectId);
            Assert.IsNotNull(objectValue);
        }
    }

    public class MockModbusSlave : IDisposable
    {
        private readonly ushort[] _holdingRegisters = new ushort[100];
        private readonly ushort[] _inputRegisters = new ushort[100];
        private readonly bool[] _coils = new bool[100];
        private readonly bool[] _discreteInputs = new bool[100];
        private readonly byte[] _serverId = new byte[] { 0x01, 0x02, 0x03 };
        private readonly byte[][] _fileRecords = new byte[10][];
        private bool _disposed;

        public MockModbusSlave()
        {
            // Initialize test data
            for (int i = 0; i < _holdingRegisters.Length; i++)
            {
                _holdingRegisters[i] = (ushort)((i + 1) * 100);
                _inputRegisters[i] = (ushort)((i + 1) * 100);
                _coils[i] = i % 2 == 0;
                _discreteInputs[i] = i % 2 == 1;
            }

            // Initialize file records
            for (int i = 0; i < _fileRecords.Length; i++)
            {
                _fileRecords[i] = new byte[100];
            }
        }

        public ushort[] ReadHoldingRegisters(ushort startingAddress, ushort quantity)
        {
            var result = new ushort[quantity];
            Array.Copy(_holdingRegisters, startingAddress, result, 0, quantity);
            return result;
        }

        public ushort[] ReadInputRegisters(ushort startingAddress, ushort quantity)
        {
            var result = new ushort[quantity];
            Array.Copy(_inputRegisters, startingAddress, result, 0, quantity);
            return result;
        }

        public bool[] ReadCoils(ushort startingAddress, ushort quantity)
        {
            var result = new bool[quantity];
            Array.Copy(_coils, startingAddress, result, 0, quantity);
            return result;
        }

        public bool[] ReadDiscreteInputs(ushort startingAddress, ushort quantity)
        {
            var result = new bool[quantity];
            Array.Copy(_discreteInputs, startingAddress, result, 0, quantity);
            return result;
        }

        public void WriteSingleRegister(ushort address, ushort value)
        {
            _holdingRegisters[address] = value;
        }

        public void WriteMultipleRegisters(ushort startingAddress, ushort[] values)
        {
            Array.Copy(values, 0, _holdingRegisters, startingAddress, values.Length);
        }

        public void WriteSingleCoil(ushort address, bool value)
        {
            _coils[address] = value;
        }

        public void WriteMultipleCoils(ushort startingAddress, bool[] values)
        {
            Array.Copy(values, 0, _coils, startingAddress, values.Length);
        }

        public ushort[] ReadWriteMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, ushort[] writeValues)
        {
            WriteMultipleRegisters(writeStartAddress, writeValues);
            return ReadHoldingRegisters(readStartAddress, readQuantity);
        }

        public void MaskWriteRegister(ushort address, ushort andMask, ushort orMask)
        {
            _holdingRegisters[address] = (ushort)((_holdingRegisters[address] & andMask) | orMask);
        }

        public ushort[] ReadFifoQueue(ushort address)
        {
            return new ushort[] { 1, 2, 3, 4, 5 };
        }

        public byte[] ReadFileRecord(byte fileNumber, ushort recordNumber, ushort length)
        {
            if (fileNumber >= _fileRecords.Length)
                throw new ArgumentOutOfRangeException(nameof(fileNumber));

            var record = _fileRecords[fileNumber];
            var result = new byte[length * 2];
            Array.Copy(record, recordNumber * 2, result, 0, result.Length);
            return result;
        }

        public void WriteFileRecord(byte fileNumber, ushort recordNumber, byte[] data)
        {
            if (fileNumber >= _fileRecords.Length)
                throw new ArgumentOutOfRangeException(nameof(fileNumber));

            var record = _fileRecords[fileNumber];
            Array.Copy(data, 0, record, recordNumber * 2, data.Length);
        }

        public byte ReadExceptionStatus()
        {
            return 0x00;
        }

        public (ushort status, ushort eventCount) GetCommEventCounter()
        {
            return (0x0000, 0x0000);
        }

        public (ushort status, ushort eventCount, ushort messageCount, byte[] events) GetCommEventLog()
        {
            return (0x0000, 0x0000, 0x0000, new byte[0]);
        }

        public byte[] ReportServerId()
        {
            return _serverId;
        }

        public (byte objectId, byte[] objectValue) ReadDeviceIdentification()
        {
            return (0x00, new byte[] { 0x01, 0x02, 0x03 });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Clean up managed resources
                }
                _disposed = true;
            }
        }
    }

    public class MockModbusMaster : IDisposable
    {
        private readonly MockModbusSlave _slave;
        private bool _disposed;

        public MockModbusMaster()
        {
            _slave = new MockModbusSlave();
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(ushort startingAddress, ushort quantity)
        {
            return await Task.Run(() => _slave.ReadHoldingRegisters(startingAddress, quantity));
        }

        public async Task<ushort[]> ReadInputRegistersAsync(ushort startingAddress, ushort quantity)
        {
            return await Task.Run(() => _slave.ReadInputRegisters(startingAddress, quantity));
        }

        public async Task<bool[]> ReadCoilsAsync(ushort startingAddress, ushort quantity)
        {
            return await Task.Run(() => _slave.ReadCoils(startingAddress, quantity));
        }

        public async Task<bool[]> ReadDiscreteInputsAsync(ushort startingAddress, ushort quantity)
        {
            return await Task.Run(() => _slave.ReadDiscreteInputs(startingAddress, quantity));
        }

        public async Task WriteSingleRegisterAsync(ushort address, ushort value)
        {
            await Task.Run(() => _slave.WriteSingleRegister(address, value));
        }

        public async Task WriteMultipleRegistersAsync(ushort startingAddress, ushort[] values)
        {
            await Task.Run(() => _slave.WriteMultipleRegisters(startingAddress, values));
        }

        public async Task WriteSingleCoilAsync(ushort address, bool value)
        {
            await Task.Run(() => _slave.WriteSingleCoil(address, value));
        }

        public async Task WriteMultipleCoilsAsync(ushort startingAddress, bool[] values)
        {
            await Task.Run(() => _slave.WriteMultipleCoils(startingAddress, values));
        }

        public async Task<ushort[]> ReadWriteMultipleRegistersAsync(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, ushort[] writeValues)
        {
            return await Task.Run(() => _slave.ReadWriteMultipleRegisters(readStartAddress, readQuantity, writeStartAddress, writeValues));
        }

        public async Task MaskWriteRegisterAsync(ushort address, ushort andMask, ushort orMask)
        {
            await Task.Run(() => _slave.MaskWriteRegister(address, andMask, orMask));
        }

        public async Task<ushort[]> ReadFifoQueueAsync(ushort address)
        {
            return await Task.Run(() => _slave.ReadFifoQueue(address));
        }

        public async Task<byte[]> ReadFileRecordAsync(byte fileNumber, ushort recordNumber, ushort length)
        {
            return await Task.Run(() => _slave.ReadFileRecord(fileNumber, recordNumber, length));
        }

        public async Task WriteFileRecordAsync(byte fileNumber, ushort recordNumber, byte[] data)
        {
            await Task.Run(() => _slave.WriteFileRecord(fileNumber, recordNumber, data));
        }

        public async Task<byte> ReadExceptionStatusAsync()
        {
            return await Task.Run(() => _slave.ReadExceptionStatus());
        }

        public async Task<(ushort status, ushort eventCount)> GetCommEventCounterAsync()
        {
            return await Task.Run(() => _slave.GetCommEventCounter());
        }

        public async Task<(ushort status, ushort eventCount, ushort messageCount, byte[] events)> GetCommEventLogAsync()
        {
            return await Task.Run(() => _slave.GetCommEventLog());
        }

        public async Task<byte[]> ReportServerIdAsync()
        {
            return await Task.Run(() => _slave.ReportServerId());
        }

        public async Task<(byte objectId, byte[] objectValue)> ReadDeviceIdentificationAsync()
        {
            return await Task.Run(() => _slave.ReadDeviceIdentification());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _slave.Dispose();
                }
                _disposed = true;
            }
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
        }
    }
} 