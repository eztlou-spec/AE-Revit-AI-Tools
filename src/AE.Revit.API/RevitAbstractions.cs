using System;
using AE.Revit.Core;

namespace AE.Revit.API;

/// <summary>Adapter boundary: implementations are Revit-version-specific and run only on Revit's API thread.</summary>
public interface IRevitDocumentAdapter
{
    string Title { get; }
    bool IsModifiable { get; }
    ITransactionScope BeginTransaction(string name);
}

public sealed class RevitTransactionService : ITransactionService
{
    private readonly IRevitDocumentAdapter _document;
    public RevitTransactionService(IRevitDocumentAdapter document) => _document = document ?? throw new ArgumentNullException(nameof(document));
    public ITransactionScope Begin(string name) => _document.BeginTransaction(name);
}
