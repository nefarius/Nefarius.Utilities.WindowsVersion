using System;
using Microsoft.Win32;

namespace Nefarius.Utilities.WindowsVersion.Util
{
    /// <summary>
    ///     Utility class for interaction with Code Integrity Policy settings.
    /// </summary>
    /// <remarks>https://www.geoffchappell.com/notes/security/whqlsettings/index.htm</remarks>
    public static class CodeIntegrityPolicyHelper
    {
        /// <summary>
        ///     Gets or sets whether the kernel allows loading of "traditional" cross-signed drivers.
        /// </summary>
        public static bool WhqlDeveloperTestMode
        {
            get
            {
                var value = (int?) Registry.GetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\CI\Policy",
                    "WhqlDeveloperTestMode", null);

                return (value.HasValue && value.Value > 0);
            }

            set => Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\CI\Policy",
                "WhqlDeveloperTestMode", Convert.ToInt32(value), RegistryValueKind.DWord);
        }
    }
}