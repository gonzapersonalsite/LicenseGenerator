using System.Collections.ObjectModel;
using LicenseGenerator.ViewModels;

namespace LicenseGenerator.Services;

public interface INotificationService
{
    ObservableCollection<NotificationViewModel> Notifications { get; }
    void Show(string title, string message, NotificationType type, int durationSeconds = 4);
    void ShowSuccess(string title, string message);
    void ShowError(string title, string message);
    void ShowWarning(string title, string message);
    void ShowInfo(string title, string message);
}
