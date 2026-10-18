# Build, test and package Flow.Launcher.Plugin.QuickCode.
# Usage:  pwsh -File build.ps1   (or powershell -File build.ps1)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repo   = $PSScriptRoot
$src    = Join-Path $repo "src/Flow.Launcher.Plugin.QuickCode/Flow.Launcher.Plugin.QuickCode.csproj"
$tests  = Join-Path $repo "tests/QuickCode.Tests/QuickCode.Tests.csproj"
$config = "Release"
$tfm    = "net9.0-windows10.0.19041.0"
$dist   = Join-Path $repo "dist"
$zip    = Join-Path $dist "Flow.Launcher.Plugin.QuickCode-v1.0.0.zip"
$stage  = Join-Path $dist "Flow.Launcher.Plugin.QuickCode-v1.0.0"

Write-Host "== restore =="
dotnet restore $src
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }

dotnet restore $tests
if ($LASTEXITCODE -ne 0) { throw "dotnet restore (tests) failed" }

Write-Host "== build =="
dotnet build $src -c $config --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

Write-Host "== test =="
dotnet test $tests -c $config --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet test failed" }

Write-Host "== package =="
if (Test-Path $stage) { Remove-Item -Recurse -Force $stage }
New-Item -ItemType Directory -Force -Path $stage | Out-Null

$bin = Join-Path $repo "src/Flow.Launcher.Plugin.QuickCode/bin/$config/$tfm"
Copy-Item (Join-Path $bin "Flow.Launcher.Plugin.QuickCode.dll") $stage
Copy-Item (Join-Path $repo "plugin.json") $stage
Copy-Item (Join-Path $repo "Images") $stage -Recurse

if (Test-Path $zip) { Remove-Item -Force $zip }
Compress-Archive -Path (Join-Path $stage "*") -DestinationPath $zip

Write-Host "== done =="
Get-Item $zip | Select-Object Name, Length | Format-List
