using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapDownloader.Services.MapCache
{
    public class HttpTileDownload : ITileDownload
    {
        public bool IsAsync = true;
        public override ImageSource Download(Uri uri, string userAgent)
        {
            ImageSource image = null;
            try
            {
                if (IsAsync)
                {
                    var request = WebRequest.CreateHttp(uri);
                    if (userAgent != null)
                    {
                        request.UserAgent = userAgent;
                    }
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        using (var memoryStream = new MemoryStream())
                        {
                            responseStream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            image = FromStream(memoryStream);
                        }
                    }
                }
                else
                {
                    image = new BitmapImage(uri);
                }
            }
            catch(WebException ex)
            {
                Debug.WriteLine("{0}: {1}: {2}", uri, ex.Status, ex.Message);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("{0}: {1}", uri, ex.Message);
            }
            return image;
        }
        
        private BitmapSource FromStream(Stream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;

        }
    }
}
