# CodeIntegrityPolicyHelper

Namespace: Nefarius.Utilities.WindowsVersion.Util

Utility class for interaction with Code Integrity Policy settings.

```csharp
public static class CodeIntegrityPolicyHelper
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CodeIntegrityPolicyHelper](./nefarius.utilities.windowsversion.util.codeintegritypolicyhelper.md)

**Remarks:**

Source: https://www.geoffchappell.com/notes/security/whqlsettings/index.htm

## Properties

### <a id="properties-whqldevelopertestmode"/>**WhqlDeveloperTestMode**

Gets or sets whether the kernel allows loading of "traditional" cross-signed drivers.

```csharp
public static bool WhqlDeveloperTestMode { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

**Remarks:**

Availability of this functionality heavily depends on the Windows build used.
