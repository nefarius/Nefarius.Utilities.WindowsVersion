using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
internal class OsUpgradeDetectionTests
{
    [Test]
    public void HasSourceOsUpgrade_SourceOsWithProductName_IsTrue()
    {
        string[] keys = ["Upgrade", "Source OS (Updated on 2024-01-01)"];

        bool result = OsUpgradeDetection.HasSourceOsUpgrade(keys, name =>
            name.StartsWith("Source OS", StringComparison.OrdinalIgnoreCase) ? "Windows 10 Pro" : null);

        Assert.That(result, Is.True);
    }

    [Test]
    public void HasSourceOsUpgrade_IsCaseInsensitive()
    {
        string[] keys = ["source os (Updated on 2024-01-01)"];

        bool result = OsUpgradeDetection.HasSourceOsUpgrade(keys, _ => "Windows 8.1");

        Assert.That(result, Is.True);
    }

    [Test]
    public void HasSourceOsUpgrade_EmptyProductName_IsFalse()
    {
        string[] keys = ["Source OS (Updated on 2024-01-01)"];

        bool result = OsUpgradeDetection.HasSourceOsUpgrade(keys, _ => string.Empty);

        Assert.That(result, Is.False);
    }

    [Test]
    public void HasSourceOsUpgrade_NoSourceOsKeys_IsFalse()
    {
        string[] keys = ["Upgrade", "Other"];

        bool result = OsUpgradeDetection.HasSourceOsUpgrade(keys, _ => "Windows 10 Pro");

        Assert.That(result, Is.False);
    }
}
