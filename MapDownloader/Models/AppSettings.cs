using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader.Models
{
    public class AppSettings
    {
        public string StringSetting { get; set; }
        public int IntegerSetting { get; set; }
        public bool BooleanSetting { get; set; }
        public string MapProviderName { get; set; }

    }
}
