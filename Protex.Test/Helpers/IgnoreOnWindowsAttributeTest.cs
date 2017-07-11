using NUnit.Framework;

namespace Protex.Test.Helpers
{
    [TestFixture]
    public class IgnoreOnWindowsAttributeTest
    {
        [Test]
        [Platform(Exclude = "Win")]
        public void TestIgnoreOnWindowsAttribute()
        {
            Assert.AreEqual(Configurator.IsUnix, true);
        }

        [Test]
        [Platform("Linux")]
        public void TestConfiguratorIsUnixOnLinux()
        {
            Assert.AreEqual(Configurator.IsUnix, true);
        }

        [Test]
        [Platform("Win")]
        public void TestConfiguratorIsUnixAnWindows()
        {
            Assert.AreEqual(Configurator.IsUnix, false);
        }
    }
}
