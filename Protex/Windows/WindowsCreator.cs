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
            return new RunnerStartInfo();
        }

        public static IRunner CreateRunner()
        {
            return new WindowsRunner();
        }
    }
}
