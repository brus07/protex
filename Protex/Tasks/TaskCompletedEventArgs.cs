using System;
using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public class TaskCompletedEventArgs : EventArgs
    {
        public TaskCompletedEventArgs()
        {
        }

        public TaskCompletedEventArgs(RunnerCommandResult res)
        {
            CommandResult = res;
        }

        public RunnerCommandResult CommandResult { protected set; get; }
    }
}