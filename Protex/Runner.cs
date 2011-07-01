using System;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

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

        /// <summary>
        /// Creates a specified ssh object and 
        /// makes a basic configuration of it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T CreateSshObject<T>() where T : SshBase
        {
            T connection = 
                (T)Activator.CreateInstance(typeof(T),
                                            this.innerConfig[RemoteOption.SSH_HostAddress],
                                            this.innerConfig[RemoteOption.SSH_UserName]);

            // Add credentials for connection
            if (this.innerConfig.HasOption(RemoteOption.SSH_IdentityFile))
            {
                if (this.innerConfig.HasOption(RemoteOption.SSH_IdentityPassphrase))
                    connection.AddIdentityFile(this.innerConfig[RemoteOption.SSH_IdentityFile],
                                        this.innerConfig[RemoteOption.SSH_IdentityPassphrase]);
                else
                    connection.AddIdentityFile(this.innerConfig[RemoteOption.SSH_IdentityFile]);
            }
            
            return connection;
        }

        /// <summary>
        /// Gets a ssh port from config file and starts a ssh connection
        /// </summary>
        /// <param name="ssh"></param>
        protected void ConnectSshObject(SshBase ssh)
        {
            int port = 22;
            if (this.innerConfig.HasOption(RemoteOption.SSH_Port))
            {
                // due to specification, if conversion fails, 
                // port variable will have zero value, that's 
                // why need to reassign it again
                if (!int.TryParse(this.innerConfig[RemoteOption.SSH_Port], out port))
                    port = 22;
            }

            // if some weird data is in config
            if (port < 1)
                port = 22;

            ssh.Connect(port);
        }

        /// <summary>
        /// Copies local source to tmp/ sources folder
        /// </summary>
        /// <param name="initialPathSource">Path to local source</param>
        /// <param name="newSourceFileName"></param>
        /// <returns></returns>
        public RunnerCommandResult CopySource(string initialPathSource, string newSourceFileName)
        {
            Scp scp = this.CreateSshObject<Scp>();

            // no output to console
            scp.Verbos = false;

            try
            {
                this.ConnectSshObject(scp);

                string toFilePath = this.innerConfig[RemoteOption.TmpFolder] + newSourceFileName;

                scp.Put(initialPathSource, toFilePath);
            }
            // will be thrown when local file is inaccessible
            // or won't be thrown, cause it's internal 
            // SharpSSH library exception
            // (leave here just for clearness)
            catch (SshTransferException)
            {
            }
            // can occur on connect
            catch (JSchException jex)
            {
                // TODO need to specify what to do in this case
                return new RunnerCommandResult() { Output = jex.Message, ReturnState = 1 };
            }
            catch (Exception)
            {
                // TODO needs specification
            }
            finally
            {
                scp.Close();
            }

            return new RunnerCommandResult() { ReturnState = 0, Output = string.Empty };
        }

        protected void scp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            // TODO specify if need to do something in this case
        }

        /// <summary>
        /// Creates needed folder structure and 
        /// moves source from temporary folder
        /// to needed solution folder
        /// </summary>
        /// <param name="pathToSource"></param>
        /// <param name="scriptParams"></param>
        /// <returns></returns>
        public RunnerCommandResult OrganizeSource(string pathToSource, string scriptParams)
        {
            SshExec exec = this.CreateSshObject<SshExec>();

            try
            {
                this.ConnectSshObject(exec);

                string exeName = this.innerConfig[RemoteOption.UserScriptsFolder] +
                                 this.innerConfig[RemoteOption.OrganizerScript];
                string command = string.Format(
                    "{0} --source={1} {2}",
                    exeName, 
                    pathToSource, 
                    scriptParams);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult() { Output = stderr, ReturnState = status };
                else
                    return new RunnerCommandResult() { Output = stdout, ReturnState = status };
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult() { Output = ex.Message, ReturnState = 1 };
            }
            finally
            {
                exec.Close();
            }
        }

        /// <summary>
        /// Compiles specified source file with tool for specified language
        /// </summary>
        /// <param name="pathToSource">Path to source file relatively to solutions/ folder</param>
        /// <param name="language">Programming language of sources</param>
        /// <param name="scriptParams">Additional parameters to script</param>
        /// <returns></returns>
        public RunnerCommandResult CompileSource(string pathToSource, SourceLanguage lang, string scriptParams)
        {
            SshExec exec = this.CreateSshObject<SshExec>();

            try
            {
                this.ConnectSshObject(exec);

                string exeName = this.innerConfig[RemoteOption.UserScriptsFolder] +
                                 this.innerConfig[RemoteOption.CompilerScript];
                string command = string.Format(
                    "{0} --solution={1} --lang={2} {3}",
                    exeName, 
                    RunnerHelper.Language[lang],
                    scriptParams);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult() { Output = stderr, ReturnState = status };
                else
                    return new RunnerCommandResult() { Output = stdout, ReturnState = status };
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult() { Output = ex.Message, ReturnState = 1 };
            }
            finally
            {
                exec.Close();
            }
        }

        /// <summary>
        /// Runs solution with specified input
        /// </summary>
        /// <param name="pathToSolution">Path to user solution 
        /// relatively to solutions/ folder</param>
        /// <param name="language">Language of runned solution</param>
        /// <param name="pathToInput">Path to file with input</param>
        /// <returns></returns>
        public RunnerCommandResult Run(string pathToSolution, SourceLanguage lang, string pathToInput)
        {
            SshExec exec = this.CreateSshObject<SshExec>();

            try
            {
                this.ConnectSshObject(exec);

                string exeName = this.innerConfig[RemoteOption.UserScriptsFolder] +
                                 this.innerConfig[RemoteOption.RunnerScript];
                
                // TODO move this strings to config file
                string command = string.Format(
                    "{0} --solution={1} --lang={2} --input={3}",
                    exeName,
                    pathToSolution,
                    RunnerHelper.Language[lang],
                    pathToInput);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult() { Output = stderr, ReturnState = status };
                else
                    return new RunnerCommandResult() { Output = stdout, ReturnState = status };
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult() { Output = ex.Message, ReturnState = 1 };
            }
            finally
            {
                exec.Close();
            }
        }

        /// <summary>
        /// Removes solution folder and source files
        /// </summary>
        /// <param name="solutionFolder">User solution folder relatively to solutions/ folder</param>
        /// <returns></returns>
        public RunnerCommandResult CleanUp(string solutionFolder)
        {
            SshExec exec = this.CreateSshObject<SshExec>();

            try
            {
                this.ConnectSshObject(exec);

                string exeName = this.innerConfig[RemoteOption.UserScriptsFolder] +
                                 this.innerConfig[RemoteOption.CleanUpScript];

                // TODO move this strings to config file
                string command = string.Format(
                    "{0} --solution={1}",
                    exeName,
                    solutionFolder);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult() { Output = stderr, ReturnState = status };
                else
                    return new RunnerCommandResult() { Output = stdout, ReturnState = status };
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult() { Output = ex.Message, ReturnState = 1 };
            }
            finally
            {
                exec.Close();
            }
        }

        public RunnerCommandResult Stop(int PID)
        {
            throw new NotImplementedException();
        }

        // TODO Remove this stupid hack with generic param        
        public string GetConfigOption<T>(T option)
        {
            throw new NotSupportedException("Should use only with RemoteOption parameter");
        }

        public string GetConfigOption(RemoteOption option)
        {
            return this.innerConfig[option];
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

