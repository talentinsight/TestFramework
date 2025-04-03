using TestFramework.Core.Logger;

namespace TestFramework.Tests
{
    public abstract class TestBase
    {
        protected ILogger? Logger { get; private set; }

        protected virtual void Setup()
        {
            Logger = new ConsoleLogger();
        }

        protected virtual void TearDown()
        {
            Logger = null;
        }
    }
} 