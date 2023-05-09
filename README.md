<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.WindowsVersion

[![Build status](https://ci.appveyor.com/api/projects/status/o82ftn53byhd757w?svg=true)](https://ci.appveyor.com/project/nefarius/nefarius-utilities-windowsversion) 
[![Nuget](https://img.shields.io/nuget/v/Nefarius.Utilities.WindowsVersion)](https://www.nuget.org/packages/Nefarius.Utilities.WindowsVersion/) [![Nuget](https://img.shields.io/nuget/dt/Nefarius.Utilities.WindowsVersion)](https://www.nuget.org/packages/Nefarius.Utilities.WindowsVersion/)

Utility classes to get detailed Windows version and some extras like UEFI and BCD properties.

## Features

- Get detailed information about most (all?) known Windows versions and editions out there
  - Service Pack (where it applies)
  - Edition like Home, Professional etc. (where it applies)
  - Server or client OS
  - Release and Build numbers
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

- `dotnet build -c:Release`
- `dotnet tool install -g XMLDoc2Markdown`
- `xmldoc2md .\bin\netstandard2.0\Nefarius.Utilities.WindowsVersion.dll .\docs\`

## Sources & 3rd party credits

- [XMLDoc2Markdown](https://charlesdevandiere.github.io/xmldoc2md/)
