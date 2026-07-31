<#
.SYNOPSIS
  Builds a sideloadable Android APK and copies it to apk/ for real-device testing / GitHub.
#>
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$outDir = Join-Path $root 'apk'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

Write-Host "Building Android APK ($Configuration) with embedded assemblies..." -ForegroundColor Cyan
dotnet build .\DustsSpaceLaunchTracker.csproj `
    -f net10.0-android `
    -c $Configuration `
    -p:EmbedAssembliesIntoApk=true `
    -p:AndroidPackageFormat=apk

if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed with exit code $LASTEXITCODE"
}

$searchRoot = Join-Path $root "bin\$Configuration\net10.0-android"
$apk = Get-ChildItem -Path $searchRoot -Recurse -Filter '*-Signed.apk' -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $apk) {
    $apk = Get-ChildItem -Path $searchRoot -Recurse -Filter '*.apk' -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -notlike '*.idsig' } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
}

if (-not $apk) {
    throw "No APK found under $searchRoot"
}

$version = '1.0'
try {
    $csproj = Get-Content (Join-Path $root 'DustsSpaceLaunchTracker.csproj') -Raw
    if ($csproj -match '<ApplicationDisplayVersion>([^<]+)</ApplicationDisplayVersion>') {
        $version = $Matches[1].Trim()
    }
} catch { }

$destName = "DustsSpaceLaunchTracker-$Configuration-v$version.apk"
$dest = Join-Path $outDir $destName
Copy-Item -Path $apk.FullName -Destination $dest -Force

Write-Host ""
Write-Host "APK ready:" -ForegroundColor Green
Write-Host "  $dest"
Write-Host "  Size: $([math]::Round((Get-Item $dest).Length / 1MB, 1)) MB"
Write-Host ""
Write-Host "Install with:" -ForegroundColor Cyan
Write-Host "  adb install -r `"$dest`""
