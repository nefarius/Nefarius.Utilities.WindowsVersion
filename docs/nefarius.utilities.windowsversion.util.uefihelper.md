# UefiHelper

Namespace: Nefarius.Utilities.WindowsVersion.Util

Utility to get UEFI details.

```csharp
public class UefiHelper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UefiHelper](./nefarius.utilities.windowsversion.util.uefihelper.md)

## Properties

### **IsSecureBootEnabled**

Checks if Secure Boot is enabled.

```csharp
public static bool IsSecureBootEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsRunningInUefiMode**

Checks if the current system is running in UEFI or Legacy BIOS mode.

```csharp
public static bool IsRunningInUefiMode { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

**Remarks:**

https://theroadtodelphi.com/2013/02/19/how-distinguish-when-windows-was-installed-in-legacy-bios-or-uefi-mode-using-delphi/

## Constructors

### **UefiHelper()**

```csharp
public UefiHelper()
```
