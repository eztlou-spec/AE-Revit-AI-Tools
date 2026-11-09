using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class FacadeRegenerateCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var ids=application.ActiveUIDocument.Selection.GetElementIds(); if(ids.Count!=1 || document.GetElement(ids.First()) is not DirectShape shape){message="Select one plugin-generated facade DirectShape.";return Result.Cancelled;}
        var owned=FacadeDiscovery.Find(document).FirstOrDefault(x=>x.Shape.Id==shape.Id); if(owned is null){message="Selected shape is not an AE-owned facade.";return Result.Cancelled;}
        if(document.GetElement(owned.Metadata.HostUniqueId) is null){message="Original host is unavailable; select a valid host face and run Facade Generator.";return Result.Cancelled;}
        message="Regeneration target validated. Full SetShape regeneration will reuse this configuration in the next update."; TaskDialog.Show("AE Revit AI Tools",message); return Result.Succeeded;
    }
}
