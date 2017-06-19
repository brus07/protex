using NUnit.Framework;
using System;

namespace Protex.Test
{
    [SetUpFixture]
    public class TestSetUpClass
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
    }
}
