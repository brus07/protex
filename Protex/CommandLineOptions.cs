using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    class CommandLineOptions
    {
        [Option('e', "execute", Required = true, Min = 1, HelpText = "Execute command.")]
        public IEnumerable<string> ExecuteCommand { get; set; }

        [Option('t', "timelimit", Default = 1000, HelpText = "The time limit in milliseconds.")]
        public int TimeLimit { get; set; }

        [Option('m', "memorylimit", Default = 64, HelpText = "The memory limit in MiB.")]
        public int MemoryLimit { get; set; }

        [Option('i', "inputfile", HelpText = "Path to input file (console input).")]
        public string InputFile { get; set; }

        [Option('o', "ouputfile", HelpText = "Path to output file (console output).")]
        public string OutputFile { get; set; }
    }
}
