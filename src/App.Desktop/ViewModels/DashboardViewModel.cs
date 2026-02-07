using System.Collections.ObjectModel;
using System.Windows.Input;
using App.Desktop.Models;
using App.Desktop.Services;

namespace App.Desktop.ViewModels;

public sealed class DashboardViewModel : ViewModelBase
{
    private readonly DashboardService _service;
    private readonly IImportWizardWindowService _importWizardWindowService;
    private string _welcomeTitle = "Hoş geldin, Admin";
    private string _dailySummary = "Bugün 12 sorgu";

    public DashboardViewModel(DashboardService service, IImportWizardWindowService importWizardWindowService)
    {
        _service = service;
        _importWizardWindowService = importWizardWindowService;
        RefreshCommand = new RelayCommand(Load);
        OpenImportWizardCommand = new RelayCommand(OpenImportWizard);
        Load();
    }

    public ObservableCollection<DashboardKpi> Kpis { get; } = [];

    public ObservableCollection<RecentActivity> RecentActivities { get; } = [];

    public string WelcomeTitle
    {
        get => _welcomeTitle;
        set
        {
            SetProperty(ref _welcomeTitle, value);
        }
    }

    public string DailySummary
    {
        get => _dailySummary;
        set
        {
            SetProperty(ref _dailySummary, value);
        }
    }

    public ICommand RefreshCommand { get; }

    public ICommand OpenImportWizardCommand { get; }

    private void Load()
    {
        Kpis.Clear();
        foreach (var kpi in _service.GetKpis())
        {
            Kpis.Add(kpi);
        }

        RecentActivities.Clear();
        foreach (var activity in _service.GetRecentActivities())
        {
            RecentActivities.Add(activity);
        }
    }

    private void OpenImportWizard()
        => _importWizardWindowService.ShowWizard();
}
