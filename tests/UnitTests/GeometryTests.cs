using AE.Revit.Geometry;
using Xunit;

namespace UnitTests;

public class GeometryTests
{
    [Fact] public void SegmentCalculatesLength() => Assert.Equal(5d, new Segment3(new Point3(0, 0, 0), new Point3(3, 4, 0)).Length, 8);
    [Theory] [InlineData(0)] [InlineData(-1)] public void PositiveValidatorRejectsNonPositiveValues(double value) => Assert.NotNull(GeometryValidation.ValidatePositive("Width", value));
}
