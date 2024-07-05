# <img src="assets/NSS-128x128.png" align="left" />Nefarius.Utilities.WindowsVersion

[![.NET](https://github.com/nefarius/Nefarius.Utilities.WindowsVersion/actions/workflows/build.yml/badge.svg)](https://github.com/nefarius/Nefarius.Utilities.WindowsVersion/actions/workflows/build.yml)
![Requirements](https://img.shields.io/badge/Requires-.NET%20Standard%202.0-blue.svg)
[![Nuget](https://img.shields.io/nuget/v/Nefarius.Utilities.WindowsVersion)](https://www.nuget.org/packages/Nefarius.Utilities.WindowsVersion/) [![Nuget](https://img.shields.io/nuget/dt/Nefarius.Utilities.WindowsVersion)](https://www.nuget.org/packages/Nefarius.Utilities.WindowsVersion/)

Utility classes to get detailed Windows version and some extras like UEFI and BCD properties.

## Features

- Get detailed information about most (all?) known Windows versions and editions out there
  - Service Pack (where it applies)
  - Edition like Home, Professional etc. (where it applies)
  - Server or client OS
  - Release and Build numbers
  - Whether the OS installation is fresh or grandfathered (in-place upgraded from an older version)
- Read and write Boot Configuration Data (BCD)
  - Currently only querying or setting test-signing is implemented
- UEFI information
  - Query if the current system is running in UEFI or legacy mode
  - Query whether SecureBoot is enabled or disabled
- Check Code Integrity (CI) settings
- ...and contributions welcome!

## Documentation

[Link to API docs](docs/index.md).

### Generating documentation

```PowerShell
dotnet build -c:Release
dotnet tool install --global Nefarius.Tools.XMLDoc2Markdown
xmldoc2md .\bin\netstandard2.0\Nefarius.Utilities.WindowsVersion.dll .\docs\
```

## Sources & 3rd party credits

- [XMLDoc2Markdown](https://charlesdevandiere.github.io/xmldoc2md/)
- [Getting Operating System Version Info - Even for Windows 10!](https://www.codeproject.com/Articles/73000/Getting-Operating-System-Version-Info-Even-for-Win)
- [List of Microsoft Windows versions](https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions)
- [Geoff Chappell - BCD Elements](https://geoffchappell.com/notes/windows/boot/bcd/elements.htm)
- [How distinguish when Windows was installed in Legacy BIOS or UEFI mode using Delphi?](https://theroadtodelphi.com/2013/02/19/how-distinguish-when-windows-was-installed-in-legacy-bios-or-uefi-mode-using-delphi/)
