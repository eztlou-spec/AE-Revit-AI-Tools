# Compatibility and verification

| Revit | Target framework | Adapter status | Verification status |
|---|---|---|---|
| 2023 | .NET Framework 4.8 | planned | not verified |
| 2024 | .NET Framework 4.8 | planned | not verified |
| 2025 | .NET 8 Windows | planned | not verified |
| 2026 | .NET 8 Windows | planned | not verified |
| 2027 | .NET 8 Windows | planned | not verified |

No Autodesk Revit SDK, Revit installation, or .NET SDK is available in the current development environment. Consequently, no Revit-dependent command, installer, or compiled artifact has been verified. The repository deliberately references Autodesk API assemblies through the `RevitApiPath` build property and never redistributes them.
