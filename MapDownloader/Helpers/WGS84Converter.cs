using MapControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MapDownloader.Helpers
{
    public class WGS84Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value is float && float.IsNaN((float)value)))
            {
                return "N/A";
            }
            else if (value != null && value is Location)
            {
                return string.Format("Mouse: {0:0.000000} , {1:0.000000}", ((Location)value).Latitude, ((Location)value).Longitude);
            }
            else if(value != null && value is double)
            {
                return string.Format("{0:0.000000}", value);
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
            //throw new NotImplementedException("WGS84Converter, ConvertBack not implemented");
        }
    }
}
