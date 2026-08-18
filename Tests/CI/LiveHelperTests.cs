using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.CI;

[TestFixture]
[Category(TestCategories.CI)]
internal class LiveHelperTests
{
    [Test]
    public void ArchitectureInfo_ReadOnlyProperties_DoNotThrow()
    {
        Assert.DoesNotThrow(() => _ = ArchitectureInfo.IsArm64);
        Assert.DoesNotThrow(() => _ = ArchitectureInfo.OsBits);
        Assert.DoesNotThrow(() => _ = ArchitectureInfo.ProcessorBits);
        Assert.That(ArchitectureInfo.ProcessorBits, Is.Not.EqualTo(ProcessorArchitecture.Unknown));
    }

    [Test]
    public void UefiHelper_ReadOnlyProperties_DoNotThrow()
    {
        Assert.DoesNotThrow(() => _ = UefiHelper.IsRunningInUefiMode);
        Assert.DoesNotThrow(() => _ = UefiHelper.IsSecureBootEnabled);
    }

    [Test]
    public void CodeIntegrityHelper_IsTestSignEnabled_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _ = CodeIntegrityHelper.IsTestSignEnabled);
    }

    [Test]
    public void OsUpgradeDetection_IsGrandfathered_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => _ = OsUpgradeDetection.IsGrandfathered);
    }
}
