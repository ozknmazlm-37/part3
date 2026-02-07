using System.Windows;

using App.Desktop.Services;
using App.Desktop.ViewModels;

namespace App.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new DashboardViewModel(new DashboardService());
    }
}
