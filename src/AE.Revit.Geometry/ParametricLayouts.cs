using System;
using System.Collections.Generic;
using AE.Revit.Core;

namespace AE.Revit.Geometry;

public readonly struct Rect2
{
    public Rect2(double x, double y, double width, double height) { X = x; Y = y; Width = width; Height = height; }
    public double X { get; } public double Y { get; } public double Width { get; } public double Height { get; }
    public bool IsValid => Width > 0 && Height > 0;
}

public sealed class PanelLayoutRequest
{
    public Rect2 Boundary { get; set; }
    public double PanelWidth { get; set; }
    public double PanelHeight { get; set; }
    public double HorizontalSpacing { get; set; }
    public double VerticalSpacing { get; set; }
    public double BoundaryOffset { get; set; }
    public FacadePattern Pattern { get; set; }
    public int Seed { get; set; } = 1;
}

public static class PanelLayoutGenerator
{
    public static IReadOnlyList<Rect2> Generate(PanelLayoutRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (!request.Boundary.IsValid || request.PanelWidth <= 0 || request.PanelHeight <= 0 || request.HorizontalSpacing < 0 || request.VerticalSpacing < 0 || request.BoundaryOffset < 0) throw new ArgumentException("Invalid panel layout parameters.");
        var result = new List<Rect2>();
        var left = request.Boundary.X + request.BoundaryOffset; var bottom = request.Boundary.Y + request.BoundaryOffset;
        var right = request.Boundary.X + request.Boundary.Width - request.BoundaryOffset; var top = request.Boundary.Y + request.Boundary.Height - request.BoundaryOffset;
        var row = 0;
        for (var y = bottom; y + request.PanelHeight <= top + 1e-9; y += request.PanelHeight + request.VerticalSpacing, row++)
        {
            var stagger = request.Pattern == FacadePattern.Diamond && row % 2 == 1 ? (request.PanelWidth + request.HorizontalSpacing) / 2 : 0;
            for (var x = left + stagger; x + request.PanelWidth <= right + 1e-9; x += request.PanelWidth + request.HorizontalSpacing)
                result.Add(new Rect2(x, y, request.PanelWidth, request.PanelHeight));
        }
        return result;
    }
}
