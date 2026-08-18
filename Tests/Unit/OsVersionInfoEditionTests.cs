using Nefarius.Utilities.WindowsVersion.Util;

namespace Tests.Unit;

[TestFixture]
[Category(TestCategories.Unit)]
internal class OsVersionInfoEditionTests
{
    private const int ProductProfessional = 0x00000030;
    private const int ProductCore = 0x00000065;
    private const int ProductEnterpriseS = 0x0000007D;
    private const int ProductIotEnterprise = 0x000000BC;
    private const int ProductUndefined = 0x00000000;
    private const int UnknownProduct = 0x0000FFFF;

    [TestCase(ProductProfessional, "Professional")]
    [TestCase(ProductCore, "Home")]
    [TestCase(ProductEnterpriseS, "Enterprise LTSC")]
    [TestCase(ProductIotEnterprise, "IoT Enterprise")]
    [TestCase(ProductUndefined, "Unknown product")]
    public void MapProductInfoToEdition_KnownCodes(int productInfo, string expected)
    {
        string edition = OsVersionInfo.MapProductInfoToEdition(productInfo);

        Assert.That(edition, Is.EqualTo(expected));
    }

    [Test]
    public void MapProductInfoToEdition_UnknownCode_ReturnsEmpty()
    {
        string edition = OsVersionInfo.MapProductInfoToEdition(UnknownProduct);

        Assert.That(edition, Is.Empty);
    }
}
