using NUnit.Framework;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test.Windows
{
    [TestFixture]
    public class WindowsCreatorTest
    {
        [Test]
        public void CreateRunnerStartInfoTest()
        {
            IRunnerStartInfo runnerStartInfo = Creator.CreateRunnerStartInfo();
            Assert.NotNull(runnerStartInfo);
        }

        [Test]
        public void CreateRunnerTest()
        {
            IRunner runner = Creator.CreateRunner();
            Assert.NotNull(runner);
        }
    }
}
