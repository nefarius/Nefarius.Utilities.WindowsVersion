#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.SystemInformation;

using JetBrains.Annotations;

using Microsoft.Win32.SafeHandles;

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
    Itanium64 = 3,

    /// <summary>
    ///     ARM64 (AArch64).
    /// </summary>
    Arm64 = 4
}

/// <summary>
///     Process and operating system architecture detection.
/// </summary>
/// <remarks>Source: https://stackoverflow.com/a/54539366/490629</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[UsedImplicitly]
public static class ArchitectureInfo
{
    /// <summary>
    ///     Gets whether the current process is running on ARM64.
    /// </summary>
    public static bool IsArm64
    {
        get
        {
            try
            {
                SafeProcessHandle handle = Process.GetCurrentProcess().SafeHandle;
                if (!PInvoke.IsWow64Process2(handle, out _, out IMAGE_FILE_MACHINE nativeMachine))
                {
                    return false;
                }

                return nativeMachine == IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM64;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }
    }

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
                PInvoke.GetNativeSystemInfo(out SYSTEM_INFO systemInfo);

                pBits = systemInfo.Anonymous.Anonymous.wProcessorArchitecture switch
                {
                    PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_AMD64 =>
                        ProcessorArchitecture.Bit64,
                    PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_IA64 =>
                        ProcessorArchitecture.Itanium64,
                    PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_INTEL =>
                        ProcessorArchitecture.Bit32,
                    PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM64 =>
                        ProcessorArchitecture.Arm64,
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

    private static bool Is32BitProcessOn64BitProcessor()
    {
        SafeProcessHandle handle = Process.GetCurrentProcess().SafeHandle;
        return PInvoke.IsWow64Process(handle, out BOOL isWow64) && isWow64;
    }
}
