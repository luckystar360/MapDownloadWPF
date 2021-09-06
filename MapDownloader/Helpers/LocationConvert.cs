using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader.Helpers
{
    public class LocationConvert
    {
        public static void ConvertXYtoLatLon(int _zoom, out double _lat, out double _lon, double _xtile, double _ytile)
        {
            double n = Math.Pow(2, _zoom);
            _lon = (_xtile * 360.0) / n - 180.0;
            double lat_rad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * _ytile / n)));
            _lat = lat_rad * 180.0 / Math.PI;
        }

        public static void ConverLatLontoXY(int _zoom, double _lat, double _lon, out double _xtile, out double _ytile)
        {
            double n = Math.Pow(2, _zoom);
            _xtile = (n * ((_lon + 180) / 360));
            _ytile = (n * (1 - (Math.Log(Math.Tan(_lat * Math.PI / 180.0) + 1 / Math.Cos(_lat * Math.PI / 180.0)) / Math.PI)) / 2);
        }
    }
}
