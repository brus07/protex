using NUnit.Framework;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test
{
    [TestFixture]
    public class RunnerStartInfoTest
    {
        const string Arguments = "arguments x x";
        const string ExecutableFile = "but.exe";
        const string InputString = "input text";
        const int MemoryLimit = 1024 * 1024 * 16; //16mb
        const int WorkingTimeLimit = 5 * 1000; //5secs

        [Test]
        public void TestAllFields()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.ExecutableFile = ExecutableFile;
            Assert.AreEqual(ExecutableFile, startInfo.ExecutableFile);
            startInfo.Arguments = Arguments;
            Assert.AreEqual(Arguments, startInfo.Arguments);
            startInfo.InputString = InputString;
            Assert.AreEqual(InputString, startInfo.InputString);
            startInfo.MemoryLimit = MemoryLimit;
            Assert.AreEqual(MemoryLimit, startInfo.MemoryLimit);
            startInfo.WorkingTimeLimit = WorkingTimeLimit;
            Assert.AreEqual(WorkingTimeLimit,startInfo.WorkingTimeLimit);
        }

        [Test]
        public void TestExecutableFileField()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.ExecutableFile = ExecutableFile;
            Assert.AreEqual(ExecutableFile, startInfo.ExecutableFile);
        }

        [Test]
        public void TestArgumentsField()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.Arguments = Arguments;
            Assert.AreEqual(Arguments, startInfo.Arguments);
        }

        [Test]
        public void TestInputStringField()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.InputString = InputString;
            Assert.AreEqual(InputString, startInfo.InputString);
        }

        [Test]
        public void TestMemoryLimitField()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.MemoryLimit = MemoryLimit;
            Assert.AreEqual(MemoryLimit, startInfo.MemoryLimit);
        }

        [Test]
        public void TestWorkingTimeLimitField()
        {
            IRunnerStartInfo startInfo = CreateStartInfo();
            startInfo.WorkingTimeLimit = WorkingTimeLimit;
            Assert.AreEqual(WorkingTimeLimit, startInfo.WorkingTimeLimit);
        }

        private static IRunnerStartInfo CreateStartInfo()
        {
            IRunnerStartInfo runnerStartInfo = Creator.CreateRunnerStartInfo();
            Assert.NotNull(runnerStartInfo);
            return runnerStartInfo;
        }
    }
}
