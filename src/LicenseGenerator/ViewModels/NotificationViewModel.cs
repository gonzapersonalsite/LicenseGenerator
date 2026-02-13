using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LicenseGenerator.Services;
using System;
using System.Threading.Tasks;

namespace LicenseGenerator.ViewModels;

public partial class NotificationViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private NotificationType _type;

    private readonly Action<NotificationViewModel> _onClose;
    private readonly int _durationSeconds;

    public NotificationViewModel(string title, string message, NotificationType type, Action<NotificationViewModel> onClose, int durationSeconds = 3)
    {
        Title = title;
        Message = message;
        Type = type;
        _onClose = onClose;
        _durationSeconds = durationSeconds;

        StartTimer();
    }

    private async void StartTimer()
    {
        await Task.Delay(TimeSpan.FromSeconds(_durationSeconds));
        Close();
    }

    [RelayCommand]
    public void Close()
    {
        _onClose?.Invoke(this);
    }
}
