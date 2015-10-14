using CommandLine;
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
            CommandLineOptions commandLineOptions = null;

            var arguments = Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(options => commandLineOptions = options);

            if (arguments is NotParsed<CommandLineOptions>)
            {
                return;
            }

            if (arguments is Parsed<CommandLineOptions>)
            {
                IRunner runner = Creator.CreateRunner();
                IRunnerStartInfo startInfo = Creator.CreateRunnerStartInfo();
                startInfo.ExecutableFile = "";
                foreach (var item in commandLineOptions.ExecuteCommand)
                {
                    startInfo.ExecutableFile += " " + item;
                }
                startInfo.MemoryLimit = commandLineOptions.MemoryLimit;
                startInfo.WorkingTimeLimit = commandLineOptions.TimeLimit;
                if (!string.IsNullOrEmpty(commandLineOptions.InputFile))
                    startInfo.InputString = File.ReadAllText(commandLineOptions.InputFile);
                IResult result = runner.Run(startInfo);

                Console.WriteLine("Working time : {0}", result.WorkingTime);
                Console.WriteLine("Peak memory used: {0}", result.PeakMemoryUsed);
                Console.WriteLine("Exit code: {0}", result.ExitCode);

                if (!string.IsNullOrEmpty(commandLineOptions.OutputFile))
                    File.WriteAllText(commandLineOptions.OutputFile, result.OutputString);
            }
        }
    }
}
