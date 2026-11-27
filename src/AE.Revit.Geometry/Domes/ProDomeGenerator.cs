using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Geometry.Domes
{
    public enum ProDomeType { Ribbed, Schwedler, Lamella, Spiral, Geodesic, SpaceFrame, Kiewitt, Islamic, Freeform, IrisConcept }
    public sealed class ProDomeSettings
    {
        public ProDomeType Type { get; set; }
        public double Diameter { get; set; } = 20;
        public double Height { get; set; } = 8;
        public int Segments { get; set; } = 24;
        public int Rings { get; set; } = 6;
        public double Opening { get; set; }
        public double LayerDepth { get; set; } = 0.5;
        public double Aspect { get; set; } = 1.25;
    }
    public static class ProDomeGenerator
    {
        public static DomeModel Generate(ProDomeSettings s)
        {
            if(s==null) throw new ArgumentNullException(nameof(s));
            if(!Enum.IsDefined(typeof(ProDomeType),s.Type) || !Valid(s.Diameter,0.1,1000) ||
               !Valid(s.Height,0.1,1000) || s.Segments<6 || s.Segments>96 ||
               s.Rings<2 || s.Rings>24 || !Valid(s.Opening,0,0.9) ||
               !Valid(s.LayerDepth,0.01,100) || !Valid(s.Aspect,0.25,4))
                throw new ArgumentOutOfRangeException(nameof(s),"Invalid dome dimensions or settings.");
            if(s.Type==ProDomeType.Geodesic || s.Type==ProDomeType.Kiewitt)
            {
                if(s.Opening!=0) throw new ArgumentException("This topology requires a closed crown.");
                return Triangular(s,s.Type==ProDomeType.Geodesic?4:Math.Max(6,s.Segments/4));
            }
            var pattern=s.Type==ProDomeType.Schwedler||s.Type==ProDomeType.SpaceFrame?DomePattern.Schwedler:
                s.Type==ProDomeType.Lamella?DomePattern.Lamella:
                s.Type==ProDomeType.Spiral||s.Type==ProDomeType.IrisConcept?DomePattern.Spiral:DomePattern.Ribbed;
            var basic=DomeGenerator.Generate(new DomeSettings { DiameterMetres=s.Diameter, HeightMetres=s.Height,
                Segments=s.Segments,Rings=s.Rings,OculusRatio=s.Opening,Pattern=pattern });
            var nodes=basic.Nodes.ToList(); var members=basic.Members.ToList();
            if(s.Type==ProDomeType.Islamic || s.Type==ProDomeType.Freeform)
            {
                for(int i=0;i<nodes.Count;i++)
                {
                    var p=nodes[i]; double t=p.Z/s.Height;
                    double factor=s.Type==ProDomeType.Islamic?1+0.35*Math.Sin(Math.PI*t):1;
                    nodes[i]=new DomeNode(p.X*factor,p.Y*factor*(s.Type==ProDomeType.Freeform?s.Aspect:1),
                        p.Z*(s.Type==ProDomeType.Islamic?1:1+0.18*p.X/(s.Diameter*0.5)));
                }
            }
            if(s.Type==ProDomeType.SpaceFrame)
            {
                if(s.LayerDepth>=Math.Min(s.Height,s.Diameter/2)*0.5) throw new ArgumentException("Layer depth is too large.");
                int n=nodes.Count;
                for(int i=0;i<n;i++)
                {
                    var p=nodes[i];
                    var x=p.X/Math.Pow(s.Diameter/2,2); var y=p.Y/Math.Pow(s.Diameter/2,2); var z=p.Z/(s.Height*s.Height);
                    double d=Math.Sqrt(x*x+y*y+z*z);
                    nodes.Add(new DomeNode(p.X-s.LayerDepth*x/d,p.Y-s.LayerDepth*y/d,p.Z-s.LayerDepth*z/d));
                }
                var original=members.ToArray();
                foreach(var m in original) { members.Add(new DomeMember(m.A+n,m.B+n,"Inner chord")); members.Add(new DomeMember(m.A,m.B+n,"Web")); }
                for(int i=0;i<n;i++) members.Add(new DomeMember(i,i+n,"Web"));
            }
            return new DomeModel(nodes,members);
        }
        private static bool Valid(double v,double min,double max) => !double.IsNaN(v)&&!double.IsInfinity(v)&&v>=min&&v<=max;
        // Radially projected triangular sectors; 4 sectors gives an octahedron-based geodesic cap.
        private static DomeModel Triangular(ProDomeSettings s,int sectors)
        {
            int f=s.Rings;
            var nodes=new List<DomeNode>(); var members=new List<DomeMember>();
            var ids=new Dictionary<string,int>(); var edges=new HashSet<long>();
            int Node(double x,double y,double z)
            {
                double len=Math.Sqrt(x*x+y*y+z*z); x/=len;y/=len;z/=len;
                string key=string.Format(System.Globalization.CultureInfo.InvariantCulture,"{0}:{1}:{2}",(long)Math.Round(x*1e9),(long)Math.Round(y*1e9),(long)Math.Round(z*1e9));
                if(ids.TryGetValue(key,out int id)) return id;
                id=nodes.Count; ids.Add(key,id);
                nodes.Add(new DomeNode(x*s.Diameter/2,y*s.Diameter/2,z*s.Height));return id;
            }
            void Edge(int a,int b)
            {
                if(a==b)return;int lo=Math.Min(a,b),hi=Math.Max(a,b);
                if(edges.Add(((long)lo<<32)|(uint)hi))members.Add(new DomeMember(lo,hi,"Triangular chord"));
            }
            for(int k=0;k<sectors;k++)
            {
                double a=2*Math.PI*k/sectors,b=2*Math.PI*(k+1)/sectors;
                var grid=new Dictionary<int,int>();
                for(int i=0;i<=f;i++) for(int j=0;j<=f-i;j++)
                    grid.Add(i*(f+1)+j,Node((i*Math.Cos(a)+j*Math.Cos(b))/f,(i*Math.Sin(a)+j*Math.Sin(b))/f,(double)(f-i-j)/f));
                for(int i=0;i<=f;i++) for(int j=0;j<=f-i;j++)
                {
                    int p=grid[i*(f+1)+j];
                    if(i+j<f){Edge(p,grid[(i+1)*(f+1)+j]);Edge(p,grid[i*(f+1)+j+1]);}
                    if(j>0)Edge(p,grid[(i+1)*(f+1)+j-1]);
                }
            }
            return new DomeModel(nodes,members);
        }
    }
}
