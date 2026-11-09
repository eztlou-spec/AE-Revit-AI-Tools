using AE.Revit.Core;
using Xunit;

namespace UnitTests;

public class RoomAndDocumentationTests
{
    [Fact] public void AuditFindsDuplicateAndUnplacedRooms()
    {
        var issues = new RoomAudit().Inspect(new[] { new RoomRecord("a", "101", "Office", 12, true, true), new RoomRecord("b", "101", "Store", 0, false, false) });
        Assert.Contains(issues, issue => issue.Code == "room.duplicate-number");
        Assert.Contains(issues, issue => issue.Code == "room.unplaced");
    }
    [Fact] public void SheetValidationRejectsExistingNumber() => Assert.Contains(new SheetPlanValidator().Validate(new[] { new SheetRequest("A-101", "Plan", "tb") }, new[] { "A-101" }), issue => issue.Code == "sheet.number.duplicate");
    [Fact] public void FinishValidationRequiresPositiveThickness() => Assert.Contains(FinishValidator.Validate(new FinishRequest("room", "finish", 0)), issue => issue.Code == "finish.thickness");
}
