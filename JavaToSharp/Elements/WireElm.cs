using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class WireElm : CircuitElm
    {
        public WireElm(int xx, int yy) : base(xx, yy)
        {
        }
        public WireElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
        }

        private const int FLAG_SHOWCURRENT = 1;
        private const int FLAG_SHOWVOLTAGE = 2;
        internal override void draw(Graphics g)
        {
          voltageColor=  setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,point1, point2);
            doDots(g);
            setBbox(point1, point2, 3);
            if (mustShowCurrent())
            {
                string s = getShortUnitText(Math.Abs(Current), "A");
                drawValues(g, s, 4);
            }
            else if (mustShowVoltage())
            {
                string s = getShortUnitText(volts[0], "В");
                drawValues(g, s, 4);
            }
            drawPosts(g);
        }

        

        internal override void stamp()
        {
            sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
        }

        protected virtual bool mustShowCurrent()
        {
            return (flags & FLAG_SHOWCURRENT) != 0;
        }

        protected virtual bool mustShowVoltage()
        {
            return (flags & FLAG_SHOWVOLTAGE) != 0;
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
            arr[0] = "провод";
            arr[1] = "I = " + getCurrentDText(Current);
            arr[2] = "V = " + getVoltageText(volts[0]);
        }
        internal override int DumpType
        {
            get
            {
                return 'w';
            }
        }
        internal override double Power
        {
            get
            {
                return 0;
            }
        }
        internal override double VoltageDiff
        {
            get
            {
                return volts[0];
            }
        }
        internal override bool isWire
        {
            get
            {
                return true;
            }
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
               
                if (mustShowCurrent())
                {
                     ei.checkbox.Text = "Показывать ток";
                }
                return ei;
            }
            if (n == 1)
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
                    flags = FLAG_SHOWCURRENT;
                else
                    flags &= ~FLAG_SHOWCURRENT;
            }
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_SHOWVOLTAGE;
                else
                    flags &= ~FLAG_SHOWVOLTAGE;
            }
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}
