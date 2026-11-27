using System;
using System.Collections.Generic;

namespace AE.Revit.Geometry.Domes
{
    public enum DomePattern { Ribbed, Schwedler, Lamella, Spiral }
    public sealed class DomeSettings
    {
        public double DiameterMetres { get; set; } = 20;
        public double HeightMetres { get; set; } = 8;
        public int Segments { get; set; } = 24;
        public int Rings { get; set; } = 8;
        public double OculusRatio { get; set; } = 0;
        public double SpiralTwistDegrees { get; set; } = 45;
        public DomePattern Pattern { get; set; } = DomePattern.Ribbed;
    }
    public sealed class DomeNode
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public DomeNode(double x, double y, double z) { X=x; Y=y; Z=z; }
    }
    public sealed class DomeMember
    {
        public int A { get; }
        public int B { get; }
        public string Role { get; }
        public DomeMember(int a, int b, string role) { A=a; B=b; Role=role; }
    }
    public sealed class DomeModel
    {
        public IReadOnlyList<DomeNode> Nodes { get; }
        public IReadOnlyList<DomeMember> Members { get; }
        internal DomeModel(List<DomeNode> nodes, List<DomeMember> members)
        { Nodes=nodes.AsReadOnly(); Members=members.AsReadOnly(); }
        public double TotalLengthMetres
        {
            get
            {
                double total=0;
                foreach(var m in Members)
                {
                    var a=Nodes[m.A]; var b=Nodes[m.B];
                    double x=a.X-b.X, y=a.Y-b.Y, z=a.Z-b.Z;
                    total+=Math.Sqrt(x*x+y*y+z*z);
                }
                return total;
            }
        }
    }
    // Conceptual ellipsoidal cap, metres, Z-up. Not a structural or kinetic solver.
    public static class DomeGenerator
    {
        private static bool Finite(double x) { return !double.IsNaN(x) && !double.IsInfinity(x); }
        public static DomeModel Generate(DomeSettings s)
        {
            if(s==null) throw new ArgumentNullException(nameof(s));
            if(!Finite(s.DiameterMetres)||s.DiameterMetres<0.1||s.DiameterMetres>1000||
               !Finite(s.HeightMetres)||s.HeightMetres<0.1||s.HeightMetres>1000||
               s.Segments<6||s.Segments>256||s.Rings<2||s.Rings>128||
               !Finite(s.OculusRatio)||s.OculusRatio<0||s.OculusRatio>=0.95||
               !Finite(s.SpiralTwistDegrees)||Math.Abs(s.SpiralTwistDegrees)>180||
               !Enum.IsDefined(typeof(DomePattern),s.Pattern))
                throw new ArgumentOutOfRangeException(nameof(s),"Invalid dome settings.");
            var nodes=new List<DomeNode>(); var members=new List<DomeMember>();
            var edges=new HashSet<long>();
            int n=s.Segments, rings=s.Rings;
            bool apex=s.OculusRatio==0;
            double limit=Math.Acos(s.OculusRatio);
            int count=apex?rings:rings+1;
            Action<int,int,string> add=(a,b,role)=>{
                if(a==b) return;
                int lo=Math.Min(a,b), hi=Math.Max(a,b);
                long key=((long)lo<<32)|(uint)hi;
                if(edges.Add(key)) members.Add(new DomeMember(lo,hi,role));
            };
            for(int j=0;j<count;j++)
            {
                double t=(double)j/rings, phi=t*limit;
                double r=s.DiameterMetres*0.5*Math.Cos(phi);
                double z=s.HeightMetres*Math.Sin(phi);
                double offset=s.Pattern==DomePattern.Spiral?t*s.SpiralTwistDegrees*Math.PI/180:
                    s.Pattern==DomePattern.Lamella?(j%2)*Math.PI/n:0;
                for(int i=0;i<n;i++)
                {
                    double theta=2*Math.PI*i/n+offset;
                    nodes.Add(new DomeNode(r*Math.Cos(theta),r*Math.Sin(theta),z));
                }
            }
            for(int j=0;j<count;j++)
            for(int i=0;i<n;i++)
            {
                int a=j*n+i, next=j*n+(i+1)%n;
                if(s.Pattern!=DomePattern.Lamella||j==0||j==count-1)
                    add(a,next,"Ring");
                if(j+1>=count) continue;
                int b=(j+1)*n+i;
                add(a,b,s.Pattern==DomePattern.Lamella?"Diagonal":"Rib");
                if(s.Pattern==DomePattern.Schwedler)
                    add(a,(j+1)*n+(i+1)%n,"Diagonal");
                if(s.Pattern==DomePattern.Lamella)
                    add(a,(j+1)*n+(i+(j%2==0?n-1:1))%n,"Diagonal");
            }
            if(apex)
            {
                int top=nodes.Count; nodes.Add(new DomeNode(0,0,s.HeightMetres));
                for(int i=0;i<n;i++) add((count-1)*n+i,top,"Crown");
            }
            return new DomeModel(nodes,members);
        }
    }
}
