using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Architecture of the currently running process.
/// </summary>
public enum SoftwareArchitecture
{
    /// <summary>
    ///     Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     32-Bits (a.k.a x86).
    /// </summary>
    Bit32 = 1,

    /// <summary>
    ///     64-Bits (a.k.a. x86_64).
    /// </summary>
    Bit64 = 2
}

/// <summary>
///     Processor architecture.
/// </summary>
public enum ProcessorArchitecture
{
    /// <summary>
    ///     Unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     32-Bits (a.k.a x86).
    /// </summary>
    Bit32 = 1,

    /// <summary>
    ///     64-Bits (a.k.a. x86_64).
    /// </summary>
    Bit64 = 2,

    /// <summary>
    ///     IA-64 (Intel Itanium architecture).
    /// </summary>
    Itanium64 = 3
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static partial class OsVersionInfo
{
    /// <summary>
    ///     Determines if the current application is 32 or 64-bit.
    /// </summary>
    public static SoftwareArchitecture ProgramBits
    {
        get
        {
            SoftwareArchitecture pBits = (IntPtr.Size * 8) switch
            {
                64 => SoftwareArchitecture.Bit64,
                32 => SoftwareArchitecture.Bit32,
                _ => SoftwareArchitecture.Unknown
            };

            return pBits;
        }
    }

    /// <summary>
    ///     Determines if the current operating system is 32 or 64-bit.
    /// </summary>
    public static SoftwareArchitecture OsBits
    {
        get
        {
            SoftwareArchitecture osBits = (IntPtr.Size * 8) switch
            {
                64 => SoftwareArchitecture.Bit64,
                32 => Is32BitProcessOn64BitProcessor() ? SoftwareArchitecture.Bit64 : SoftwareArchitecture.Bit32,
                _ => SoftwareArchitecture.Unknown
            };

            return osBits;
        }
    }

    /// <summary>
    ///     Determines if the current processor is 32 or 64-bit.
    /// </summary>
    public static ProcessorArchitecture ProcessorBits
    {
        get
        {
            ProcessorArchitecture pBits = ProcessorArchitecture.Unknown;

            try
            {
                SYSTEM_INFO systemInfo = new();
                GetNativeSystemInfo(ref systemInfo);

                pBits = systemInfo.uProcessorInfo.wProcessorArchitecture switch
                {
                    9 => // PROCESSOR_ARCHITECTURE_AMD64
                        ProcessorArchitecture.Bit64,
                    6 => // PROCESSOR_ARCHITECTURE_IA64
                        ProcessorArchitecture.Itanium64,
                    0 => // PROCESSOR_ARCHITECTURE_INTEL
                        ProcessorArchitecture.Bit32,
                    _ => ProcessorArchitecture.Unknown
                };
            }
            catch
            {
                // Ignore        
            }

            return pBits;
        }
    }

    private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
    {
        IntPtr handle = LoadLibrary("kernel32");

        if (handle == IntPtr.Zero)
        {
            return null;
        }

        IntPtr fnPtr = GetProcAddress(handle, "IsWow64Process");

        if (fnPtr != IntPtr.Zero)
        {
            return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(fnPtr,
                typeof(IsWow64ProcessDelegate));
        }

        return null;
    }

    private static bool Is32BitProcessOn64BitProcessor()
    {
        IsWow64ProcessDelegate fnDelegate = GetIsWow64ProcessDelegate();

        if (fnDelegate == null)
        {
            return false;
        }

        bool retVal = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out bool isWow64);

        return retVal && isWow64;
    }

    private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);
}