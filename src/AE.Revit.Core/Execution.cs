using System;

namespace AE.Revit.Core;

public interface ITransactionScope : IDisposable
{
    void Commit();
    void RollBack();
}

public interface ITransactionService
{
    ITransactionScope Begin(string name);
}

public sealed class CommandExecutor
{
    private readonly ITransactionService _transactions;
    public CommandExecutor(ITransactionService transactions) => _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
    public CommandResult Execute<T>(IRevitCommand<T> command, T request)
    {
        var issues = command.Validate(request);
        if (issues.Any(issue => issue.Severity == CommandSeverity.Error)) return CommandResult.Failure("Validation failed.", issues.ToArray());
        using var transaction = _transactions.Begin(command.Id);
        try { var result = command.Execute(request); if (result.Succeeded) transaction.Commit(); else transaction.RollBack(); return result; }
        catch (Exception ex) { transaction.RollBack(); return CommandResult.Failure("The command was rolled back.", new ValidationIssue("execution.exception", ex.Message)); }
    }
}
