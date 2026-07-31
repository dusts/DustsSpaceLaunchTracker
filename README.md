# DustsSpaceLaunchTracker

.NET MAUI app that tracks **upcoming** and **previous** space launches using
[The Space Devs Launch Library 2](https://thespacedevs.com/).

## Features

- Upcoming + previous tabs with infinite-scroll pagination
- Search and status filter
- Pull-to-refresh
- Launch detail (mission, pad, agency, webcast link)
- Live countdown + local/UTC NET times
- Offline-friendly file cache (stale-while-revalidate on network errors)
- Optional API token for higher rate limits
- Unit tests for JSON parsing, services, cache, and helpers

## Solution layout

```
DustsSpaceLaunchTracker/          # MAUI app (repo root)
tests/
  DustsSpaceLaunchTracker.Tests/  # xUnit unit tests
DustsSpaceLaunchTracker.slnx
```

| Path | Purpose |
|------|---------|
| `DustsSpaceLaunchTracker.csproj` | MAUI app (Android, iOS, Mac Catalyst, Windows) |
| `tests/DustsSpaceLaunchTracker.Tests/` | xUnit tests for shared logic (models, services, helpers) |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MAUI workloads for the platforms you target, e.g.:

```powershell
dotnet workload install maui
```

## Run

### Windows

```powershell
dotnet run --project DustsSpaceLaunchTracker.csproj -f net10.0-windows10.0.19041.0
```

### Android (emulator or device)

With Visual Studio / `dotnet` install targets (Fast Deployment works here):

```powershell
dotnet build DustsSpaceLaunchTracker.csproj -f net10.0-android -t:Run
```

For a standalone APK you can `adb install` (assemblies embedded):

```powershell
dotnet build DustsSpaceLaunchTracker.csproj -f net10.0-android -c Debug -p:EmbedAssembliesIntoApk=true
adb install -r bin\Debug\net10.0-android\com.companyname.dustsspacelaunchtracker-Signed.apk
```

> **Note:** A plain Debug APK without `EmbedAssembliesIntoApk=true` expects
> Fast Deployment and will crash if installed only with `adb install`.

## Tests

```powershell
dotnet test tests\DustsSpaceLaunchTracker.Tests\DustsSpaceLaunchTracker.Tests.csproj
```

Or from the solution:

```powershell
dotnet test DustsSpaceLaunchTracker.slnx
```

Coverage includes:

- JSON deserialization (including `orbital_launch_attempt_count: null` on previous launches)
- `LaunchService` success, cache fallback, and previous-list routing
- `FileLaunchCache` round-trip
- Auth header token injection
- Countdown / UTC formatting
- API routes and status filters

## Configuration

| Env var | Purpose |
|---------|---------|
| `DUSTS_LL_API_BASE` | Override API host (default: `https://lldev.thespacedevs.com/` in DEBUG, production host in Release) |
| `DUSTS_LL_API_TOKEN` | The Space Devs API token (`Authorization: Token …`) |

Other knobs (page size, list/detail modes, cache TTL) live in
`Configuration/AppConfig.cs`. Route paths are in `Configuration/ApiRoutes.cs`.

## Architecture (short)

```
Views / ViewModels
       │
       ▼
 ILaunchService  ──►  ITheSpaceDevsApi (Refit)
       │
       └──►  ILaunchCache (file + memory)
```

- List screens use API `mode=normal`
- Detail uses `mode=detailed`
- Refit paths are `/2.2.0/...` with host-only base URL (avoids HttpClient path drop + Refit `/` rules)

## License

Personal / educational project unless otherwise noted.
