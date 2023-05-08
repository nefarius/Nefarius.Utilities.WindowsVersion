using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.WindowsVersion.Util;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
public static partial class OsVersionInfo
{
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
        [FieldOffset(0)]
        internal readonly uint dwOemId;

        [FieldOffset(0)]
        internal readonly ushort wProcessorArchitecture;

        [FieldOffset(2)]
        internal readonly ushort wReserved;
    }

    #endregion _PROCESSOR_INFO_UNION

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
}