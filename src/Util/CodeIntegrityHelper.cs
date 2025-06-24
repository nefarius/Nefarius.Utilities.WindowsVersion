using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;

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
    public static bool IsTestSignEnabled
    {
        get
        {
            IntPtr pIntegrity = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEM_CODEINTEGRITY_INFORMATION>());

            try
            {
                using FreeLibrarySafeHandle ntDll = PInvoke.GetModuleHandle("ntdll.dll");
                FARPROC ptr = PInvoke.GetProcAddress(ntDll, "NtQuerySystemInformation");

                NtQuerySystemInformation ntQuerySystemInformation =
                    Marshal.GetDelegateForFunctionPointer<NtQuerySystemInformation>(ptr);

                SYSTEM_CODEINTEGRITY_INFORMATION integrity;
                integrity.Length = (uint)Marshal.SizeOf<SYSTEM_CODEINTEGRITY_INFORMATION>();
                integrity.CodeIntegrityOptions = 0;

                Marshal.StructureToPtr(integrity, pIntegrity, false);

                // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/codeintegrity.htm
                int status = ntQuerySystemInformation(
                    103, // SystemCodeIntegrityInformation (0x67)
                    pIntegrity,
                    integrity.Length,
                    out _
                );

                int error = Marshal.GetLastWin32Error();

                if (status != (int)WIN32_ERROR.ERROR_SUCCESS)
                {
                    throw new Win32Exception(error, "NtQuerySystemInformation failed");
                }

                integrity = Marshal.PtrToStructure<SYSTEM_CODEINTEGRITY_INFORMATION>(pIntegrity);

                return (integrity.CodeIntegrityOptions & /* CODEINTEGRITY_OPTION_TESTSIGN */ 0x02) != 0;
            }
            finally
            {
                Marshal.FreeHGlobal(pIntegrity);
            }
        }
    }

    #region Native

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int NtQuerySystemInformation(
        uint systemInformationClass,
        IntPtr systemInformation,
        uint systemInformationLength,
        out uint returnLength);

    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private struct SYSTEM_CODEINTEGRITY_INFORMATION
    {
        public uint Length;
        public uint CodeIntegrityOptions;
    }

    #endregion
}