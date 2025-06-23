using System.Diagnostics.CodeAnalysis;

using Windows.Win32;
using Windows.Win32.UI.Shell;

namespace Nefarius.Utilities.WindowsVersion.Util;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class OsVersionInfo
{
    /// <summary>
    ///     True if Windows Server (any version) is detected, false otherwise.
    /// </summary>
    public static bool IsWindowsServer => PInvoke.IsOS(OS.OS_ANYSERVER);
}