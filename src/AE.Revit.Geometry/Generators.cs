using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Geometry;

public readonly struct Member3 { public Member3(Point3 a, Point3 b) { A=a; B=b; } public Point3 A { get; } public Point3 B { get; } }
public static class StructuralGenerators
{
    public static IReadOnlyList<Member3> SpaceFrame(double span, double depth, int bays)
    {
        if (span <= 0 || depth <= 0 || bays < 1) throw new ArgumentException("Span, depth and bays must be positive.");
        var members = new List<Member3>(); var step = span / bays;
        for (var i=0;i<bays;i++) { var a=new Point3(i*step,0,0); var b=new Point3((i+1)*step,0,0); var ta=new Point3(i*step,0,depth); var tb=new Point3((i+1)*step,0,depth); members.Add(new Member3(a,b)); members.Add(new Member3(ta,tb)); members.Add(new Member3(a,tb)); members.Add(new Member3(b,ta)); }
        return members;
    }
    public static IReadOnlyList<Member3> Dome(double radius, int segments, int rings)
    {
        if (radius <= 0 || segments < 3 || rings < 1) throw new ArgumentException("Invalid dome parameters.");
        var result = new List<Member3>();
        for(var r=0;r<rings;r++) for(var s=0;s<segments;s++) { var p=Sphere(radius,r,rings,s,segments); result.Add(new Member3(p,Sphere(radius,r,rings,(s+1)%segments,segments))); result.Add(new Member3(p,Sphere(radius,r+1,rings,s,segments))); }
        return result;
    }
    private static Point3 Sphere(double radius,int ring,int rings,int segment,int segments) { var phi=Math.PI*ring/(2*rings); var theta=2*Math.PI*segment/segments; return new Point3(radius*Math.Cos(phi)*Math.Cos(theta),radius*Math.Cos(phi)*Math.Sin(theta),radius*Math.Sin(phi)); }
}

public sealed class MassEstimate { public MassEstimate(double footprint, int floors, double floorHeight) { if(footprint<=0||floors<1||floorHeight<=0) throw new ArgumentException("Invalid mass values."); Footprint=footprint; Floors=floors; FloorHeight=floorHeight; } public double Footprint {get;} public int Floors {get;} public double FloorHeight {get;} public double GrossArea => Footprint*Floors; public double Volume => GrossArea*FloorHeight; }
public static class PatternGenerators
{
    public static IReadOnlyList<Rect2> RunningBond(Rect2 boundary,double width,double height,double joint)
    {
        if(!boundary.IsValid||width<=0||height<=0||joint<0) throw new ArgumentException("Invalid tile layout."); var result=new List<Rect2>(); var row=0;
        for(var y=boundary.Y;y+height<=boundary.Y+boundary.Height;y+=height+joint,row++) for(var x=boundary.X+(row%2==0?0:width/2);x+width<=boundary.X+boundary.Width;x+=width+joint) result.Add(new Rect2(x,y,width,height)); return result;
    }
    public static IReadOnlyList<Rect2> Louvers(Rect2 boundary,double blade,double spacing,bool vertical) => vertical ? PanelLayoutGenerator.Generate(new PanelLayoutRequest{Boundary=boundary,PanelWidth=blade,PanelHeight=boundary.Height,HorizontalSpacing=spacing,Pattern=AE.Revit.Core.FacadePattern.VerticalLouvers}) : PanelLayoutGenerator.Generate(new PanelLayoutRequest{Boundary=boundary,PanelWidth=boundary.Width,PanelHeight=blade,VerticalSpacing=spacing,Pattern=AE.Revit.Core.FacadePattern.HorizontalLouvers});
}
