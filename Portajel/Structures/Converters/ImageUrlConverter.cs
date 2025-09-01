using System;
using System.Globalization;
using Microsoft.Maui.Devices; // Add this for DeviceDisplay

namespace Portajel.Structures.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        private double density = 1.0;

        public ImageUrlConverter()
        {
            try
            {
                density = DeviceDisplay.MainDisplayInfo.Density;
            }
            catch
            {
                // Fallback to 1.0 if not available
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
            {
                int size = 64;

                if (parameter is string sizeParam && int.TryParse(sizeParam, out int parsedSize))
                {
                    size = parsedSize;
                }

                int pixelSize = (int)Math.Round(size * density);
                return $"{url}?&fillHeight={pixelSize}&fillHeight={pixelSize}&quality=96";
            }

            return value; // Return the original value if it's not a valid string
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // ConvertBack is not needed in this case
        }
    }
}