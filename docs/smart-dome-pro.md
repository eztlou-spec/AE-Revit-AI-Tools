# AE Smart Dome Generator PRO — development release

Ten conceptual topology options, including static Iris Concept, plus an independent rigid-petal motion demonstrator. The Geodesic option uses a projected octahedral hemisphere; Kiewitt uses projected triangular sectors. Islamic and Freeform are stylized profiles, not a general surface-fitting solver. Opening is a static oculus ratio except PetalMotion, which rotates rigid petals 0–90 degrees about base chords. Petal members may overlap, especially in the closed pose. The SpaceFrame uses an inward normal offset and webs. All dimensions are metres except section in mm.

## Install and smoke test
Close Revit. On Windows with the matching licensed Autodesk Revit API and SDK run:
`powershell -ExecutionPolicy Bypass -File scripts/install-smart-dome.ps1 -RevitVersion 2026`
The script gates installation on tests and successful build, copies the add-in to LocalAppData and registers its manifest. Existing AE add-in for that version is replaced by the manifest. Revit 2023–2027 are build targets, not verified compatibility claims.

Open a writable project, set an active work plane, launch AE REVIT AI TOOLS > Advanced Structures > Smart Dome. Choose a type, check the preview, generate and pick centre. Select the generated DirectShape and run again to change parameters. Do not manually move or rotate it before regeneration: the saved original centre is reused. Inspect undo/redo and save/reopen. Test all types, invalid input, and cancel paths.

## Limits
No working Hoberman linkage, coupled mechanism solver, collision checking, cladding, material selection, native structural framing, connection design or load analysis. Generic Model DirectShape contains square solid members; quantity is centreline length, not a certified bill of materials. No live Autodesk host test or successful build has been observed in this development session. Do not use for structural sizing or fabrication.
