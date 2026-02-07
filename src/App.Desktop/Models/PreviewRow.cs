namespace App.Desktop.Models;

public sealed class PreviewRow
{
    public PreviewRow(string displayText)
    {
        DisplayText = displayText;
    }

    public string DisplayText { get; }
}
