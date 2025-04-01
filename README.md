# TestFramework

A comprehensive C# test framework designed for industrial automation testing, specifically for C++ applications and industrial communication protocols.

## Project Overview

This test framework provides a structured approach to testing automation systems with the following key features:

- **Modular Architecture**: Easily extensible with new test types and protocols
- **Logger System**: Multiple logging options (Console, File, Mock) with a factory pattern
- **Protocol Testing**: Built-in support for industrial protocols like Modbus TCP
- **Configuration Management**: Centralized configuration via JSON files
- **Test Execution and Reporting**: Structured test execution with detailed results

## Project Structure

```
TestFramework/
├── TestFramework.Core/              # Core framework library
│   ├── Logger/                      # Logging system
│   │   ├── ILogger.cs               # Logger interface
│   │   ├── ConsoleLogger.cs         # Console implementation
│   │   ├── FileLogger.cs            # File implementation
│   │   ├── MockLogger.cs            # Mock for testing
│   │   ├── LoggerFactory.cs         # Factory pattern implementation
│   │   └── LoggerType.cs            # Logger type enumeration
│   ├── Utils/                       # Utility classes
│   │   ├── ProcessHelper.cs         # Helper for running external processes
│   │   ├── FileHelper.cs            # File operations helper
│   │   └── NetworkHelper.cs         # Network operations helper
│   ├── Tests/                       # Test base classes
│   │   └── ProtocolTest.cs          # Base class for protocol testing
│   ├── ConfigReader.cs              # Configuration management (Singleton)
│   ├── ITestRunner.cs               # Test runner interface
│   ├── TestBase.cs                  # Base class for all tests
│   ├── CppTestRunner.cs             # Test runner for C++ applications
│   └── appsettings.json             # Configuration file
├── TestFramework.Tests/             # Test project
│   ├── Logger/                      # Tests for logger components
│   │   ├── ConsoleLoggerTests.cs    # Tests for ConsoleLogger
│   │   ├── FileLoggerTests.cs       # Tests for FileLogger
│   │   ├── MockLoggerTests.cs       # Tests for MockLogger
│   │   └── LoggerFactoryTests.cs    # Tests for LoggerFactory
│   └── Integration/                 # Integration tests
│       └── ModbusTests.cs           # Tests for Modbus implementation
└── README.md                        # Project documentation
```

## Design Patterns Used

The framework demonstrates several important design patterns:

1. **Factory Pattern**: Used in `LoggerFactory` to create different logger types
2. **Singleton Pattern**: Used in `ConfigReader` for configuration management
3. **Strategy Pattern**: Various logger implementations for the `ILogger` interface
4. **Template Method Pattern**: Base classes like `TestBase` with abstract methods for specific implementations

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or other compatible IDE

### Building the Project

```bash
# Clone the repository
git clone https://github.com/yourusername/TestFramework.git
cd TestFramework

# Build the solution
dotnet build

# Run the tests
dotnet test
```

## Usage Examples

### Creating a Simple Test

```csharp
public class MyTest : TestBase
{
    public MyTest() : base(LoggerType.Console)
    {
    }

    protected override void Setup()
    {
        Logger.Log("Setting up test");
        // Setup code
    }

    protected override void RunTest()
    {
        Logger.Log("Running test");
        // Test implementation
        Assert(true, "This assertion should pass");
    }

    protected override void TearDown()
    {
        Logger.Log("Cleaning up");
        // Cleanup code
    }
}

// Run the test
var test = new MyTest();
var result = test.Execute();
Console.WriteLine($"Test status: {result.Status}");
```

### Testing a C++ Application

```csharp
// Configure the test runner
var config = new Dictionary<string, string>
{
    { "ExecutablePath", "/path/to/cpp/app.exe" }
};

// Create and initialize the test runner
var runner = new CppTestRunner();
runner.Initialize(config);

// Run all tests
var results = runner.RunAllTests();

// Display results
foreach (var result in results)
{
    Console.WriteLine($"Test: {result.TestName}, Status: {result.Status}");
}
```

### Testing a Modbus Device

```csharp
// Create a Modbus test
var test = new ModbusTest(
    deviceIp: "192.168.1.10",
    devicePort: 502,
    unitId: 1,
    startAddress: 100,
    quantity: 10
);

// Execute the test
var result = test.Execute();

// Check the result
if (result.Status == TestStatus.Passed)
{
    Console.WriteLine("Modbus test passed successfully!");
}
else
{
    Console.WriteLine($"Test failed: {result.Message}");
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 