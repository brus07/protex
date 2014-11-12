using System;
using ProtexCore.Config;

namespace ProtexCore.Runner
{
    internal class LocalRunner : IRunner
    {
        private LocalConfig innerConfig;

        public LocalRunner()
        {
        }

        public LocalRunner(LocalRunner copy)
            : this(copy.innerConfig)
        {
        }

        public LocalRunner(LocalConfig config)
        {
            innerConfig = config;
        }

        public RunnerCommandResult CopySource(string initialPathSource, string newSourceFileName)
        {
            throw new NotImplementedException();
        }

        public RunnerCommandResult OrganizeSource(string sourceName, string scriptParams)
        {
            throw new NotImplementedException();
        }

        public RunnerCommandResult CompileSource(string pathToSource, SourceLanguage language, string scriptParams)
        {
            throw new NotImplementedException();
        }

        public RunnerCommandResult Run(string pathToSolution, SourceLanguage language, string pathToInput,
            string scriptParams)
        {
            throw new NotImplementedException();
        }

        public RunnerCommandResult CleanUp(string solutionFolder)
        {
            throw new NotImplementedException();
        }

        public RunnerCommandResult Stop(int pid)
        {
            throw new NotImplementedException();
        }
    }
}