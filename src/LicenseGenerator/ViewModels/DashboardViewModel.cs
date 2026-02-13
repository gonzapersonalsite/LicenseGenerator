using CommunityToolkit.Mvvm.ComponentModel;
using LicenseGenerator.Services;
using System.Linq;

namespace LicenseGenerator.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ILicenseGeneratorService _licenseService;

    [ObservableProperty]
    private int _totalApps;

    [ObservableProperty]
    private int _totalLicenses;

    [ObservableProperty]
    private string? _lastLicenseApp;

    public DashboardViewModel(ILicenseGeneratorService licenseService)
    {
        _licenseService = licenseService;
        RefreshStats();
    }

    private void RefreshStats()
    {
        var stats = _licenseService.GetStats();
        TotalApps = stats.AppsCount;
        TotalLicenses = stats.LicensesCount;

        var history = _licenseService.GetLicenseHistory().FirstOrDefault();
        if (history != null)
        {
            LastLicenseApp = $"{history.AppId} ({history.RegistrationName})";
        }
        else
        {
            LastLicenseApp = "Ninguna todavía";
        }
    }
}
