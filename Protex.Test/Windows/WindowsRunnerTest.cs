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
        public void TestEmptyExecutable()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\EmptyExecutable.exe";
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(0, result.ExitCode);
        }

        [Test]
        public void TestTimeLimit()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\InfiniteLoopTimeLimit.exe";
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.Less(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);
        }

        [Test]
        public void TestSimpleEcho()
        {
            const string input = "474";
            const string expectedOutput = "Output: " + input+"\r\n";

            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\SimpleEcho.exe";
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            startInfo.InputString = input;

            IRunner runner = WindowsCreator.CreateRunner();
            IResult result = runner.Run(startInfo);

            Assert.AreEqual(0, result.ExitCode);
            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(expectedOutput, result.OutputString);
        }
    }
}
