using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Geometry;
public readonly struct Point2 { public Point2(double x,double y){X=x;Y=y;} public double X{get;} public double Y{get;} }
public sealed class Polygon2
{
    public Polygon2(IEnumerable<Point2> points) { Points=points?.ToArray()??throw new ArgumentNullException(nameof(points)); if(Points.Count<3) throw new ArgumentException("A polygon needs three points."); }
    public IReadOnlyList<Point2> Points{get;}
    public bool Contains(Point2 p) { var inside=false; for(var i=0,j=Points.Count-1;i<Points.Count;j=i++) { var a=Points[i];var b=Points[j]; if(((a.Y>p.Y)!=(b.Y>p.Y))&&(p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X)) inside=!inside; } return inside; }
}
public static class FacadeClipping
{
    public static bool ContainsFootprint(Polygon2 outer,IEnumerable<Polygon2> holes,Rect2 panel)
    {
        if(!panel.IsValid) return false; var corners=new[]{new Point2(panel.X,panel.Y),new Point2(panel.X+panel.Width,panel.Y),new Point2(panel.X+panel.Width,panel.Y+panel.Height),new Point2(panel.X,panel.Y+panel.Height)};
        return corners.All(outer.Contains)&&!corners.Any(p=>(holes??Enumerable.Empty<Polygon2>()).Any(h=>h.Contains(p)));
    }
}
