using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestFramework.Core;

namespace TestFramework.Tests.Application
{
    [TestClass]
    public class ConfigurationTests
    {
        private string _testConfigPath;
        private Configuration _config;

        [TestInitialize]
        public void Setup()
        {
            _testConfigPath = Path.Combine(Path.GetTempPath(), "test_config.json");
            _config = new Configuration(_testConfigPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testConfigPath))
            {
                File.Delete(_testConfigPath);
            }
        }

        [TestMethod]
        public void LoadConfiguration_ValidFile_LoadsSuccessfully()
        {
            // Arrange
            File.WriteAllText(_testConfigPath, "{\n  \"key\": \"value\"\n}");

            // Act
            _config.Load();

            // Assert
            Assert.AreEqual("value", _config.GetValue("key"));
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LoadConfiguration_InvalidFile_ThrowsException()
        {
            // Act
            _config.Load();
        }

        [TestMethod]
        public void SaveConfiguration_ValidData_SavesSuccessfully()
        {
            // Arrange
            _config.SetValue("key", "value");

            // Act
            _config.Save();

            // Assert
            Assert.IsTrue(File.Exists(_testConfigPath));
            string content = File.ReadAllText(_testConfigPath);
            Assert.IsTrue(content.Contains("\"key\": \"value\""));
        }

        [TestMethod]
        public void GetValue_ExistingKey_ReturnsValue()
        {
            // Arrange
            _config.SetValue("testKey", "testValue");

            // Act
            string value = _config.GetValue("testKey");

            // Assert
            Assert.AreEqual("testValue", value);
        }

        [TestMethod]
        public void GetValue_NonExistingKey_ReturnsNull()
        {
            // Act
            string value = _config.GetValue("nonexistent");

            // Assert
            Assert.IsNull(value);
        }
    }
} 