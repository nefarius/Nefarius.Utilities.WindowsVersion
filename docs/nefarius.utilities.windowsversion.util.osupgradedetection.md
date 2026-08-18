# OsUpgradeDetection

Namespace: Nefarius.Utilities.WindowsVersion.Util

Utilities to detect if this system is a fresh or upgraded installation.

```csharp
public static class OsUpgradeDetection
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [OsUpgradeDetection](./nefarius.utilities.windowsversion.util.osupgradedetection.md)

**Remarks:**

Source: https://superuser.com/a/1184670

## Properties

### <a id="properties-isgrandfathered"/>**IsGrandfathered**

Gets whether the system has been in-place upgraded.

```csharp
public static bool IsGrandfathered { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)<br>

## Methods

### <a id="methods-hassourceosupgrade"/>**HasSourceOsUpgrade(IEnumerable&lt;String&gt;, Func&lt;String, String&gt;)**

True when a `Source OS*` setup subkey reports a non-empty ProductName.

```csharp
internal static bool HasSourceOsUpgrade(IEnumerable<String> subKeyNames, Func<String, String> getProductName)
```

#### Parameters

`subKeyNames` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1)<[String](https://learn.microsoft.com/dotnet/api/system.string)><br>

`getProductName` [Func](https://learn.microsoft.com/dotnet/api/system.func-2)<[String](https://learn.microsoft.com/dotnet/api/system.string), [String](https://learn.microsoft.com/dotnet/api/system.string)><br>

#### Returns

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)
