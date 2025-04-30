using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
            {
                // Default size
                int size = 64;

                // Check if a parameter is provided and parse it
                if (parameter is string sizeParam && int.TryParse(sizeParam, out int parsedSize))
                {
                    size = parsedSize;
                }

                // Append the query string to the URL
                return $"{url}?&maxWidth={size}&maxHeight={size}&width={size}&height={size}";
            }

            return value; // Return the original value if it's not a valid string
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // ConvertBack is not needed in this case
        }
    }

}
