﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    public interface IRunnerStartInfo
    {
        string ExecutableFile { get; set; }
        string Arguments { get; set; }
        string InputFile { get; set; }

        /// <summary>
        /// Limits of working time in milliseconds.
        /// </summary>
        int WorkingTimeLimit { get; set; }
        
        /// <summary>
        /// Limits of memory in bytes.
        /// </summary>
        int MemoryLimit { get; set; }
    }
}
