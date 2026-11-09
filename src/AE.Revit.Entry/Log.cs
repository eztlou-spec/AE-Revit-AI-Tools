using System;
using System.IO;

namespace AE.Revit.Entry;

internal static class Log
{
    private static readonly object Sync = new();
    private static string PathToLog => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AhmedEztlou", "AERevitAiTools", "ae-revit-ai-tools.log");
    internal static void Error(string message, Exception exception) => Write("ERROR", message + Environment.NewLine + exception);
    internal static void Info(string message) => Write("INFO", message);
    private static void Write(string level, string message)
    {
        lock (Sync)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathToLog)!);
            File.AppendAllText(PathToLog, $"{DateTimeOffset.Now:O} [{level}] {message}{Environment.NewLine}");
        }
    }
}
