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
            startInfo.Arguments = runnerStartInfo.Arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;

            IResult result = new Result();

            try
            {
                Process process = Process.Start(startInfo);
                process.StandardInput.WriteLine(runnerStartInfo.InputString);
                int maximumUserTime = runnerStartInfo.WorkingTimeLimit * UserWorkingTimeKoef;
                while(process.HasExited == false)
                {
                    try
                    {
                        double realWorkingTime = (DateTime.Now - process.StartTime).TotalMilliseconds;
                        result.PeakMemoryUsed = Math.Max(result.PeakMemoryUsed, process.PeakVirtualMemorySize64 / (1024 * 1024));
                        if (realWorkingTime > maximumUserTime)
                        {
                            result.WorkingTime = maximumUserTime;
                            process.Kill();
                        }
                        else if (process.TotalProcessorTime.TotalMilliseconds > runnerStartInfo.WorkingTimeLimit
                            /*|| result.PeakMemoryUsed > runnerStartInfo.MemoryLimit*/)
                        {
                            result.WorkingTime = (int)(process.TotalProcessorTime.TotalMilliseconds);
                            process.Kill();
                        }
                        Thread.Sleep(50);
                    }
                    catch(InvalidOperationException)
                    {
                        //result.PeakMemoryUsed = Math.Max(result.PeakMemoryUsed, 1);
                        result.WorkingTime = Math.Max(result.WorkingTime, 15);
                    }
                }

                result.ExitCode = process.ExitCode;
                if (result.ExitCode != -1)
                {
                    result.OutputString = process.StandardOutput.ReadToEnd();
                    result.ErrorOutputString = process.StandardError.ReadToEnd();
                    result.WorkingTime = (int)(process.TotalProcessorTime.TotalMilliseconds);
                    result.WorkingTime = Math.Max(result.WorkingTime, 15);
                }
                process.Dispose();
            }
            catch
            {
                throw;
            }

            return result;
        }
    }
}
