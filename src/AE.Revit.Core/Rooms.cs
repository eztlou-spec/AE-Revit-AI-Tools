using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Core;

public sealed class RoomRecord
{
    public RoomRecord(string uniqueId, string number, string name, double area, bool isPlaced, bool isEnclosed)
        => (UniqueId, Number, Name, Area, IsPlaced, IsEnclosed) = (uniqueId, number, name, area, isPlaced, isEnclosed);
    public string UniqueId { get; }
    public string Number { get; }
    public string Name { get; }
    public double Area { get; }
    public bool IsPlaced { get; }
    public bool IsEnclosed { get; }
}

public sealed class RoomAudit
{
    public IReadOnlyList<ValidationIssue> Inspect(IEnumerable<RoomRecord> rooms)
    {
        if (rooms is null) throw new ArgumentNullException(nameof(rooms));
        var records = rooms.ToArray();
        var issues = new List<ValidationIssue>();
        issues.AddRange(records.Where(r => !r.IsPlaced).Select(r => new ValidationIssue("room.unplaced", $"Room '{r.Number}' is unplaced.", CommandSeverity.Warning)));
        issues.AddRange(records.Where(r => r.IsPlaced && !r.IsEnclosed).Select(r => new ValidationIssue("room.unenclosed", $"Room '{r.Number}' is not enclosed.")));
        issues.AddRange(records.Where(r => r.IsPlaced && r.Area <= 0).Select(r => new ValidationIssue("room.area", $"Room '{r.Number}' has no measurable area.")));
        issues.AddRange(records.Where(r => !string.IsNullOrWhiteSpace(r.Number)).GroupBy(r => r.Number, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => new ValidationIssue("room.duplicate-number", $"Room number '{g.Key}' is used by {g.Count()} rooms.")));
        return issues;
    }
}
