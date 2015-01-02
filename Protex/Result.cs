using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    class Result : IResult
    {
        public int ExitCode { get; set; }

        public int WorkingTime { get; set; }

        public long PeakMemoryUsed { get; set; }

        public string OutputFile { get; set; }

        public string OutputString { get; set; }

        public string ErrorOutputFile { get; set; }

        public Result()
        {
            ExitCode = -1;
        }
    }
}
