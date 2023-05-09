using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.WindowsVersion.Util;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class OsVersionInfo
{
    private const int OS_ANYSERVER = 29;

    /// <summary>
    ///     True if Windows Server (any version) is detected, false otherwise.
    /// </summary>
    public static bool IsWindowsServer => IsOS(OS_ANYSERVER);

    [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
    private static extern bool IsOS(int os);
}