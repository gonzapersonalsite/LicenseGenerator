using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Models;
using LicenseGenerator.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace LicenseGenerator.ViewModels;

public partial class HistoryViewModel : PaginatedViewModelBase
{
    private readonly ILicenseGeneratorService _licenseService;
    private readonly ILanguageService _languageService;
    private readonly INotificationService _notificationService;
    private List<LicenseData> _allLicenses = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isConfirmDeleteOpen;

    [ObservableProperty]
    private LicenseData? _licenseToDelete;

    public ObservableCollection<LicenseData> FilteredLicenses { get; } = new();

    public HistoryViewModel(ILicenseGeneratorService licenseService, ILanguageService languageService, INotificationService notificationService)
    {
        _licenseService = licenseService;
        _languageService = languageService;
        _notificationService = notificationService;
        RefreshHistory();
    }

    private void RefreshHistory()
    {
        _allLicenses = _licenseService.GetLicenseHistory().ToList();
        UpdateFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1;
        UpdateFilter();
    }

    [RelayCommand]
    private void Search()
    {
        CurrentPage = 1;
        UpdateFilter();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        CurrentPage = 1;
        UpdateFilter();
    }

    protected override void UpdateFilter()
    {
        var query = _allLicenses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            query = query.Where(l => l.RegistrationName.ToLower().Contains(search) || 
                                    l.AppId.ToLower().Contains(search) ||
                                    l.HardwareId.ToLower().Contains(search));
        }

        TotalCount = query.Count();
        
        var paged = query.Skip((CurrentPage - 1) * PageSize)
                         .Take(PageSize)
                         .ToList();

        FilteredLicenses.Clear();
        foreach (var item in paged)
        {
            FilteredLicenses.Add(item);
        }
    }

    [RelayCommand]
    private async Task CopyLicense(LicenseData? license)
    {
        if (license == null) return;

        try
        {
            string json = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
            string base64 = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var clipboard = desktop.MainWindow?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(base64);
                    _notificationService.ShowInfo(_languageService["CommonInfo"], _languageService["Notification.CopySuccess"]);
                }
            }
        }
        catch 
        { 
            _notificationService.ShowError(_languageService["CommonError"], "Error al copiar.");
        }
    }

    [RelayCommand]
    private void DeleteLicense(LicenseData? license)
    {
        if (license == null) return;
        LicenseToDelete = license;
        IsConfirmDeleteOpen = true;
    }

    [RelayCommand]
    private void ConfirmDelete()
    {
        if (LicenseToDelete != null && !string.IsNullOrEmpty(LicenseToDelete.FileName))
        {
            if (_licenseService.DeleteLicense(LicenseToDelete.FileName))
            {
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Notification.DeleteSuccess"]);
                RefreshHistory();
            }
        }
        CancelDelete();
    }

    [RelayCommand]
    private void CancelDelete()
    {
        IsConfirmDeleteOpen = false;
        LicenseToDelete = null;
    }
}
