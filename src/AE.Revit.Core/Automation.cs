using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Core;

public sealed class AutomationRequest { public AutomationRequest(string text) => Text=text ?? throw new ArgumentNullException(nameof(text)); public string Text { get; } }
public sealed class AutomationIntent { public AutomationIntent(string commandId, IReadOnlyDictionary<string,string> parameters) => (CommandId,Parameters)=(commandId,parameters); public string CommandId {get;} public IReadOnlyDictionary<string,string> Parameters {get;} }
public interface IIntentParser { AutomationIntent? Parse(AutomationRequest request); }
public sealed class OfflineIntentParser : IIntentParser
{
    private static readonly (string phrase,string id)[] Rules = { ("room audit","room.audit"), ("dimension","documentation.dimension"), ("sheet","documentation.sheet"), ("floor finish","finishing.floor"), ("facade","parametric.facade") };
    public AutomationIntent? Parse(AutomationRequest request) { var rule=Rules.FirstOrDefault(r=>request.Text.IndexOf(r.phrase,StringComparison.OrdinalIgnoreCase)>=0); return rule.id is null ? null : new AutomationIntent(rule.id,new Dictionary<string,string>()); }
}
public sealed class BatchPlan<T> { public BatchPlan(IEnumerable<T> items) => Items=items?.ToArray()??throw new ArgumentNullException(nameof(items)); public IReadOnlyList<T> Items {get;} }
