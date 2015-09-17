using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    class Configurator
    {
        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }
    }
}
