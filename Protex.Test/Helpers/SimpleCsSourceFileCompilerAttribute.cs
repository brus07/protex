using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace Protex.Test.Helpers
{
    class SimpleCsSourceFileCompilerAttribute : Attribute, ITestAction
    {
        private static string WorkingDirectory = Path.Combine(ConstansContainer.ExecutableFilesPath, "Project");
        private static string TemporaryExecutableFilesDirectory = @"../TemporaryExecutableFiles/";

        private string baseFileName;

        public SimpleCsSourceFileCompilerAttribute(string baseFileName)
        {
            this.baseFileName = baseFileName;
        }

        public void AfterTest(ITest test)
        {
        }

        public void BeforeTest(ITest test)
        {
            PrepareExecutableFile();
        }

        public ActionTargets Targets
        {
            get
            {
                return ActionTargets.Test | ActionTargets.Suite;
            }
        }

        public void PrepareExecutableFile()
        {
            bool needCompile = false;

            if (!Directory.Exists(Path.Combine(WorkingDirectory, TemporaryExecutableFilesDirectory)))
            {
                Directory.CreateDirectory(Path.Combine(WorkingDirectory, TemporaryExecutableFilesDirectory));
                Thread.Sleep(50);
            }

            string executableFile = Path.Combine(WorkingDirectory, TemporaryExecutableFilesDirectory, baseFileName + ".exe");
            string sourceFile = Path.Combine(WorkingDirectory, baseFileName + ".cs");

            //not exists
            if (!File.Exists(executableFile))
                needCompile = true;

            //very old
            if ((DateTime.Now - (new FileInfo(executableFile).LastWriteTime)).TotalHours > 1)
                needCompile = true;

            //source code file was updated
            if ((new FileInfo(sourceFile).LastWriteTime) > (new FileInfo(executableFile).LastWriteTime))
                needCompile = true;

            if (needCompile)
                CompileFile(baseFileName);
        }

        private void CompileFile(string baseFileName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Configurator.Compiler;
            startInfo.Arguments = string.Format(" /out:{0}{1}.exe {1}.cs", TemporaryExecutableFilesDirectory, baseFileName);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = WorkingDirectory;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            Thread.Sleep(50);

            string outputString = process.StandardOutput.ReadToEnd();
        }
    }
}
