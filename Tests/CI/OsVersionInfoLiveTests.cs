using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.CI;

[TestFixture]
[Category(TestCategories.CI)]
internal class OsVersionInfoLiveTests
{
    [Test]
    public void VersionFields_ArePopulated()
    {
        Assert.That(OsVersionInfo.MajorVersion, Is.EqualTo(10));
        Assert.That(OsVersionInfo.BuildVersion, Is.Not.Null);
        Assert.That(OsVersionInfo.BuildVersion!.Value, Is.GreaterThan(0));
        Assert.That(OsVersionInfo.VersionString, Is.Not.Empty);
        Assert.That(OsVersionInfo.Edition, Is.Not.Empty);
        Assert.That(OsVersionInfo.Name, Does.Contain("Windows"));
        Assert.That(OsVersionInfo.Name, Is.Not.EqualTo("Unknown"));
    }

    [Test]
    public void Windows10Flag_IsTrueOnModernHosts()
    {
        Assert.That(OsVersionInfo.IsWindows10, Is.True);
    }

    [Test]
    public void Name_MatchesServerOrClientFlags()
    {
        int build = OsVersionInfo.BuildVersion ?? 0;

        if (OsVersionInfo.IsWindowsServer)
        {
            Assert.That(OsVersionInfo.IsWindows11, Is.False);
            Assert.That(OsVersionInfo.Name, Does.StartWith("Windows Server"));

            if (build >= 26100)
            {
                Assert.That(OsVersionInfo.Name, Is.EqualTo("Windows Server 2025"));
            }
        }
        else
        {
            if (build >= 22000)
            {
                Assert.That(OsVersionInfo.IsWindows11, Is.True);
                Assert.That(OsVersionInfo.Name, Does.StartWith("Windows 11"));
            }
            else
            {
                Assert.That(OsVersionInfo.IsWindows11, Is.False);
                Assert.That(OsVersionInfo.Name, Does.StartWith("Windows 10"));
            }
        }
    }

    [Test]
    public void IsUacDisabled_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _ = OsVersionInfo.IsUacDisabled);
    }
}
