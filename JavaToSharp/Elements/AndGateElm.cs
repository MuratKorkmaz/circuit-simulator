using System;
using System.Drawing;
using JavaToSharp.Elements;

namespace JavaToSharp
{
    internal class AndGateElm : GateElm
    {
        protected AndGateElm(int xx, int yy) : base(xx, yy)
        {
        }

        protected AndGateElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal override void setPoints()
        {
            base.setPoints();

            // 0=topleft, 1-10 = top curve, 11 = right, 12-21=bottom curve,
            // 22 = bottom left
            Point[] triPoints = newPointArray(23);
            interpPoint2(lead1, lead2, triPoints[0], triPoints[22], 0, hs2);
            int i;
            for (i = 0; i != 10; i++)
            {
                double a = i*.1;
                double b = Math.Sqrt(1-a*a);
                interpPoint2(lead1, lead2, triPoints[i+1], triPoints[21-i],.5+a/2, b*hs2);
            }

            triPoints[11] = lead2;
            if (isInverting)
            {
                pcircle = interpPoint(point1, point2,.5+(ww+4)/dn);
                lead2 = interpPoint(point1, point2,.5+(ww+8)/dn);
            }
            gatePoly = createPolygon(triPoints);
        }
        internal override string GateName
        {
            get
            {
                return "элемент И";
            }
        }
        internal override bool calcFunction()
        {
            int i;
            bool f = true;
            for (i = 0; i != inputCount; i++)
                f &= getInput(i);
            return f;
        }
        internal override int DumpType
        {
            get
            {
                return 150;
            }
        }
    }
}
