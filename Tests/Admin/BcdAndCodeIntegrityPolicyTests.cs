using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.Admin;

[TestFixture]
[Explicit("Mutates BCD / CI policy; run elevated on a lab machine.")]
[Category(TestCategories.Admin)]
[Category(TestCategories.Destructive)]
internal class BcdAndCodeIntegrityPolicyTests
{
    [Test]
    public void AllowPrereleaseSignatures_RoundTrips()
    {
        bool original = BcdHelper.AllowPrereleaseSignatures;

        try
        {
            BcdHelper.AllowPrereleaseSignatures = !original;
            Assert.That(BcdHelper.AllowPrereleaseSignatures, Is.EqualTo(!original));
        }
        finally
        {
            BcdHelper.AllowPrereleaseSignatures = original;
            Assert.That(BcdHelper.AllowPrereleaseSignatures, Is.EqualTo(original));
        }
    }

    [Test]
    public void WhqlDeveloperTestMode_RoundTrips()
    {
        bool original = CodeIntegrityPolicyHelper.WhqlDeveloperTestMode;

        try
        {
            CodeIntegrityPolicyHelper.WhqlDeveloperTestMode = !original;
            Assert.That(CodeIntegrityPolicyHelper.WhqlDeveloperTestMode, Is.EqualTo(!original));
        }
        finally
        {
            CodeIntegrityPolicyHelper.WhqlDeveloperTestMode = original;
            Assert.That(CodeIntegrityPolicyHelper.WhqlDeveloperTestMode, Is.EqualTo(original));
        }
    }
}
