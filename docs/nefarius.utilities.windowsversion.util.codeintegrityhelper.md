# CodeIntegrityHelper

Namespace: Nefarius.Utilities.WindowsVersion.Util

Utility class for receiving code integrity states currently enforced.

```csharp
public static class CodeIntegrityHelper
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [CodeIntegrityHelper](./nefarius.utilities.windowsversion.util.codeintegrityhelper.md)

**Remarks:**

Source: https://www.geoffchappell.com/studies/windows/km/ntoskrnl/api/ex/sysinfo/codeintegrity.htm

## Properties

### <a id="properties-istestsignenabled"/>**IsTestSignEnabled**

Determines if the system is currently in TESTSIGNING mode.

```csharp
public static bool IsTestSignEnabled { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>
