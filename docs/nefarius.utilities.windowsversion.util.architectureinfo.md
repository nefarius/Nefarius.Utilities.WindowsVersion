# ArchitectureInfo

Namespace: Nefarius.Utilities.WindowsVersion.Util

Process and operating system architecture detection.

```csharp
public static class ArchitectureInfo
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ArchitectureInfo](./nefarius.utilities.windowsversion.util.architectureinfo.md)

**Remarks:**

Source: https://stackoverflow.com/a/54539366/490629

## Properties

### <a id="properties-isarm64"/>**IsArm64**

Gets whether the current process is running on ARM64.

```csharp
public static bool IsArm64 { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-osbits"/>**OsBits**

Determines if the current operating system is 32 or 64-bit.

```csharp
public static SoftwareArchitecture OsBits { get; }
```

#### Property Value

[SoftwareArchitecture](./nefarius.utilities.windowsversion.util.softwarearchitecture.md)<br>

### <a id="properties-processorbits"/>**ProcessorBits**

Determines if the current processor is 32 or 64-bit.

```csharp
public static ProcessorArchitecture ProcessorBits { get; }
```

#### Property Value

[ProcessorArchitecture](./nefarius.utilities.windowsversion.util.processorarchitecture.md)<br>

### <a id="properties-programbits"/>**ProgramBits**

Determines if the current application is 32 or 64-bit.

```csharp
public static SoftwareArchitecture ProgramBits { get; }
```

#### Property Value

[SoftwareArchitecture](./nefarius.utilities.windowsversion.util.softwarearchitecture.md)<br>
