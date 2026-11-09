param([Parameter(Mandatory)][ValidateSet('2023','2024','2025','2026','2027')][string]$RevitVersion)
$ErrorActionPreference = 'Stop'
$manifest = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion\AE.Revit.AI.Tools.addin"
if (Test-Path -LiteralPath $manifest) { Remove-Item -LiteralPath $manifest -Force; Write-Host "Removed $manifest" } else { Write-Host 'AE Revit AI Tools manifest was not installed.' }
