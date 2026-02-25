using System;

namespace LicenseGenerator.Services;

public interface ILoggingService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? ex = null);
    void LogDebug(string message);
    string LogDirectory { get; }
    void CleanupOldLogs(int daysToKeep = 7);
}
