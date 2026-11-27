using System;
using System.Linq;
using AE.Revit.Geometry.Domes;
using Xunit;

public class DomeGeneratorTests
{
    [Theory]
    [InlineData(DomePattern.Ribbed)]
    [InlineData(DomePattern.Schwedler)]
    [InlineData(DomePattern.Lamella)]
    [InlineData(DomePattern.Spiral)]
    public void ClosedCapHasUniqueApexAndValidEdges(DomePattern pattern)
    {
        var s = new DomeSettings { Pattern=pattern, Segments=12, Rings=4 };
        var model = DomeGenerator.Generate(s);
        Assert.Equal(49, model.Nodes.Count);
        Assert.Single(model.Nodes.Where(n=>Math.Abs(n.Z-8)<1e-9));
        Assert.All(model.Members, m=>{
            Assert.InRange(m.A,0,model.Nodes.Count-1);
            Assert.InRange(m.B,0,model.Nodes.Count-1);
            Assert.True(m.A<m.B);
        });
        Assert.Equal(model.Members.Count,model.Members.Select(m=>new {m.A,m.B}).Distinct().Count());
        Assert.True(model.TotalLengthMetres>0);
    }

    [Fact]
    public void OculusHasExpectedRadius()
    {
        var model = DomeGenerator.Generate(new DomeSettings { Segments=12,Rings=4,OculusRatio=0.4 });
        Assert.Equal(60,model.Nodes.Count);
        foreach(var p in model.Nodes.Skip(48))
            Assert.Equal(4,Math.Sqrt(p.X*p.X+p.Y*p.Y),8);
    }

    [Fact]
    public void RejectsInvalidSettings()
    {
        Assert.Throws<ArgumentOutOfRangeException>(()=>DomeGenerator.Generate(new DomeSettings { DiameterMetres=double.NaN }));
        Assert.Throws<ArgumentOutOfRangeException>(()=>DomeGenerator.Generate(new DomeSettings { Segments=0 }));
    }
}
