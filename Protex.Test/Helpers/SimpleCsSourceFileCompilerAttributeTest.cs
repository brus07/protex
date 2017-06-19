using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Protex.Test.Helpers
{
    [TestFixture]
    class SimpleCsSourceFileCompilerAttributeTest
    {
        private static string BaseFileNameForTest = "EmptyExecutableForTest";

        private static string WorkingDirectory = Path.Combine(ConstansContainer.ExecutableFilesPath, "Project");
        private static string TemporaryExecutableFilesDirectory = @"../TemporaryExecutableFiles/";

        [Test]
        [SimpleCsSourceFileCompilerAttribute("EmptyExecutableForTest")]
        [TestCase()]
        [TestCase()]
        public void TestSimleCompilerForTestsWithAttribute()
        {
            string executableFile = Path.Combine(WorkingDirectory, TemporaryExecutableFilesDirectory, BaseFileNameForTest + ".exe");
            Assert.True(File.Exists(executableFile));
            new FileInfo(executableFile).Delete();
            while (File.Exists(executableFile))
                Thread.Sleep(5);
            Assert.False(File.Exists(executableFile));
        }

        [Test]
        public void TestSimleCompilerForTests()
        {
            string executableFile = Path.Combine(WorkingDirectory, TemporaryExecutableFilesDirectory, BaseFileNameForTest + ".exe");
            if (File.Exists(executableFile))
            {
                File.Delete(executableFile);
                while (File.Exists(executableFile))
                    Thread.Sleep(5);
            }

            Assert.False(File.Exists(executableFile), "File exists");

            var compilerAttribute = new SimpleCsSourceFileCompilerAttribute(BaseFileNameForTest);
            compilerAttribute.PrepareExecutableFile();

            Assert.True(File.Exists(executableFile), "Not compiled");
            File.Delete(executableFile);
            while (File.Exists(executableFile))
                Thread.Sleep(5);
            Assert.False(File.Exists(executableFile), "Not deleted");
        }
    }
}
