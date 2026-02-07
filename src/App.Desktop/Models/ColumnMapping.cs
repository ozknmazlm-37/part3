using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App.Desktop.Models;

public sealed class ColumnMapping : INotifyPropertyChanged
{
    private string? _sourceColumn;

    public ColumnMapping(string targetField)
    {
        TargetField = targetField;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TargetField { get; }

    public string? SourceColumn
    {
        get => _sourceColumn;
        set
        {
            if (_sourceColumn != value)
            {
                _sourceColumn = value;
                OnPropertyChanged();
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
