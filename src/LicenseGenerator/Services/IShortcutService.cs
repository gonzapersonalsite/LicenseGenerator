using System;

namespace LicenseGenerator.Services;

public interface IShortcutService
{
    /// <summary>
    /// Creates a desktop shortcut for the application.
    /// </summary>
    /// <param name="shortcutName">The display name of the shortcut (e.g., "License Generator").</param>
    /// <param name="targetPath">The full path to the executable.</param>
    /// <param name="description">An optional description for the shortcut.</param>
    /// <returns>True if created successfully, false otherwise.</returns>
    bool CreateDesktopShortcut(string shortcutName, string targetPath, string? description = null);
}
