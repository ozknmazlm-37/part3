namespace App.Desktop.Models;

public sealed class ValidationIssue
{
    public ValidationIssue(string message, string severity)
    {
        Message = message;
        Severity = severity;
    }

    public string Message { get; }

    public string Severity { get; }
}
