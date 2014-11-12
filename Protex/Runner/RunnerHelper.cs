using System.Collections.Generic;

namespace ProtexCore.Runner
{
    public static class RunnerHelper
    {
        public static Dictionary<SourceLanguage, string> Language;

        static RunnerHelper()
        {
            Language = new Dictionary<SourceLanguage, string>
            {
                {SourceLanguage.C, "c"},
                {SourceLanguage.Cpp, "cpp"},
                {SourceLanguage.Java, "java"},
                {SourceLanguage.Pascal, "pas"},
                {SourceLanguage.Ruby, "ruby"},
                {SourceLanguage.Python, "py"},
                {SourceLanguage.Perl, "perl"},
                {SourceLanguage.PHP, "php"},
                {SourceLanguage.Lisp, "lisp"}
            };
        }
    }
}