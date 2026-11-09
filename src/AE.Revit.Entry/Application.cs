using System;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry;

public sealed class Application : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        try
        {
            try { application.CreateRibbonTab("AE REVIT AI TOOLS"); } catch { /* tab already exists */ }
            AddPanel(application, "Home", ("Dashboard", typeof(Commands.DashboardCommand)), ("Settings", typeof(Commands.SettingsCommand)));
            AddPanel(application, "Documentation", ("Room Audit", typeof(Commands.RoomAuditCommand)), ("Smart Dimension", typeof(Commands.SmartDimensionCommand)), ("Auto Sheets", typeof(Commands.AutoSheetsCommand)), ("Auto Tag", typeof(Commands.AutoTagCommand)));
            AddPanel(application, "Finishing", ("Floor Finish", typeof(Commands.FloorFinishCommand)));
            AddPanel(application, "Parametric", ("Rectangular Facade", typeof(Commands.FacadeCommand)));
            ExternalEventDispatcher.Initialize();
            return Result.Succeeded;
        }
        catch (Exception exception) { Log.Error("Application startup failed", exception); return Result.Failed; }
    }

    public Result OnShutdown(UIControlledApplication application) { ExternalEventDispatcher.Shutdown(); return Result.Succeeded; }

    private static void AddPanel(UIControlledApplication app, string panelName, params (string text, Type command)[] buttons)
    {
        var panel = app.CreateRibbonPanel("AE REVIT AI TOOLS", panelName);
        foreach (var button in buttons)
        {
            panel.AddItem(new PushButtonData(button.command.Name, button.text, typeof(Application).Assembly.Location, button.command.FullName));
        }
    }
}
