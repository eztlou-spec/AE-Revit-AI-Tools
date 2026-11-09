using System;
using System.Collections.Generic;

namespace AE.Revit.Core;

public enum FacadePattern { Rectangular, VerticalLouvers, HorizontalLouvers, Perforated, Diamond, Triangular }
public sealed class FacadeParameters
{
    public double PanelWidthMillimetres { get; set; } = 1200;
    public double PanelHeightMillimetres { get; set; } = 1200;
    public double HorizontalSpacingMillimetres { get; set; } = 150;
    public double VerticalSpacingMillimetres { get; set; } = 150;
    public double DepthMillimetres { get; set; } = 50;
    public double BoundaryOffsetMillimetres { get; set; }
    public FacadePattern Pattern { get; set; } = FacadePattern.Rectangular;
    public string? MaterialName { get; set; }
    public IReadOnlyList<ValidationIssue> Validate()
    {
        var issues = new List<ValidationIssue>();
        foreach (var pair in new[] { ("Panel width", PanelWidthMillimetres), ("Panel height", PanelHeightMillimetres), ("Panel depth", DepthMillimetres) }) if (pair.Item2 <= 0 || double.IsNaN(pair.Item2) || double.IsInfinity(pair.Item2)) issues.Add(new ValidationIssue("facade.parameter", $"{pair.Item1} must be positive."));
        if (HorizontalSpacingMillimetres < 0 || VerticalSpacingMillimetres < 0 || BoundaryOffsetMillimetres < 0) issues.Add(new ValidationIssue("facade.spacing", "Spacing and boundary offset cannot be negative."));
        return issues;
    }
}
