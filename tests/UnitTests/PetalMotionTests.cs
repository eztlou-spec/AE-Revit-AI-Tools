using System;
using AE.Revit.Geometry.Domes;
using Xunit;
public class PetalMotionTests
{
    [Fact]
    public void RigidPetalMotionPreservesLengthsAndHingeCorners()
    {
        var s=new ProDomeSettings{Type=ProDomeType.PetalMotion,Segments=12,Rings=6};
        var a=ProDomeGenerator.Generate(s);
        s.Opening=1;
        var b=ProDomeGenerator.Generate(s);
        Assert.Equal(a.Nodes.Count,b.Nodes.Count);
        Assert.Equal(a.Members.Count,b.Members.Count);
        double L(DomeModel m,DomeMember e){var p=m.Nodes[e.A];var q=m.Nodes[e.B];return Math.Sqrt(Math.Pow(p.X-q.X,2)+Math.Pow(p.Y-q.Y,2)+Math.Pow(p.Z-q.Z,2));}
        for(int i=0;i<a.Members.Count;i++)Assert.Equal(L(a,a.Members[i]),L(b,b.Members[i]),8);
        for(int k=0;k<s.Segments;k++)foreach(int i in new[]{0,2})
        {
            int n=k*(s.Rings*3+1)+i;
            Assert.Equal(a.Nodes[n].X,b.Nodes[n].X,8);Assert.Equal(a.Nodes[n].Y,b.Nodes[n].Y,8);Assert.Equal(a.Nodes[n].Z,b.Nodes[n].Z,8);
        }
        Assert.True(Math.Abs(a.Nodes[18].X-b.Nodes[18].X)>1);
    }
}
