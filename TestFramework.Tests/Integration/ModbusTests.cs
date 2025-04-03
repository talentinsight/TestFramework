using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Core.Tests;

namespace TestFramework.Tests.Integration
{
    [TestFixture]
    public class ModbusTests
    {
        private MockCppApplication _application;
        private MockLogger _logger;
        private ModbusTest _modbusTest;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication();
            _modbusTest = new ModbusTest(_application, _logger);
        }

        [Test]
        public async Task WhenReadingHoldingRegisters_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x03, 0x04, 0x00, 0x0A, 0x00, 0x0B });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully read holding registers"));
        }

        [Test]
        public async Task WhenReadingInputRegisters_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x04, 0x04, 0x00, 0x0C, 0x00, 0x0D });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully read input registers"));
        }

        [Test]
        public async Task WhenReadingCoils_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x01, 0x01, 0x01 });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully read coils"));
        }

        [Test]
        public async Task WhenReadingDiscreteInputs_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x02, 0x01, 0x00 });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully read discrete inputs"));
        }

        [Test]
        public async Task WhenWritingSingleRegister_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x06, 0x00, 0x01, 0x00, 0x0A });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully wrote single register"));
        }

        [Test]
        public async Task WhenWritingMultipleRegisters_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x10, 0x00, 0x01, 0x00, 0x02 });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully wrote multiple registers"));
        }

        [Test]
        public async Task WhenWritingSingleCoil_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00 });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully wrote single coil"));
        }

        [Test]
        public async Task WhenWritingMultipleCoils_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x0F, 0x00, 0x01, 0x00, 0x02 });

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("Successfully wrote multiple coils"));
        }

        [Test]
        public async Task WhenModbusErrorOccurs_ErrorIsReported()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x83, 0x02 }); // Exception code 2

            // Act
            var result = await _modbusTest.RunTest();

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Message, Contains.Substring("Modbus error"));
        }
    }
} 