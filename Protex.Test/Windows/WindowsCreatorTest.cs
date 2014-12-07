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
            IRunnerStartInfo runnerStartInfo = WindowsCreator.CreateRunnerStartInfo();
            Assert.NotNull(runnerStartInfo);
        }

        [Test]
        public void CreateRunnerTest()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            Assert.NotNull(runner);
        }
    }
}
