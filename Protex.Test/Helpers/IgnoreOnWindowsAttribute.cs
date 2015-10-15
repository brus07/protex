using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test.Helpers
{
    class IgnoreOnWindowsAttribute : Attribute, ITestAction
    {
        private readonly bool IsUnix;

        public IgnoreOnWindowsAttribute()
        {
            IsUnix = Configurator.IsUnix;
        }

        public void BeforeTest(TestDetails testDetails)
        {
            if (!IsUnix)
            {
                Assert.Ignore("Test is Ignored on Windows");
            }
        }

        public void AfterTest(TestDetails testDetails) { }

        public ActionTargets Targets { get; private set; }
    }
}
