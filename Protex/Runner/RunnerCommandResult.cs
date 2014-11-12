namespace ProtexCore.Runner
{
    public class RunnerCommandResult
    {
        /// <summary>
        /// Used to return 0 if command is ok, and
        /// non-zero result if some error happened
        /// </summary>
        public int ReturnState { get; set; }

        /// <summary>
        /// Stdout of command
        /// </summary>
        public string Output { get; set; }
    }
}