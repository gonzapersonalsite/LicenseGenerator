using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace LicenseGenerator.Converters;

public class LocalizationConverter : IValueConverter, IMultiValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return GetTranslation(value as string);
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count > 0 && values[0] is string key)
        {
            return GetTranslation(key);
        }
        return string.Empty;
    }

    private string GetTranslation(string? key)
    {
        if (!string.IsNullOrEmpty(key) && Application.Current != null)
        {
            if (Application.Current.Resources.TryGetResource(key, null, out var resource) && resource is string translated)
            {
                return translated;
            }
            return $"[{key}]";
        }
        return key ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
