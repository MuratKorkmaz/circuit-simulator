using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class SwitchElm : CircuitElm
    {
        internal bool momentary;
        // position 0 == closed, position 1 == open
        internal int position, posCount;
        public SwitchElm(int xx, int yy) : base(xx, yy)
        {
            momentary = false;
            position = 0;
            posCount = 2;
        }
        internal SwitchElm(int xx, int yy, bool mm) : base(xx, yy)
        {
            position = (mm) ? 1 : 0;
            momentary = mm;
            posCount = 2;
        }
        public SwitchElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string str = st.nextToken();
            if (String.CompareOrdinal(str, "true") == 0)
            {
                position = (this is LogicInputElm) ? 0 : 1;
            }
            else if (String.CompareOrdinal(str, "false") == 0)
            {
                 position = (this is LogicInputElm) ? 1 : 0;
            }
            else
            {
                string sPosition = str;
                bool isParsedPosition = int.TryParse(sPosition, out position);
                if (!isParsedPosition)
                {
                    throw new Exception("Не удалось привести к типу int");
                }
            }
            string sMomentary = (st.nextToken());
            bool isParsedMomentary = bool.TryParse(sMomentary, out momentary);
            if (!isParsedMomentary)
            {
                throw new Exception("Не удалось привести к типу bool");
            }
            posCount = 2;
        }
        internal override int DumpType
        {
            get
            {
                return 's';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + position + " " + momentary;
        }

        private Point ps;
        private new Point ps2;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            ps = new Point();
            ps2 = new Point();
        }

        internal override void draw(Graphics g)
        {
            const int openhs = 16;
            int hs1 = (position == 1) ? 0 : 2;
            int hs2 = (position == 1) ? openhs : 2;
            setBbox(point1, point2, openhs);

            draw2Leads(g);

            if (position == 0)
                doDots(g);

            if (!needsHighlight())
                g.GetNearestColor(whiteColor);
            interpPoint(lead1, lead2, ps, 0, hs1);
            interpPoint(lead1, lead2, ps2, 1, hs2);
            myPen = new Pen(whiteColor);
            drawThickLine(g, myPen,ps, ps2);
            drawPosts(g);
        }

        internal override void calculateCurrent()
        {
            if (position == 1)
                current = 0;
        }
        internal override void stamp()
        {
            if (position == 0)
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return (position == 1) ? 0 : 1;
            }
        }
        internal void mouseUp()
        {
            if (momentary)
                toggle();
        }
        internal virtual void toggle()
        {
            position++;
            if (position >= posCount)
                position = 0;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = (momentary) ? "Кнопка" : "Выключатель";
            if (position == 1)
            {
                arr[1] = "разомкн.";
                arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
            }
            else
            {
                arr[1] = "замкн.";
                arr[2] = "V = " + getVoltageText(volts[0]);
                arr[3] = "I = " + getCurrentDText(Current);
            }
        }
        internal override bool getConnection(int n1, int n2)
        {
            return position == 0;
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
                if (momentary)
                {
                    ei.checkbox.Text = "Самовозврат";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                momentary = ei.checkbox.Checked;
        }
    }
}
