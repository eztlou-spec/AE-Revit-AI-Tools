# Build recovery report

## Build partitions

| Partition | Projects | CI behavior |
|---|---|---|
| Revit-independent | `AE.Revit.Core`, `AE.Revit.Geometry`, `AE.Revit.API`, `UnitTests` | restored, built and unit-tested on `windows-latest` with .NET 8 |
| Revit host | `AE.Revit.Entry` | requires licensed local Autodesk `RevitAPI.dll` and `RevitAPIUI.dll`; deliberately excluded from public runner setup |

The entry assembly targets `net48` for Revit 2023/2024 and `net8.0-windows` for Revit 2025–2027. This is a source-level compatibility configuration, not a verification claim. Build it with the matching installed Revit API folder:

```powershell
.\scripts\build.ps1 -RevitVersion 2027 -RevitApiPath 'C:\Program Files\Autodesk\Revit 2027'
```

The build guard rejects an entry build without those licensed assemblies. CI never downloads Autodesk assemblies from unofficial sources.
