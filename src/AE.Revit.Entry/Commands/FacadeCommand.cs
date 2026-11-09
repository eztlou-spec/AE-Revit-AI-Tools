using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace AE.Revit.Entry.Commands;

/// <summary>Rectangular panel MVP. Dimensions are currently 4' x 4', 6" spacing, and 2" depth; a parameter dialog is the next step.</summary>
public sealed class FacadeCommand : CommandBase
{
    private const string ApplicationId = "com.ahmedeztlou.aerevitaitools.facade";
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        Reference reference;
        try { reference = application.ActiveUIDocument.Selection.PickObject(ObjectType.Face, "Select a planar facade host face."); }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
        var host = document.GetElement(reference);
        var face = host?.GetGeometryObjectFromReference(reference) as PlanarFace;
        if (host is null || face is null) { message = "Select a planar face."; return Result.Cancelled; }
        const double panelWidth = 4.0, panelHeight = 4.0, spacing = 0.5, thickness = 2.0 / 12.0;
        var bounds = face.GetBoundingBox();
        if (bounds.Max.U - bounds.Min.U < panelWidth || bounds.Max.V - bounds.Min.V < panelHeight) { message = "The host face is smaller than one panel."; return Result.Cancelled; }
        if (!Confirm("Regenerate facade panels on the selected host face? Existing AE panels for this host will be replaced.")) return Result.Cancelled;
        using var group = new TransactionGroup(document, "AE Rectangular Facade"); group.Start();
        using (var transaction = new Transaction(document, "Generate facade panels"))
        {
            transaction.Start();
            var previous = new FilteredElementCollector(document).OfClass(typeof(DirectShape)).Cast<DirectShape>().Where(shape => shape.ApplicationId == ApplicationId && shape.ApplicationDataId == host.UniqueId).Select(shape => shape.Id).ToList();
            if (previous.Count > 0) document.Delete(previous);
            var solids = CreatePanels(face, bounds, panelWidth, panelHeight, spacing, thickness);
            if (solids.Count == 0) { transaction.RollBack(); message = "No panels fit inside the host-face bounding box."; return Result.Cancelled; }
            var shape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
            shape.ApplicationId = ApplicationId; shape.ApplicationDataId = host.UniqueId; shape.SetShape(solids);
            transaction.Commit(); message = $"Generated {solids.Count} facade panels.";
        }
        group.Assimilate(); return Result.Succeeded;
    }

    private static List<GeometryObject> CreatePanels(PlanarFace face, BoundingBoxUV bounds, double width, double height, double spacing, double thickness)
    {
        var solids = new List<GeometryObject>();
        for (var u = bounds.Min.U; u + width <= bounds.Max.U + 1e-9; u += width + spacing)
        for (var v = bounds.Min.V; v + height <= bounds.Max.V + 1e-9; v += height + spacing)
        {
            var p0 = face.Evaluate(new UV(u, v)); var p1 = face.Evaluate(new UV(u + width, v)); var p2 = face.Evaluate(new UV(u + width, v + height)); var p3 = face.Evaluate(new UV(u, v + height));
            var loop = new CurveLoop(); loop.Append(Line.CreateBound(p0, p1)); loop.Append(Line.CreateBound(p1, p2)); loop.Append(Line.CreateBound(p2, p3)); loop.Append(Line.CreateBound(p3, p0));
            solids.Add(GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { loop }, face.FaceNormal, thickness));
        }
        return solids;
    }
}
