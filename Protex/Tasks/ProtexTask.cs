using System.Collections.Generic;
using System.Text;
using ProtexCore.Runner;

namespace ProtexCore.Tasks
{
    public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs args);

    public class ProtexTask
    {
        /// <summary>
        /// Additional options that can be passed to a script 
        /// both on remote server or local
        /// </summary>
        protected Dictionary<string, string> ScriptOptions;

        public ProtexTask()
        {
            IsActive = false;
            IsFinished = false;

            ScriptOptions = new Dictionary<string, string>();
        }

        /// <summary>
        /// Occurs when task completed.
        /// </summary>
        public event TaskCompletedEventHandler TaskCompleted;

        /// <summary>
        /// Raises TaskCompleted event when task is completed
        /// </summary>
        /// <param name="result"></param>
        internal virtual void OnTaskCompleted(RunnerCommandResult result)
        {
            IsActive = false;
            IsFinished = true;

            if (TaskCompleted != null)
                TaskCompleted(this,
                    new TaskCompletedEventArgs(result));
        }

        /// <summary>
        /// Main method for task execution
        /// </summary>
        internal virtual RunnerCommandResult Perform(IRunner runner)
        {
            return new RunnerCommandResult {Output = string.Empty, ReturnState = 0};
        }

        /// <summary>
        /// Will join inner dictionary in two ways:
        /// 1. If both key and value are present, than option will be "--key=value"
        /// 2. If only key is present, than option will be just "--key" of "-k" if key's length is 1
        /// </summary>
        /// <returns>String with joined params</returns>
        protected virtual string DictionaryToParams()
        {
            if (ScriptOptions == null)
                return string.Empty;

            const char whitespace = ' ';
            StringBuilder sb = new StringBuilder();
            bool first = true;

            foreach (var pair in ScriptOptions)
            {
                if (first)
                    first = false;
                else
                    sb.Append(whitespace);

                if (pair.Key.Length > 1)
                    sb.Append("--");
                else
                    sb.Append('-');

                sb.Append(pair.Key);

                // append value if only it's not empty
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    if (pair.Key.Length > 1)
                        sb.Append("=");
                    else
                        sb.Append(' ');

                    sb.Append(pair.Value);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProtexTask"/> is finished.
        /// </summary>
        /// <value>
        /// <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished { get; set; }
    }
}