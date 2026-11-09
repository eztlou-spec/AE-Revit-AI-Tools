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
        Reference reference; try { reference=application.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face,"Select the stored host face."); } catch(Autodesk.Revit.Exceptions.OperationCanceledException){return Result.Cancelled;}
        if(document.GetElement(reference)?.Id!=document.GetElement(owned.Metadata.HostUniqueId).Id || document.GetElement(reference)?.GetGeometryObjectFromReference(reference) is not PlanarFace face){message="Select a planar face belonging to the stored host.";return Result.Cancelled;}
        var dialog=new UI.FacadeParameterDialog(document); if(dialog.ShowDialog()!=true||dialog.Parameters is null)return Result.Cancelled;
        var bounds=face.GetBoundingBox(); var loops=RevitFaceBoundaryAdapter.Extract(face); var outer=loops.OrderByDescending(x=>x.Points.Count).First(); var holes=loops.Where(x=>!ReferenceEquals(x,outer)).ToArray(); var solids=FacadeCommand.CreatePanels(face,bounds,UnitUtils.ConvertToInternalUnits(dialog.Parameters.PanelWidthMillimetres,UnitTypeId.Millimeters),UnitUtils.ConvertToInternalUnits(dialog.Parameters.PanelHeightMillimetres,UnitTypeId.Millimeters),UnitUtils.ConvertToInternalUnits(dialog.Parameters.HorizontalSpacingMillimetres,UnitTypeId.Millimeters),UnitUtils.ConvertToInternalUnits(dialog.Parameters.VerticalSpacingMillimetres,UnitTypeId.Millimeters),UnitUtils.ConvertToInternalUnits(dialog.Parameters.DepthMillimetres,UnitTypeId.Millimeters),outer,holes,dialog.SelectedMaterialId.HasValue?new ElementId(dialog.SelectedMaterialId.Value):ElementId.InvalidElementId); if(solids.Count==0){message="No valid replacement panels fit the selected face.";return Result.Cancelled;} using var transaction=new Transaction(document,"Regenerate AE Facade");transaction.Start();shape.SetShape(solids);FacadeStorage.Write(shape,owned.Metadata);transaction.Commit();message="Facade regenerated.";return Result.Succeeded;
    }
}
