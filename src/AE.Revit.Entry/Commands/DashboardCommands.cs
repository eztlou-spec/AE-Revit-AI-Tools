using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class DashboardCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var window = new Window { Title = "AE Revit AI Tools", Width = 420, Height = 230, WindowStartupLocation = WindowStartupLocation.CenterScreen, Content = new System.Windows.Controls.TextBlock { Margin = new Thickness(24), TextWrapping = TextWrapping.Wrap, Text = "AE Revit AI Tools Dashboard\n\nUse the Ribbon commands to run Room Audit, Smart Dimension, Auto Sheets, Auto Tag, Floor Finish, and Rectangular Facade. Revit operations run through ExternalEvent on Revit's API thread." } };
        window.Show(); message = "Dashboard opened."; return Result.Succeeded;
    }
}
public sealed class SettingsCommand : CommandBase { protected override Result Execute(UIApplication application, Document document, ref string message) { TaskDialog.Show("AE Revit AI Tools", "Settings persistence will be available after localization and user-profile storage are implemented."); message = "Settings information displayed."; return Result.Succeeded; } }
