using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

// http://www.codeproject.com/Articles/73000/Getting-Operating-System-Version-Info-Even-for-Win
//https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions

//Thanks to Member 7861383, Scott Vickery for the Windows 8.1 update and workaround.
//I have moved it to the beginning of the Name property, though...

//Thakts to Brisingr Aerowing for help with the Windows 10 adapatation
// Maintained and extended by Benjamin Höglinger-Stelzer 2018-2022

namespace Nefarius.Utilities.WindowsVersion.Util
{
    /// <summary>
    ///     Provides detailed information about the host operating system.
    /// </summary>
    public static class OsVersionInfo
    {
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Windows_10_version_history#Channels
        /// </summary>
        private static readonly List<string> Windows10ReleaseIds = new List<string>
        {
            "1507",
            "1607",
            "1703",
            "1709",
            "1803",
            "1809",
            "1903",
            "1909",
            "2004",
            "2009"
        };

        #region SERVICE PACK

        /// <summary>
        ///     Gets the service pack information of the operating system running on this computer.
        /// </summary>
        public static string ServicePack
        {
            get
            {
                var servicePack = string.Empty;
                var osVersionInfo = new OSVERSIONINFOEX {dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX))};


                if (GetVersionEx(ref osVersionInfo)) servicePack = osVersionInfo.szCSDVersion;

