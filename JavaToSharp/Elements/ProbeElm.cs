using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class ProbeElm : CircuitElm
    {
        private const int FLAG_SHOWVOLTAGE = 1;
        public ProbeElm(int xx, int yy) : base(xx, yy)
        {
        }
        public ProbeElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
        }
        internal override int DumpType
        {
            get
            {
                return 'p';
            }
        }

        private Point center;
        internal override void setPoints()
        {
            base.setPoints();
            // swap points so that we subtract higher from lower
            if (point2.Y < point1.Y)
            {
                Point x = point1;
                point1 = point2;
                point2 = x;
            }
            center = interpPoint(point1, point2,.5);
        }
        internal override void draw(Graphics g)
        {
            int hs = 8;
            setBbox(point1, point2, hs);
            bool selected = (needsHighlight() || sim.plotYElm == this);
            double len = (selected || sim.dragElm == this) ? 16 : dn-32;
            calcLeads((int) len);
            voltageColor= setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            if (selected)
                g.GetNearestColor(selectColor);
            drawThickLine(g, myPen,point1, lead1);
            voltageColor = setVoltageColor(g, volts[1]);
            if (selected)
                g.GetNearestColor(selectColor);
            drawThickLine(g, myPen ,lead2, point2);
            Font f = new Font("SansSerif", 14, FontStyle.Bold); 
            g.Font = f;
            if (this == sim.plotXElm)
                drawCenteredText(g, "X", center.X, center.Y, true);
            if (this == sim.plotYElm)
                drawCenteredText(g, "Y", center.X, center.Y, true);
            if (mustShowVoltage())
            {
                string s = getShortUnitText(volts[0], "В");
                drawValues(g, s, 4);
            }
            drawPosts(g);
        }

        protected virtual bool mustShowVoltage()
        {
            return (flags & FLAG_SHOWVOLTAGE) != 0;
        }

        internal override void getInfo(string[] arr)
        {
            arr[0] = "проба осциллографа";
            arr[1] = "Vd = " + getVoltageText(VoltageDiff);
        }
        internal override bool getConnection(int n1, int n2)
        {
            return false;
        }

        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if (mustShowVoltage())
                {
                    ei.checkbox.Text = "Показывать напряжение";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_SHOWVOLTAGE;
                else
                    flags &= ~FLAG_SHOWVOLTAGE;
            }
        }
    }
}

