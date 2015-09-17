using NUnit.Framework;
using Protex.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Protex.Test
{
    [TestFixture]
    public class RunExeVariantTest
    {
        [Test]
        public void TestExeEmptyArgument()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("EmptyExecutable")]
        public void TestExeEmptyExecutable()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.Arguments = string.Format(" -e {0}", Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "EmptyExecutable.exe"));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Less(resultValues[0], 1000);
            Assert.AreEqual(0, resultValues[2]);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("InfiniteLoopTimeLimit")]
        public void TestExeInfiniteLoopTimeLimit()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.Arguments = string.Format(" -e {0}", Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "InfiniteLoopTimeLimit.exe"));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Greater(resultValues[0], 1000);
            Assert.Less(resultValues[0], 1100);
            Assert.AreEqual(-1, resultValues[2]);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("Return47Error")]
        public void TestExeReturnCode()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.Arguments = string.Format(" -e {0}", Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "Return47Error.exe"));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.AreEqual(47, resultValues[2]);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("HelloWorld")]
        public void TestExeOutputArgument()
        {
            const string expectedOutput = "Hello World!\r\n";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            string exeFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath,  "HelloWorld.exe");
            string outputFile = "output.file";
            startInfo.Arguments = string.Format(" -e {0} -o {1}", exeFile, outputFile);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Less(resultValues[0], 1000);
            Assert.AreEqual(0, resultValues[2]);

            Assert.AreEqual(expectedOutput, File.ReadAllText(outputFile));
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("SimpleEcho")]
        public void TestExeInputAndOutputArguments()
        {
            const string input = "474";
            const string expectedOutput = "Output: " + input + "\r\n";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            string exeFile = Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "SimpleEcho.exe");
            string inputFile = "input.file";
            File.WriteAllText(inputFile, input);
            string outputFile = "output.file";
            startInfo.Arguments = string.Format(" -e {0} -i {1} -o {2}", exeFile, inputFile, outputFile);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Less(resultValues[0], 1000);
            Assert.AreEqual(0, resultValues[2]);

            Assert.AreEqual(expectedOutput, File.ReadAllText(outputFile));
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("InfiniteLoopTimeLimit")]
        public void TestExeTimeLimitArgument()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.Arguments = string.Format("  -t 2000 -e {0}", Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "InfiniteLoopTimeLimit.exe"));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Greater(resultValues[0], 2000);
            Assert.Less(resultValues[0], 2100);
            Assert.AreEqual(-1, resultValues[2]);
        }

        [Test]
        [SimpleCsSourceFileCompilerAttribute("InfiniteLoopTimeLimit")]
        public void TestExeMemoryLimitArgument()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "protex.exe";
            startInfo.Arguments = string.Format("  -m 100 -e {0}", Path.Combine(ConstansContainer.TemporaryExecutableFilesPath, "InfiniteLoopTimeLimit.exe"));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
            int[] resultValues = ParseExecuteResults(outputString);
            Assert.Less(resultValues[0], 1100);
            Assert.LessOrEqual(resultValues[1], 100);
            Assert.AreEqual(-1, resultValues[2]);
        }

        private static int[] ParseExecuteResults(string output)
        {
            List<string> stringes = new List<string>();
            foreach (var item in output.Split())
            {
                if (!string.IsNullOrWhiteSpace(item))
                    stringes.Add(item);
            }
            List<int> result = new List<int>();
            result.Add(int.Parse(stringes[3])); //time
            result.Add(int.Parse(stringes[7])); //memory
            result.Add(int.Parse(stringes[10])); //exit code
            return result.ToArray();
        }
    }
}
