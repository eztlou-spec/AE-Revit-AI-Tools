param([ValidateSet('2023','2024','2025','2026','2027')][string]$RevitVersion='2026',[string]$RevitApiPath)
$ErrorActionPreference='Stop'
if ([string]::IsNullOrWhiteSpace($RevitApiPath)) { $RevitApiPath=Join-Path $env:ProgramFiles "Autodesk\Revit $RevitVersion" }
$root=Split-Path $PSScriptRoot -Parent
dotnet test (Join-Path $root 'tests\UnitTests\UnitTests.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw "Unit tests failed ($LASTEXITCODE)." }
& (Join-Path $PSScriptRoot 'build.ps1') -RevitVersion $RevitVersion -RevitApiPath $RevitApiPath
$source=Join-Path $root "artifacts\Revit$RevitVersion"
$destination=Join-Path $env:LOCALAPPDATA "AERevitAI\Revit$RevitVersion"
New-Item -ItemType Directory -Force -Path $destination | Out-Null
Get-ChildItem -LiteralPath $source -File | Copy-Item -Destination $destination -Force
& (Join-Path $PSScriptRoot 'install-manifest.ps1') -RevitVersion $RevitVersion -AssemblyPath (Join-Path $destination 'AE.Revit.Entry.dll')
Write-Host 'Installation completed. Restart Revit and open AE REVIT AI TOOLS > Advanced Structures > Smart Dome.'
