#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Win32;

// http://www.codeproject.com/Articles/73000/Getting-Operating-System-Version-Info-Even-for-Win
//https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions

//Thanks to Member 7861383, Scott Vickery for the Windows 8.1 update and workaround.
//I have moved it to the beginning of the Name property, though...

//Thakts to Brisingr Aerowing for help with the Windows 10 adaptation
// Maintained and extended by Benjamin Höglinger-Stelzer 2018-2022

// Modified and extended by Benjamin "Nefarius" Höglinger-Stelzer 2022-2023

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Provides detailed information about the host operating system.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static partial class OsVersionInfo
{
    /// <summary>
    ///     https://en.wikipedia.org/wiki/Windows_10_version_history#Channels
    /// </summary>
    private static readonly List<string> Windows10ReleaseIds = new()
    {
        "1507", // <-- 1st public release of Windows 10 codenamed "Threshold 1"
        "1607",
        "1703",
        "1709",
        "1803",
        "1809",
        "1903",
        "1909",
        "2004",
        "2009" // <-- last time this value was actually updated by Microsoft
    };

    #region SERVICE PACK

    /// <summary>
    ///     Gets the service pack information of the operating system running on this computer.
    /// </summary>
    public static string ServicePack
    {
        get
        {
            string servicePack = string.Empty;
            OSVERSIONINFOEX osVersionInfo = new() { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };


            if (GetVersionEx(ref osVersionInfo))
            {
                servicePack = osVersionInfo.szCSDVersion;
            }

            return servicePack;
        }
    }

    #endregion SERVICE PACK

    #region Windows 10/Server 2016+ Detection

    /// <summary>
    ///     True if the current system is Windows 10 or newer, false otherwise.
    /// </summary>
    /// <remarks>This also includes Windows 11 due to the stupidity and inconsistency of Microsoft's versioning strategy.</remarks>
    public static bool IsWindows10
    {
        get
        {
            string? releaseId = (string?)Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion",
                "ReleaseId", null);

            return !string.IsNullOrEmpty(releaseId) && Windows10ReleaseIds.Any(id => id.Contains(releaseId));
        }
    }

    #endregion

    #region EDITION

    /// <summary>
    ///     Gets the edition of the operating system running on this computer.
    /// </summary>
    public static string Edition
    {
        get
        {
            string edition = string.Empty;

            OperatingSystem osVersion = Environment.OSVersion;
            OSVERSIONINFOEX osVersionInfo = new() { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };

            if (GetVersionEx(ref osVersionInfo))
            {
                int majorVersion = osVersion.Version.Major;
                int minorVersion = osVersion.Version.Minor;
                byte productType = osVersionInfo.wProductType;
                short suiteMask = osVersionInfo.wSuiteMask;

                #region VERSION 4

                if (majorVersion == 4)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        // Windows NT 4.0 Workstation
                        edition = "Workstation";
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                        {
                            edition = "Enterprise Server";
                        }
                        else
                        {
                            edition = "Standard Server";
                        }
                    }
                }

                #endregion VERSION 4

                #region VERSION 5

                else if (majorVersion == 5)
                {
                    if (productType == VER_NT_WORKSTATION)
                    {
                        if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                        {
                            edition = "Home";
                        }
                        else
                        {
                            if (GetSystemMetrics(86) == 0) // 86 == SM_TABLETPC
                            {
                                edition = "Professional";
                            }
                            else
                            {
                                edition = "Tablet Edition";
                            }
                        }
                    }
                    else if (productType == VER_NT_SERVER)
                    {
                        if (minorVersion == 0)
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                edition = "Datacenter Server";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                edition = "Advanced Server";
                            }
                            else
                            {
                                edition = "Server";
                            }
                        }
                        else
                        {
                            if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                            {
                                edition = "Datacenter";
                            }
                            else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                edition = "Enterprise";
                            }
                            else if ((suiteMask & VER_SUITE_BLADE) != 0)
                            {
                                edition = "Web Edition";
                            }
                            else
                            {
                                edition = "Standard";
                            }
                        }
                    }
                }

                #endregion VERSION 5

                #region VERSION 6

                else if (majorVersion == 6)
                {
                    int ed;
                    if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                    {
                        switch (ed)
                        {
                            case PRODUCT_BUSINESS:
                                edition = "Business";
                                break;
                            case PRODUCT_BUSINESS_N:
                                edition = "Business N";
                                break;
                            case PRODUCT_CLUSTER_SERVER:
                                edition = "HPC Edition";
                                break;
                            case PRODUCT_CLUSTER_SERVER_V:
                                edition = "HPC Edition without Hyper-V";
                                break;
                            case PRODUCT_DATACENTER_SERVER:
                                edition = "Datacenter Server";
                                break;
                            case PRODUCT_DATACENTER_SERVER_CORE:
                                edition = "Datacenter Server (core installation)";
                                break;
                            case PRODUCT_DATACENTER_SERVER_V:
                                edition = "Datacenter Server without Hyper-V";
                                break;
                            case PRODUCT_DATACENTER_SERVER_CORE_V:
                                edition = "Datacenter Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_EMBEDDED:
                                edition = "Embedded";
                                break;
                            case PRODUCT_ENTERPRISE:
                                edition = "Enterprise";
                                break;
                            case PRODUCT_ENTERPRISE_N:
                                edition = "Enterprise N";
                                break;
                            case PRODUCT_ENTERPRISE_E:
                                edition = "Enterprise E";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER:
                                edition = "Enterprise Server";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_CORE:
                                edition = "Enterprise Server (core installation)";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                edition = "Enterprise Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_IA64:
                                edition = "Enterprise Server for Itanium-based Systems";
                                break;
                            case PRODUCT_ENTERPRISE_SERVER_V:
                                edition = "Enterprise Server without Hyper-V";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                edition = "Essential Business Server MGMT";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                edition = "Essential Business Server ADDL";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                edition = "Essential Business Server MGMTSVC";
                                break;
                            case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                edition = "Essential Business Server ADDLSVC";
                                break;
                            case PRODUCT_HOME_BASIC:
                                edition = "Home Basic";
                                break;
                            case PRODUCT_HOME_BASIC_N:
                                edition = "Home Basic N";
                                break;
                            case PRODUCT_HOME_BASIC_E:
                                edition = "Home Basic E";
                                break;
                            case PRODUCT_HOME_PREMIUM:
                                edition = "Home Premium";
                                break;
                            case PRODUCT_HOME_PREMIUM_N:
                                edition = "Home Premium N";
                                break;
                            case PRODUCT_HOME_PREMIUM_E:
                                edition = "Home Premium E";
                                break;
                            case PRODUCT_HOME_PREMIUM_SERVER:
                                edition = "Home Premium Server";
                                break;
                            case PRODUCT_HYPERV:
                                edition = "Microsoft Hyper-V Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                edition = "Windows Essential Business Management Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                edition = "Windows Essential Business Messaging Server";
                                break;
                            case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                edition = "Windows Essential Business Security Server";
                                break;
                            case PRODUCT_PROFESSIONAL:
                                edition = "Professional";
                                break;
                            case PRODUCT_PROFESSIONAL_N:
                                edition = "Professional N";
                                break;
                            case PRODUCT_PROFESSIONAL_E:
                                edition = "Professional E";
                                break;
                            case PRODUCT_SB_SOLUTION_SERVER:
                                edition = "SB Solution Server";
                                break;
                            case PRODUCT_SB_SOLUTION_SERVER_EM:
                                edition = "SB Solution Server EM";
                                break;
                            case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                edition = "Server for SB Solutions";
                                break;
                            case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                edition = "Server for SB Solutions EM";
                                break;
                            case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                edition = "Windows Essential Server Solutions";
                                break;
                            case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                edition = "Windows Essential Server Solutions without Hyper-V";
                                break;
                            case PRODUCT_SERVER_FOUNDATION:
                                edition = "Server Foundation";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER:
                                edition = "Windows Small Business Server";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
                                edition = "Windows Small Business Server Premium";
                                break;
                            case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                edition = "Windows Small Business Server Premium (core installation)";
                                break;
                            case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                edition = "Solution Embedded Server";
                                break;
                            case PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE:
                                edition = "Solution Embedded Server (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER:
                                edition = "Standard Server";
                                break;
                            case PRODUCT_STANDARD_SERVER_CORE:
                                edition = "Standard Server (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                edition = "Standard Server Solutions";
                                break;
                            case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                edition = "Standard Server Solutions (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_CORE_V:
                                edition = "Standard Server without Hyper-V (core installation)";
                                break;
                            case PRODUCT_STANDARD_SERVER_V:
                                edition = "Standard Server without Hyper-V";
                                break;
                            case PRODUCT_STARTER:
                                edition = "Starter";
                                break;
                            case PRODUCT_STARTER_N:
                                edition = "Starter N";
                                break;
                            case PRODUCT_STARTER_E:
                                edition = "Starter E";
                                break;
                            case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                edition = "Enterprise Storage Server";
                                break;
                            case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                edition = "Enterprise Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_EXPRESS_SERVER:
                                edition = "Express Storage Server";
                                break;
                            case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                edition = "Express Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_STANDARD_SERVER:
                                edition = "Standard Storage Server";
                                break;
                            case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                edition = "Standard Storage Server (core installation)";
                                break;
                            case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                edition = "Workgroup Storage Server";
                                break;
                            case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                edition = "Workgroup Storage Server (core installation)";
                                break;
                            case PRODUCT_UNDEFINED:
                                edition = "Unknown product";
                                break;
                            case PRODUCT_ULTIMATE:
                                edition = "Ultimate";
                                break;
                            case PRODUCT_ULTIMATE_N:
                                edition = "Ultimate N";
                                break;
                            case PRODUCT_ULTIMATE_E:
                                edition = "Ultimate E";
                                break;
                            case PRODUCT_WEB_SERVER:
                                edition = "Web Server";
                                break;
                            case PRODUCT_WEB_SERVER_CORE:
                                edition = "Web Server (core installation)";
                                break;
                        }
                    }
                }

                #endregion VERSION 6
            }

            return edition;
        }
    }

    #endregion EDITION

    /// <summary>
    ///     Checks whether the UAC is turned off, which can lead to installation issues.
    /// </summary>
    public static bool IsUacDisabled
    {
        get
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");

            return !int.TryParse(key?.GetValue("ConsentPromptBehaviorAdmin")?.ToString(), out int value) ||
                   value != 5;
        }
    }

    #region NAME

    private static string? ReleaseId => (string?)Registry.GetValue(
        @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion",
        "ReleaseId", null);

    /// <summary>
    ///     Gets the name of the operating system running on this computer.
    /// </summary>
    public static string Name
    {
        get
        {
            string name = "Unknown";

            OperatingSystem osVersion = Environment.OSVersion;
            OSVERSIONINFOEX osVersionInfo = new() { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };

            if (!GetVersionEx(ref osVersionInfo))
            {
                return name;
            }

            int majorVersion = osVersion.Version.Major;
            int minorVersion = osVersion.Version.Minor;

            // TODO: deprecate this. Use a proper manifest. Always.
            if (majorVersion == 6 && minorVersion == 2)
            {
                //The registry read workaround is by Scott Vickery. Thanks a lot for the help!

                //http://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx

                // For applications that have been manifested for Windows 8.1 & Windows 10. Applications not manifested for 8.1 or 10 will return the Windows 8 OS version value (6.2). 
                // By reading the registry, we'll get the exact version - meaning we can even compare against  Win 8 and Win 8.1.
                string? exactVersion =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                        "CurrentVersion", null) as string;
                if (!string.IsNullOrEmpty(exactVersion))
                {
                    string[] splitResult = exactVersion!.Split('.');
                    majorVersion = Convert.ToInt32(splitResult[0]);
                    minorVersion = Convert.ToInt32(splitResult[1]);
                }

                if (IsWindows10)
                {
                    majorVersion = 10;
                    minorVersion = 0;
                }
            }

            switch (osVersion.Platform)
            {
                case PlatformID.Win32S:
                    name = "Windows 3.1";
                    break;
                case PlatformID.WinCE:
                    name = "Windows CE";
                    break;
                case PlatformID.Win32Windows:
                    {
                        if (majorVersion == 4)
                        {
                            string csdVersion = osVersionInfo.szCSDVersion;
                            switch (minorVersion)
                            {
                                case 0:
                                    if (csdVersion == "B" || csdVersion == "C")
                                    {
                                        name = "Windows 95 OSR2";
                                    }
                                    else
                                    {
                                        name = "Windows 95";
                                    }

                                    break;
                                case 10:
                                    if (csdVersion == "A")
                                    {
                                        name = "Windows 98 Second Edition";
                                    }
                                    else
                                    {
                                        name = "Windows 98";
                                    }

                                    break;
                                case 90:
                                    name = "Windows Me";
                                    break;
                            }
                        }

                        break;
                    }
                case PlatformID.Win32NT:
                    {
                        byte productType = osVersionInfo.wProductType;

                        switch (majorVersion)
                        {
                            case 3:
                                name = "Windows NT 3.51";
                                break;
                            case 4:
                                switch (productType)
                                {
                                    case 1:
                                        name = "Windows NT 4.0";
                                        break;
                                    case 3:
                                        name = "Windows NT 4.0 Server";
                                        break;
                                }

                                break;
                            case 5:
                                switch (minorVersion)
                                {
                                    case 0:
                                        name = "Windows 2000";
                                        break;
                                    case 1:
                                        name = "Windows XP";
                                        break;
                                    case 2:
                                        name = "Windows Server 2003";
                                        break;
                                }

                                break;
                            case 6:
                                name = ParseVistaThrough8(minorVersion, productType);

                                break;
                            case 10:
                                name = ParseWindows10Version(minorVersion, productType, ReleaseId);

                                break;
                        }

                        break;
                    }
            }

            return name;
        }
    }

    private static string ParseVistaThrough8(int minorVersion, byte productType)
    {
        switch (minorVersion)
        {
            case 0:
                switch (productType)
                {
                    case 1:
                        return "Windows Vista";
                    case 3:
                        return "Windows Server 2008";
                }

                break;

            case 1:
                switch (productType)
                {
                    case 1:
                        return "Windows 7";
                    case 3:
                        return "Windows Server 2008 R2";
                }

                break;
            case 2:
                switch (productType)
                {
                    case 1:
                        return "Windows 8";
                    case 3:
                        return "Windows Server 2012";
                }

                break;
            case 3:
                switch (productType)
                {
                    case 1:
                        return "Windows 8.1";
                    case 3:
                        return "Windows Server 2012 R2";
                }

                break;
        }

        return string.Empty;
    }

    private static string ParseWindows10Version(int minorVersion, byte productType, string releaseId)
    {
        switch (minorVersion)
        {
            case 0:
                switch (productType)
                {
                    case 1:
                        return $"Windows 10 ({ReleaseId})";
                    case 3:
                        switch (releaseId)
                        {
                            case "1607":
                                return "Windows Server 2016";
                            case "1809":
                                return "Windows Server 2019";
                        }

                        break;
                }

                break;
        }

        return string.Empty;
    }

    #endregion NAME

    #region VERSION

    #region BUILD

    /// <summary>
    ///     Gets the build version number of the operating system running on this computer.
    /// </summary>
    public static int? BuildVersion
    {
        get
        {
            string? value = (string?)Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentBuildNumber",
                null
            );

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return null;
        }
    }

    #endregion BUILD

    #region FULL

    #region STRING

    /// <summary>
    ///     Gets the full version string of the operating system running on this computer.
    /// </summary>
    public static string VersionString => Version.ToString();

    #endregion STRING

    #region VERSION

    /// <summary>
    ///     Gets the full version of the operating system running on this computer.
    /// </summary>
    public static Version Version => new(MajorVersion, MinorVersion, BuildVersion ?? 0, RevisionVersion);

    #endregion VERSION

    #endregion FULL

    #region MAJOR

    /// <summary>
    ///     Gets the major version number of the operating system running on this computer.
    /// </summary>
    public static int MajorVersion
    {
        get
        {
            if (IsWindows10)
            {
                return 10;
            }

            string? exactVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentVersion", null) as string;

            if (string.IsNullOrEmpty(exactVersion))
            {
                return Environment.OSVersion.Version.Major;
            }

            string[] splitVersion = exactVersion!.Split('.');
            return int.Parse(splitVersion[0]);
        }
    }

    #endregion MAJOR

    #region MINOR

    /// <summary>
    ///     Gets the minor version number of the operating system running on this computer.
    /// </summary>
    public static int MinorVersion
    {
        get
        {
            if (IsWindows10)
            {
                return 0;
            }

            string? exactVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentVersion", null) as string;

            if (string.IsNullOrEmpty(exactVersion))
            {
                return Environment.OSVersion.Version.Minor;
            }

            string[] splitVersion = exactVersion!.Split('.');
            return int.Parse(splitVersion[1]);
        }
    }

    #endregion MINOR

    #region REVISION

    /// <summary>
    ///     Gets the revision version number of the operating system running on this computer.
    /// </summary>
    public static int RevisionVersion => IsWindows10 ? 0 : Environment.OSVersion.Version.Revision;

    #endregion REVISION

    #endregion VERSION
}