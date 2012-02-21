using System.Collections.Generic;

namespace JavaToSharp
{
    class Polygon
    {
        public List<int> xpoints { get; private set; }
        public List<int> ypoints { get; private set; }
        public int npoints { get; private set; } 

        public Polygon()
        {
            xpoints = new List<int>();
            ypoints = new List<int>();
            npoints = 0;
        }

        public void addPoint(int x, int y)
        {
            xpoints.Add(x);
            ypoints.Add(y);
            npoints++;
        }
    }
}
