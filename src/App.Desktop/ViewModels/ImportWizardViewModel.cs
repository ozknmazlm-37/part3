using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using App.Desktop.Models;
using App.Desktop.Services;
using Microsoft.Win32;

namespace App.Desktop.ViewModels;

public sealed class ImportWizardViewModel : ViewModelBase
{
    private readonly ExcelImportService _service;
    private readonly IReadOnlyList<string> _targetFields =
    [
        "Sayaç Seri No",
        "Abone No",
        "Abone Adı",
        "Kofre No",
        "X Koordinat",
        "Y Koordinat",
        "Adres"
    ];

    private string? _filePath;
    private string? _selectedSheet;
    private int _currentStepIndex;
    private string _importSummary = "Hazırlanıyor...";
    private int _importedCount;
    private int _errorCount;

    public ImportWizardViewModel(ExcelImportService service)
    {
        _service = service;
        BrowseFileCommand = new RelayCommand(BrowseFile);
        NextStepCommand = new RelayCommand(NextStep, CanGoNext);
        PreviousStepCommand = new RelayCommand(PreviousStep, CanGoPrevious);
    }

    public ObservableCollection<string> Sheets { get; } = [];

    public ObservableCollection<string> Headers { get; } = [];

    public ObservableCollection<ColumnMapping> Mappings { get; } = [];

    public ObservableCollection<ValidationIssue> ValidationIssues { get; } = [];

    public ObservableCollection<PreviewRow> PreviewRows { get; } = [];

    public ICommand BrowseFileCommand { get; }

    public ICommand NextStepCommand { get; }

    public ICommand PreviousStepCommand { get; }

    public string? FilePath
    {
        get => _filePath;
        set
        {
            if (SetProperty(ref _filePath, value))
            {
                LoadSheets();
            }
        }
    }

    public string? SelectedSheet
    {
        get => _selectedSheet;
        set
        {
            if (SetProperty(ref _selectedSheet, value))
            {
                LoadHeaders();
            }
        }
    }

    public int CurrentStepIndex
    {
        get => _currentStepIndex;
        set
        {
            if (SetProperty(ref _currentStepIndex, value))
            {
                LoadStepData();
                RaiseNavigationCanExecute();
            }
        }
    }

    public string ImportSummary
    {
        get => _importSummary;
        set => SetProperty(ref _importSummary, value);
    }

    public int ImportedCount
    {
        get => _importedCount;
        set => SetProperty(ref _importedCount, value);
    }

    public int ErrorCount
    {
        get => _errorCount;
        set => SetProperty(ref _errorCount, value);
    }

    private void BrowseFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Excel Dosyaları|*.xlsx;*.xls",
            Title = "Excel dosyası seç"
        };

        if (dialog.ShowDialog() == true)
        {
            FilePath = dialog.FileName;
        }
    }

    private void LoadSheets()
    {
        Sheets.Clear();
        Headers.Clear();
        Mappings.Clear();
        ValidationIssues.Clear();
        PreviewRows.Clear();

        if (string.IsNullOrWhiteSpace(FilePath))
        {
            return;
        }

        foreach (var sheet in _service.GetSheetNames(FilePath))
        {
            Sheets.Add(sheet);
        }

        SelectedSheet = Sheets.FirstOrDefault();
    }

    private void LoadHeaders()
    {
        Headers.Clear();
        Mappings.Clear();

        if (string.IsNullOrWhiteSpace(FilePath) || string.IsNullOrWhiteSpace(SelectedSheet))
        {
            return;
        }

        foreach (var header in _service.GetHeaders(FilePath, SelectedSheet))
        {
            Headers.Add(header);
        }

        foreach (var field in _targetFields)
        {
            Mappings.Add(new ColumnMapping(field));
        }
    }

    private void LoadStepData()
    {
        if (CurrentStepIndex == 2)
        {
            if (Mappings.Count == 0)
            {
                foreach (var field in _targetFields)
                {
                    Mappings.Add(new ColumnMapping(field));
                }
            }
        }

        if (CurrentStepIndex == 3)
        {
            ValidateMappings();
        }

        if (CurrentStepIndex == 4)
        {
            LoadPreview();
        }

        if (CurrentStepIndex == 5)
        {
            BuildSummary();
        }
    }

    private void ValidateMappings()
    {
        ValidationIssues.Clear();

        var requiredFields = new[] { "Sayaç Seri No", "Kofre No" };
        foreach (var requiredField in requiredFields)
        {
            var mapping = Mappings.FirstOrDefault(item => item.TargetField == requiredField);
            if (mapping is null || string.IsNullOrWhiteSpace(mapping.SourceColumn))
            {
                ValidationIssues.Add(new ValidationIssue($"{requiredField} alanı eşlenmedi.", "Kritik"));
            }
        }

        if (ValidationIssues.Count == 0)
        {
            ValidationIssues.Add(new ValidationIssue("Tüm zorunlu alanlar eşlendi.", "Bilgi"));
        }
    }

    private void LoadPreview()
    {
        PreviewRows.Clear();

        if (string.IsNullOrWhiteSpace(FilePath) || string.IsNullOrWhiteSpace(SelectedSheet))
        {
            return;
        }

        var rows = _service.GetPreviewRows(FilePath, SelectedSheet, 10);
        foreach (var row in rows)
        {
            PreviewRows.Add(new PreviewRow(string.Join(" | ", row)));
        }
    }

    private void BuildSummary()
    {
        ImportedCount = PreviewRows.Count;
        ErrorCount = ValidationIssues.Count(issue => issue.Severity == "Kritik");
        ImportSummary = $"Önizlemeye göre {ImportedCount} kayıt hazır. {ErrorCount} kritik uyarı bulundu.";
    }

    private void NextStep()
    {
        if (CurrentStepIndex < 5)
        {
            CurrentStepIndex++;
        }
    }

    private bool CanGoNext() => CurrentStepIndex < 5;

    private void PreviousStep()
    {
        if (CurrentStepIndex > 0)
        {
            CurrentStepIndex--;
        }
    }

    private bool CanGoPrevious() => CurrentStepIndex > 0;

    private void RaiseNavigationCanExecute()
    {
        if (NextStepCommand is RelayCommand next)
        {
            next.RaiseCanExecuteChanged();
        }

        if (PreviousStepCommand is RelayCommand previous)
        {
            previous.RaiseCanExecuteChanged();
        }
    }
}
