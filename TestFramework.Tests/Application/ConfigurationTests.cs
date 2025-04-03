using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Configuration;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ConfigurationTests
    {
        private string _testConfigPath;
        private ConfigurationManager _configManager;
        private MockLogger _logger;

        [SetUp]
        public void Setup()
        {
            _testConfigPath = Path.Combine(Path.GetTempPath(), "test_config.json");
            _logger = new MockLogger();
            _configManager = new ConfigurationManager(_logger);
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_testConfigPath))
            {
                File.Delete(_testConfigPath);
            }
        }

        [Test]
        public async Task LoadConfiguration_ValidFile_LoadsSuccessfully()
        {
            // Arrange
            var config = new TestConfig
            {
                TestTimeout = 5000,
                RetryCount = 3,
                LogLevel = "DEBUG"
            };
            await File.WriteAllTextAsync(_testConfigPath, System.Text.Json.JsonSerializer.Serialize(config));

            // Act
            var result = await _configManager.LoadConfigurationAsync<TestConfig>(_testConfigPath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestTimeout, Is.EqualTo(5000));
            Assert.That(result.RetryCount, Is.EqualTo(3));
            Assert.That(result.LogLevel, Is.EqualTo("DEBUG"));
        }

        [Test]
        public async Task LoadConfiguration_InvalidFile_ThrowsException()
        {
            // Arrange
            await File.WriteAllTextAsync(_testConfigPath, "invalid json");

            // Act & Assert
            Assert.ThrowsAsync<System.Text.Json.JsonException>(async () =>
                await _configManager.LoadConfigurationAsync<TestConfig>(_testConfigPath));
        }

        [Test]
        public async Task SaveConfiguration_ValidConfig_SavesSuccessfully()
        {
            // Arrange
            var config = new TestConfig
            {
                TestTimeout = 5000,
                RetryCount = 3,
                LogLevel = "DEBUG"
            };

            // Act
            await _configManager.SaveConfigurationAsync(_testConfigPath, config);

            // Assert
            Assert.That(File.Exists(_testConfigPath), Is.True);
            var savedConfig = System.Text.Json.JsonSerializer.Deserialize<TestConfig>(
                await File.ReadAllTextAsync(_testConfigPath));
            Assert.That(savedConfig, Is.Not.Null);
            Assert.That(savedConfig.TestTimeout, Is.EqualTo(5000));
            Assert.That(savedConfig.RetryCount, Is.EqualTo(3));
            Assert.That(savedConfig.LogLevel, Is.EqualTo("DEBUG"));
        }

        [Test]
        public async Task GetConfiguration_AfterLoad_ReturnsSameInstance()
        {
            // Arrange
            var config = new TestConfig { TestTimeout = 5000 };
            await _configManager.SaveConfigurationAsync(_testConfigPath, config);
            await _configManager.LoadConfigurationAsync<TestConfig>(_testConfigPath);

            // Act
            var result1 = _configManager.GetConfiguration<TestConfig>();
            var result2 = _configManager.GetConfiguration<TestConfig>();

            // Assert
            Assert.That(result1, Is.SameAs(result2));
        }

        [Test]
        public void GetConfiguration_WithoutLoad_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _configManager.GetConfiguration<TestConfig>());
        }
    }

    public class TestConfig
    {
        public int TestTimeout { get; set; }
        public int RetryCount { get; set; }
        public string LogLevel { get; set; } = string.Empty;
    }
} 