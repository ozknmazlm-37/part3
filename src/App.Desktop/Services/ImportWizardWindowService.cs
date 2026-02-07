using System.Windows;
using App.Desktop.ViewModels;

namespace App.Desktop.Services;

public interface IImportWizardWindowService
{
    void ShowWizard();
}

public sealed class ImportWizardWindowService : IImportWizardWindowService
{
    public void ShowWizard()
    {
        var viewModel = new ImportWizardViewModel(new ExcelImportService());
        var window = new ExcelImportWindow
        {
            Owner = Application.Current?.MainWindow,
            DataContext = viewModel
        };

        window.ShowDialog();
    }
}
