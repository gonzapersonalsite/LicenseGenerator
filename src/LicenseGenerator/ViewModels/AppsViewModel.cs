using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

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
    private readonly ILoggingService _loggingService;
    private List<AppItemInfo>_allApps = new();

    [ObservableProperty]
    private string _newAppId = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isConfirmDeleteOpen;

    [ObservableProperty]
    private string? _appToDelete;

    public ObservableCollection<AppItemInfo> FilteredApps { get; } = new();

    public AppsViewModel(
        ILicenseGeneratorService licenseService, 
        ILanguageService languageService, 
        INotificationService notificationService,
        ILoggingService loggingService)
    {
        _licenseService = licenseService;
        _languageService = languageService;
        _notificationService = notificationService;
        _loggingService = loggingService;
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

    private string ResolveActualPath(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                // In MSIX, AppData/Local is redirected to a virtual folder.
                // External processes like explorer.exe cannot access the virtual path (C:\Users\...\AppData\Local\...)
                // because it doesn't physically exist in the global namespace.
                // We must provide the "real" path inside the Packages folder.
                
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (path.StartsWith(localAppData, StringComparison.OrdinalIgnoreCase))
                {
                    // Attempt to find if we are running in a package context
                    // MSIX AppData is usually at %LOCALAPPDATA%\Packages\<PackageId>\LocalCache\Local\...
                    // We can try to find the actual directory by looking at the parent chain or known patterns.
                    
                    var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    var licenseGenIndex = -1;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Equals("LicenseGenerator", StringComparison.OrdinalIgnoreCase))
                        {
                            licenseGenIndex = i;
                            break;
                        }
                    }

                    if (licenseGenIndex != -1)
                    {
                        // Check if we are already in a Packages path (to avoid double mapping)
                        if (path.Contains("Packages", StringComparison.OrdinalIgnoreCase) && path.Contains("LocalCache", StringComparison.OrdinalIgnoreCase))
                        {
                            return path; 
                        }

                        // Try to find the package folder in AppData\Local\Packages
                        string packagesPath = Path.Combine(localAppData, "Packages");
                        if (Directory.Exists(packagesPath))
                        {
                            // Search for our package (gonzalo.dev.LicenseGenerator)
                            // The ID in manifest is "gonzalo.dev.LicenseGenerator"
                            var dirs = Directory.GetDirectories(packagesPath, "*LicenseGenerator*");
                            if (dirs.Length > 0)
                            {
                                // Join the rest of the path after "LicenseGenerator"
                                string subPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts.Skip(licenseGenIndex + 1));
                                string actualPath = Path.Combine(dirs[0], "LocalCache", "Local", "LicenseGenerator", subPath);
                                
                                if (Directory.Exists(actualPath) || File.Exists(actualPath))
                                {
                                    _loggingService.LogDebug($"Resolved MSIX Path: {actualPath}");
                                    return actualPath;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogDebug($"MSIX path resolution failed: {ex.Message}");
            }
        }
        return path;
    }

    [RelayCommand]
    private async Task CopyPublicKey(string appId)
    {
        try
        {
            string publicKey = _licenseService.GetPublicKey(appId);
            if (string.IsNullOrEmpty(publicKey))
            {
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.OpenFolderError"]);
                return;
            }

            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                var clipboard = Avalonia.Controls.TopLevel.GetTopLevel(desktop.MainWindow)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(publicKey);
                    _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Notification.CopySuccess"]);
                }
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Error copying public key for app {appId}", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.OpenFolderError"]);
        }
    }

    [RelayCommand]
    private void OpenKeyFolder(string appId)
    {
        try
        {
            string keysDir = _licenseService.GetAppKeysDirectory(appId);
            _loggingService.LogInfo($"Attempting to open keys folder: {keysDir}");

            if (!Directory.Exists(keysDir))
            {
                _loggingService.LogWarning($"Keys folder does not exist: {keysDir}");
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.OpenFolderError"]);
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string resolvedPath = ResolveActualPath(keysDir);
                _loggingService.LogDebug($"Opening resolved path: {resolvedPath}");

                Process.Start(new ProcessStartInfo
                {
                    FileName = resolvedPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start(new ProcessStartInfo("xdg-open", keysDir) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start(new ProcessStartInfo("open", keysDir) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Error opening keys folder for app {appId}", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.OpenFolderError"]);
        }
    }
}