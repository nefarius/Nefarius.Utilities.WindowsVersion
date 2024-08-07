﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Utility to get UEFI details.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class UefiHelper
{
    /// <summary>
    ///     Checks if Secure Boot is enabled.
    /// </summary>
    public static bool IsSecureBootEnabled
    {
        get
        {
            int? val = (int?)Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State",
                "UEFISecureBootEnabled",
                0);

            return val > 0;
        }
    }

    /// <summary>
    ///     Checks if the current system is running in UEFI or Legacy BIOS mode.
    /// </summary>
    /// <remarks>Source: https://theroadtodelphi.com/2013/02/19/how-distinguish-when-windows-was-installed-in-legacy-bios-or-uefi-mode-using-delphi/</remarks>
    public static bool IsRunningInUefiMode
    {
        get
        {
            // The arguments submitted are dummy values; GetLastError will
            // report ERROR_INVALID_FUNCTION on Legacy BIOS systems.
            GetFirmwareEnvironmentVariable(
                string.Empty,
                "{00000000-0000-0000-0000-000000000000}",
                IntPtr.Zero, 0);

            return Marshal.GetLastWin32Error() != 0x01; // ERROR_INVALID_FUNCTION
        }
    }

    #region Native

    [DllImport("kernel32.dll", EntryPoint = "GetFirmwareEnvironmentVariableW", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern uint GetFirmwareEnvironmentVariable(string lpName, string lpGuid, IntPtr pBuffer,
        uint nSize);

    #endregion
}