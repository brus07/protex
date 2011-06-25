using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    /// <summary>
    /// Class, that contains data, needed to 
    /// run a specified program
    /// </summary>
    public class RunnerStartInfo
    {
        /// <summary>
        /// Command, used to run an application
        /// (can contain several commands,
        ///  like "python solution.py")
        /// </summary>
        string AppRunString { get; set; }
        
        /// <summary>
        /// Path to input data storage
        /// </summary>
        string STD_IN;

        /// <summary>
        /// Name of output file
        /// </summary>
        string STD_OUT;

        /// <summary>
        /// Name of errors file
        /// </summary>
        string STD_ERR;

        /// <summary>
        /// Path to directory with solution
        /// </summary>
        string WorkingDirectory;
    }
}
