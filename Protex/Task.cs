using System;
using System.Collections.Generic;
using System.Text;

namespace ProtexCore
{
	public class TaskCompletedEventArgs : EventArgs
	{
        protected RunnerCommandResult commandResult;
		
		public TaskCompletedEventArgs ()
			:base()
		{
		}

        public TaskCompletedEventArgs(RunnerCommandResult res)
		{
            this.commandResult = res;
		}

        public RunnerCommandResult CommandResult
        {
            get { return this.commandResult; }
        }
	}
	
	public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs args);
	
	public class ProtexTask
	{
        /// <summary>
        /// Additional options that can be passed to a script 
        /// both on remote server or local
        /// </summary>
        protected Dictionary<string, string> scriptOptions;

		public ProtexTask ()
		{
			this.IsActive = false;
			this.IsFinished = false;

            this.scriptOptions = new Dictionary<string, string>();
		}
		
		/// <summary>
		/// Occurs when task completed.
		/// </summary>
		public event TaskCompletedEventHandler TaskCompleted;
		
        /// <summary>
        /// Raises TaskCompleted event when task is completed
        /// </summary>
        /// <param name="output"></param>
        internal virtual void OnTaskCompleted(RunnerCommandResult result)
		{
            this.IsActive = false;
            this.IsFinished = true;

			if (TaskCompleted != null)
				TaskCompleted (this, 
					new TaskCompletedEventArgs (result));
		}
		
		/// <summary>
		/// Main method for task execution
		/// </summary>
        public virtual RunnerCommandResult Perform(IRunner runner)
		{
            return new RunnerCommandResult() { Output = string.Empty, ReturnState = 0 };
		}

        /// <summary>
        /// Will join inner dictionary in two ways:
        /// 1. If both key and value are present, than option will be "key=value"
        /// 2. If only key is present, than option will be just "key"
        /// </summary>
        /// <returns>String with joined params</returns>
        protected virtual string DictionaryToParams()
        {
            char whitespace = ' ';
            StringBuilder sb = new StringBuilder();
            bool first = true;

            foreach (var pair in this.scriptOptions)
            {
                if (first)
                    first = false;
                else
                    sb.Append(whitespace);

                sb.Append(pair.Key);

                // append value if only it's not empty
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    sb.Append("=");
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
		public bool IsActive
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ProtexCore.ProtexTask"/> is finished.
		/// </summary>
		/// <value>
		/// <c>true</c> if finished; otherwise, <c>false</c>.
		/// </value>
		public bool IsFinished
		{
			get;
			set;
		}
	}
	
	public class OrganizeSourceTask : ProtexTask
	{
        protected string initialSourcePath;
        protected string newSourceFileName;

        /// <summary>
        /// Creates a new instance of OrganizeSource Class
        /// </summary>
        /// <param name="pathToSource">Path to user source file in local temporary storage</param>
        /// <param name="scriptParams">Params of script, that will be passed to organize script (like "--filename=main.cpp")</param>
        public OrganizeSourceTask(string pathToSource,
                                  string newSourceFileName,
                                  Dictionary<string, string> scriptParams)
            : base()
        {
            this.scriptOptions = scriptParams;
            this.initialSourcePath = pathToSource;
            this.newSourceFileName = newSourceFileName;
        }        

        public override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult copyResult = runner.CopySource(this.initialSourcePath, this.newSourceFileName);

            if (copyResult.ReturnState != 0)
                return copyResult;

            // if copy is ok, that try to run
            // organizer script wherever it is

            string pathToSource = runner.GetConfigOption(RemoteOption.TmpFolder) + this.newSourceFileName;
            RunnerCommandResult organizerResult = 
                runner.OrganizeSource(
                    pathToSource,
                    this.DictionaryToParams());

            return organizerResult;
        }
	}

    public enum SourceLanguage { CSharp, C, Cpp, Java, Pascal, Ruby, Python, Perl, PHP, Lisp }

    public class CompileSourceTask : ProtexTask
    {
        // language of sources
        protected SourceLanguage language;

        // path to source, relatively to
        // solutions/ folder
        protected string pathToSource;

        public CompileSourceTask(SourceLanguage sourceLanguage,
                                 string sourcePath, 
                                 Dictionary<string, string> scriptParams)
        {
            this.language = sourceLanguage;
            this.scriptOptions = scriptParams;
            this.pathToSource = sourcePath;
        }


        public override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.CompileSource(
                this.pathToSource, 
                this.language,
                this.DictionaryToParams());

            return result;
        }
    }

    public class RunTask : ProtexTask
    {
        protected string pathToSolution;
        protected SourceLanguage language;
        protected string pathToInput;

        public RunTask(string solutionPath, 
                       SourceLanguage sourceLanguage,
                       string inputPath)
        {
            this.pathToSolution = solutionPath;
            this.language = sourceLanguage;
            this.pathToInput = inputPath;
        }

        public override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.Run(
                this.pathToSolution,
                this.language,
                this.pathToInput);

            return result;
        }
    }

    public class CleanUpTask : ProtexTask
    {
        protected string solutionFolderPath;

        public CleanUpTask(string solutionFolder)
        {
            this.solutionFolderPath = solutionFolder;
        }

        public override RunnerCommandResult Perform(IRunner runner)
        {
            RunnerCommandResult result = runner.CleanUp(this.solutionFolderPath);
            return result;
        }
    }
}

