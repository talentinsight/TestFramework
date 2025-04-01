using NUnit.Framework;
using System.Linq;
using TestFramework.Core;
using TestFramework.Core.Application;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void WhenLoadingValidConfiguration_ShouldSucceed()
        {
            // Arrange
            var test = new ConfigurationLoadTest("config.json");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenValidatingConfiguration_ShouldSucceed()
        {
            // Arrange
            var test = new ConfigurationValidationTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenRetrievingValidConfigValue_ShouldReturnValue()
        {
            // Arrange
            var test = new ConfigurationValueTest("LogLevel", "INFO");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
        
        [Test]
        public void WhenRetrievingInvalidConfigValue_ShouldFail()
        {
            // Arrange
            var test = new ConfigurationValueTest("NonExistentKey", "SomeValue");
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Failed));
        }
        
        [Test]
        public void WhenRetrievingAllConfigValues_ShouldReturnAllValues()
        {
            // Arrange
            var test = new ConfigurationAllValuesTest();
            
            // Act
            var result = test.Execute();
            
            // Assert
            Assert.That(result.Status, Is.EqualTo(TestStatus.Passed));
        }
    }
    
    // Configuration load test
    public class ConfigurationLoadTest : CppApplicationTest
    {
        private readonly string _configPath;
        
        public ConfigurationLoadTest(string configPath)
        {
            _configPath = configPath;
        }
        
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            AssertTrue(Application.LoadConfiguration(_configPath), 
                $"Failed to load configuration from {_configPath}");
        }
    }
    
    // Configuration validation test
    public class ConfigurationValidationTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // First load configuration
            AssertTrue(Application.LoadConfiguration("config.json"), 
                "Failed to load configuration");
                
            // Then validate it
            AssertTrue(Application.ValidateConfiguration(), 
                "Configuration validation failed");
        }
    }
    
    // Configuration value test
    public class ConfigurationValueTest : CppApplicationTest
    {
        private readonly string _key;
        private readonly string _expectedValue;
        
        public ConfigurationValueTest(string key, string expectedValue)
        {
            _key = key;
            _expectedValue = expectedValue;
        }
        
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // First load configuration
            AssertTrue(Application.LoadConfiguration("config.json"), 
                "Failed to load configuration");
                
            // Then get and check a specific value
            string value = Application.GetConfigurationValue(_key);
            
            if (value == null)
            {
                throw new System.Exception($"Configuration key '{_key}' not found");
            }
            
            AssertEqual(_expectedValue, value, 
                $"Configuration value for '{_key}' doesn't match expected value");
        }
    }
    
    // Configuration all values test
    public class ConfigurationAllValuesTest : CppApplicationTest
    {
        protected override ICppApplication CreateApplication()
        {
            return new MockCppApplication();
        }
        
        protected override void RunTest()
        {
            // First load configuration
            AssertTrue(Application.LoadConfiguration("config.json"), 
                "Failed to load configuration");
                
            // Then get all values
            var values = Application.GetAllConfigurationValues();
            
            AssertTrue(values.Count > 0, "Configuration should contain at least one value");
            AssertTrue(values.ContainsKey("LogLevel"), "Configuration should contain LogLevel");
            AssertTrue(values.ContainsKey("MaxConnections"), "Configuration should contain MaxConnections");
            AssertTrue(values.ContainsKey("Timeout"), "Configuration should contain Timeout");
        }
    }
} 