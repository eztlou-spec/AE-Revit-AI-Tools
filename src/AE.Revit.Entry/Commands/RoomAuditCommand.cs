using System.Linq;
using AE.Revit.Core;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class RoomAuditCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var rooms = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Cast<SpatialElement>().Select(room => new RoomRecord(room.UniqueId, room.get_Parameter(BuiltInParameter.ROOM_NUMBER)?.AsString() ?? "", room.Name, room.Area, room.Location is not null, room.Area > 0));
        var issues = new RoomAudit().Inspect(rooms);
        message = issues.Count == 0 ? "No room audit issues found." : string.Join("\n", issues.Select(issue => $"[{issue.Severity}] {issue.Message}"));
        TaskDialog.Show("AE Room Audit", message);
        return Result.Succeeded;
    }
}
