using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using System;
using System.Linq;

namespace LicenseGenerator.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ILicenseGeneratorService _licenseService;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private int _totalApps;

    [ObservableProperty]
    private int _totalLicenses;

    [ObservableProperty]
    private int _activeLicenses;

    [ObservableProperty]
    private int _expiredLicenses;

    [ObservableProperty]
    private int _activePercentage;

    [ObservableProperty]
    private string? _lastLicenseApp;

    public DashboardViewModel(ILicenseGeneratorService licenseService, ILanguageService languageService)
    {
        _licenseService = licenseService;
        _languageService = languageService;
        RefreshStats();
    }

    private void RefreshStats()
    {
        var stats = _licenseService.GetStats();
        TotalApps = stats.AppsCount;
        TotalLicenses = stats.LicensesCount;

        var historyList = _licenseService.GetLicenseHistory().ToList();
        var latest = historyList.FirstOrDefault();
        if (latest != null)
        {
            LastLicenseApp = $"{latest.AppId} ({latest.RegistrationName})";
        }
        else
        {
            LastLicenseApp = _languageService.GetString("DashboardNoLicenseYet");
        }

        // Active vs expired
        if (historyList.Count == 0)
        {
            ActiveLicenses = 0;
            ExpiredLicenses = 0;
            ActivePercentage = 0;
        }
        else
        {
            var now = DateTime.Now;
            ActiveLicenses = historyList.Count(l =>
                l.ExpirationDate == null || l.ExpirationDate >= now);
            ExpiredLicenses = historyList.Count(l =>
                l.ExpirationDate != null && l.ExpirationDate < now);

            ActivePercentage = (int)Math.Round(ActiveLicenses * 100.0 / historyList.Count);
        }
    }
}