                return servicePack;
            }
        }

        #endregion SERVICE PACK

        #region Windows 10/Server 2016+ Detection

        public static bool IsWindows10
        {
            get
            {
                var releaseId = (string) Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion",
                    "ReleaseId", null);

                return !string.IsNullOrEmpty(releaseId) && Windows10ReleaseIds.Any(id => id.Contains(releaseId));
            }
        }

        #endregion

        #region DELEGATE DECLARATION

        private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

        #endregion DELEGATE DECLARATION

        #region ENUMS

        public enum SoftwareArchitecture
        {
            Unknown = 0,
            Bit32 = 1,
            Bit64 = 2
        }

        public enum ProcessorArchitecture
        {
            Unknown = 0,
            Bit32 = 1,
            Bit64 = 2,
            Itanium64 = 3
        }

        #endregion ENUMS

        #region BITS

        /// <summary>
        ///     Determines if the current application is 32 or 64-bit.
        /// </summary>
        public static SoftwareArchitecture ProgramBits
        {
            get
            {
                var pbits = SoftwareArchitecture.Unknown;

                switch (IntPtr.Size * 8)
                {
                    case 64:
                        pbits = SoftwareArchitecture.Bit64;
                        break;

                    case 32:
                        pbits = SoftwareArchitecture.Bit32;
                        break;

                    default:
                        pbits = SoftwareArchitecture.Unknown;
                        break;
                }

                return pbits;
            }
        }

        public static SoftwareArchitecture OsBits
        {
            get
            {
                var osbits = SoftwareArchitecture.Unknown;

                switch (IntPtr.Size * 8)
                {
                    case 64:
                        osbits = SoftwareArchitecture.Bit64;
                        break;

                    case 32:
                        if (Is32BitProcessOn64BitProcessor())
                            osbits = SoftwareArchitecture.Bit64;
                        else
                            osbits = SoftwareArchitecture.Bit32;
                        break;

                    default:
                        osbits = SoftwareArchitecture.Unknown;
                        break;
                }

                return osbits;
            }
        }

        /// <summary>
        ///     Determines if the current processor is 32 or 64-bit.
        /// </summary>
        public static ProcessorArchitecture ProcessorBits
        {
            get
            {
                var pbits = ProcessorArchitecture.Unknown;

                try
                {
                    var systemInfo = new SYSTEM_INFO();
                    GetNativeSystemInfo(ref systemInfo);

                    switch (systemInfo.uProcessorInfo.wProcessorArchitecture)
                    {
                        case 9: // PROCESSOR_ARCHITECTURE_AMD64
                            pbits = ProcessorArchitecture.Bit64;
                            break;
                        case 6: // PROCESSOR_ARCHITECTURE_IA64
                            pbits = ProcessorArchitecture.Itanium64;
                            break;
                        case 0: // PROCESSOR_ARCHITECTURE_INTEL
                            pbits = ProcessorArchitecture.Bit32;
                            break;
                        default: // PROCESSOR_ARCHITECTURE_UNKNOWN
                            pbits = ProcessorArchitecture.Unknown;
                            break;
                    }
                }
                catch
                {
                    // Ignore        
                }

                return pbits;
            }
        }

        #endregion BITS

        #region EDITION

        private static string _edition;

        /// <summary>
        ///     Gets the edition of the operating system running on this computer.
        /// </summary>
        public static string Edition
        {
            get
            {
                if (_edition != null)
                    return _edition; //***** RETURN *****//

                var edition = string.Empty;

                var osVersion = Environment.OSVersion;
                var osVersionInfo = new OSVERSIONINFOEX {dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX))};

                if (GetVersionEx(ref osVersionInfo))
                {
                    var majorVersion = osVersion.Version.Major;
                    var minorVersion = osVersion.Version.Minor;
                    var productType = osVersionInfo.wProductType;
                    var suiteMask = osVersionInfo.wSuiteMask;

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
                                edition = "Enterprise Server";
                            else
                                edition = "Standard Server";
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
                                    edition = "Professional";
                                else
                                    edition = "Tablet Edition";
                            }
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if (minorVersion == 0)
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                    edition = "Datacenter Server";
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                    edition = "Advanced Server";
                                else
                                    edition = "Server";
                            }
                            else
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                    edition = "Datacenter";
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                    edition = "Enterprise";
                                else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                    edition = "Web Edition";
                                else
                                    edition = "Standard";
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

                    #endregion VERSION 6
                }

                _edition = edition;
                return edition;
            }
        }

        #endregion EDITION

        #region NAME

        private static string _name;

        private static string ReleaseId => (string) Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion",
            "ReleaseId", null);

        /// <summary>
        ///     Gets the name of the operating system running on this computer.
        /// </summary>
        public static string Name
        {
            get
            {
                if (_name != null)
                    return _name; //***** RETURN *****//

                var name = "unknown";

                var osVersion = Environment.OSVersion;
                var osVersionInfo = new OSVERSIONINFOEX {dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX))};

                if (GetVersionEx(ref osVersionInfo))
                {
                    var majorVersion = osVersion.Version.Major;
                    var minorVersion = osVersion.Version.Minor;

                    // TODO: deprecate this. Use a proper manifest. Always.
                    if (majorVersion == 6 && minorVersion == 2)
                    {
                        //The registry read workaround is by Scott Vickery. Thanks a lot for the help!

                        //http://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx

                        // For applications that have been manifested for Windows 8.1 & Windows 10. Applications not manifested for 8.1 or 10 will return the Windows 8 OS version value (6.2). 
                        // By reading the registry, we'll get the exact version - meaning we can even compare against  Win 8 and Win 8.1.
                        var exactVersion =
                            Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                                "CurrentVersion", null) as string;
                        if (!string.IsNullOrEmpty(exactVersion))
                        {
                            var splitResult = exactVersion.Split('.');
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
                                var csdVersion = osVersionInfo.szCSDVersion;
                                switch (minorVersion)
                                {
                                    case 0:
                                        if (csdVersion == "B" || csdVersion == "C")
                                            name = "Windows 95 OSR2";
                                        else
                                            name = "Windows 95";
                                        break;
                                    case 10:
                                        if (csdVersion == "A")
                                            name = "Windows 98 Second Edition";
                                        else
                                            name = "Windows 98";
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
                            var productType = osVersionInfo.wProductType;

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
                }

                _name = name;
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

        #region PINVOKE

        #region GET

        #region PRODUCT INFO

        [DllImport("Kernel32.dll")]
        private static extern bool GetProductInfo(
            int osMajorVersion,
            int osMinorVersion,
            int spMajorVersion,
            int spMinorVersion,
            out int edition);

        #endregion PRODUCT INFO

        #region VERSION

        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

        #endregion VERSION

        #region SYSTEMMETRICS

        [DllImport("user32")]
        private static extern int GetSystemMetrics(int nIndex);

        #endregion SYSTEMMETRICS

        #region SYSTEMINFO

        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        #endregion SYSTEMINFO

        #endregion GET

        #region OSVERSIONINFOEX

        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public readonly int dwMajorVersion;
            public readonly int dwMinorVersion;
            public readonly int dwBuildNumber;
            public readonly int dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public readonly string szCSDVersion;

            public readonly short wServicePackMajor;
            public readonly short wServicePackMinor;
            public readonly short wSuiteMask;
            public readonly byte wProductType;
            public readonly byte wReserved;
        }

        #endregion OSVERSIONINFOEX

        #region SYSTEM_INFO

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            internal readonly _PROCESSOR_INFO_UNION uProcessorInfo;
            public readonly uint dwPageSize;
            public readonly IntPtr lpMinimumApplicationAddress;
            public readonly IntPtr lpMaximumApplicationAddress;
            public readonly IntPtr dwActiveProcessorMask;
            public readonly uint dwNumberOfProcessors;
            public readonly uint dwProcessorType;
            public readonly uint dwAllocationGranularity;
            public readonly ushort dwProcessorLevel;
            public readonly ushort dwProcessorRevision;
        }

        #endregion SYSTEM_INFO

        #region _PROCESSOR_INFO_UNION

        [StructLayout(LayoutKind.Explicit)]
        private struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)] internal readonly uint dwOemId;
            [FieldOffset(0)] internal readonly ushort wProcessorArchitecture;
            [FieldOffset(2)] internal readonly ushort wReserved;
        }

        #endregion _PROCESSOR_INFO_UNION

        #region 64 BIT OS DETECTION

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

        #endregion 64 BIT OS DETECTION

        #region PRODUCT

        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;
        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;
        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;
        private const int PRODUCT_STARTER_N = 0x0000002F;
        private const int PRODUCT_PROFESSIONAL = 0x00000030;
        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;
        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;
        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;

        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE = 0x00000039;

        //private const int ???? = 0x0000003A;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;
        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;
        private const int PRODUCT_EMBEDDED = 0x00000041;
        private const int PRODUCT_STARTER_E = 0x00000042;
        private const int PRODUCT_HOME_BASIC_E = 0x00000043;
        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;
        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;
        private const int PRODUCT_ENTERPRISE_E = 0x00000046;

        private const int PRODUCT_ULTIMATE_E = 0x00000047;
        //private const int PRODUCT_UNLICENSED = 0xABCDABCD;

        #endregion PRODUCT

        #region VERSIONS

        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;

        #endregion VERSIONS

        #endregion PINVOKE

        #region VERSION

        #region BUILD

        /// <summary>
        ///     Gets the build version number of the operating system running on this computer.
        /// </summary>
        public static int BuildVersion =>
            int.Parse((string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                "CurrentBuildNumber", "0"));

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
        public static Version Version => new Version(MajorVersion, MinorVersion, BuildVersion, RevisionVersion);

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
                if (IsWindows10) return 10;
                var exactVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "CurrentVersion", null) as string;

                if (string.IsNullOrEmpty(exactVersion)) return Environment.OSVersion.Version.Major;

                var splitVersion = exactVersion.Split('.');
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
                if (IsWindows10) return 0;
                var exactVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "CurrentVersion", null) as string;

                if (string.IsNullOrEmpty(exactVersion)) return Environment.OSVersion.Version.Minor;

                var splitVersion = exactVersion.Split('.');
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

        #region 64 BIT OS DETECTION

        private static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
        {
            var handle = LoadLibrary("kernel32");

            if (handle == IntPtr.Zero) return null;

            var fnPtr = GetProcAddress(handle, "IsWow64Process");

            if (fnPtr != IntPtr.Zero)
                return (IsWow64ProcessDelegate) Marshal.GetDelegateForFunctionPointer(fnPtr,
                    typeof(IsWow64ProcessDelegate));

            return null;
        }

        private static bool Is32BitProcessOn64BitProcessor()
        {
            var fnDelegate = GetIsWow64ProcessDelegate();

            if (fnDelegate == null) return false;

            var retVal = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out var isWow64);

            return retVal && isWow64;
        }

        #endregion 64 BIT OS DETECTION
    }
}