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

        public string InputString { get; set; }

        /// <summary>
        /// Limits of working time in milliseconds.
        /// </summary>
        public int WorkingTimeLimit { get; set; }

        /// <summary>
        /// Limits of memory in KiB.
        /// </summary>
        public int MemoryLimit { get; set; }
    }
}
