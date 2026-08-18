namespace Tests;

/// <summary>
///     Shared NUnit category names for filtering CI vs elevated suites.
/// </summary>
internal static class TestCategories
{
    public const string Unit = "Unit";
    public const string CI = "CI";
    public const string Admin = "Admin";
    public const string Destructive = "Destructive";
}
