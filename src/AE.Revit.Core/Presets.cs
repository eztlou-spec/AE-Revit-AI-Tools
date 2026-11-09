using System;
using System.Collections.Generic;
using System.IO;

namespace AE.Revit.Core;
public sealed class ToolPreset { public ToolPreset(string toolId,string name,string json) { ToolId=toolId; Name=name; Json=json; } public string ToolId {get;} public string Name {get;} public string Json {get;} }
public sealed class PresetStore
{
    private readonly string _root;
    public PresetStore(string root) => _root=root ?? throw new ArgumentNullException(nameof(root));
    public void Save(ToolPreset preset) { if(string.IsNullOrWhiteSpace(preset.ToolId)||string.IsNullOrWhiteSpace(preset.Name)) throw new ArgumentException("Preset tool and name are required."); Directory.CreateDirectory(_root); File.WriteAllText(Path.Combine(_root, Safe(preset.ToolId)+"--"+Safe(preset.Name)+".json"),preset.Json); }
    public ToolPreset? Load(string toolId,string name) { var path=Path.Combine(_root,Safe(toolId)+"--"+Safe(name)+".json"); return File.Exists(path)?new ToolPreset(toolId,name,File.ReadAllText(path)):null; }
    private static string Safe(string value) { foreach(var c in Path.GetInvalidFileNameChars()) value=value.Replace(c,'_'); return value; }
}
public sealed class ExecutionRecord { public ExecutionRecord(string toolId,string message,bool succeeded) { ToolId=toolId; Message=message; Succeeded=succeeded; At=DateTimeOffset.UtcNow; } public string ToolId {get;} public string Message {get;} public bool Succeeded {get;} public DateTimeOffset At {get;} }
