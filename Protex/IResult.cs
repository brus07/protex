using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    public interface IResult
    {
        int ExitCode { get; set; }

        /// <summary>
        /// Worked time in milliseconds.
        /// </summary>
        int WorkingTime { get; set; }

        /// <summary>
        /// Maximum used memory in bytes.
        /// </summary>
        long PeakMemoryUsed { get; set; }

        string OutputString { get; set; }

        string ErrorOutputString { get; set; }
    }
}
