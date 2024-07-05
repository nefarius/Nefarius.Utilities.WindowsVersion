# OsVersionInfo

Namespace: Nefarius.Utilities.WindowsVersion.Util

Provides detailed information about the host operating system.

```csharp
public static class OsVersionInfo
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [OsVersionInfo](./nefarius.utilities.windowsversion.util.osversioninfo.md)

## Properties

### <a id="properties-buildversion"/>**BuildVersion**

Gets the build version number of the operating system running on this computer.

```csharp
public static Nullable<Int32> BuildVersion { get; }
```

#### Property Value

[Nullable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### <a id="properties-edition"/>**Edition**

Gets the edition of the operating system running on this computer.

```csharp
public static string Edition { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-isuacdisabled"/>**IsUacDisabled**

Checks whether the UAC is turned off, which can lead to installation issues.

```csharp
public static bool IsUacDisabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-iswindows10"/>**IsWindows10**

True if the current system is Windows 10 or newer, false otherwise.

```csharp
public static bool IsWindows10 { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

**Remarks:**

This also includes Windows 11 due to the stupidity and inconsistency of Microsoft's versioning strategy.

### <a id="properties-iswindowsserver"/>**IsWindowsServer**

True if Windows Server (any version) is detected, false otherwise.

```csharp
public static bool IsWindowsServer { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-majorversion"/>**MajorVersion**

Gets the major version number of the operating system running on this computer.

```csharp
public static int MajorVersion { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### <a id="properties-minorversion"/>**MinorVersion**

Gets the minor version number of the operating system running on this computer.

```csharp
public static int MinorVersion { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### <a id="properties-name"/>**Name**

Gets the name of the operating system running on this computer.

```csharp
public static string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-revisionversion"/>**RevisionVersion**

Gets the revision version number of the operating system running on this computer.

```csharp
public static int RevisionVersion { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### <a id="properties-servicepack"/>**ServicePack**

Gets the service pack information of the operating system running on this computer.

```csharp
public static string ServicePack { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-version"/>**Version**

Gets the full version of the operating system running on this computer.

```csharp
public static Version Version { get; }
```

#### Property Value

[Version](https://docs.microsoft.com/en-us/dotnet/api/system.version)<br>

### <a id="properties-versionstring"/>**VersionString**

Gets the full version string of the operating system running on this computer.

```csharp
public static string VersionString { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
