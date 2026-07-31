# Device APK

Sideloadable Android package for testing on a real phone.

| File | Notes |
|------|--------|
| `DustsSpaceLaunchTracker-Debug-v1.0.apk` | Debug build, assemblies embedded (~84 MB) |

## Install

```powershell
adb install -r .\apk\DustsSpaceLaunchTracker-Debug-v1.0.apk
```

Or copy the APK to the device and open it (allow installs from unknown sources if prompted).

## Rebuild

```powershell
.\scripts\build-device-apk.ps1
```

## GitHub

This folder is part of the repo so you can download the APK from GitHub after push
(e.g. browse `apk/` in the repository on github.com).
