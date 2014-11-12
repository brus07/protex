using System.Collections.Generic;
using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public class CompileSourceTask : ProtexTask
    {
        // language of sources
        protected SourceLanguage Language;

        // path to source, relatively to
        // solutions/ folder
        protected string PathToSource;

        public CompileSourceTask(SourceLanguage sourceLanguage,
            string sourcePath,
            Dictionary<string, string> scriptParams)
        {
            Language = sourceLanguage;
            ScriptOptions = scriptParams;
            PathToSource = sourcePath;
        }


        internal override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.CompileSource(
                PathToSource,
                Language,
                DictionaryToParams());

            return result;
        }
    }
}