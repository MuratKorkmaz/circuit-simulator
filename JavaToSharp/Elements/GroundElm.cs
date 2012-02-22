using System.Drawing;

namespace JavaToSharp.Elements
{
    internal class GroundElm : CircuitElm
    {
        public GroundElm(int xx, int yy) : base(xx, yy)
        {
        }
        public GroundElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
        }
        internal override int DumpType
        {
            get
            {
                return 'g';
            }
        }
        internal override int PostCount
        {
            get
            {
                return 1;
            }
        }
        internal override void draw(Graphics g)
        {
            setVoltageColor(g, 0);
            drawThickLine(g, point1, point2);
            int i;
            for (i = 0; i != 3; i++)
            {
                int a = 10-i*4;
                int b = i*5; // -10;
                interpPoint2(point1, point2, ps1, ps2, 1+b/dn, a);
                drawThickLine(g, ps1, ps2);
            }
            doDots(g);
            interpPoint(point1, point2, ps2, 1+11.0/dn);
            setBbox(point1, ps2, 11);
            drawPost(g, x, y, nodes[0]);
        }
        internal override void setCurrent(int xp, double c)
        {
            current = -c;
        }
        internal override void stamp()
        {
            sim.stampVoltageSource(0, nodes[0], voltSource, 0);
        }
        internal override double VoltageDiff
        {
            get
            {
                return 0;
            }
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "заземление";
            arr[1] = "I = " + getCurrentText(Current);
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}
