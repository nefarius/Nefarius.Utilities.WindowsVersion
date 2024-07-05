using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utility class for interaction with Code Integrity Policy settings.
/// </summary>
/// <remarks>https://www.geoffchappell.com/notes/security/whqlsettings/index.htm</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class CodeIntegrityPolicyHelper
{
    /// <summary>
    ///     Gets or sets whether the kernel allows loading of "traditional" cross-signed drivers.
    /// </summary>
    /// <remarks>Availability of this functionality heavily depends on the Windows build used.</remarks>
    public static bool WhqlDeveloperTestMode
    {
        get
        {
            int? value = (int?)Registry.GetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\CI\Policy",
                "WhqlDeveloperTestMode", null);

            return value is > 0;
        }

        set => Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\CI\Policy",
            "WhqlDeveloperTestMode", Convert.ToInt32(value), RegistryValueKind.DWord);
    }
}