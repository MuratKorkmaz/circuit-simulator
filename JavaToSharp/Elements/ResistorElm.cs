using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class ResistorElm : CircuitElm
    {
        internal double resistance;
        public ResistorElm(int xx, int yy) : base(xx, yy)
        {
            resistance = 100;
        }
        public ResistorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sResistance = st.nextToken();
            bool isParsedResistance = double.TryParse(sResistance,out resistance);
            if (!isParsedResistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
        }
        internal override int DumpType
        {
            get
            {
                return 'r';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + resistance;
        }

        internal Point ps3, ps4;
        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            ps3 = new Point();
            ps4 = new Point();
        }

        internal override void draw(Graphics g)
        {
            int segments = 16;
            int i;
            int ox = 0;
            double v1 = volts[0];
            double v2 = volts[1];
            draw2Leads(g);
            double segf = 1.0/segments;
            doDots(g);
            drawPosts(g);
        }

        internal override void calculateCurrent()
        {
            current = (volts[0]-volts[1])/resistance;
            //System.out.print(this + " res current set to " + current + "\n");
        }
        internal override void stamp()
        {
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "резистор";
            getBasicInfo(arr);
            arr[3] = "R = " + getUnitText(resistance, "Ом");
            arr[4] = "P = " + getUnitText(Power, "Вт");
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Сопротивление (Ом)", resistance, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (ei.value > 0)
                resistance = ei.value;
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}
