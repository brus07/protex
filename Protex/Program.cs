using Protex.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Protex
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args);

            if (arguments.Errors.Count() == 0)
            {
                IRunner runner = Creator.CreateRunner();
                IRunnerStartInfo startInfo = Creator.CreateRunnerStartInfo();
                startInfo.ExecutableFile = arguments.Value.ExecuteCommand;
                startInfo.MemoryLimit = arguments.Value.MemoryLimit;
                startInfo.WorkingTimeLimit = arguments.Value.TimeLimit;
                if (!string.IsNullOrEmpty(arguments.Value.InputFile))
                    startInfo.InputString = File.ReadAllText(arguments.Value.InputFile);
                IResult result = runner.Run(startInfo);

                Console.WriteLine("Working time : {0}", result.WorkingTime);
                Console.WriteLine("Peak memory used: {0}", result.PeakMemoryUsed);
                Console.WriteLine("Exit code: {0}", result.ExitCode);

                if (!string.IsNullOrEmpty(arguments.Value.OutputFile))
                    File.WriteAllText(arguments.Value.OutputFile, result.OutputString);
            }
        }
    }
}
