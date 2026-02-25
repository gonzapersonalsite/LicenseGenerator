using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using Avalonia;
using Avalonia.Styling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseGenerator.ViewModels;

public record SettingsOption<T>(T Value, string DisplayKey);

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILanguageService _languageService;
    private readonly IDataService _dataService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;
    private readonly ILoggingService _loggingService;
    private readonly IShortcutService _shortcutService;

    public ILanguageService LanguageService => _languageService;

    [ObservableProperty]
    private SettingsOption<string> _selectedTheme = default!;

    [ObservableProperty]
    private SettingsOption<double> _selectedFontSize = default!;

    [ObservableProperty]
    private SettingsOption<string> _selectedLanguage = default!;

    [ObservableProperty]
    private bool _isResetConfirmOpen;

    [ObservableProperty]
    private bool _isImportConfirmOpen;

    public ObservableCollection<SettingsOption<string>> Themes { get; } = new();
    public ObservableCollection<SettingsOption<double>> FontSizes { get; } = new();
    public ObservableCollection<SettingsOption<string>> Languages { get; } = new();

    public SettingsViewModel(
        ISettingsService settingsService,
        ILanguageService languageService,
        IDataService dataService,
        IDialogService dialogService,
        INotificationService notificationService,
        ILoggingService loggingService,
        IShortcutService shortcutService)
    {
        _settingsService = settingsService;
        _languageService = languageService;
        _dataService = dataService;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _loggingService = loggingService;
        _shortcutService = shortcutService;

        InitializeOptions();
        LoadCurrentSettings();
    }

    private void InitializeOptions()
    {
        Themes.Clear();
        Themes.Add(new SettingsOption<string>("Light", "ThemeLight"));
        Themes.Add(new SettingsOption<string>("Dark", "ThemeDark"));
        Themes.Add(new SettingsOption<string>("System", "ThemeSystem"));

        FontSizes.Clear();
        FontSizes.Add(new SettingsOption<double>(0.8, "LangFontSizeSmall"));
        FontSizes.Add(new SettingsOption<double>(1.0, "LangFontSizeNormal"));
        FontSizes.Add(new SettingsOption<double>(1.2, "LangFontSizeLarge"));
        FontSizes.Add(new SettingsOption<double>(1.5, "LangFontSizeExtraLarge"));

        Languages.Clear();
        foreach (var langCode in _languageService.AvailableLanguages)
        {
            string labelKey = langCode switch
            {
                "es-ES" => "LangSpanish",
                "en-US" => "LangEnglish",
                "de-DE" => "LangGerman",
                "pt-BR" => "LangPortuguese",
                "zh-CN" => "LangChinese",
                _ => langCode // Fallback to the code itself for new languages
            };
            Languages.Add(new SettingsOption<string>(langCode, labelKey));
        }
    }

    private void LoadCurrentSettings()
    {
        SelectedTheme = Themes.FirstOrDefault(t => t.Value == _settingsService.AppTheme) ?? Themes[1]; // Default Dark
        SelectedFontSize = FontSizes.FirstOrDefault(f => f.Value == _settingsService.FontSizeScaling) ?? FontSizes[1]; // Default 1.0
        
        // Match the language from service (which already handled system/saved logic)
        SelectedLanguage = Languages.FirstOrDefault(l => l.Value == _languageService.CurrentLanguage) ?? Languages.First();
    }

    partial void OnSelectedThemeChanged(SettingsOption<string> value)
    {
        if (value == null) return;
        if (_settingsService.AppTheme != value.Value)
        {
            _settingsService.AppTheme = value.Value;
            ApplyTheme(value.Value);
        }
    }

    partial void OnSelectedFontSizeChanged(SettingsOption<double> value)
    {
        if (value == null) return;
        if (_settingsService.FontSizeScaling != value.Value)
        {
            _settingsService.FontSizeScaling = value.Value;
            if (Application.Current is App app)
            {
                app.ApplyFontSize(value.Value);
            }
        }
    }

    partial void OnSelectedLanguageChanged(SettingsOption<string> value)
    {
        if (value == null) return;
        if (_settingsService.CurrentLanguage != value.Value)
        {
            _settingsService.CurrentLanguage = value.Value;
            _languageService.SetLanguage(value.Value);
        }
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                "System" => ThemeVariant.Default,
                _ => ThemeVariant.Dark
            };
        }
    }

    [RelayCommand]
    private void ShowResetConfirm()
    {
        IsResetConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelReset()
    {
        IsResetConfirmOpen = false;
    }

    [RelayCommand]
    private void ConfirmReset()
    {
        // 1. Reset service
        _settingsService.ResetToDefaults();
        
        // 2. Refresh everything
        _languageService.SetLanguage(_settingsService.CurrentLanguage);
        ApplyTheme(_settingsService.AppTheme);
        if (Application.Current is App app)
        {
            app.ApplyFontSize(_settingsService.FontSizeScaling);
        }

        // 3. Update VM state
        LoadCurrentSettings();
        IsResetConfirmOpen = false;
        _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Settings.ResetSuccess"]);
    }

    [RelayCommand]
    private async Task ExportData()
    {
        var defaultName = $"LicenseGenerator_Backup_{DateTime.Now:yyyyMMdd}.zip";
        var path = await _dialogService.SaveFileAsync(
            _languageService["Backup.ExportTitle"],
            defaultName,
            new[] { "zip" });

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                await _dataService.ExportDataAsync(path);
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Backup.ExportSuccess"]);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Export error", ex);
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Backup.ExportError"]);
            }
        }
    }

    [RelayCommand]
    private void ShowImportConfirm()
    {
        IsImportConfirmOpen = true;
    }

    [RelayCommand]
    private void CancelImport()
    {
        IsImportConfirmOpen = false;
    }

    [RelayCommand]
    private async Task ConfirmImport()
    {
        IsImportConfirmOpen = false;
        
        var path = await _dialogService.OpenFileAsync(
            _languageService["Backup.ImportTitle"],
            new[] { "zip" });

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                await _dataService.ImportDataAsync(path);
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Backup.ImportSuccess"]);
                
                // Restart app to apply changes safely
                await Task.Delay(4000); // Give enough time for the user to read the success message
                System.Diagnostics.Process.Start(Environment.ProcessPath!);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Import error", ex);
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Backup.ImportError"]);
            }
        }
    }

    [RelayCommand]
    private void CreateShortcut()
    {
        try
        {
            var exePath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exePath)) return;

            var success = _shortcutService.CreateDesktopShortcut(
                "License Generator", 
                exePath, 
                _languageService["SettingsShortcutDescription"]);

            if (success)
            {
                _notificationService.ShowSuccess(_languageService["CommonSuccess"], _languageService["Notification.ShortcutCreated"]);
            }
            else
            {
                _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.ShortcutError"]);
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error in CreateShortcut command", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.ShortcutError"]);
        }
    }

    [RelayCommand]
    private void OpenSupportLink()
    {
        try
        {
            var url = "https://buymeacoffee.com/gonzalomartinezgarcia";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error opening support link", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Support.OpenError"]);
        }
    }

    [RelayCommand]
    private void OpenRepositoryLink()
    {
        try
        {
            var url = "https://github.com/gonzapersonalsite/LicenseGenerator";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error opening repository link", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Repository.OpenError"]);
        }
    }

    [RelayCommand]
    private void OpenLogsFolder()
    {
        try
        {
            var logDir = _loggingService.LogDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string resolvedPath = ResolveActualPath(logDir);
                _loggingService.LogDebug($"Opening logs resolved path: {resolvedPath}");
                Process.Start(new ProcessStartInfo { FileName = resolvedPath, UseShellExecute = true, Verb = "open" });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start(new ProcessStartInfo("xdg-open", logDir) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Error opening logs folder", ex);
            _notificationService.ShowError(_languageService["CommonError"], _languageService["Notification.OpenFolderError"]);
        }
    }

    private string ResolveActualPath(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (path.StartsWith(localAppData, StringComparison.OrdinalIgnoreCase))
                {
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
                        if (path.Contains("Packages", StringComparison.OrdinalIgnoreCase) && path.Contains("LocalCache", StringComparison.OrdinalIgnoreCase))
                        {
                            return path;
                        }

                        string packagesPath = Path.Combine(localAppData, "Packages");
                        if (Directory.Exists(packagesPath))
                        {
                            var dirs = Directory.GetDirectories(packagesPath, "*LicenseGenerator*");
                            if (dirs.Length > 0)
                            {
                                string subPath = string.Join(Path.DirectorySeparatorChar.ToString(), parts.Skip(licenseGenIndex + 1));
                                string actualPath = Path.Combine(dirs[0], "LocalCache", "Local", "LicenseGenerator", subPath);
                                if (Directory.Exists(actualPath) || File.Exists(actualPath))
                                {
                                    return actualPath;
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Fallback */ }
        }
        return path;
    }
}
