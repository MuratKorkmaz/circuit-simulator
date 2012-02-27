using System.Drawing;

namespace JavaToSharp
{
    internal class JfetElm : MosfetElm
    {
        internal JfetElm(int xx, int yy, bool pnpflag) : base(xx, yy, pnpflag)
        {
            noDiagonal = true;
        }
        public JfetElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            noDiagonal = true;
        }

        internal Polygon gatePoly;
        internal new Polygon arrowPoly;
        internal Point gatePt;

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, hs);
           voltageColor = setVoltageColor(g, volts[1]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,src[0], src[1]);
            drawThickLine(g, myPen,src[1], src[2]);
         voltageColor=   setVoltageColor(g, volts[2]);
         myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,drn[0], drn[1]);
            drawThickLine(g, myPen,drn[1], drn[2]);
          voltageColor =  setVoltageColor(g, volts[0]);
          myPen = new Pen(voltageColor);
            myBrush = new SolidBrush(voltageColor);
            drawThickLine(g, myPen ,point1, gatePt);
            g.FillPolygon(myBrush ,arrowPoly.Points.ToArray());
            g.FillPolygon(myBrush,gatePoly.Points.ToArray());
            curcount = updateDotCount(-ids, curcount);
            if (curcount != 0)
            {
                drawDots(g, src[0], src[1], curcount);
                drawDots(g, src[1], src[2], curcount+8);
                drawDots(g, drn[0], drn[1], -curcount);
                drawDots(g, drn[1], drn[2], -(curcount+8));
            }
            drawPosts(g);
        }
        internal override void setPoints()
        {
            base.setPoints();

            // find the coordinates of the various points we need to draw
            // the JFET.
            int hs2 = hs*dsign;
            src = newPointArray(3);
            drn = newPointArray(3);
            interpPoint2(point1, point2, src[0], drn[0], 1, hs2);
            interpPoint2(point1, point2, src[1], drn[1], 1, hs2/2);
            interpPoint2(point1, point2, src[2], drn[2], 1-10/dn, hs2/2);

            gatePt = interpPoint(point1, point2, 1-14/dn);

            Point[] ra = newPointArray(4);
            interpPoint2(point1, point2, ra[0], ra[1], 1-13/dn, hs);
            interpPoint2(point1, point2, ra[2], ra[3], 1-10/dn, hs);
            gatePoly = createPolygon(ra[0], ra[1], ra[3], ra[2]);
            if (pnp == -1)
            {
                Point x = interpPoint(gatePt, point1, 18/dn);
                arrowPoly = calcArrow(gatePt, x, 8, 3);
            }
            else
                arrowPoly = calcArrow(point1, gatePt, 8, 3);
        }
        internal override int DumpType
        {
            get
            {
                return 'j';
            }
        }
        // these values are taken from Hayes+Horowitz p155
        internal override double DefaultThreshold
        {
            get
            {
                return -4;
            }
        }
        internal override double Beta
        {
            get
            {
                return.00125;
            }
        }
        internal override void getInfo(string[] arr)
        {
            getFetInfo(arr, "JFET");
        }
    }
}
