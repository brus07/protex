using NUnit.Framework;
using Protex.Test.Helpers;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Protex.Test.Windows
{
    [TestFixture]
    public class WindowsRunnerTest
    {
        [Test]
        [SimpleCsSourceFileCompilerAttribute("EmptyExecutableForTest")]
        public void TestEmptyExecutable()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "EmptyExecutable.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(0, result.ExitCode);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("InfiniteLoopTimeLimit")]
        public void TestTimeLimit()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "InfiniteLoopTimeLimit.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.Less(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(-1, result.ExitCode);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("HelloWorld")]
        public void TestHelloWorld()
        {
            const string expectedOutput = "Hello World!\r\n";

            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "HelloWorld.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));

            IRunner runner = WindowsCreator.CreateRunner();
            IResult result = runner.Run(startInfo);

            Assert.AreEqual(0, result.ExitCode);
            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(expectedOutput.TrimEnd(), result.OutputString.TrimEnd());
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("SimpleEcho")]
        public void TestSimpleEcho()
        {
            const string input = "474";
            const string expectedOutput = "Output: " + input+"\r\n";

            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "SimpleEcho.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            startInfo.InputString = input;

            IRunner runner = WindowsCreator.CreateRunner();
            IResult result = runner.Run(startInfo);

            Assert.AreEqual(0, result.ExitCode);
            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(expectedOutput.TrimEnd(), result.OutputString.TrimEnd());
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("Return47Error")]
        public void TestReturnCode()
        {
            IRunner runner = WindowsCreator.CreateRunner();
            IRunnerStartInfo startInfo = WindowsCreator.CreateRunnerStartInfo();
            startInfo.ExecutableFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "Return47Error.exe");
            Assert.IsTrue(System.IO.File.Exists(startInfo.ExecutableFile));
            IResult result = runner.Run(startInfo);

            Assert.Greater(startInfo.WorkingTimeLimit, result.WorkingTime);
            Assert.AreEqual(47, result.ExitCode);
        }
    }
}
