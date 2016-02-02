using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Protex.Unix
{
    class UnixRunner: IRunner
    {
        private const int UserWorkingTimeKoef = 5;

        public IResult Run(IRunnerStartInfo runnerStartInfo)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Path.Combine("..", "..", "..", "Tools", "timeout", "timeout");
            startInfo.Arguments = string.Format(" -t {0} -m {1} --detect-hangups ", runnerStartInfo.WorkingTimeLimit / 1000, runnerStartInfo.MemoryLimit * 1024) + " " + runnerStartInfo.ExecutableFile + " " + runnerStartInfo.Arguments;
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
                while (process.HasExited == false)
                {
                    try
                    {
                        double realWorkingTime = (DateTime.Now - process.StartTime).TotalMilliseconds;
                        if (realWorkingTime > maximumUserTime)
                        {
                            result.PeakMemoryUsed = 0;
                            result.WorkingTime = (int)realWorkingTime;
                            process.Kill();
                        }
                        Thread.Sleep(500);
                    }
                    catch (InvalidOperationException)
                    {
                        result.PeakMemoryUsed = Math.Max(result.PeakMemoryUsed, 1);
                        result.WorkingTime = Math.Max(result.WorkingTime, 15);
                    }
                }

                result.ExitCode = process.ExitCode;
                if (result.ExitCode != -1)
                {
                    result.OutputString = process.StandardOutput.ReadToEnd();

                    string standardErrorOutput = process.StandardError.ReadToEnd().TrimEnd();
                    string[] errorOutputLines = standardErrorOutput.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    int timeoutOutputLines = 2;
                    if (!errorOutputLines[errorOutputLines.Length - 1].Contains("<time "))
                        timeoutOutputLines = 1;
                    //need only first lines without last two lines
                    result.ErrorOutputString = "";
                    for (int i = 0; i < errorOutputLines.Length - timeoutOutputLines; i++)
                        result.ErrorOutputString = string.Concat(result.ErrorOutputString, errorOutputLines[i], Environment.NewLine);

                    //only last two lines
                    string timeoutOutputResults = errorOutputLines[errorOutputLines.Length - timeoutOutputLines];
                    if (timeoutOutputLines > 1)
                        timeoutOutputResults += string.Concat(Environment.NewLine, errorOutputLines[errorOutputLines.Length - 1]);
                    result = ParseOutputFromTimeout(timeoutOutputResults, result);
                }
                process.Dispose();
            }
            catch
            {
                throw;
            }

            return result;
        }

        private IResult ParseOutputFromTimeout(string timeoutResult, IResult result)
        {
            //FINISHED CPU 0.00 MEM 0 MAXMEM 3288 STALE 1
            //TIMEOUT CPU 5.21 MEM 26344 MAXMEM 26344 STALE 0
            //MEM CPU 0.15 MEM 26344 MAXMEM 26344 STALE 0
            //HANGUP CPU 0.02 MEM 2364 MAXMEM 2364 STALE 6
            //SIGNAL CPU 0.01 MEM 2364 MAXMEM 2364 STALE 1
            string[] splits = timeoutResult.Split();
            string reason = splits[0];

            switch (splits[0])
            {
                case "FINISHED":
                    break;
                case "TIMEOUT":
                    result.ExitCode = -1;
                    break;
                case "MEM":
                    result.ExitCode = -1;
                    break;
                case "HANGUP":
                    result.ExitCode = -1;
                    break;
                case "SIGNAL":
                    break;
                default:
                    throw new ApplicationException(string.Format("Unknown timeout message={0}", timeoutResult));
                    break;
            }

            int time = (int)(double.Parse(splits[2]) * 1000);
            int maxmemory = int.Parse(splits[6]) / 1024;

            result.WorkingTime = Math.Max(result.WorkingTime, time);
            result.PeakMemoryUsed = Math.Max(result.PeakMemoryUsed, maxmemory);
            return result;
        }
    }
}
