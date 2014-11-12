using System.Collections.Generic;
using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public class OrganizeSourceTask : ProtexTask
    {
        protected string InitialSourcePath;
        protected string NewSourceFileName;

        /// <summary>
        /// Creates a new instance of OrganizeSource Class
        /// </summary>
        /// <param name="pathToSource">Path to user source file in local temporary storage</param>
        /// <param name="newSourceFileName"></param>
        /// <param name="scriptParams">Params of script, that will be passed to organize script (like "--filename=main.cpp")</param>
        public OrganizeSourceTask(string pathToSource,
            string newSourceFileName,
            Dictionary<string, string> scriptParams)
        {
            ScriptOptions = scriptParams;
            InitialSourcePath = pathToSource;
            NewSourceFileName = newSourceFileName;
        }

        internal override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult copyResult = runner.CopySource(InitialSourcePath, NewSourceFileName);

            if (copyResult.ReturnState != 0)
                return copyResult;

            // if copy is ok, that try to run
            // organizer script wherever it is

            RunnerCommandResult organizerResult =
                runner.OrganizeSource(
                    NewSourceFileName,
                    DictionaryToParams());

            return organizerResult;
        }
    }
}