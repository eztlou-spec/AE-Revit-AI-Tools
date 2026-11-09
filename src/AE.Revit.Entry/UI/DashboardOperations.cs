using Autodesk.Revit.UI;
namespace AE.Revit.Entry.UI;
internal static class DashboardOperations
{
    internal static void RunRoomAudit(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Room Audit is available from the Ribbon. Dashboard execution requires the active external-command context and is intentionally routed to that command to preserve Revit selection semantics.");
    internal static void OpenFacade(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Select Rectangular Facade from the Ribbon to enter Revit face-selection mode.");
    internal static void OpenFloorFinish(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Select Floor Finish from the Ribbon to use the active Revit selection.");
    internal static void OpenSheets(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Select Auto Sheets from the Ribbon to create a sheet.");
    internal static void OpenTags(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Select Auto Tag from the Ribbon to tag your selection.");
    internal static void OpenDimensions(UIApplication app) => TaskDialog.Show("AE Revit AI Tools", "Select Smart Dimension from the Ribbon to dimension the selected wall.");
}
