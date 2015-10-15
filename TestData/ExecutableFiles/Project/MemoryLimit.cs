using System;
using System.Text;

namespace MemoryLimit
{
    static class Program
    {
        static void Main()
        {
        StringBuilder sb = new StringBuilder();
            for (int i=0; i<10000000; i++)
            {
                sb.Append('a');
            }
            Console.Read();
        }
    }
}
