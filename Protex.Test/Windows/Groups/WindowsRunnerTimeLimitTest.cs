using NUnit.Framework;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test.Windows.Groups
{
    [TestFixture]
    public class WindowsRunnerTimeLimitTest
    {
        private const int UserWorkingTimeKoef = 3;

        [Test]
        public void TestInfiniteLoop()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\InfiniteLoopTimeLimit.exe";
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.Greater(startInfo.WorkingTimeLimit*UserWorkingTimeKoef, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);

            Console.WriteLine("Executed time: {0}{1}Expected near 1 second.", result.WorkingTime, Environment.NewLine);
        }

        [Test]
        public void TestWaitingInput()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\SimpleEcho.exe";
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(startInfo.WorkingTimeLimit * UserWorkingTimeKoef, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);

            Console.WriteLine("Executed time: {0}{1}Expected near 3 seconds.", result.WorkingTime, Environment.NewLine);
        }

        [Test]
        public void TestInfiniteLoopWait4seconds()
        {
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\InfiniteLoopTimeLimit.exe";
            startInfo.WorkingTimeLimit = 4000; //4 secs

            IRunner runner = WindowsCreator.CreateRunner();
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.Greater(startInfo.WorkingTimeLimit * UserWorkingTimeKoef, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);

            Console.WriteLine("Executed time: {0}{1}Expected near 4 seconds.", result.WorkingTime, Environment.NewLine);
        }

        [Test]
        public void TestWaitingInputFor2seconds()
        {
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = @"..\..\..\TestData\ExecutableFiles\SimpleEcho.exe";
            startInfo.WorkingTimeLimit = 2000; //2 secs

            IRunner runner = WindowsCreator.CreateRunner();
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.LessOrEqual(startInfo.WorkingTimeLimit * UserWorkingTimeKoef, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);

            Console.WriteLine("Executed time: {0}{1}Expected near 6 seconds.", result.WorkingTime, Environment.NewLine);
        }
    }
}
