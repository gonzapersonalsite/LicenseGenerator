using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace LicenseGenerator.Converters;

public class ResourceKeyToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string key && Application.Current != null)
        {
            if (Application.Current.TryGetResource(key, Application.Current.ActualThemeVariant, out var resource))
            {
                return resource;
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
