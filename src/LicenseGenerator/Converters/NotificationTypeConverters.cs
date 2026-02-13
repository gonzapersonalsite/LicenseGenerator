using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LicenseGenerator.Services;
using System;
using System.Globalization;

namespace LicenseGenerator.Converters;

public class NotificationTypeToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => GetBrush("ColorSuccess", Brushes.Green),
                NotificationType.Error => GetBrush("ColorDanger", Brushes.Red),
                NotificationType.Warning => GetBrush("ColorWarning", Brushes.Orange), 
                NotificationType.Info => GetBrush("ColorPrimary", Brushes.Blue),
                _ => Brushes.Gray
            };
        }
        return Brushes.Gray;
    }

    private IBrush GetBrush(string key, IBrush fallback)
    {
        if (Application.Current != null && 
            Application.Current.TryGetResource(key, Application.Current.ActualThemeVariant, out var resource) && 
            resource is IBrush brush)
        {
            return brush;
        }
        return fallback;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NotificationTypeToIconConverter : IValueConverter
{
    // Material Design Icons paths
    private const string SuccessIcon = "M12 2C6.5 2 2 6.5 2 12S6.5 22 12 22 22 17.5 22 12 17.5 2 12 2M10 17L5 12L6.41 10.59L10 14.17L17.59 6.58L19 8L10 17Z";
    private const string ErrorIcon = "M12 2C17.53 2 22 6.47 22 12C22 17.53 17.53 22 12 22C6.47 22 2 17.53 2 12C2 6.47 6.47 2 12 2M15.59 7L12 10.59L8.41 7L7 8.41L10.59 12L7 15.59L8.41 17L12 13.41L15.59 17L17 15.59L13.41 12L17 8.41L15.59 7Z";
    private const string WarningIcon = "M12 2L1 21H23M12 6L19.53 19H4.47M11 10V14H13V10M11 16V18H13V16";
    private const string InfoIcon = "M11,9H13V7H11M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M11,17H13V11H11V17Z";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
         if (value is NotificationType type)
        {
            var path = type switch
            {
                NotificationType.Success => SuccessIcon,
                NotificationType.Error => ErrorIcon,
                NotificationType.Warning => WarningIcon,
                NotificationType.Info => InfoIcon,
                _ => InfoIcon
            };
            return StreamGeometry.Parse(path);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
