using NUnit.Framework;
using Protex.Test.Helpers;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Protex.Test.Windows.Groups
{
    [TestFixture]
    public class WindowsRunnerMemoryLimitTest
    {
        [Test]
        [SimpleCsSourceFileCompilerAttribute("MemoryLimit")]
        public void TestMemoryLimit()
        {
            IRunner runner = Creator.CreateRunner();
            IRunnerStartInfo startInfo = Creator.CreateRunnerStartInfo();
            startInfo.MemoryLimit = 30;
            startInfo.WorkingTimeLimit = 3000;
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "MemoryLimit.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(result.WorkingTime, startInfo.WorkingTimeLimit);
            Assert.GreaterOrEqual(result.PeakMemoryUsed, startInfo.MemoryLimit);
            Assert.AreEqual(-1, result.ExitCode);

            Console.WriteLine("Executed time: {0}{2}PeakMemoryUsed: {1}{2}", result.WorkingTime, result.PeakMemoryUsed, Environment.NewLine);
        }
        [Test]
        [SimpleCsSourceFileCompilerAttribute("MemoryLimit")]
        public void TestUpperMemoryLimit()
        {
            IRunner runner = Creator.CreateRunner();
            IRunnerStartInfo startInfo = Creator.CreateRunnerStartInfo();
            startInfo.MemoryLimit = 70;
            startInfo.WorkingTimeLimit = 3000;
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "MemoryLimit.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(result.WorkingTime, startInfo.WorkingTimeLimit);
            Assert.LessOrEqual(result.PeakMemoryUsed, startInfo.MemoryLimit);
            Assert.AreEqual(0, result.ExitCode);

            Console.WriteLine("Executed time: {0}{2}PeakMemoryUsed: {1}{2}", result.WorkingTime, result.PeakMemoryUsed, Environment.NewLine);
        }
    }
}
