using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Diagnostics;

namespace LicenseGenerator.Services;

public class ShortcutService : IShortcutService
{
    private readonly ILoggingService _loggingService;

    public ShortcutService(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    public bool CreateDesktopShortcut(string shortcutName, string targetPath, string? description = null)
    {
        try
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (string.IsNullOrEmpty(desktopPath))
            {
                // Fallback for some Linux environments
                desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop");
            }

            if (!Directory.Exists(desktopPath))
            {
                _loggingService.LogError($"Desktop path not found: {desktopPath}");
                return false;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CreateWindowsShortcut(desktopPath, shortcutName, targetPath, description);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CreateLinuxShortcut(desktopPath, shortcutName, targetPath, description);
            }

            _loggingService.LogWarning($"Shortcut creation not supported on this platform: {RuntimeInformation.OSDescription}");
            return false;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Unexpected error in CreateDesktopShortcut", ex);
            return false;
        }
    }

    private bool CreateWindowsShortcut(string desktopPath, string shortcutName, string targetPath, string? description)
    {
        try
        {
            var shortcutPath = Path.Combine(desktopPath, $"{shortcutName}.lnk");
            
            // Native Windows API (IShellLink) implementation
            var link = (IShellLinkW)new ShellLink();
            link.SetPath(targetPath);
            link.SetDescription(description ?? "");
            link.SetWorkingDirectory(Path.GetDirectoryName(targetPath) ?? "");

            var persistFile = (IPersistFile)link;
            persistFile.Save(shortcutPath, true);

            _loggingService.LogInfo($"Native Windows shortcut created: {shortcutPath}");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to create native Windows shortcut using IShellLink", ex);
            return false;
        }
    }

    private bool CreateLinuxShortcut(string desktopPath, string shortcutName, string targetPath, string? description)
    {
        try
        {
            var fileName = shortcutName.ToLower().Replace(" ", "-") + ".desktop";
            var shortcutPath = Path.Combine(desktopPath, fileName);
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app.png");

            var content = $"""
                [Desktop Entry]
                Name={shortcutName}
                Comment={description ?? ""}
                Exec="{targetPath}"
                Icon={iconPath}
                Type=Application
                Terminal=false
                Categories=Development;
                """;

            File.WriteAllText(shortcutPath, content);
            
            // Make it executable
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{shortcutPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                })?.WaitForExit();
            }
            catch { /* Ignore chmod errors */ }

            _loggingService.LogInfo($"Linux shortcut created: {shortcutPath}");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to create Linux .desktop shortcut", ex);
            return false;
        }
    }

    #region Windows COM Interop Definitions

    [ComImport]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellLinkW
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport]
    [Guid("0000010b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPersistFile
    {
        void GetClassID(out Guid pClassID);
        [PreserveSig] int IsDirty();
        void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);
        void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
        void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
        void GetCurFile([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ppszFileName);
    }

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    private class ShellLink { }

    #endregion
}
