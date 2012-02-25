using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class LogicInputElm : SwitchElm
    {
        internal readonly int FLAG_TERNARY = 1;
        internal readonly int FLAG_NUMERIC = 2;
        internal double hiV, loV;
        private bool Ternary;
        public LogicInputElm(int xx, int yy) : base(xx, yy, false)
        {
            hiV = 5;
            loV = 0;
        }
        public LogicInputElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            try
            {
                string sHiV = st.nextToken();
                bool isParsedHiV = double.TryParse(sHiV, out hiV);
                if (!isParsedHiV)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                string sLov = st.nextToken();
                bool isParsedLov = double.TryParse(sLov, out loV);
                if (!isParsedLov)
                {  
                    throw new Exception("Не удалось привести к типу double");
                }
            }
            catch (Exception e)
            {
                hiV = 5;
                loV = 0;
            }
            if (Ternary)
                posCount = 3;
        }
        internal virtual bool isTernary
        {
            get
            {
                return (flags & FLAG_TERNARY) != 0;
            }
        }
        internal virtual bool isNumeric
        {
            get
            {
                return (flags & (FLAG_TERNARY|FLAG_NUMERIC)) != 0;
            }
        }
        internal override int DumpType
        {
            get
            {
                return 'L';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + hiV + " " + loV;
        }
        internal override int PostCount
        {
            get
            {
                return 1;
            }
        }
        internal override void setPoints()
        {
            base.setPoints();
            lead1 = interpPoint(point1, point2, 1-12/dn);
        }
        internal override void draw(Graphics g)
        {
            Font f = new Font("SansSerif", 20, FontStyle.Bold);
            g.Font = f;
            g.GetNearestColor(needsHighlight() ? selectColor : whiteColor);
            string s = position == 0 ? "L" : "H";
            if (isNumeric)
                s = "" + position;
            setBbox(point1, lead1, 0);
            drawCenteredText(g, s, x2, y2, true);
           voltageColor= setVoltageColor(g, volts[0]);
           myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,point1, lead1);
            updateDotCount();
            drawDots(g, point1, lead1, curcount);
            drawPosts(g);
        }
        internal override void setCurrent(int vs, double c)
        {
            current = -c;
        }
        internal override void stamp()
        {
            double v = (position == 0) ? loV : hiV;
            if (Ternary)
                v = position * 2.5;
            sim.stampVoltageSource(0, nodes[0], voltSource, v);
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal override double VoltageDiff
        {
            get
            {
                return volts[0];
            }
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "логический вход";
            arr[1] = (position == 0) ? "low" : "high";
            if (isNumeric)
                arr[1] = "" + position;
            arr[1] += " (" + getVoltageText(volts[0]) + ")";
            arr[2] = "I = " + getCurrentText(Current);
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, 0, 0);
                ei.checkbox = new CheckBox();
                if (momentary)
                {
                    ei.checkbox.Text = "Самовозврат";
                }
                
                return ei;
            }
            if (n == 1)
                return new EditInfo("Напряжение высокого уровня", hiV, 10, -10);
            if (n == 2)
                return new EditInfo("Напряжение низкого уровня", loV, 10, -10);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                momentary = ei.checkbox.Checked;
            if (n == 1)
                hiV = ei.value;
            if (n == 2)
                loV = ei.value;
        }
    }
}
