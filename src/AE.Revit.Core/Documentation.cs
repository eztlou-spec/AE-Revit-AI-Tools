using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Core;

public sealed class SheetRequest
{
    public SheetRequest(string number, string name, string titleBlockId) => (Number, Name, TitleBlockId) = (number, name, titleBlockId);
    public string Number { get; }
    public string Name { get; }
    public string TitleBlockId { get; }
}

public sealed class SheetPlanValidator
{
    public IReadOnlyList<ValidationIssue> Validate(IEnumerable<SheetRequest> requests, IEnumerable<string> existingNumbers)
    {
        var planned = requests?.ToArray() ?? throw new ArgumentNullException(nameof(requests));
        var known = new HashSet<string>(existingNumbers ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        var issues = new List<ValidationIssue>();
        foreach (var request in planned)
        {
            if (string.IsNullOrWhiteSpace(request.Number)) issues.Add(new ValidationIssue("sheet.number.required", "Sheet number is required."));
            else if (!known.Add(request.Number)) issues.Add(new ValidationIssue("sheet.number.duplicate", $"Sheet number '{request.Number}' already exists or is planned twice."));
            if (string.IsNullOrWhiteSpace(request.Name)) issues.Add(new ValidationIssue("sheet.name.required", "Sheet name is required."));
            if (string.IsNullOrWhiteSpace(request.TitleBlockId)) issues.Add(new ValidationIssue("sheet.titleblock.required", "A title block must be selected."));
        }
        return issues;
    }
}
