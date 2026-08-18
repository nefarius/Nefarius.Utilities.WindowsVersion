using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
internal class OsVersionInfoNameTests
{
    private const byte Workstation = 1;
    private const byte DomainController = 2;
    private const byte Server = 3;

    [TestCase(21999, "21H2", "Windows 10 (21H2)")]
    [TestCase(22000, "21H2", "Windows 11 (21H2)")]
    [TestCase(26100, "24H2", "Windows 11 (24H2)")]
    public void ParseWindows10Version_Workstation_UsesBuildThreshold(int build, string label, string expected)
    {
        string name = OsVersionInfo.ParseWindows10Version(0, Workstation, build, label);

        Assert.That(name, Is.EqualTo(expected));
    }

    [TestCase(14392, "1607", "Windows Server (1607)")]
    [TestCase(14393, "1607", "Windows Server 2016")]
    [TestCase(17763, "1809", "Windows Server 2019")]
    [TestCase(20348, "21H2", "Windows Server 2022")]
    [TestCase(26100, "24H2", "Windows Server 2025")]
    public void ParseWindows10Version_Server_UsesBuildThreshold(int build, string label, string expected)
    {
        string name = OsVersionInfo.ParseWindows10Version(0, Server, build, label);

        Assert.That(name, Is.EqualTo(expected));
    }

    [Test]
    public void ParseWindows10Version_DomainController_UsesServerNames()
    {
        string name = OsVersionInfo.ParseWindows10Version(0, DomainController, 26100, "24H2");

        Assert.That(name, Is.EqualTo("Windows Server 2025"));
    }

    [Test]
    public void ParseWindows10Version_NonZeroMinor_ReturnsEmpty()
    {
        string name = OsVersionInfo.ParseWindows10Version(1, Workstation, 22000, "21H2");

        Assert.That(name, Is.Empty);
    }

    [TestCase(0, Workstation, "Windows Vista")]
    [TestCase(0, Server, "Windows Server 2008")]
    [TestCase(1, Workstation, "Windows 7")]
    [TestCase(1, Server, "Windows Server 2008 R2")]
    [TestCase(2, Workstation, "Windows 8")]
    [TestCase(2, Server, "Windows Server 2012")]
    [TestCase(3, Workstation, "Windows 8.1")]
    [TestCase(3, Server, "Windows Server 2012 R2")]
    public void ParseVistaThrough8_KnownSkus(int minorVersion, byte productType, string expected)
    {
        string name = OsVersionInfo.ParseVistaThrough8(minorVersion, productType);

        Assert.That(name, Is.EqualTo(expected));
    }

    [Test]
    public void ParseVistaThrough8_UnknownCombination_ReturnsEmpty()
    {
        string name = OsVersionInfo.ParseVistaThrough8(4, Workstation);

        Assert.That(name, Is.Empty);
    }
}
