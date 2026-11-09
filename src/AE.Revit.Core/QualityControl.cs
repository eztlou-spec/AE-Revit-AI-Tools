using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Core;

public sealed class QualityItem
{
    public QualityItem(string id, string category, string name, string? materialName = null) => (Id, Category, Name, MaterialName) = (id, category, name, materialName);
    public string Id { get; } public string Category { get; } public string Name { get; } public string? MaterialName { get; }
}
public sealed class QualityAudit
{
    public IReadOnlyList<ValidationIssue> Inspect(IEnumerable<QualityItem> items)
    {
        var data = items?.ToArray() ?? throw new ArgumentNullException(nameof(items)); var results = new List<ValidationIssue>();
        results.AddRange(data.Where(i => string.IsNullOrWhiteSpace(i.Name)).Select(i => new ValidationIssue("qc.name", $"{i.Category} {i.Id} has no name.")));
        results.AddRange(data.Where(i => string.IsNullOrWhiteSpace(i.MaterialName)).Select(i => new ValidationIssue("qc.material", $"{i.Category} {i.Id} has no material.", CommandSeverity.Warning)));
        results.AddRange(data.GroupBy(i => $"{i.Category}\u001f{i.Name}", StringComparer.OrdinalIgnoreCase).Where(g => !string.IsNullOrWhiteSpace(g.First().Name) && g.Count() > 1).Select(g => new ValidationIssue("qc.duplicate-name", $"{g.First().Category} name '{g.First().Name}' is duplicated.")));
        return results;
    }
}
