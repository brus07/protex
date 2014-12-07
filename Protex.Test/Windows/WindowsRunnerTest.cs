using NUnit.Framework;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test.Windows
{
    [TestFixture]
    public class WindowsRunnerTest
    {
        [Test]
        public void Demo()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            IResult result = runner.Run(startInfo);
        }
    }
}
