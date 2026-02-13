using System.Collections.ObjectModel;
using Avalonia.Threading;
using LicenseGenerator.ViewModels;

namespace LicenseGenerator.Services;

public class NotificationService : INotificationService
{
    public ObservableCollection<NotificationViewModel> Notifications { get; } = new();

    public void Show(string title, string message, NotificationType type, int durationSeconds = 4)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var notification = new NotificationViewModel(title, message, type, Remove, durationSeconds);
            Notifications.Add(notification);
        });
    }

    private void Remove(NotificationViewModel notification)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (Notifications.Contains(notification))
            {
                Notifications.Remove(notification);
            }
        });
    }

    public void ShowSuccess(string title, string message) => Show(title, message, NotificationType.Success);
    public void ShowError(string title, string message) => Show(title, message, NotificationType.Error, 5);
    public void ShowWarning(string title, string message) => Show(title, message, NotificationType.Warning);
    public void ShowInfo(string title, string message) => Show(title, message, NotificationType.Info);
}
