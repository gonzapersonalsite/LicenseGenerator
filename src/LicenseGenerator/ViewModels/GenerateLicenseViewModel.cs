using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseGenerator.ViewModels;

public partial class GenerateLicenseViewModel : ViewModelBase
{
    private readonly ILicenseGeneratorService _licenseService;
    private readonly ILanguageService _languageService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string? _selectedApp;

    [ObservableProperty]
    private string _registrationName = string.Empty;

    [ObservableProperty]
    private string _hardwareId = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _expirationDate; // Null means PERMANENT

    [ObservableProperty]
    private string? _generatedLicense;

    public ObservableCollection<string> AvailableApps { get; } = new();

    public GenerateLicenseViewModel(ILicenseGeneratorService licenseService, ILanguageService languageService, INotificationService notificationService)
    {
        _licenseService = licenseService;
        _languageService = languageService;
        _notificationService = notificationService;
        LoadApps();
    }

    private void LoadApps()
    {
        AvailableApps.Clear();
        foreach (var app in _licenseService.GetAvailableApps())
        {
            AvailableApps.Add(app);
        }
        SelectedApp = AvailableApps.FirstOrDefault();
    }

    [RelayCommand]
    private void Generate()
    {
        if (string.IsNullOrEmpty(SelectedApp))
        {
            _notificationService.ShowWarning(_languageService["CommonWarning"], _languageService["Notification.AppRequired"]);
            return;
        }

        if (string.IsNullOrWhiteSpace(RegistrationName))
        {
            _notificationService.ShowWarning(_languageService["CommonWarning"], _languageService["Notification.CustomerRequired"]);
            return;
        }

        try
        {
            GeneratedLicense = _licenseService.GenerateLicense(
                SelectedApp, 
                RegistrationName, 
                HardwareId, 
                ExpirationDate?.DateTime);
            
            _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Notification.LicenseGenerated"]);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(_languageService["CommonError"], ex.Message);
        }
    }

    [RelayCommand]
    private void ClearExpiration() => ExpirationDate = null;

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        if (string.IsNullOrEmpty(GeneratedLicense)) return;

        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var clipboard = desktop.MainWindow?.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(GeneratedLicense);
                _notificationService.ShowInfo(_languageService["CommonInfo"], _languageService["Notification.LicenseCopied"]);
            }
        }
    }
}
