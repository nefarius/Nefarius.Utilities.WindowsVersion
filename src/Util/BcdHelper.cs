#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Management;

using Nefarius.Utilities.WindowsVersion.Exceptions;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utility to interact with the Boot Configuration Database.
/// </summary>
/// <remarks>Source: https://geoffchappell.com/notes/windows/boot/bcd/elements.htm</remarks>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class BcdHelper
{
    private const uint BcdLibraryBoolean_AllowPrereleaseSignatures = 0x16000049;

    /// <summary>
    ///     Gets or sets the current value of BCDE_LIBRARY_TYPE_ALLOW_PRERELEASE_SIGNATURES from the default boot entry.
    /// </summary>
    public static bool AllowPrereleaseSignatures
    {
        get
        {
            ConnectionOptions connectionOptions =
                new() { Impersonation = ImpersonationLevel.Impersonate, EnablePrivileges = true };

            ManagementScope managementScope = new(@"root\WMI", connectionOptions);

            using ManagementObject bootMgrObj = new(managementScope,
                new ManagementPath(
                    "root\\WMI:BcdObject.Id=\"{fa926493-6f1c-4193-a414-58f0b2456d1e}\",StoreFilePath=\"\""),
                null);
            ManagementBaseObject inParams = bootMgrObj.GetMethodParameters("GetElement");

            inParams["Type"] = BcdLibraryBoolean_AllowPrereleaseSignatures;
            ManagementBaseObject? outParams = bootMgrObj.InvokeMethod("GetElement", inParams, null);
            ManagementBaseObject? outObj = (ManagementBaseObject?)outParams?.Properties["Element"].Value;

            bool allowPrereleaseSignatures = outObj != null && (bool)outObj.GetPropertyValue("Boolean");

            return allowPrereleaseSignatures;
        }

        set
        {
            ConnectionOptions connectionOptions =
                new() { Impersonation = ImpersonationLevel.Impersonate, EnablePrivileges = true };

            ManagementScope managementScope = new(@"root\WMI", connectionOptions);

            using ManagementObject bootMgrObj = new(managementScope,
                new ManagementPath(
                    "root\\WMI:BcdObject.Id=\"{fa926493-6f1c-4193-a414-58f0b2456d1e}\",StoreFilePath=\"\""),
                null);
            ManagementBaseObject inParams = bootMgrObj.GetMethodParameters("SetBooleanElement");

            inParams["Type"] = BcdLibraryBoolean_AllowPrereleaseSignatures;
            inParams["Boolean"] = value;
            ManagementBaseObject? outParams = bootMgrObj.InvokeMethod("SetBooleanElement", inParams, null);
            object? ret = outParams?.Properties["ReturnValue"].Value;
            bool returnValue = ret != null && (bool)ret;

            if (!returnValue)
            {
                throw new BcdAlterAllowPrereleaseSignaturesFailedException(
                    "Couldn't change TESTSIGNING state");
            }
        }
    }
}