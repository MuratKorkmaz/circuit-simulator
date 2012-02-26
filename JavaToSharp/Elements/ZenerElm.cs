// Zener code contributed by J. Mike Rollins
// http://www.camotruck.net/rollins/simulator.html

using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class ZenerElm : DiodeElm
    {
        public ZenerElm(int xx, int yy) : base(xx, yy)
        {
            zvoltage = default_zvoltage;
            setup();
        }
        public ZenerElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            string sZvoltage = st.nextToken();
            bool isParsed = double.TryParse(sZvoltage, out zvoltage);
            if (!isParsed)
            {
               throw new Exception("Не удалось привести к типу double");
            }
            setup();
        }
        internal override void setup()
        {
            diode.leakage = 5e-6; // 1N4004 is 5.0 uAmp
            base.setup();
        }
        internal override int DumpType
        {
            get
            {
                return 'z';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + zvoltage;
        }

        private readonly new int hs = 8;
        private new Polygon poly;
        private new Point[] cathode;
        private Point[] wing;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(16);
            cathode = newPointArray(2);
            wing = newPointArray(2);
            Point[] pa = newPointArray(2);
            interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
            interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
            interpPoint(cathode[0], cathode[1], wing[0], -0.2, -hs);
            interpPoint(cathode[1], cathode[0], wing[1], -0.2, -hs);
            poly = createPolygon(pa[0], pa[1], lead2);
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, hs);

            double v1 = volts[0];
            double v2 = volts[1];

            draw2Leads(g);

            

            // draw thing arrow is pointing to
            voltageColor = setVoltageColor(g, v2);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,cathode[0], cathode[1]);

            // draw wings on cathode
            drawThickLine(g, myPen,wing[0], cathode[0]);
            drawThickLine(g, myPen,wing[1], cathode[1]);

            doDots(g);
            drawPosts(g);
        }

       

        private const double default_zvoltage = 5.6;

        internal override void getInfo(string[] arr)
        {
            base.getInfo(arr);
            arr[0] = "Стабилитрон";
            arr[5] = "Vz = " + getVoltageText(zvoltage);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Прямое падение напряжения @ 1A", fwdrop, 10, 1000);
            if (n == 1)
                return new EditInfo("Напряжение стабилизации @ 5mA", zvoltage, 1, 25);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                fwdrop = ei.value;
            if (n == 1)
                zvoltage = ei.value;
            setup();
        }
    }
}
