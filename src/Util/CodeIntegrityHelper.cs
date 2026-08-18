#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Wdk.System.SystemInformation;
using Windows.Win32.Foundation;
using Windows.Win32.System.WindowsProgramming;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utility class for receiving code integrity states currently enforced.
/// </summary>
/// <remarks>Source: https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/codeintegrity.htm</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class CodeIntegrityHelper
{
    /// <summary>
    ///     Determines if the system is currently in TESTSIGNING mode.
    /// </summary>
    public static unsafe bool IsTestSignEnabled
    {
        get
        {
            SYSTEM_CODEINTEGRITY_INFORMATION integrity = new()
            {
                Length = (uint)sizeof(SYSTEM_CODEINTEGRITY_INFORMATION)
            };

            uint returnLength = 0;
            NTSTATUS status = Windows.Wdk.PInvoke.NtQuerySystemInformation(
                SYSTEM_INFORMATION_CLASS.SystemCodeIntegrityInformation,
                &integrity,
                integrity.Length,
                ref returnLength);

            if ((int)status < 0)
            {
                throw new NtStatusException(status);
            }

            return (integrity.CodeIntegrityOptions & /* CODEINTEGRITY_OPTION_TESTSIGN */ 0x02) != 0;
        }
    }

    private sealed class NtStatusException : Exception
    {
        internal NtStatusException(NTSTATUS status)
            : base($"NtQuerySystemInformation failed with NTSTATUS {status}")
        {
            HResult = status;
        }
    }
}
