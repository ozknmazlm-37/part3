using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App.Desktop.Models;
using App.Desktop.Services;

namespace App.Desktop.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DashboardService _service;
    private string _welcomeTitle = "Hoş geldin, Admin";
    private string _dailySummary = "Bugün 12 sorgu";

    public DashboardViewModel(DashboardService service)
    {
        _service = service;
        RefreshCommand = new RelayCommand(Load);
        Load();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<DashboardKpi> Kpis { get; } = [];

    public ObservableCollection<RecentActivity> RecentActivities { get; } = [];

    public string WelcomeTitle
    {
        get => _welcomeTitle;
        set
        {
            if (_welcomeTitle != value)
            {
                _welcomeTitle = value;
                OnPropertyChanged();
            }
        }
    }

    public string DailySummary
    {
        get => _dailySummary;
        set
        {
            if (_dailySummary != value)
            {
                _dailySummary = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand RefreshCommand { get; }

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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
