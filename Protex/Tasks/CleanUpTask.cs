using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public class CleanUpTask : ProtexTask
    {
        protected string SolutionFolderPath;

        public CleanUpTask(string solutionFolder)
        {
            SolutionFolderPath = solutionFolder;
        }

        internal override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.CleanUp(SolutionFolderPath);
            return result;
        }
    }
}