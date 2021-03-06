﻿using System;
using System.Configuration;

namespace Protex.Test.Helpers
{
    class Configurator
    {
        public static string Compiler
        {
            get
            {
                string currentCompiler = ConfigurationManager.AppSettings["CscCompiler"];

                //for Unix with Mono (mcs)
                if (IsUnix)
                    currentCompiler = ConfigurationManager.AppSettings["McsCompiler"];

                return currentCompiler;
            }
        }

        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }
    }
}
