using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader.Models
{
    public class Polyline
    {
        public String Name { get; set; }
        private LocationCollection _locations;
        public LocationCollection Locations { get; set; }
    }
}
