using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;

[Transaction(TransactionMode.Manual)]
public abstract class CommandBase : IExternalCommand
{
    public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
    {
        try
        {
            if (data.Application.ActiveUIDocument?.Document is not Document document) { message = "Open a project document first."; return Result.Cancelled; }
            return Execute(data.Application, document, ref message);
        }
        catch (Exception exception) { Log.Error(GetType().Name + " failed", exception); message = exception.Message; return Result.Failed; }
    }
    protected abstract Result Execute(UIApplication application, Document document, ref string message);
    protected static bool Confirm(string text) => TaskDialog.Show("AE Revit AI Tools", text, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No) == TaskDialogResult.Yes;
}
