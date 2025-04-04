using System;
using System.Threading.Tasks;
using TestFramework.Core.Tests;

namespace TestFramework.Tests.Tests
{
    public interface ITest : IDisposable
    {
        string Name { get; }
        TestType TestType { get; }
        Task<bool> InitializeAsync();
        Task<bool> RunAsync();
        Task<bool> StopAsync();
    }
} 