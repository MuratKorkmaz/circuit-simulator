using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class SparkGapElm : CircuitElm
    {
        private double resistance;
        private double onresistance;
        private double offresistance;
        private double breakdown;
        private double holdcurrent;
        private bool state;
        public SparkGapElm(int xx, int yy) : base(xx, yy)
        {
            offresistance = 1e9;
            onresistance = 1e3;
            breakdown = 1e3;
            holdcurrent = 0.001;
            state = false;
        }
        public SparkGapElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sOnresistance = st.nextToken();
            bool isParsedOnresistance = double.TryParse(sOnresistance, out onresistance);
            if (!isParsedOnresistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sOffresistance = st.nextToken();
            bool isParsedOffresistance = double.TryParse(sOffresistance, out offresistance);
            if (!isParsedOffresistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sBreakdown = st.nextToken();
            bool isParsedBreakdown = double.TryParse(sBreakdown , out breakdown);
            if (!isParsedBreakdown)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sHoldcurrent = st.nextToken();
            bool isParsedHoldcurrent = double.TryParse(sHoldcurrent, out holdcurrent);
            if (!isParsedHoldcurrent)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override int DumpType
        {
            get
            {
                return 187;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + onresistance + " " + offresistance + " " + breakdown + " " + holdcurrent;
        }

        private Polygon arrow1;
        private Polygon arrow2;

        internal override void setPoints()
        {
            base.setPoints();
            const int dist = 16;
            const int alen = 8;
            calcLeads(dist+alen);
            Point p1 = interpPoint(point1, point2, (dn-alen)/(2*dn));
            arrow1 = calcArrow(point1, p1, alen, alen);
            p1 = interpPoint(point1, point2, (dn+alen)/(2*dn));
            arrow2 = calcArrow(point2, p1, alen, alen);
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, 8);
            draw2Leads(g);
            voltageColor = setVoltageColor(g, volts[0]);
            myBrush = new SolidBrush(voltageColor);
            g.FillPolygon(myBrush ,arrow1.Points.ToArray());
            voltageColor = setVoltageColor(g, volts[1]);
            myBrush = new SolidBrush(voltageColor);
            g.FillPolygon(myBrush,arrow2.Points.ToArray());
            if (state)
                doDots(g);
            drawPosts(g);
        }

        internal override void calculateCurrent()
        {
            double vd = volts[0] - volts[1];
            current = vd/resistance;
        }

        internal override void reset()
        {
            base.reset();
            state = false;
        }

        internal override void startIteration()
        {
            if (Math.Abs(current) < holdcurrent)
                state = false;
            double vd = volts[0] - volts[1];
            if (Math.Abs(vd) > breakdown)
                state = true;
        }

        internal override void doStep()
        {
            resistance = (state) ? onresistance : offresistance;
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "искровой промежуток";
            getBasicInfo(arr);
            arr[3] = state ? "on" : "off";
            arr[4] = "Ron = " + getUnitText(onresistance, "Ом");
            arr[5] = "Roff = " + getUnitText(offresistance, "Ом");
            arr[6] = "Vbreakdown = " + getUnitText(breakdown, "В");
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Сопротивление вкл. (Ом)", onresistance, 0, 0);
            if (n == 1)
                return new EditInfo("Сопротивление выкл. (Ом)", offresistance, 0, 0);
            if (n == 2)
                return new EditInfo("Напряжение пробоя", breakdown, 0, 0);
            if (n == 3)
                return new EditInfo("Ток удержания (A)", holdcurrent, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (ei.value > 0 && n == 0)
                onresistance = ei.value;
            if (ei.value > 0 && n == 1)
                offresistance = ei.value;
            if (ei.value > 0 && n == 2)
                breakdown = ei.value;
            if (ei.value > 0 && n == 3)
                holdcurrent = ei.value;
        }
        internal override bool needsShortcut()
        {
            return false;
        }
    }
}

