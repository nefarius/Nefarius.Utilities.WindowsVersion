#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Win32;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utilities to detect if this system is a fresh or upgraded installation.
/// </summary>
/// <remarks>Source: https://superuser.com/a/1184670</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class OsUpgradeDetection
{
    /// <summary>
    ///     Gets whether the system has been in-place upgraded.
    /// </summary>
    public static bool IsGrandfathered
    {
        get
        {
            using RegistryKey? setupKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\Setup");
            if (setupKey is null)
            {
                return false;
            }

            using RegistryKey? upgradeKey = setupKey.OpenSubKey("Upgrade");
            if (upgradeKey is null)
            {
                return false;
            }

            return HasSourceOsUpgrade(setupKey.GetSubKeyNames(), sosKeyName =>
            {
                using RegistryKey? sosKey = setupKey.OpenSubKey(sosKeyName);
                return sosKey?.GetValue("ProductName") as string;
            });
        }
    }

    /// <summary>
    ///     True when a <c>Source OS*</c> setup subkey reports a non-empty ProductName.
    /// </summary>
    internal static bool HasSourceOsUpgrade(IEnumerable<string> subKeyNames, Func<string, string?> getProductName)
    {
        foreach (string sosKeyName in subKeyNames.Where(v =>
                     v.StartsWith("Source OS", StringComparison.InvariantCultureIgnoreCase)))
        {
            if (!string.IsNullOrEmpty(getProductName(sosKeyName)))
            {
                return true;
            }
        }

        return false;
    }
}
