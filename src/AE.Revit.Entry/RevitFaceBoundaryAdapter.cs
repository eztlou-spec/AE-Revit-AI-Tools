using System;
using System.Collections.Generic;
using System.Linq;
using AE.Revit.Geometry;
using Autodesk.Revit.DB;

namespace AE.Revit.Entry;

/// <summary>Extracts tessellated face loops in the face's UV coordinate system. Loop classification remains consumer-owned.</summary>
internal static class RevitFaceBoundaryAdapter
{
    private const double Tolerance = 1e-8;
    internal static IReadOnlyList<Polygon2> Extract(PlanarFace face)
    {
        if (face is null) throw new ArgumentNullException(nameof(face));
        var loops = new List<Polygon2>();
        foreach (EdgeArray edgeLoop in face.EdgeLoops)
        {
            var points = new List<Point2>();
            foreach (Edge edge in edgeLoop)
                foreach (XYZ point in edge.Tessellate())
                {
                    var uv = face.Project(point).UV;
                    if (points.Count == 0 || Math.Abs(points[^1].X - uv.U) > Tolerance || Math.Abs(points[^1].Y - uv.V) > Tolerance) points.Add(new Point2(uv.U, uv.V));
                }
            if (points.Count > 1 && Math.Abs(points[0].X - points[^1].X) <= Tolerance && Math.Abs(points[0].Y - points[^1].Y) <= Tolerance) points.RemoveAt(points.Count - 1);
            if (points.Count < 3) throw new InvalidOperationException("Face contains a degenerate edge loop.");
            loops.Add(new Polygon2(points));
        }
        if (loops.Count == 0) throw new InvalidOperationException("Face has no usable edge loops.");
        return loops;
    }
}
