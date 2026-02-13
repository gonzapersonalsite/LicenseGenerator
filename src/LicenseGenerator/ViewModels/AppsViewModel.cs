using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseGenerator.ViewModels;

public partial class AppItemInfo : ObservableObject
{
    public required string Name { get; init; }
    public int LicenseCount { get; init; }
    public string DisplayText => $"{Name} ({LicenseCount} {(LicenseCount == 1 ? "licencia" : "licencias")})";
}

public partial class AppsViewModel : PaginatedViewModelBase
{
    private readonly ILicenseGeneratorService _licenseService;
    private readonly ILanguageService _languageService;
    private readonly INotificationService _notificationService;
    private List<AppItemInfo> _allApps = new();

    [ObservableProperty]
    private string _newAppId = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isConfirmDeleteOpen;

    [ObservableProperty]
    private string? _appToDelete;

    public ObservableCollection<AppItemInfo> FilteredApps { get; } = new();

    public AppsViewModel(ILicenseGeneratorService licenseService, ILanguageService languageService, INotificationService notificationService)
    {
        _licenseService = licenseService;
        _languageService = languageService;
        _notificationService = notificationService;
        LoadApps();
    }

    private void LoadApps()
    {
        _allApps = _licenseService.GetAvailableApps()
                                  .Select(appId => new AppItemInfo 
                                  { 
                                      Name = appId, 
                                      LicenseCount = _licenseService.GetLicenseCountForApp(appId) 
                                  })
                                  .OrderBy(a => a.Name)
                                  .ToList();
        UpdateFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 1;
        UpdateFilter();
    }

    protected override void UpdateFilter()
    {
        var query = _allApps.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(search));
        }

        TotalCount = query.Count();

        var paged = query.Skip((CurrentPage - 1) * PageSize)
                         .Take(PageSize)
                         .ToList();

        FilteredApps.Clear();
        foreach (var app in paged)
        {
            FilteredApps.Add(app);
        }
    }

    [RelayCommand]
    private void CreateApp()
    {
        if (string.IsNullOrWhiteSpace(NewAppId))
        {
            _notificationService.ShowWarning(_languageService["CommonWarning"], _languageService["Notification.AppIdRequired"]);
            return;
        }

        if (_licenseService.CreateApp(NewAppId))
        {
            _notificationService.ShowSuccess(_languageService["CommonSuccess"], string.Format(_languageService["Notification.AppCreated"], NewAppId));
            LoadApps();
            NewAppId = string.Empty;
        }
        else
        {
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.AppCreateError"]);
        }
    }

    [RelayCommand]
    private void DeleteApp(string appId)
    {
        AppToDelete = appId;
        IsConfirmDeleteOpen = true;
    }

    [RelayCommand]
    private void ConfirmDelete()
    {
        if (!string.IsNullOrEmpty(AppToDelete))
        {
            string deletedApp = AppToDelete;
            if (_licenseService.DeleteApp(deletedApp))
            {
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], string.Format(_languageService["Notification.AppDeleted"], deletedApp));
                LoadApps();
            }
        }
        CancelDelete();
    }

    [RelayCommand]
    private void CancelDelete()
    {
        IsConfirmDeleteOpen = false;
        AppToDelete = null;
    }
}
