# Testing

This repository mixes hermetic unit tests, Windows CI integration tests against the live OS, and an Explicit admin suite that mutates BCD or Code Integrity policy.

## Quick start (safe default)

On Windows:

```powershell
dotnet test Tests/Tests.csproj -c Release --filter "Category=Unit|Category=CI"
```

A bare `dotnet test` also skips `[Explicit]` tests, so Admin / Destructive cases will not run unless selected.

## Categories

| Category | Purpose | Runs on GitHub-hosted CI |
|----------|---------|--------------------------|
| `Unit` | Version/edition mapping and other helpers that do not need a live OS | Yes (`windows-latest`) |
| `CI` | Live Windows, read-only queries (version, architecture, UEFI, CI state) | Yes (`windows-latest`) |
| `Admin` | Needs an elevated process | No — Explicit |
| `Destructive` | Toggles BCD test-signing or WHQL developer test mode | No — Explicit |

GitHub `windows-latest` is a Windows Server image. CI tests assert the Server naming path there and stay consistent on a local client OS.

## Admin / Destructive suite

Run elevated when exercising Admin / Destructive tests:

```powershell
dotnet test Tests/Tests.csproj -c Release --filter "Category=Admin|Category=Destructive"
```

**Warning:** Destructive tests change BCD test-signing (`AllowPrereleaseSignatures`) and `WhqlDeveloperTestMode`. They restore the previous value afterwards, but still require a machine you are willing to reboot or recover if something goes wrong.

## CI layout

- **Ubuntu `build` job** — multi-TFM compile gate.
- **Windows `test-windows` job** — `Category=Unit|Category=CI` with TRX + coverage artifacts.
- **NuGet publish** — the same Windows test filter must pass before pack/push.
