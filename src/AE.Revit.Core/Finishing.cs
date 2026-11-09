using System;
using System.Collections.Generic;

namespace AE.Revit.Core;

public sealed class FinishRequest
{
    public FinishRequest(string roomUniqueId, string finishTypeId, double thickness) => (RoomUniqueId, FinishTypeId, Thickness) = (roomUniqueId, finishTypeId, thickness);
    public string RoomUniqueId { get; }
    public string FinishTypeId { get; }
    public double Thickness { get; }
}

public static class FinishValidator
{
    public static IReadOnlyList<ValidationIssue> Validate(FinishRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var issues = new List<ValidationIssue>();
        if (string.IsNullOrWhiteSpace(request.RoomUniqueId)) issues.Add(new ValidationIssue("finish.room.required", "A placed room is required."));
        if (string.IsNullOrWhiteSpace(request.FinishTypeId)) issues.Add(new ValidationIssue("finish.type.required", "A finish type is required."));
        if (request.Thickness <= 0) issues.Add(new ValidationIssue("finish.thickness", "Finish thickness must be positive."));
        return issues;
    }
}
