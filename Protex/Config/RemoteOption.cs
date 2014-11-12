namespace ProtexCore.Config
{
    public enum RemoteOption
    {
        SSH_Port,
        /* Possible replacement for UserName and password */
        SSH_IdentityFile,
        SSH_IdentityPassphrase,
        /* Connection options */
        SSH_HostAddress,
        SSH_UserName,
        SSH_Password,
        /* Special folders pathes:
         * 
         * UserScriptsFolder - folder with all needed scripts
         *         TmpFolder - user sources are first saved here
         *   SolutionsFolder - path to folder with compiled solutions
        */
        UserScriptsFolder,
        TmpFolder,
        SolutionsFolder,
        /* Scripts names:
         * 
         * OrganizerScript - moves file from temporary location to 
         *                   special folder 
         *  CompilerScript - compiles user sources if they need to
         *                   be compiled
         *    RunnerScript - runs user solution in right way with 
         *                   special security restrictions or without
         *                   them. Also measures time.
         *   CleanUpScript - removes temporary data (e.g. user solution)
        */
        OrganizerScript,
        CompilerScript,
        RunnerScript,
        CleanUpScript
    }
}