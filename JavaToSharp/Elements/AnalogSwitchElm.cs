using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class AnalogSwitchElm : CircuitElm
    {
        internal const int FLAG_INVERT = 1;
        private double resistance;
        internal double r_on, r_off;

        protected AnalogSwitchElm(int xx, int yy) : base(xx, yy)
        {
            r_on = 20;
            r_off = 1e10;
        }

        protected AnalogSwitchElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            r_on = 20;
            r_off = 1e10;
            try
            {
                r_on = new (double)double?(st.nextToken());
                r_off = new (double)double?(st.nextToken());
            }
            catch (Exception e)
            {
            }

        }
        internal override string dump()
        {
            return base.dump() + " " + r_on + " " + r_off;
        }

        internal override int DumpType
        {
            get
            {
                return 159;
            }
        }
        internal bool open;

        private Point ps;
        private Point point3;
        private Point lead3;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            ps = new Point();
            const int openhs = 16;
            point3 = interpPoint(point1, point2,.5, -openhs);
            lead3 = interpPoint(point1, point2,.5, -openhs/2);
        }

        internal override void draw(Graphics g)
        {
            const int openhs = 16;
            int hs = (open) ? openhs : 0;
            setBbox(point1, point2, openhs);

            draw2Leads(g);

            g.Color = lightGrayColor;
            interpPoint(lead1, lead2, ps, 1, hs);
            drawThickLine(g, lead1, ps);

            setVoltageColor(g, volts[2]);
            drawThickLine(g, point3, lead3);

            if (!open)
                doDots(g);
            drawPosts(g);
        }

        protected override void calculateCurrent()
        {
            current = (volts[0]-volts[1])/resistance;
        }

        // we need this to be able to change the matrix for each step
        internal override bool nonLinear()
        {
            return true;
        }

        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal override void doStep()
        {
            open = (volts[2] < 2.5);
            if ((flags & FLAG_INVERT) != 0)
                open = !open;
            resistance = (open) ? r_off : r_on;
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void drag(int xx, int yy)
        {
            xx = sim.snapGrid(xx);
            yy = sim.snapGrid(yy);
            if (abs(x-xx) < abs(y-yy))
                xx = x;
            else
                yy = y;
            int q1 = abs(x-xx)+abs(y-yy);
            int q2 = (q1/2) % sim.gridSize;
            if (q2 != 0)
                return;
            x2 = xx;
            y2 = yy;
            setPoints();
        }
        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : (n == 1) ? point2 : point3;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "аналоговый выключатель";
            arr[1] = open ? "разомкн." : "замкн.";
            arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
            arr[3] = "I = " + getCurrentDText(Current);
            arr[4] = "Vc = " + getVoltageText(volts[2]);
        }
        // we have to just assume current will flow either way, even though that
        // might cause singular matrix errors
        internal override bool getConnection(int n1, int n2)
        {
            if (n1 == 2 || n2 == 2)
                return false;
            return true;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new Checkbox("Нормально замкнутый", (flags & FLAG_INVERT) != 0);
                return ei;
            }
            if (n == 1)
                return new EditInfo("Сопротивление во вкл. состоянии (Ом)", r_on, 0, 0);
            if (n == 2)
                return new EditInfo("Сопротивление в выкл. состоянии (Ом)", r_off, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                flags = (ei.checkbox.State) ? (flags | FLAG_INVERT) : (flags & ~FLAG_INVERT);
            if (n == 1 && ei.value > 0)
                r_on = ei.value;
            if (n == 2 && ei.value > 0)
                r_off = ei.value;
        }
    }
}

