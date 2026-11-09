using System;
using AE.Revit.Core;

namespace AE.Revit.Geometry;

public readonly struct Point3
{
    public Point3(double x, double y, double z) => (X, Y, Z) = (x, y, z);
    public double X { get; } public double Y { get; } public double Z { get; }
    public double DistanceTo(Point3 other) => Math.Sqrt(Math.Pow(X-other.X, 2) + Math.Pow(Y-other.Y, 2) + Math.Pow(Z-other.Z, 2));
}

public readonly struct Segment3
{
    public Segment3(Point3 start, Point3 end)
    {
        if (start.DistanceTo(end) <= 1e-9) throw new ArgumentException("A segment requires two distinct points.");
        Start = start; End = end;
    }
    public Point3 Start { get; } public Point3 End { get; }
    public double Length => Start.DistanceTo(End);
}

public static class GeometryValidation
{
    public static ValidationIssue? ValidatePositive(string name, double value) => value > 0 && !double.IsNaN(value) && !double.IsInfinity(value) ? null : new ValidationIssue("geometry.positive", $"{name} must be a finite positive number.");
}
