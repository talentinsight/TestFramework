using System.Collections.Generic;
using System.Threading.Tasks;
using TestFramework.Core.Models;
using TestFramework.Tests.Tests;

namespace TestFramework.Tests.Runners
{
    public interface ITestRunner
    {
        Task<TestResult> RunTestAsync(ITest test);
        Task<IEnumerable<TestResult>> RunTestsAsync(params ITest[] tests);
    }
} 