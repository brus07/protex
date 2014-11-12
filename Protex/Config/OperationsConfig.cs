using System.Collections.Generic;

namespace ProtexCore.Config
{
    public abstract class OperationsConfig<T>
    {
        protected Dictionary<T, string> Options;
        protected Dictionary<string, T> KnownOptions;

        public OperationsConfig()
        {
            Options = new Dictionary<T, string>();
            // just to be clear
            KnownOptions = null;
        }

        /// <summary>
        /// Gets the <see cref="OperationsConfig{T}"/> with the specified option.
        /// </summary>
        /// <param name='option'>
        /// Option.
        /// </param>
        public string this[T option]
        {
            get { return Options[option]; }
        }

        public bool HasOption(T option)
        {
            return Options.ContainsKey(option);
        }

        public abstract void ParseConfig(string pathToXml, string rootElementName);
    }
}