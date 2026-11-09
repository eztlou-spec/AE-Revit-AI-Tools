using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AE.Revit.Entry.UI;

public sealed class DashboardTool : INotifyPropertyChanged
{
    public DashboardTool(string title, string description, bool available, Action? execute) { Title=title; Description=description; IsAvailable=available; Execute=new DelegateCommand(_ => execute?.Invoke(), _ => available && execute is not null); }
    public string Title { get; } public string Description { get; } public bool IsAvailable { get; } public ICommand Execute { get; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
public sealed class DashboardSection { public DashboardSection(string name, params DashboardTool[] tools) { Name=name; Tools=new ObservableCollection<DashboardTool>(tools); } public string Name {get;} public ObservableCollection<DashboardTool> Tools {get;} }
public sealed class DashboardViewModel
{
    public DashboardViewModel()
    {
        Sections = new ObservableCollection<DashboardSection>
        {
            new("Home", new DashboardTool("Room Audit", "Audit rooms in the active project.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.RunRoomAudit))),
            new("Parametric Studio", new DashboardTool("Rectangular Facade", "Select a planar host face and generate tracked DirectShape panels.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.OpenFacade)), new DashboardTool("Louvers", "Revit adapter pending.", false, null), new DashboardTool("Space Frame", "Revit adapter pending.", false, null), new DashboardTool("Dome", "Revit adapter pending.", false, null), new DashboardTool("Mass", "Revit adapter pending.", false, null), new DashboardTool("Floor Pattern", "Revit adapter pending.", false, null)),
            new("BIM Modeling", new DashboardTool("Smart Walls", "Not implemented.", false, null), new DashboardTool("Smart Levels", "Not implemented.", false, null)),
            new("Smart Finishing", new DashboardTool("Floor Finish", "Create a floor finish from a selected room.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.OpenFloorFinish))),
            new("Documentation", new DashboardTool("Auto Sheets", "Create a sheet with the first available title block.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.OpenSheets)), new DashboardTool("Auto Tag", "Tag selected point-based elements.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.OpenTags)), new DashboardTool("Smart Dimension", "Dimension a selected wall in a plan view.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.OpenDimensions))),
            new("Site Tools", new DashboardTool("Topography", "Not implemented.", false, null)), new("Materials", new DashboardTool("Material Manager", "Not implemented.", false, null)), new("Family Library", new DashboardTool("Family Library", "Not implemented.", false, null)), new("QA/QC", new DashboardTool("QA/QC", "Room audit currently available.", true, () => ExternalEventDispatcher.Enqueue(DashboardOperations.RunRoomAudit))), new("AI Assistant", new DashboardTool("Offline command parser", "Domain parser implemented; Revit UI pending.", false, null)), new("Batch Automation", new DashboardTool("Batch queue", "Not implemented.", false, null)), new("Settings", new DashboardTool("Settings", "Not implemented.", false, null))
        };
    }
    public ObservableCollection<DashboardSection> Sections { get; }
}
public sealed class DelegateCommand : ICommand { private readonly Action<object?> _execute; private readonly Func<object?,bool> _can; public DelegateCommand(Action<object?> execute, Func<object?,bool> can) { _execute=execute; _can=can; } public bool CanExecute(object? p)=>_can(p); public void Execute(object? p)=>_execute(p); public event EventHandler? CanExecuteChanged; }
