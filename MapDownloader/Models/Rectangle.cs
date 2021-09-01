using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDownloader.Models
{
    public class Rectangle
    {
        public string Name { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Width { get { return Math.Abs(X1 - X2); } }
        public double Height { get { return Math.Abs(Y1 - Y2); } }

    }
}
