using AE.Revit.Geometry;
using Xunit;

namespace UnitTests;

public class GeometryTests
{
    [Fact] public void SegmentCalculatesLength() => Assert.Equal(5d, new Segment3(new Point3(0, 0, 0), new Point3(3, 4, 0)).Length, 8);
    [Theory] [InlineData(0)] [InlineData(-1)] public void PositiveValidatorRejectsNonPositiveValues(double value) => Assert.NotNull(GeometryValidation.ValidatePositive("Width", value));
    [Fact] public void FacadeParametersRejectNegativeSpacing() => Assert.NotEmpty(new AE.Revit.Core.FacadeParameters { HorizontalSpacingMillimetres = -1 }.Validate());
    [Fact] public void LayoutRespectsBoundaryOffset() => Assert.Single(PanelLayoutGenerator.Generate(new PanelLayoutRequest { Boundary = new Rect2(0, 0, 12, 12), PanelWidth = 10, PanelHeight = 10, BoundaryOffset = 1 }));
    [Fact] public void SpaceFrameCreatesMembers() => Assert.NotEmpty(StructuralGenerators.SpaceFrame(10, 1, 2));
    [Fact] public void MassCalculatesQuantities() => Assert.Equal(300d, new MassEstimate(100, 3, 3).Volume);
    [Fact] public void ClippingRejectsPanelCrossingHole() { var outer=new Polygon2(new[]{new Point2(0,0),new Point2(10,0),new Point2(10,10),new Point2(0,10)}); var hole=new Polygon2(new[]{new Point2(3,3),new Point2(7,3),new Point2(7,7),new Point2(3,7)}); Assert.False(FacadeClipping.ContainsFootprint(outer,new[]{hole},new Rect2(2,2,4,4))); }
}
