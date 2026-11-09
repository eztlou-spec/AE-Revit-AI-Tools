param(
  [ValidateSet('2023','2024','2025','2026','2027')][string]$RevitVersion,
  [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
  [string]$RevitApiPath
)

$ErrorActionPreference = 'Stop'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'The .NET SDK is required. Install the .NET 8 SDK (and .NET Framework 4.8 developer pack for Revit 2023/2024).' }
if ([string]::IsNullOrWhiteSpace($RevitApiPath)) { throw 'Pass -RevitApiPath to the folder containing RevitAPI.dll and RevitAPIUI.dll.' }
if (-not (Test-Path (Join-Path $RevitApiPath 'RevitAPI.dll')) -or -not (Test-Path (Join-Path $RevitApiPath 'RevitAPIUI.dll'))) { throw 'RevitApiPath must be an authorized Revit installation or SDK folder containing RevitAPI.dll and RevitAPIUI.dll.' }

$framework = if ([int]$RevitVersion -le 2024) { 'net48' } else { 'net8.0-windows' }
dotnet build (Join-Path $PSScriptRoot '..\src\AE.Revit.Entry\AE.Revit.Entry.csproj') --configuration $Configuration --framework $framework "/p:RevitApiPath=$RevitApiPath"
$output = Join-Path $PSScriptRoot "..\src\AE.Revit.Entry\bin\$Configuration\$framework"
if (-not (Test-Path (Join-Path $output 'AE.Revit.Entry.dll'))) { throw "Expected entry assembly was not produced: $output" }
$release = Join-Path $PSScriptRoot "..\artifacts\Revit$RevitVersion"
New-Item -ItemType Directory -Force -Path $release | Out-Null
Copy-Item -LiteralPath (Join-Path $output 'AE.Revit.Entry.dll') -Destination $release -Force
Copy-Item -LiteralPath (Join-Path $output 'AE.Revit.Entry.pdb') -Destination $release -Force -ErrorAction SilentlyContinue
Get-ChildItem -LiteralPath $output -Filter '*.dll' | Where-Object { $_.Name -notin @('RevitAPI.dll','RevitAPIUI.dll') } | Copy-Item -Destination $release -Force
Write-Host "Release assembled: $release"
