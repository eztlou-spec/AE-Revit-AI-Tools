param(
  [Parameter(Mandatory)][ValidateSet('2023','2024','2025','2026','2027')][string]$RevitVersion,
  [Parameter(Mandatory)][string]$AssemblyPath
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path -LiteralPath $AssemblyPath)) { throw "Assembly not found: $AssemblyPath" }
$manifestDirectory = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
New-Item -ItemType Directory -Force -Path $manifestDirectory | Out-Null
$template = Get-Content -Raw (Join-Path $PSScriptRoot '..\src\AE.Revit.Entry\AERevitAiTools.addin.template')
$template.Replace('{{ASSEMBLY_PATH}}', [System.Security.SecurityElement]::Escape((Resolve-Path $AssemblyPath))) | Set-Content -Encoding UTF8 (Join-Path $manifestDirectory 'AE.Revit.AI.Tools.addin')
Write-Host "Installed manifest for Revit $RevitVersion in $manifestDirectory"
