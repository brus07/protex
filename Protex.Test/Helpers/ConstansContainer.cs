using System.IO;

namespace Protex.Test.Helpers
{
    class ConstansContainer
    {
        public static string ExecutableFilesPath = Path.Combine("..", "..", "..", "..", "TestData", "ExecutableFiles");

        public static string TemporaryExecutableFilesPath = Path.Combine(ExecutableFilesPath, "TemporaryExecutableFiles");
    }
}
