using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace LicenseGenerator.Converters;

public class LocalizationFormatConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 1 || values[0] is not string key)
            return string.Empty;

        if (Application.Current != null && 
            Application.Current.Resources.TryGetResource(key, null, out var resource) && 
            resource is string format)
        {
            try
            {
                var args = values.Skip(1).ToArray();
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        return $"[{key}]";
    }
}
