using System.Collections.Generic;
using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public class RunTask : ProtexTask
    {
        protected string PathToSolution;
        protected SourceLanguage Language;
        protected string PathToInput;

        public RunTask(string solutionPath,
            SourceLanguage sourceLanguage,
            string inputPath,
            Dictionary<string, string> scriptParams)
        {
            PathToSolution = solutionPath;
            Language = sourceLanguage;
            PathToInput = inputPath;
            ScriptOptions = scriptParams;
        }

        internal override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.Run(
                PathToSolution,
                Language,
                PathToInput,
                DictionaryToParams());

            return result;
        }
    }
}