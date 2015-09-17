using Protex.Unix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Windows
{
    public static class Creator
    {
        public static IRunnerStartInfo CreateRunnerStartInfo()
        {
            IRunnerStartInfo runner = new RunnerStartInfo();
            runner.WorkingTimeLimit = 1000; //1 sec
            runner.MemoryLimit = 64; //64mb
            return runner;
        }

        public static IRunner CreateRunner()
        {
            if (Configurator.IsUnix)
                return new UnixRunner();
            return new WindowsRunner();
        }
    }
}
