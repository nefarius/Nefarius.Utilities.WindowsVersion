#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.Win32;

// http://www.codeproject.com/Articles/73000/Getting-Operating-System-Version-Info-Even-for-Win
//https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions

//Thanks to Member 7861383, Scott Vickery for the Windows 8.1 update and workaround.
//I have moved it to the beginning of the Name property, though...

//Thakts to Brisingr Aerowing for help with the Windows 10 adaptation
// Maintained and extended by Benjamin Höglinger-Stelzer 2018-2022

// Modified and extended by Benjamin "Nefarius" Höglinger-Stelzer 2022-2026

namespace Nefarius.Utilities.WindowsVersion.Util;

/// <summary>
///     Provides detailed information about the host operating system.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static partial class OsVersionInfo
{
    private const string NtCurrentVersionKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
    private const int Windows11MinBuild = 22000;
    private const int WindowsServer2016MinBuild = 14393;
    private const int WindowsServer2019MinBuild = 17763;
    private const int WindowsServer2022MinBuild = 20348;
    private const int WindowsServer2025MinBuild = 26100;

    private static object? GetNtCurrentVersionValue(string name)
    {
        return Registry.GetValue(NtCurrentVersionKey, name, null);
    }

    private static string? GetNtCurrentVersionString(string name)
    {
        return GetNtCurrentVersionValue(name) as string;
    }

    private static int? GetNtCurrentVersionDword(string name)
    {
        return GetNtCurrentVersionValue(name) switch
        {
            int value => value,
            _ => null
        };
    }

    /// <summary>
    ///     Feature-update label such as 22H2 or 24H2, when the OS reports one.
    /// </summary>
    public static string? DisplayVersion => GetNtCurrentVersionString("DisplayVersion");

    private static string? ReleaseId => GetNtCurrentVersionString("ReleaseId");

    private static string? EditionId => GetNtCurrentVersionString("EditionID");

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
    public static bool IsWindows10 =>
        GetNtCurrentVersionDword("CurrentMajorVersionNumber") >= 10 ||
        !string.IsNullOrEmpty(ReleaseId);

    /// <summary>
    ///     True if the current system is a Windows 11 client (build 22000 or later).
    /// </summary>
    public static bool IsWindows11 => !IsWindowsServer && (BuildVersion ?? 0) >= Windows11MinBuild;

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

            OSVERSIONINFOEX osVersionInfo = new() { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };

            if (!GetVersionEx(ref osVersionInfo))
            {
                return edition;
            }

            int majorVersion = MajorVersion;
            int minorVersion = MinorVersion;
            byte productType = osVersionInfo.wProductType;
            short suiteMask = osVersionInfo.wSuiteMask;

            #region VERSION 4

            switch (majorVersion)
            {
                case 4 when productType == VER_NT_WORKSTATION:
                    // Windows NT 4.0 Workstation
                    edition = "Workstation";
                    break;
                case 4:
                    {
                        if (productType == VER_NT_SERVER)
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

                        break;
                    }
                case 5 when productType == VER_NT_WORKSTATION:
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

                        break;
                    }
                case 5:
                    {
                        if (productType == VER_NT_SERVER)
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

                        break;
                    }
                case >= 6:
                    {
                        if (GetProductInfo(majorVersion, minorVersion,
                                osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                                out int ed))
                        {
                            edition = ed switch
                            {
                                PRODUCT_BUSINESS => "Business",
                                PRODUCT_BUSINESS_N => "Business N",
                                PRODUCT_CLUSTER_SERVER => "HPC Edition",
                                PRODUCT_CLUSTER_SERVER_V => "HPC Edition without Hyper-V",
                                PRODUCT_DATACENTER_SERVER => "Datacenter Server",
                                PRODUCT_DATACENTER_SERVER_CORE => "Datacenter Server (core installation)",
                                PRODUCT_DATACENTER_SERVER_V => "Datacenter Server without Hyper-V",
                                PRODUCT_DATACENTER_SERVER_CORE_V =>
                                    "Datacenter Server without Hyper-V (core installation)",
                                PRODUCT_EMBEDDED => "Embedded",
                                PRODUCT_ENTERPRISE => "Enterprise",
                                PRODUCT_ENTERPRISE_N => "Enterprise N",
                                PRODUCT_ENTERPRISE_E => "Enterprise E",
                                PRODUCT_ENTERPRISE_SERVER => "Enterprise Server",
                                PRODUCT_ENTERPRISE_SERVER_CORE => "Enterprise Server (core installation)",
                                PRODUCT_ENTERPRISE_SERVER_CORE_V =>
                                    "Enterprise Server without Hyper-V (core installation)",
                                PRODUCT_ENTERPRISE_SERVER_IA64 => "Enterprise Server for Itanium-based Systems",
                                PRODUCT_ENTERPRISE_SERVER_V => "Enterprise Server without Hyper-V",
                                PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT => "Essential Business Server MGMT",
                                PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL => "Essential Business Server ADDL",
                                PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC => "Essential Business Server MGMTSVC",
                                PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC => "Essential Business Server ADDLSVC",
                                PRODUCT_HOME_BASIC => "Home Basic",
                                PRODUCT_HOME_BASIC_N => "Home Basic N",
                                PRODUCT_HOME_BASIC_E => "Home Basic E",
                                PRODUCT_HOME_PREMIUM => "Home Premium",
                                PRODUCT_HOME_PREMIUM_N => "Home Premium N",
                                PRODUCT_HOME_PREMIUM_E => "Home Premium E",
                                PRODUCT_HOME_PREMIUM_SERVER => "Home Premium Server",
                                PRODUCT_HYPERV => "Microsoft Hyper-V Server",
                                PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT =>
                                    "Windows Essential Business Management Server",
                                PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING =>
                                    "Windows Essential Business Messaging Server",
                                PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY => "Windows Essential Business Security Server",
                                PRODUCT_PROFESSIONAL => "Professional",
                                PRODUCT_PROFESSIONAL_N => "Professional N",
                                PRODUCT_PROFESSIONAL_E => "Professional E",
                                PRODUCT_SB_SOLUTION_SERVER => "SB Solution Server",
                                PRODUCT_SB_SOLUTION_SERVER_EM => "SB Solution Server EM",
                                PRODUCT_SERVER_FOR_SB_SOLUTIONS => "Server for SB Solutions",
                                PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM => "Server for SB Solutions EM",
                                PRODUCT_SERVER_FOR_SMALLBUSINESS => "Windows Essential Server Solutions",
                                PRODUCT_SERVER_FOR_SMALLBUSINESS_V =>
                                    "Windows Essential Server Solutions without Hyper-V",
                                PRODUCT_SERVER_FOUNDATION => "Server Foundation",
                                PRODUCT_SMALLBUSINESS_SERVER => "Windows Small Business Server",
                                PRODUCT_SMALLBUSINESS_SERVER_PREMIUM => "Windows Small Business Server Premium",
                                PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE =>
                                    "Windows Small Business Server Premium (core installation)",
                                PRODUCT_SOLUTION_EMBEDDEDSERVER => "Solution Embedded Server",
                                PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE => "Solution Embedded Server (core installation)",
                                PRODUCT_STANDARD_SERVER => "Standard Server",
                                PRODUCT_STANDARD_SERVER_CORE => "Standard Server (core installation)",
                                PRODUCT_STANDARD_SERVER_SOLUTIONS => "Standard Server Solutions",
                                PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE =>
                                    "Standard Server Solutions (core installation)",
                                PRODUCT_STANDARD_SERVER_CORE_V => "Standard Server without Hyper-V (core installation)",
                                PRODUCT_STANDARD_SERVER_V => "Standard Server without Hyper-V",
                                PRODUCT_STARTER => "Starter",
                                PRODUCT_STARTER_N => "Starter N",
                                PRODUCT_STARTER_E => "Starter E",
                                PRODUCT_STORAGE_ENTERPRISE_SERVER => "Enterprise Storage Server",
                                PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE =>
                                    "Enterprise Storage Server (core installation)",
                                PRODUCT_STORAGE_EXPRESS_SERVER => "Express Storage Server",
                                PRODUCT_STORAGE_EXPRESS_SERVER_CORE => "Express Storage Server (core installation)",
                                PRODUCT_STORAGE_STANDARD_SERVER => "Standard Storage Server",
                                PRODUCT_STORAGE_STANDARD_SERVER_CORE => "Standard Storage Server (core installation)",
                                PRODUCT_STORAGE_WORKGROUP_SERVER => "Workgroup Storage Server",
                                PRODUCT_STORAGE_WORKGROUP_SERVER_CORE => "Workgroup Storage Server (core installation)",
                                PRODUCT_UNDEFINED => "Unknown product",
                                PRODUCT_ULTIMATE => "Ultimate",
                                PRODUCT_ULTIMATE_N => "Ultimate N",
                                PRODUCT_ULTIMATE_E => "Ultimate E",
                                PRODUCT_WEB_SERVER => "Web Server",
                                PRODUCT_WEB_SERVER_CORE => "Web Server (core installation)",
                                PRODUCT_CORE => "Home",
                                PRODUCT_CORE_N => "Home N",
                                PRODUCT_CORE_COUNTRYSPECIFIC => "Home China",
                                PRODUCT_CORE_SINGLELANGUAGE => "Home Single Language",
                                PRODUCT_EDUCATION => "Education",
                                PRODUCT_EDUCATION_N => "Education N",
                                PRODUCT_ENTERPRISE_S => "Enterprise LTSC",
                                PRODUCT_ENTERPRISE_S_N => "Enterprise N LTSC",
                                PRODUCT_PRO_WORKSTATION => "Pro for Workstations",
                                PRODUCT_PRO_WORKSTATION_N => "Pro for Workstations N",
                                PRODUCT_CLOUD => "S",
                                PRODUCT_CLOUDN => "S N",
                                PRODUCT_IOTENTERPRISE => "IoT Enterprise",
                                PRODUCT_IOTENTERPRISES => "IoT Enterprise LTSC",
                                _ => edition
                            };
                        }

                        if (string.IsNullOrEmpty(edition))
                        {
                            edition = EditionId ?? string.Empty;
                        }

                        break;
                    }
            }

            #endregion VERSION 6

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

            return int.TryParse(key?.GetValue("EnableLUA")?.ToString(), out int enableLua) &&
                   enableLua == 0;
        }
    }

    #region NAME

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

            int majorVersion = MajorVersion;
            int minorVersion = MinorVersion;

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
                                name = ParseWindows10Version(minorVersion, productType);

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

    private static string ParseWindows10Version(int minorVersion, byte productType)
    {
        if (minorVersion != 0)
        {
            return string.Empty;
        }

        string label = DisplayVersion ?? ReleaseId ?? BuildVersion?.ToString() ?? "0";
        int build = BuildVersion ?? 0;

        switch (productType)
        {
            case VER_NT_WORKSTATION:
                return build >= Windows11MinBuild
                    ? $"Windows 11 ({label})"
                    : $"Windows 10 ({label})";
            case VER_NT_DOMAIN_CONTROLLER:
            case VER_NT_SERVER:
                if (build >= WindowsServer2025MinBuild)
                {
                    return "Windows Server 2025";
                }

                if (build >= WindowsServer2022MinBuild)
                {
                    return "Windows Server 2022";
                }

                if (build >= WindowsServer2019MinBuild)
                {
                    return "Windows Server 2019";
                }

                if (build >= WindowsServer2016MinBuild)
                {
                    return "Windows Server 2016";
                }

                return $"Windows Server ({label})";
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
            string? value = GetNtCurrentVersionString("CurrentBuildNumber");

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
            int? major = GetNtCurrentVersionDword("CurrentMajorVersionNumber");
            if (major.HasValue)
            {
                return major.Value;
            }

            string? exactVersion = GetNtCurrentVersionString("CurrentVersion");

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
            int? minor = GetNtCurrentVersionDword("CurrentMinorVersionNumber");
            if (minor.HasValue)
            {
                return minor.Value;
            }

            string? exactVersion = GetNtCurrentVersionString("CurrentVersion");

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
    public static int RevisionVersion =>
        GetNtCurrentVersionDword("UBR") ??
        (IsWindows10 ? 0 : Environment.OSVersion.Version.Revision);

    #endregion REVISION

    #endregion VERSION
}