using TestFramework.Core.Logger;

namespace TestFramework.Tests
{
    public abstract class TestBase
    {
        protected ILogger? Logger { get; private set; }

        protected void SetupBase()
        {
            Logger = new ConsoleLogger();
        }

        protected void TearDownBase()
        {
            Logger = null;
        }
    }
} 