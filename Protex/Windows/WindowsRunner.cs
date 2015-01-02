using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Protex.Windows
{
    class WindowsRunner: IRunner
    {
        private const int UserWorkingTimeKoef = 3;

        public IResult Run(IRunnerStartInfo runnerStartInfo)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = runnerStartInfo.ExecutableFile;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;

            IResult result = new Result();

            try
            {
                Process process = Process.Start(startInfo);
                DateTime startTime = DateTime.Now;
                int maximumUserTime = runnerStartInfo.WorkingTimeLimit * UserWorkingTimeKoef;
                while(process.HasExited == false)
                {
                    result.WorkingTime = (int)((DateTime.Now - startTime).TotalMilliseconds);
                    result.PeakMemoryUsed = Math.Max(result.PeakMemoryUsed, process.PeakVirtualMemorySize64);
                    if (result.WorkingTime > maximumUserTime)
                    {
                        result.WorkingTime = maximumUserTime;
                        process.Kill();
                    }
                    Thread.Sleep(50);
                }
                //process.WaitForExit(5000);
                result.ExitCode = process.ExitCode;
                process.StandardOutput.ReadToEnd();
                process.Dispose();
            }
            catch
            {
                throw;
            }

            return new Result();
        }
    }
}
