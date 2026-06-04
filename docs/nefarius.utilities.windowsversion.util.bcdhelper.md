# BcdHelper

Namespace: Nefarius.Utilities.WindowsVersion.Util

Utility to interact with the Boot Configuration Database.

```csharp
public static class BcdHelper
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [BcdHelper](./nefarius.utilities.windowsversion.util.bcdhelper.md)

**Remarks:**

Source: https://geoffchappell.com/notes/windows/boot/bcd/elements.htm

## Properties

### <a id="properties-allowprereleasesignatures"/>**AllowPrereleaseSignatures**

Gets or sets the current value of BCDE_LIBRARY_TYPE_ALLOW_PRERELEASE_SIGNATURES from the default boot entry.

```csharp
public static bool AllowPrereleaseSignatures { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>
