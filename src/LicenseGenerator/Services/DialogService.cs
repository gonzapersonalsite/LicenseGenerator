using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace LicenseGenerator.Services;

public class DialogService : IDialogService
{
    private Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    public async Task<string?> SaveFileAsync(string title, string defaultFileName, string[] extensions)
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Backup Files")
                {
                    Patterns = extensions.Select(e => $"*.{e}").ToList()
                }
            }
        });

        return file?.Path.LocalPath;
    }

    public async Task<string?> OpenFileAsync(string title, string[] extensions)
    {
        var window = GetMainWindow();
        if (window == null) return null;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Backup Files")
                {
                    Patterns = extensions.Select(e => $"*.{e}").ToList()
                }
            }
        });

        return files?.FirstOrDefault()?.Path.LocalPath;
    }

    public Task<bool> ShowConfirmationDialogAsync(string title, string message, string? confirmText = null, string? cancelText = null)
    {
        // For LicenseGenerator, we are using the overlay-based confirmation in the ViewModels
        // such as IsResetConfirmOpen in SettingsViewModel.
        // This method can be implemented if we need a standard MessageBox, 
        // but for now the overlay pattern is preferred for consistency with the existing code.
        return Task.FromResult(true); 
    }
}
