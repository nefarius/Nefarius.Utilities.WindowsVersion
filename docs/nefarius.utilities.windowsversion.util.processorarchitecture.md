# ProcessorArchitecture

Namespace: Nefarius.Utilities.WindowsVersion.Util

Processor architecture.

```csharp
public enum ProcessorArchitecture
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → [ProcessorArchitecture](./nefarius.utilities.windowsversion.util.processorarchitecture.md)<br>
Implements [IComparable](https://learn.microsoft.com/dotnet/api/system.icomparable), [ISpanFormattable](https://learn.microsoft.com/dotnet/api/system.ispanformattable), [IFormattable](https://learn.microsoft.com/dotnet/api/system.iformattable), [IConvertible](https://learn.microsoft.com/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Unknown | 0 | Unknown. |
| Bit32 | 1 | 32-Bits (a.k.a x86). |
| Bit64 | 2 | 64-Bits (a.k.a. x86_64). |
| Itanium64 | 3 | IA-64 (Intel Itanium architecture). |
| Arm64 | 4 | ARM64 (AArch64). |
