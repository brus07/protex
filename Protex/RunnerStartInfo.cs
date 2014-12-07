using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    class RunnerStartInfo : IRunnerStartInfo
    {
        public string ExecutableFile { get; set; }

        public string Arguments { get; set; }

        public string InputFile { get; set; }

        public int WorkingTimeLimit { get; set; }

        public int MemoryLimit { get; set; }
    }
}
