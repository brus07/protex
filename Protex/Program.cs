using Microsoft.Extensions.CommandLineUtils;
using Protex.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Protex
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = "Protex";
            app.Description = "Protected executer (Protex) is C# library for running executable files with protections and limits.";

            CommandOption executeOption = app.Option(
                "-e | --execute <command>",
                "Execute command.",
                CommandOptionType.MultipleValue);

            CommandOption timelimitOption = app.Option(
                "-t | --timelimit <milliseconds>",
                "The time limit in milliseconds (default = 1000).",
                CommandOptionType.SingleValue);

            CommandOption memorylimitOption = app.Option(
                "-m | --memorylimit <MiB>",
                "The memory limit in MiB (default = 64).",
                CommandOptionType.SingleValue);

            CommandOption inputFileOption = app.Option(
                "-i | --inputfile <file>",
                "Path to input file (console input).",
                CommandOptionType.SingleValue);

            CommandOption ouputFileOption = app.Option(
                "-o | --ouputfile <file>",
                "Path to input file (console input).",
                CommandOptionType.SingleValue);
/*
            CommandOption verbosityOption = app.Option(
                "-v | --verbose",
                "Execute command.",
                CommandOptionType.NoValue);
*/
            app.HelpOption("-? | -h | --help");

            app.OnExecute(() =>
            {
                bool badFormat = false;

                string executeCommand = "";
                if (executeOption.HasValue())
                {
                    foreach (var item in executeOption.Values)
                    {
                        executeCommand += " " + item;
                    }
                }
                else
                    badFormat = true;

                int timeLimit = 1000;
                if (timelimitOption.HasValue())
                {
                    if (!int.TryParse(timelimitOption.Value(), out timeLimit))
                        badFormat = true;
                }

                int memoryLimit = 64;
                if (memorylimitOption.HasValue())
                {
                    if (!int.TryParse(memorylimitOption.Value(), out memoryLimit))
                        badFormat = true;
                }

                string inputFile = "";
                if (inputFileOption.HasValue())
                    inputFile = inputFileOption.Value();

                string ouputFile = "";
                if (ouputFileOption.HasValue())
                    ouputFile = ouputFileOption.Value();

                if (badFormat)
                {
                    app.ShowHint();
                    return 1;
                }

                IRunner runner = Creator.CreateRunner();
                IRunnerStartInfo startInfo = Creator.CreateRunnerStartInfo();
                startInfo.ExecutableFile = executeCommand;

                startInfo.MemoryLimit = memoryLimit;
                startInfo.WorkingTimeLimit = timeLimit;
                if (!string.IsNullOrEmpty(inputFile))
                    startInfo.InputString = File.ReadAllText(inputFile);
                IResult result = runner.Run(startInfo);

                Console.WriteLine("Working time : {0}", result.WorkingTime);
                Console.WriteLine("Peak memory used: {0}", result.PeakMemoryUsed);
                Console.WriteLine("Exit code: {0}", result.ExitCode);

                if (!string.IsNullOrEmpty(ouputFile))
                    File.WriteAllText(ouputFile, result.OutputString);

                return 0;
            });

            app.Execute(args);
        }
    }
}
