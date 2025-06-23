// See https://aka.ms/new-console-template for more information

using Nefarius.Utilities.WindowsVersion.Util;

var t = CodeIntegrityHelper.IsTestSignEnabled;

Console.WriteLine(OperatingSystem.IsWindowsVersionAtLeast(10));