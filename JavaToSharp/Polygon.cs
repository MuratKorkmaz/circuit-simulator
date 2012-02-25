using System.Collections.Generic;
using System.Drawing;

namespace JavaToSharp
{
    class Polygon
    {
        //private List<PointF> Points { get; set; }
        public List<PointF> Points { get; private set; }
        public List<int> xpoints
        {
            get
            {
                var xs = new List<int>();
                foreach (var point in Points)
                {
                    xs.Add((int)point.X);
                }
                return xs;
            }
        }

        public List<int> ypoints
        {
            get
            {
                var ys = new List<int>();
                foreach (var point in Points)
                {
                    ys.Add((int)point.Y);
                }
                return ys;
            }
        }

        public int npoints { get; private set; } 

        public Polygon()
        {
            Points = new List<PointF>();
            npoints = 0;
        }

        public void addPoint(int x, int y)
        {
            Points.Add(new Point(x, y));
            npoints++;
        }
    }
}
