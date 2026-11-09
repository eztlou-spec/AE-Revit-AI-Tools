using System;
using System.Collections.Generic;
using System.Linq;

namespace AE.Revit.Core;

public enum CommandSeverity { Info, Warning, Error }

public sealed class ValidationIssue
{
    public ValidationIssue(string code, string message, CommandSeverity severity = CommandSeverity.Error)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Severity = severity;
    }
    public string Code { get; }
    public string Message { get; }
    public CommandSeverity Severity { get; }
}

public sealed class CommandResult
{
    private CommandResult(bool succeeded, string message, IReadOnlyList<ValidationIssue> issues)
        => (Succeeded, Message, Issues) = (succeeded, message, issues);
    public bool Succeeded { get; }
    public string Message { get; }
    public IReadOnlyList<ValidationIssue> Issues { get; }
    public static CommandResult Success(string message) => new(true, message, Array.Empty<ValidationIssue>());
    public static CommandResult Failure(string message, params ValidationIssue[] issues) => new(false, message, issues);
}

public interface IRevitCommand<in TRequest>
{
    string Id { get; }
    IReadOnlyCollection<ValidationIssue> Validate(TRequest request);
    CommandResult Execute(TRequest request);
}

public sealed class CommandRegistry
{
    private readonly Dictionary<string, object> _commands = new(StringComparer.OrdinalIgnoreCase);
    public void Register<TRequest>(IRevitCommand<TRequest> command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        if (!_commands.TryAdd(command.Id, command)) throw new InvalidOperationException($"Command '{command.Id}' is already registered.");
    }
    public bool Contains(string id) => _commands.ContainsKey(id);
    public IReadOnlyCollection<string> Ids => _commands.Keys.OrderBy(x => x).ToArray();
}
