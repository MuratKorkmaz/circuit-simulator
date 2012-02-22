using System.Drawing;

namespace JavaToSharp
{

    internal class AnalogSwitch2Elm : AnalogSwitchElm
    {
        public AnalogSwitch2Elm(int xx, int yy) : base(xx, yy)
        {
        }
        public AnalogSwitch2Elm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }

        private const int openhs = 16;
        private Point[] swposts;
        private Point[] swpoles;
        private Point ctlPoint;
        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            swposts = newPointArray(2);
            swpoles = newPointArray(2);
            interpPoint2(lead1, lead2, swpoles[0], swpoles[1], 1, openhs);
            interpPoint2(point1, point2, swposts[0], swposts[1], 1, openhs);
            ctlPoint = interpPoint(point1, point2,.5, openhs);
        }
        internal override int PostCount
        {
            get
            {
                return 4;
            }
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, openhs);

            // draw first lead
            voltageColor = setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen,point1, lead1);

            // draw second lead
            voltageColor  = setVoltageColor(g, volts[1]);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen,swpoles[0], swposts[0]);

            // draw third lead
            voltageColor = setVoltageColor(g, volts[2]);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen,swpoles[1], swposts[1]);

            // draw switch
            myPen = new Pen(lightGrayColor);
            int position = (open) ? 1 : 0;
            drawThickLine(g, myPen,lead1, swpoles[position]);

            updateDotCount();
            drawDots(g, point1, lead1, curcount);
            drawDots(g, swpoles[position], swposts[position], curcount);
            drawPosts(g);
        }

        

       

        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : (n == 3) ? ctlPoint : swposts[n-1];
        }
        internal override int DumpType
        {
            get
            {
                return 160;
            }
        }

        internal override void calculateCurrent()
        {
            if (open)
                current = (volts[0]-volts[2])/r_on;
            else
                current = (volts[0]-volts[1])/r_on;
        }

        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
            sim.stampNonLinear(nodes[2]);
        }
        internal override void doStep()
        {
            open = (volts[3] < 2.5);
            if ((flags & FLAG_INVERT) != 0)
                open = !open;
            if (open)
            {
                sim.stampResistor(nodes[0], nodes[2], r_on);
                sim.stampResistor(nodes[0], nodes[1], r_off);
            }
            else
            {
                sim.stampResistor(nodes[0], nodes[1], r_on);
                sim.stampResistor(nodes[0], nodes[2], r_off);
            }
        }

        internal override bool getConnection(int n1, int n2)
        {
            if (n1 == 3 || n2 == 3)
                return false;
            return true;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "аналоговый выключатель (SPDT)";
            arr[1] = "I = " + getCurrentDText(Current);
        }
    }
}

