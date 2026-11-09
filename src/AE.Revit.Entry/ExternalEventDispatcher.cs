using System;
using System.Collections.Concurrent;
using Autodesk.Revit.UI;

namespace AE.Revit.Entry;

public sealed class ExternalEventDispatcher : IExternalEventHandler, IDisposable
{
    private readonly ConcurrentQueue<Action<UIApplication>> _queue = new();
    private ExternalEvent? _event;
    private static ExternalEventDispatcher? _instance;
    public static void Initialize() { _instance ??= new ExternalEventDispatcher(); _instance._event ??= ExternalEvent.Create(_instance); }
    public static void Enqueue(Action<UIApplication> operation)
    {
        if (_instance?._event is null) throw new InvalidOperationException("AE Revit AI Tools is not initialized.");
        _instance._queue.Enqueue(operation ?? throw new ArgumentNullException(nameof(operation)));
        _instance._event.Raise();
    }
    public void Execute(UIApplication application)
    {
        while (_queue.TryDequeue(out var operation))
        {
            try { operation(application); } catch (Exception exception) { Log.Error("External event failed", exception); TaskDialog.Show("AE Revit AI Tools", exception.Message); }
        }
    }
    public string GetName() => "AE Revit AI Tools Dispatcher";
    public void Dispose() { _event?.Dispose(); _event = null; }
    public static void Shutdown() { _instance?.Dispose(); _instance = null; }
}
