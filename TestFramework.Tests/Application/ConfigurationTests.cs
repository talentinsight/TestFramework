using System;
using System.IO;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Core.Configuration;
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class ConfigurationTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;
        private string _testConfigPath;
        private ConfigurationManager _configManager;

        public ConfigurationTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
            _testConfigPath = Path.Combine(Path.GetTempPath(), "test_config.json");
            _configManager = new ConfigurationManager(_logger);
        }

        public void Dispose()
        {
            _application.Dispose();
            if (File.Exists(_testConfigPath))
            {
                File.Delete(_testConfigPath);
            }
        }

        [Fact]
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
            Assert.NotNull(result);
            Assert.Equal(5000, result.TestTimeout);
            Assert.Equal(3, result.RetryCount);
            Assert.Equal("DEBUG", result.LogLevel);
        }

        [Fact]
        public async Task LoadConfiguration_InvalidFile_ThrowsException()
        {
            // Arrange
            await File.WriteAllTextAsync(_testConfigPath, "invalid json");

            // Act & Assert
            await Assert.ThrowsAsync<System.Text.Json.JsonException>(async () =>
                await _configManager.LoadConfigurationAsync<TestConfig>(_testConfigPath));
        }

        [Fact]
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
            Assert.True(File.Exists(_testConfigPath));
            var savedConfig = System.Text.Json.JsonSerializer.Deserialize<TestConfig>(
                await File.ReadAllTextAsync(_testConfigPath));
            Assert.NotNull(savedConfig);
            Assert.Equal(5000, savedConfig.TestTimeout);
            Assert.Equal(3, savedConfig.RetryCount);
            Assert.Equal("DEBUG", savedConfig.LogLevel);
        }

        [Fact]
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
            Assert.Same(result1, result2);
        }

        [Fact]
        public void GetConfiguration_WithoutLoad_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _configManager.GetConfiguration<TestConfig>());
        }

        [Fact]
        public async Task WhenConfigurationIsSet_StateIsUpdated()
        {
            // Arrange
            var config = new TestFramework.Core.Application.ApplicationConfiguration
            {
                Port = 502,
                Host = "localhost",
                Timeout = 1000
            };

            // Act
            var result = await _application.SetConfigurationAsync(config);

            // Assert
            Assert.True(result);
            Assert.Equal(config.Port, _application.Configuration.Port);
            Assert.Equal(config.Host, _application.Configuration.Host);
            Assert.Equal(config.Timeout, _application.Configuration.Timeout);
        }

        [Fact]
        public async Task WhenConfigurationIsInvalid_ReturnsFalse()
        {
            // Arrange
            var config = new TestFramework.Core.Application.ApplicationConfiguration
            {
                Port = -1,
                Host = "",
                Timeout = 0
            };

            // Act
            var result = await _application.SetConfigurationAsync(config);

            // Assert
            Assert.False(result);
            Assert.Equal("Invalid configuration", _application.LastError);
        }

        [Fact]
        public async Task WhenConfigurationIsSetWhileRunning_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            var config = new TestFramework.Core.Application.ApplicationConfiguration
            {
                Port = 502,
                Host = "localhost",
                Timeout = 1000
            };

            // Act
            var result = await _application.SetConfigurationAsync(config);

            // Assert
            Assert.False(result);
            Assert.Equal("Cannot change configuration while running", _application.LastError);
        }
    }

    public class TestConfig
    {
        public int TestTimeout { get; set; }
        public int RetryCount { get; set; }
        public string LogLevel { get; set; } = string.Empty;
    }
} 