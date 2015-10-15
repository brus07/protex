using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex.Test.Helpers
{
    [TestFixture]
    public class IgnoreOnWindowsAttributeTest
    {
        [Test]
        [IgnoreOnWindows]
        public void TestIgnoreOnWindowsAttribute()
        {
            Assert.AreEqual(Configurator.IsUnix, true);
        }
    }
}
