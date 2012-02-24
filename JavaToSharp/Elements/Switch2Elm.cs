using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class Switch2Elm : SwitchElm
    {
        private readonly int link;
        private const int FLAG_CENTER_OFF = 1;

        public Switch2Elm(int xx, int yy) : base(xx, yy, false)
        {
            noDiagonal = true;
        }
        internal Switch2Elm(int xx, int yy, bool mm) : base(xx, yy, mm)
        {
            noDiagonal = true;
        }
        public Switch2Elm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            string sLink = st.nextToken();
            bool isParsedLink = int.TryParse(sLink, out link);
            if(!isParsedLink)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            noDiagonal = true;
        }
        internal override int DumpType
        {
            get
            {
                return 'S';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + link;
        }

        private const int openhs = 16;
        private Point[] swposts;
        private Point[] swpoles;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            swposts = newPointArray(2);
            swpoles = newPointArray(3);
            interpPoint2(lead1, lead2, swpoles[0], swpoles[1], 1, openhs);
            swpoles[2] = lead2;
            interpPoint2(point1, point2, swposts[0], swposts[1], 1, openhs);
            posCount = hasCenterOff() ? 3 : 2;
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, openhs);

            // draw first lead
            voltageColor= setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen ,point1, lead1);

            // draw second lead
            voltageColor = setVoltageColor(g, volts[1]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen, swpoles[0], swposts[0]);

            // draw third lead
            voltageColor = setVoltageColor(g, volts[2]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen, swpoles[1], swposts[1]);

            // draw switch
            if (!needsHighlight())
                g.Color = whiteColor;
            drawThickLine(g, myPen, lead1, swpoles[position]);

            updateDotCount();
            drawDots(g, point1, lead1, curcount);
            if (position != 2)
                drawDots(g, swpoles[position], swposts[position], curcount);
            drawPosts(g);
        }
        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : swposts[n-1];
        }
        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override void calculateCurrent()
        {
            if (position == 2)
                current = 0;
        }
        internal override void stamp()
        {
            if (position == 2) // in center?
                return;
            sim.stampVoltageSource(nodes[0], nodes[position+1], voltSource, 0);
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return (position == 2) ? 0 : 1;
            }
        }
        internal override void toggle()
        {
            base.toggle();
            if (link != 0)
            {
                int i;
                for (i = 0; i != sim.elmList.Size(); i++)
                {
                    object o = sim.elmList.elementAt(i);
                    if (o is Switch2Elm)
                    {
                        Switch2Elm s2 = (Switch2Elm) o;
                        if (s2.link == link)
                            s2.position = position;
                    }
                }
            }
        }
        internal override bool getConnection(int n1, int n2)
        {
            if (position == 2)
                return false;
            return comparePair(n1, n2, 0, 1+position);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = (link == 0) ? "switch (SPDT)" : "switch (DPDT)";
            arr[1] = "I = " + getCurrentDText(Current);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if (hasCenterOff())
                {
                    ei.checkbox.Text = "Center Off";
                }
                return ei;
            }
            return base.getEditInfo(n);
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 1)
            {
                flags &= ~FLAG_CENTER_OFF;
                if (ei.checkbox.Checked)
                    flags |= FLAG_CENTER_OFF;
                if (hasCenterOff())
                    momentary = false;
                setPoints();
            }
            else
                base.setEditValue(n, ei);
        }

        protected virtual bool hasCenterOff()
        {
            return (flags & FLAG_CENTER_OFF) != 0;
        }
    }
}
