using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class DashboardCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var window = new UI.DashboardWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
        window.Show(); message = "Dashboard opened."; return Result.Succeeded;
    }
}
public sealed class SettingsCommand : CommandBase { protected override Result Execute(UIApplication application, Document document, ref string message) { TaskDialog.Show("AE Revit AI Tools", "Settings persistence will be available after localization and user-profile storage are implemented."); message = "Settings information displayed."; return Result.Succeeded; } }
