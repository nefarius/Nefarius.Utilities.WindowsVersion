using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
internal class ArchitectureInfoTests
{
    [Test]
    public void ProgramBits_MatchesPointerSize()
    {
        SoftwareArchitecture expected = (IntPtr.Size * 8) switch
        {
            64 => SoftwareArchitecture.Bit64,
            32 => SoftwareArchitecture.Bit32,
            _ => SoftwareArchitecture.Unknown
        };

        Assert.That(ArchitectureInfo.ProgramBits, Is.EqualTo(expected));
    }
}
