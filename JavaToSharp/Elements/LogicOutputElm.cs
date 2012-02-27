using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class LogicOutputElm : CircuitElm
    {
        internal readonly int FLAG_TERNARY = 1;
        internal readonly int FLAG_NUMERIC = 2;
        internal readonly int FLAG_PULLDOWN = 4;
        internal double threshold;
        internal string value;
        public LogicOutputElm(int xx, int yy) : base(xx, yy)
        {
            threshold = 2.5;
        }
        public LogicOutputElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            try
            {
                string sThreshold = st.nextToken();
                bool isParsedThreshold = double.TryParse(sThreshold, out threshold);
                if (!isParsedThreshold)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
            }
            catch (Exception e)
            {
                threshold = 2.5;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + threshold;
        }
        internal override int DumpType
        {
            get
            {
                return 'M';
            }
        }
        internal override int PostCount
        {
            get
            {
                return 1;
            }
        }
        internal virtual bool IsTernary
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
        internal virtual bool needsPullDown()
        {
            return (flags & FLAG_PULLDOWN) != 0;
        }
        internal override void setPoints()
        {
            base.setPoints();
            lead1 = interpPoint(point1, point2, 1-12/dn);
        }
        internal override void draw(Graphics g)
        {
            Font f = new Font("SansSerif", 20, FontStyle.Bold);
	
            //g.setColor(needsHighlight() ? selectColor : lightGrayColor);
            g.GetNearestColor(lightGrayColor);
            string s = (volts[0] < threshold) ? "L" : "H";
            if (IsTernary)
            {
                if (volts[0] > 3.75)
                    s = "2";
                else if (volts[0] > 1.25)
                    s = "1";
                else
                    s = "0";
            }
            else if (isNumeric)
                s = (volts[0] < threshold) ? "0" : "1";
            value = s;
            setBbox(point1, lead1, 0);
            drawCenteredText(g, f,s, x2, y2, true);
            voltageColor  =	setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen, point1, lead1);
            drawPosts(g);
        }
        internal override void stamp()
        {
            if (needsPullDown())
                sim.stampResistor(nodes[0], 0, 1e6);
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
            arr[0] = "логический выход";
            arr[1] = (volts[0] < threshold) ? "low" : "high";
            if (isNumeric)
                arr[1] = value;
            arr[2] = "V = " + getVoltageText(volts[0]);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Порог", threshold, 10, -10);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if (needsPullDown())
                {
                    ei.checkbox.Text = "Требуется подтяжка (pullUp)";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                threshold = ei.value;
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_PULLDOWN;
                else
                    flags &= ~FLAG_PULLDOWN;
            }
        }
    }
}
