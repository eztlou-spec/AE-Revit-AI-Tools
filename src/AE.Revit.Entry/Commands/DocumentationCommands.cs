using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry.Commands;
public sealed class SmartDimensionCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        if (document.ActiveView.ViewType is not (ViewType.FloorPlan or ViewType.EngineeringPlan)) { message = "Smart Dimension requires a plan view."; return Result.Cancelled; }
        var walls = application.ActiveUIDocument.Selection.GetElementIds().Select(document.GetElement).OfType<Wall>().ToArray();
        if (walls.Length != 1 || walls[0].Location is not LocationCurve location) { message = "Select exactly one straight wall."; return Result.Cancelled; }
        var curve = location.Curve;
        if (!curve.IsBound) { message = "The selected wall must have bounded geometry."; return Result.Cancelled; }
        var first = curve.GetEndPointReference(0); var second = curve.GetEndPointReference(1);
        if (first is null || second is null) { message = "Could not obtain dimension references for the wall."; return Result.Cancelled; }
        if (!Confirm("Create an endpoint dimension for the selected wall?")) return Result.Cancelled;
        var references = new ReferenceArray(); references.Append(first); references.Append(second);
        using var transaction = new Transaction(document, "AE Smart Dimension"); transaction.Start();
        document.Create.NewDimension(document.ActiveView, Line.CreateBound(curve.GetEndPoint(0) + XYZ.BasisY, curve.GetEndPoint(1) + XYZ.BasisY), references);
        transaction.Commit(); message = "Created one wall dimension."; return Result.Succeeded;
    }
}

public sealed class AutoSheetsCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        var titleBlock = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType().OfType<FamilySymbol>().FirstOrDefault();
        if (titleBlock is null) { message = "Load a title block family before creating sheets."; return Result.Cancelled; }
        var existing = new HashSet<string>(new FilteredElementCollector(document).OfClass(typeof(ViewSheet)).Cast<ViewSheet>().Select(sheet => sheet.SheetNumber), StringComparer.OrdinalIgnoreCase);
        var next = Enumerable.Range(1, 999).Select(n => $"AE-{n:000}").First(number => !existing.Contains(number));
        if (!Confirm($"Create sheet {next} using title block '{titleBlock.Name}'?")) return Result.Cancelled;
        using var transaction = new Transaction(document, "AE Auto Sheet"); transaction.Start(); var sheet = ViewSheet.Create(document, titleBlock.Id); sheet.SheetNumber = next; sheet.Name = "AE Generated Sheet"; transaction.Commit(); message = $"Created sheet {next}."; return Result.Succeeded;
    }
}

public sealed class AutoTagCommand : CommandBase
{
    protected override Result Execute(UIApplication application, Document document, ref string message)
    {
        if (document.ActiveView is View3D) { message = "Auto Tag requires a 2D view."; return Result.Cancelled; }
        var elements = application.ActiveUIDocument.Selection.GetElementIds().Select(document.GetElement).Where(element => element?.Location is LocationPoint).ToArray();
        if (elements.Length == 0) { message = "Select taggable point-based elements in the active view."; return Result.Cancelled; }
        if (!Confirm($"Create category tags for {elements.Length} selected elements?")) return Result.Cancelled;
        var created = 0; using var transaction = new Transaction(document, "AE Auto Tag"); transaction.Start();
        foreach (var element in elements) { var point = ((LocationPoint)element.Location).Point; try { IndependentTag.Create(document, document.ActiveView.Id, new Reference(element), false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, point); created++; } catch (Autodesk.Revit.Exceptions.ArgumentException) { } }
        transaction.Commit(); message = $"Created {created} tags."; return Result.Succeeded;
    }
}
