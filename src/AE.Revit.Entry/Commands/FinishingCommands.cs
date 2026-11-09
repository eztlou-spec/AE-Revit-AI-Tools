using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class FloorFinishCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var room = application.ActiveUIDocument.Selection.GetElementIds().Select(document.GetElement).OfType<Room>().SingleOrDefault();
        if (room is null || room.Location is null || room.Area <= 0) { message = "Select exactly one placed, enclosed room."; return Result.Cancelled; }
        var floorType = new FilteredElementCollector(document).OfClass(typeof(FloorType)).Cast<FloorType>().FirstOrDefault();
        if (floorType is null || room.Level is null) { message = "A floor type and room level are required."; return Result.Cancelled; }
        var segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
        if (segments is null || segments.Count == 0) { message = "The room has no closed boundary."; return Result.Cancelled; }
        var loops = new List<CurveLoop>();
        foreach (var boundary in segments) { var loop = new CurveLoop(); foreach (var segment in boundary) loop.Append(segment.GetCurve()); loops.Add(loop); }
        if (!Confirm($"Create a floor finish for room {room.Number}?")) return Result.Cancelled;
        using var group = new TransactionGroup(document, "AE Room Floor Finish"); group.Start();
        using (var transaction = new Transaction(document, "Create room floor finish")) { transaction.Start(); Floor.Create(document, loops, floorType.Id, room.Level.Id); transaction.Commit(); }
        group.Assimilate(); message = "Created a room-based floor finish."; return Result.Succeeded;
    }
}
