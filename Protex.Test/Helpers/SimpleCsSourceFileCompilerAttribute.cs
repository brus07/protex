using NUnit.Framework;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Protex.Test.Helpers
{
    class SimpleCsSourceFileCompilerAttribute : Attribute, ITestAction
    {
        private static string WorkingDirectory = @"../../../TestData/ExecutableFiles/Project";
        private static string TemporaryExecutableFilesDirectory = @"../TemporaryExecutableFiles/";

        private string baseFileName;

        public SimpleCsSourceFileCompilerAttribute(string baseFileName)
        {
            this.baseFileName = baseFileName;
        }

        public void AfterTest(TestDetails testDetails)
        {
            //throw new NotImplementedException();
        }

        public void BeforeTest(TestDetails testDetails)
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
            PrepareAppSettingForCurrentEnvironment();
            string compiler = ConfigurationManager.AppSettings["Compiler"];

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = compiler;
            startInfo.Arguments = string.Format(" /out:{0}{1}.exe {1}.cs", TemporaryExecutableFilesDirectory, baseFileName);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = WorkingDirectory;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            Assert.IsTrue(process.HasExited);
            Assert.AreEqual(0, process.ExitCode);

            string outputString = process.StandardOutput.ReadToEnd();
        }

        private static void PrepareAppSettingForCurrentEnvironment()
        {
            ConfigurationManager.AppSettings["Compiler"] = ConfigurationManager.AppSettings["CscCompiler"];

            //for Unix with Mono (mcs)
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                ConfigurationManager.AppSettings["Compiler"] = ConfigurationManager.AppSettings["McsCompiler"];
        }
    }
}
