# AE Revit AI Tools

An original, modular BIM automation add-in for Autodesk Revit, created for Ahmed Eztlou. The project is intentionally designed as version-specific builds: Revit 2023–2024 use .NET Framework 4.8, while Revit 2025–2027 use .NET 8 for Windows.

## Current implementation status

Milestone 1 has started. The repository includes a solution, a Revit-independent command/validation/transaction foundation, geometry primitives with tests, Revit adapter boundaries, an `.addin` manifest template, and build/manifest scripts. Ribbon, dashboard, and Revit API implementations are not implemented yet.

## Build on a Revit development workstation

Install the .NET 8 SDK, the .NET Framework 4.8 developer pack (for Revit 2023/2024), and the matching Revit SDK/API assemblies. Then run:

```powershell
.\scripts\build.ps1 -RevitVersion 2025 -RevitApiPath 'C:\Program Files\Autodesk\Revit 2025'
```

The script never copies Autodesk DLLs into an output installer. Create an add-in manifest after a successful build:

```powershell
.\scripts\install-manifest.ps1 -RevitVersion 2025 -AssemblyPath '<path-to-AE.Revit.Entry.dll>'
```

Read [compatibility notes](docs/compatibility.md) before claiming support for a Revit release.
