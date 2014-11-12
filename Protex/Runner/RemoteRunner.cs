using System;
using ProtexCore.Config;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ProtexCore.Runner
{
    internal class RemoteRunner : IRunner
    {
        private RemoteConfig innerConfig;

        public RemoteRunner()
        {
        }

        public RemoteRunner(RemoteRunner copy)
            : this(copy.innerConfig)
        {
        }

        public RemoteRunner(RemoteConfig config)
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
                (T) Activator.CreateInstance(typeof (T),
                    innerConfig[RemoteOption.SSH_HostAddress],
                    innerConfig[RemoteOption.SSH_UserName]);

            // Add credentials for connection
            if (innerConfig.HasOption(RemoteOption.SSH_IdentityFile))
            {
                if (innerConfig.HasOption(RemoteOption.SSH_IdentityPassphrase))
                    connection.AddIdentityFile(innerConfig[RemoteOption.SSH_IdentityFile],
                        innerConfig[RemoteOption.SSH_IdentityPassphrase]);
                else
                    connection.AddIdentityFile(innerConfig[RemoteOption.SSH_IdentityFile]);
            }
            else
                connection.Password = innerConfig[RemoteOption.SSH_Password];

            return connection;
        }

        /// <summary>
        /// Gets a ssh port from config file and starts a ssh connection
        /// </summary>
        /// <param name="ssh"></param>
        protected void ConnectSshObject(SshBase ssh)
        {
            int port = 22;
            if (innerConfig.HasOption(RemoteOption.SSH_Port))
            {
                // due to specification, if conversion fails, 
                // port variable will have zero value, that's 
                // why need to reassign it again
                if (!int.TryParse(innerConfig[RemoteOption.SSH_Port], out port))
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
            Scp scp = CreateSshObject<Scp>();

            // no output to console
            scp.Verbos = false;

            try
            {
                ConnectSshObject(scp);

                string toFilePath = innerConfig[RemoteOption.TmpFolder] + newSourceFileName;

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
                return new RunnerCommandResult {Output = jex.Message, ReturnState = 1};
            }
            catch (Exception)
            {
                // TODO needs specification
            }
            finally
            {
                scp.Close();
            }

            return new RunnerCommandResult {ReturnState = 0, Output = string.Empty};
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
        /// <param name="sourceName">Name of source file</param>
        /// <param name="scriptParams"></param>
        /// <returns></returns>
        public RunnerCommandResult OrganizeSource(string sourceName, string scriptParams)
        {
            SshExec exec = CreateSshObject<SshExec>();

            try
            {
                ConnectSshObject(exec);

                string exeName = innerConfig[RemoteOption.UserScriptsFolder] +
                                 innerConfig[RemoteOption.OrganizerScript];

                string sourcePath = innerConfig[RemoteOption.TmpFolder] +
                                    sourceName;

                string command = string.Format(
                    "{0} --source={1} {2}",
                    exeName,
                    sourcePath,
                    scriptParams);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult {Output = stderr, ReturnState = status};
                else
                    return new RunnerCommandResult {Output = stdout, ReturnState = status};
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult {Output = ex.Message, ReturnState = 1};
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
        /// <param name="lang">Programming language of sources</param>
        /// <param name="scriptParams">Additional parameters to script</param>
        /// <returns></returns>
        public RunnerCommandResult CompileSource(string pathToSource, SourceLanguage lang, string scriptParams)
        {
            SshExec exec = CreateSshObject<SshExec>();

            try
            {
                ConnectSshObject(exec);

                string exeName = innerConfig[RemoteOption.UserScriptsFolder] +
                                 innerConfig[RemoteOption.CompilerScript];
                string command = string.Format(
                    "{0} --solution={1} --lang={2}",
                    exeName,
                    RunnerHelper.Language[lang],
                    scriptParams);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult {Output = stderr, ReturnState = status};
                else
                    return new RunnerCommandResult {Output = stdout, ReturnState = status};
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult {Output = ex.Message, ReturnState = 1};
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
        /// <param name="lang">Language of runned solution</param>
        /// <param name="pathToInput">Path to file with input</param>
        /// <param name="scriptParams">Additional parameters</param>
        /// <returns></returns>
        public RunnerCommandResult Run(string pathToSolution, SourceLanguage lang, string pathToInput,
            string scriptParams)
        {
            SshExec exec = CreateSshObject<SshExec>();

            try
            {
                ConnectSshObject(exec);

                string exeName = innerConfig[RemoteOption.UserScriptsFolder] +
                                 innerConfig[RemoteOption.RunnerScript];

                // TODO move this strings to config file
                string command = string.Format(
                    "{0} --solution={1} --lang={2} --input={3} {4}",
                    exeName,
                    pathToSolution,
                    RunnerHelper.Language[lang],
                    pathToInput,
                    scriptParams);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult {Output = stderr, ReturnState = status};
                else
                    return new RunnerCommandResult {Output = stdout, ReturnState = status};
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult {Output = ex.Message, ReturnState = 1};
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
            SshExec exec = CreateSshObject<SshExec>();

            try
            {
                ConnectSshObject(exec);

                string exeName = innerConfig[RemoteOption.UserScriptsFolder] +
                                 innerConfig[RemoteOption.CleanUpScript];

                // TODO move this strings to config file
                string command = string.Format(
                    "{0} --solution={1}",
                    exeName,
                    solutionFolder);

                string stdout = string.Empty;
                string stderr = string.Empty;

                int status = exec.RunCommand(command, ref stdout, ref stderr);

                if (stderr.Length > 0)
                    return new RunnerCommandResult {Output = stderr, ReturnState = status};
                else
                    return new RunnerCommandResult {Output = stdout, ReturnState = status};
            }
            catch (Exception ex)
            {
                return new RunnerCommandResult {Output = ex.Message, ReturnState = 1};
            }
            finally
            {
                exec.Close();
            }
        }

        public RunnerCommandResult Stop(int pid)
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
            return innerConfig[option];
        }
    }
}