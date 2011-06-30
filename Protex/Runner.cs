using System;

namespace ProtexCore
{
    public enum RunSecurity { Usual, Secured }

    internal class RunnerCommandResult
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

	internal interface IRunner
	{
        /// <summary>
        /// Copies local source to tmp/ sources folder
        /// </summary>
        /// <param name="initialPathSource">Path to local source</param>
        /// <param name="newSourceFileName"></param>
        /// <returns></returns>
        RunnerCommandResult CopySource(string initialPathSource, string newSourceFileName);

        /// <summary>
        /// Creates needed folder structure and 
        /// moves source from temporary folder
        /// to needed solution folder
        /// </summary>
        /// <param name="pathToSource"></param>
        /// <param name="scriptParams"></param>
        /// <returns></returns>
        RunnerCommandResult OrganizeSource(string pathToSource, string scriptParams);

        /// <summary>
        /// Compiles specified source file with tool for specified language
        /// </summary>
        /// <param name="pathToSource">Path to source file relatively to solutions/ folder</param>
        /// <param name="language">Programming language of sources</param>
        /// <param name="scriptParams">Additional parameters to script</param>
        /// <returns></returns>
        RunnerCommandResult CompileSource(string pathToSource, SourceLanguage language, string scriptParams);

        /// <summary>
        /// Runs solution with specified input
        /// </summary>
        /// <param name="pathToSolution">Path to user solution 
        /// relatively to solutions/ folder</param>
        /// <param name="language">Language of runned solution</param>
        /// <param name="pathToInput">Path to file with input</param>
        /// <returns></returns>
        RunnerCommandResult Run(string pathToSolution, SourceLanguage language, string pathToInput);

        /// <summary>
        /// Removes solution folder and source files
        /// </summary>
        /// <param name="solutionFolder">User solution folder relatively to solutions/ folder</param>
        /// <returns></returns>
        RunnerCommandResult CleanUp(string solutionFolder);

        /// <summary>
        /// Stops execution of task with specified Process ID
        /// </summary>
        /// <returns></returns>
        RunnerCommandResult Stop(int PID);

        /// <summary>
        /// Gets a specified option from configuration file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        string GetConfigOption<T>(T option);
	}
	
	internal class RemoteRunner : IRunner
	{
        RemoteConfig innerConfig;

		public RemoteRunner ()
			:base()
		{
		}

        public RemoteRunner(RemoteRunner copy)
            :this(copy.innerConfig)
        {
        }
		
		public RemoteRunner (RemoteConfig config)
		{
            innerConfig = config;
		}
	}
	
	internal class LocalRunner : IRunner
	{
        LocalConfig innerConfig;

		public LocalRunner()
			:base()
		{
		}

        public LocalRunner(LocalRunner copy)
            : this(copy.innerConfig)
        {
        }
		
		public LocalRunner (LocalConfig config)
		{
            this.innerConfig = config;
		}
	}
}

