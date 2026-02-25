using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LicenseGenerator.Services;

public class LoggingService : ILoggingService
{
    private readonly string _logDirectory;
    private readonly object _lock = new object();

    public string LogDirectory => _logDirectory;

    public LoggingService()
    {
        _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LicenseGenerator", "Logs");
        
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    private string GetCurrentLogFilePath()
    {
        return Path.Combine(_logDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
    }

    public void LogInfo(string message)
    {
        WriteToFile("INFO", message);
    }

    public void LogDebug(string message)
    {
#if DEBUG
        WriteToFile("DEBUG", message);
#endif
    }

    public void LogWarning(string message)
    {
        WriteToFile("WARN", message);
    }

    public void LogError(string message, Exception? ex = null)
    {
        var fullMessage = message;
        if (ex != null)
        {
            fullMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
        }
        WriteToFile("ERROR", fullMessage);
    }

    private void WriteToFile(string level, string message)
    {
        lock (_lock)
        {
            try
            {
                var filePath = GetCurrentLogFilePath();
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(filePath, logEntry);
            }
            catch (Exception)
            {
                // Last resort: we can't log the logging failure to the file
                System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {message}");
            }
        }
    }

    public void CleanupOldLogs(int daysToKeep = 7)
    {
        try
        {
            var files = Directory.GetFiles(_logDirectory, "log_*.txt");
            var threshold = DateTime.Now.Date.AddDays(-daysToKeep);

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var datePart = fileName.Replace("log_", "");

                if (DateTime.TryParse(datePart, out DateTime fileDate))
                {
                    if (fileDate < threshold)
                    {
                        File.Delete(file);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Error during log cleanup", ex);
        }
    }
}
