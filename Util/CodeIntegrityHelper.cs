using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace Nefarius.Utilities.WindowsVersion.Util
{
    /// <summary>
    ///     Utility class for receiving code integrity states currently enforced.
    /// </summary>
    public static class CodeIntegrityHelper
    {
        /// <summary>
        ///     Determines if the system is currently in TESTSIGNING mode.
        /// </summary>
        public static bool IsTestSignEnabled
        {
            get
            {
                var pIntegrity = Marshal.AllocHGlobal(Marshal.SizeOf<SYSTEM_CODEINTEGRITY_INFORMATION>());

                try
                {
                    var fptr = Kernel32.GetProcAddress(new Kernel32.SafeLibraryHandle(Kernel32.GetModuleHandle("ntdll.dll")),
                        "NtQuerySystemInformation");

                    var ntQuerySystemInformation =
                        Marshal.GetDelegateForFunctionPointer<NtQuerySystemInformation>(fptr);

                    SYSTEM_CODEINTEGRITY_INFORMATION integrity;
                    integrity.Length = (uint) Marshal.SizeOf<SYSTEM_CODEINTEGRITY_INFORMATION>();
                    integrity.CodeIntegrityOptions = 0;

                    Marshal.StructureToPtr(integrity, pIntegrity, false);

                    // https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/codeintegrity.htm
                    var status = ntQuerySystemInformation(
                        103, // SystemCodeIntegrityInformation (0x67)
                        pIntegrity,
                        integrity.Length,
                        out _
                    );

                    var error = Kernel32.GetLastError();

                    if (status != 0)
                        throw new Win32Exception(Kernel32.GetLastError(),
                            "NtQuerySystemInformation failed");

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
            uint SystemInformationClass,
            IntPtr SystemInformation,
            uint SystemInformationLength,
            out uint ReturnLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_CODEINTEGRITY_INFORMATION
        {
            public uint Length;
            public uint CodeIntegrityOptions;
        }

        #endregion
    }
}