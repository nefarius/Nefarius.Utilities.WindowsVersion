using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Win32;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utilities to detect if this system is a fresh or upgraded installation.
/// </summary>
/// <remarks>https://superuser.com/a/1184670</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class OsUpgradeDetection
{
    /// <summary>
    ///     Gets whether the system has been in-place upgraded.
    /// </summary>
    public static bool IsGrandfathered
    {
        get
        {
            using RegistryKey setupKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\Setup");
            using RegistryKey upgradeKey = setupKey?.OpenSubKey("Upgrade");
            // if this key isn't there, don't look any further
            if (upgradeKey == null)
            {
                return false;
            }

            // only look at "Source OS (...)" sub-keys
            foreach (string sosKeyName in setupKey.GetSubKeyNames().Where(v =>
                         v.StartsWith("Source OS", StringComparison.InvariantCultureIgnoreCase)))
            {
                using RegistryKey sosKey = setupKey.OpenSubKey(sosKeyName);
                string productName = sosKey?.GetValue("ProductName") as string;

                if (string.IsNullOrEmpty(productName))
                {
                    continue;
                }

                // TODO: untested but should work
                if (productName.StartsWith("Windows 7", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                if (productName.StartsWith("Windows 8", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}