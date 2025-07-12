using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace Portajel.Desktop.Components;

public class FileUriToBitmapConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        // value may be a string or a Uri
        try
        {
            if (value is string s && Uri.TryCreate(s, UriKind.Absolute, out var u1) && u1.IsFile)
            {
                return new Bitmap(u1.LocalPath);
            }

            if (value is Uri u2 && u2.IsFile)
            {
                return new Bitmap(u2.LocalPath);
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}