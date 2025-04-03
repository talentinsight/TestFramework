using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Core.Tests;
using TestFramework.Tests.Application;

namespace TestFramework.Tests.Integration
{
    [TestFixture]
    public class ModbusTests
    {
        private MockCppApplication _application;
        private MockLogger _logger;
        private ModbusTest _modbusTest;

        public ModbusTests()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication(_logger);
            _modbusTest = new ModbusTest("127.0.0.1", 502, 1, 0, 10);
        }

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication(_logger);
            _modbusTest = new ModbusTest("127.0.0.1", 502, 1, 0, 10);
        }

        [Test]
        public void WhenReadingHoldingRegisters_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x03, 0x04, 0x00, 0x0A, 0x00, 0x0B });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenReadingInputRegisters_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x04, 0x04, 0x00, 0x0C, 0x00, 0x0D });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenReadingCoils_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x01, 0x01, 0x01 });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenReadingDiscreteInputs_ValuesAreReturned()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x02, 0x01, 0x00 });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenWritingSingleRegister_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x06, 0x00, 0x01, 0x00, 0x0A });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenWritingMultipleRegisters_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x10, 0x00, 0x01, 0x00, 0x02 });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenWritingSingleCoil_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x05, 0x00, 0x01, 0xFF, 0x00 });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenWritingMultipleCoils_OperationSucceeds()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x0F, 0x00, 0x01, 0x00, 0x02 });

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.True);
        }

        [Test]
        public void WhenModbusErrorOccurs_ErrorIsReported()
        {
            // Arrange
            _application.SetModbusResponse(new byte[] { 0x01, 0x83, 0x02 }); // Exception code 2

            // Act
            _modbusTest.Execute();

            // Assert
            Assert.That(_modbusTest.IsSuccess, Is.False);
        }
    }
} 