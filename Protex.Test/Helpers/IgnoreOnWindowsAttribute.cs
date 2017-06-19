using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;

namespace Protex.Test.Helpers
{
    class IgnoreOnWindowsAttribute : Attribute, ITestAction
    {
        private readonly bool IsUnix;

        public IgnoreOnWindowsAttribute()
        {
            IsUnix = Configurator.IsUnix;
        }

        public void BeforeTest(ITest test)
        {
            if (!IsUnix)
            {
                Assert.Ignore("Test is Ignored on Windows");
            }
        }

        public void AfterTest(ITest test)
        {
        }

        public ActionTargets Targets { get; private set; }
    }
}
