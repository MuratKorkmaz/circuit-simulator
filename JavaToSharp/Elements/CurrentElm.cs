using System;
using System.Drawing;

namespace JavaToSharp.Elements
{
    internal class CurrentElm : CircuitElm
    {
        internal double currentValue;
        public CurrentElm(int xx, int yy) : base(xx, yy)
        {
            currentValue =.01;
        }
        public CurrentElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            try
            {
                string sCurrentValue = st.nextToken();
                bool isParsedCurrentValue = double.TryParse(sCurrentValue, out currentValue);
                if (!isParsedCurrentValue)
                {
		        
                }

            }
            catch (Exception e)
            {
                currentValue =.01;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + currentValue;
        }
        internal override int DumpType
        {
            get
            {
                return 'i';
            }
        }

        internal Polygon arrow;
        internal Point ashaft1, ashaft2, center;
        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(26);
            ashaft1 = interpPoint(lead1, lead2,.25);
            ashaft2 = interpPoint(lead1, lead2,.6);
            center = interpPoint(lead1, lead2,.5);
            Point p2 = interpPoint(lead1, lead2,.75);
            arrow = calcArrow(center, p2, 4, 4);
        }
        internal override void draw(Graphics g)
        {
            int cr = 12;
            draw2Leads(g);  
         voltageColor =   setVoltageColor(g, (volts[0]+volts[1])/2);
            myPen = new Pen(voltageColor);
            drawThickCircle(g, center.X, center.Y, cr);
            drawThickLine(g,myPen,ashaft1, ashaft2);
            myBrush = new SolidBrush(voltageColor);
            g.FillPolygon(myBrush,arrow.Points.ToArray());
            setBbox(point1, point2, cr);
            doDots(g);
           
            drawPosts(g);
        }
        internal override void stamp()
        {
            current = currentValue;
            sim.stampCurrentSource(nodes[0], nodes[1], current);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Ток (A)", currentValue, 0,.1);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            currentValue = ei.value;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "источник тока";
            getBasicInfo(arr);
        }
        internal override double VoltageDiff
        {
            get
            {
                return volts[1] - volts[0];
            }
        }
    }
}