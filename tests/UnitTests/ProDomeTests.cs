using System;
using System.Linq;
using AE.Revit.Geometry.Domes;
using Xunit;

public class ProDomeTests
{
    [Theory]
    [InlineData(ProDomeType.Ribbed)]
    [InlineData(ProDomeType.Schwedler)]
    [InlineData(ProDomeType.Lamella)]
    [InlineData(ProDomeType.Spiral)]
    [InlineData(ProDomeType.Geodesic)]
    [InlineData(ProDomeType.SpaceFrame)]
    [InlineData(ProDomeType.Kiewitt)]
    [InlineData(ProDomeType.Islamic)]
    [InlineData(ProDomeType.Freeform)]
    [InlineData(ProDomeType.IrisConcept)]
    public void TopologyIsValid(ProDomeType type)
    {
        var m=ProDomeGenerator.Generate(new ProDomeSettings{Type=type,Segments=24,Rings=6});
        Assert.NotEmpty(m.Nodes);Assert.NotEmpty(m.Members);
        Assert.All(m.Members,e=>{
            Assert.InRange(e.A,0,m.Nodes.Count-1);Assert.InRange(e.B,0,m.Nodes.Count-1);
            Assert.NotEqual(e.A,e.B);
            var a=m.Nodes[e.A];var b=m.Nodes[e.B];
            Assert.True(Math.Abs(a.X-b.X)+Math.Abs(a.Y-b.Y)+Math.Abs(a.Z-b.Z)>1e-9);
        });
        Assert.Equal(m.Members.Count,m.Members.Select(e=>new {A=Math.Min(e.A,e.B),B=Math.Max(e.A,e.B)}).Distinct().Count());
        Assert.True(m.TotalLengthMetres>0);
        Assert.Equal(m.Nodes.Count,m.Members.SelectMany(e=>new[]{e.A,e.B}).Distinct().Count());
    }
    [Fact]
    public void GeodesicHasWeldedSectorSeams()
    {
        var m=ProDomeGenerator.Generate(new ProDomeSettings{Type=ProDomeType.Geodesic,Rings=6});
        Assert.Equal(85,m.Nodes.Count);
        Assert.Single(m.Nodes.Where(n=>Math.Abs(n.Z-8)<1e-9));
    }
    [Fact]
    public void SpaceFrameHasTwoLayers()
    {
        var a=ProDomeGenerator.Generate(new ProDomeSettings{Type=ProDomeType.Schwedler});
        var b=ProDomeGenerator.Generate(new ProDomeSettings{Type=ProDomeType.SpaceFrame});
        Assert.Equal(2*a.Nodes.Count,b.Nodes.Count);
        Assert.Equal(3*a.Members.Count+a.Nodes.Count,b.Members.Count);
    }
    [Fact]
    public void InvalidOpeningIsRejected()
    {
        Assert.Throws<ArgumentException>(()=>ProDomeGenerator.Generate(new ProDomeSettings{Type=ProDomeType.Geodesic,Opening=0.3}));
    }
}
