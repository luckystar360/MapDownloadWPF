using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MapDownloader.Services.MapCache
{
    public abstract class ITileDownload
    {
        public delegate void TimeOutDelegate();
        public event TimeOutDelegate TimeOutCallback;
        public ITileDownload()
        {

        }
        ~ITileDownload() { }

        public abstract ImageSource Download(Uri uri, string userAgent=null);
    }
}
