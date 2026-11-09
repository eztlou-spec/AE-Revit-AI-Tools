param(
  [ValidateSet('2023','2024','2025','2026','2027')][string]$RevitVersion,
  [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
  [string]$RevitApiPath
)

$ErrorActionPreference = 'Stop'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'The .NET SDK is required. Install the .NET 8 SDK (and .NET Framework 4.8 developer pack for Revit 2023/2024).' }
if ([string]::IsNullOrWhiteSpace($RevitApiPath)) { throw 'Pass -RevitApiPath to the folder containing RevitAPI.dll and RevitAPIUI.dll.' }

$framework = if ([int]$RevitVersion -le 2024) { 'net48' } else { 'net8.0-windows' }
dotnet build ..\AE-Revit-AI-Tools.sln --configuration $Configuration --framework $framework "/p:RevitApiPath=$RevitApiPath"
