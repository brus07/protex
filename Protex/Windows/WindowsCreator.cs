using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Windows
{
    public static class WindowsCreator
    {
        public static IRunnerStartInfo CreateRunnerStartInfo()
        {
            IRunnerStartInfo runner = new RunnerStartInfo();
            runner.WorkingTimeLimit = 1000; //1 sec
            runner.MemoryLimit = 64 * 1024 * 1024; //64mb
            return runner;
        }

        public static IRunner CreateRunner()
        {
            return new WindowsRunner();
        }
    }
}
