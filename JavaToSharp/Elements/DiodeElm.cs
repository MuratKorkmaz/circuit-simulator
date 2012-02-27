using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class DiodeElm : CircuitElm
    {
        internal Diode diode;
        internal const int FLAG_FWDROP = 1;
        internal readonly double defaultdrop = .805904783;
        internal double fwdrop, zvoltage;

        public DiodeElm(int xx, int yy)
            : base(xx, yy)
        {
            diode = new Diode(sim);
            fwdrop = defaultdrop;
            zvoltage = 0;
            setup();
        }
        public DiodeElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	    {
	        diode = new Diode(sim);
	        fwdrop = defaultdrop;
	        zvoltage = 0;
	        if ((f & FLAG_FWDROP) > 0)
	    {
	    	try
		{
		    string sFwdrop = (st.nextToken());
            bool isParsed = double.TryParse(sFwdrop, out zvoltage);
            if (!isParsed)
            {
                throw new Exception("Не удалось привести к типу double");
            }
		}
	    	catch (Exception e)
		{
		}
	    }
	        setup();
	    }
        internal override bool nonLinear()
        {
            return true;
        }

        internal virtual void setup()
        {
            diode.setup(fwdrop, zvoltage);
        }

        internal override int DumpType
        {
            get
            {
                return 'd';
            }
        }
        internal override string dump()
        {
            flags |= FLAG_FWDROP;
            return base.dump() + " " + fwdrop;
        }


        internal readonly int hs = 8;
        internal Polygon poly;
        internal Point[] cathode;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(16);
            cathode = newPointArray(2);
            Point[] pa = newPointArray(2);
            interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
            interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
            poly = createPolygon(pa[0], pa[1], lead2);
        }

        internal override void draw(Graphics g)
        {
            drawDiode(g);
            doDots(g);
            drawPosts(g);
        }

        internal override void reset()
        {
            diode.reset();
            volts[0] = volts[1] = curcount = 0;
        }

        internal virtual void drawDiode(Graphics g)
        {
            setBbox(point1, point2, hs);

            double v1 = volts[0];
            double v2 = volts[1];

            draw2Leads(g);

            // draw arrow thingy
            voltageColor =  setVoltageColor(g, v1);
            myBrush = new SolidBrush(voltageColor);
            g.FillPolygon(myBrush,poly.Points.ToArray());

            // draw thing arrow is pointing to
           voltageColor= setVoltageColor(g, v2);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen, cathode[0], cathode[1]);
        }

        internal override void stamp()
        {
            diode.stamp(nodes[0], nodes[1]);
        }
        internal override void doStep()
        {
            diode.doStep(volts[0] - volts[1]);
        }

        internal override void calculateCurrent()
        {
            current = diode.calculateCurrent(volts[0] - volts[1]);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "диод";
            arr[1] = "I = " + getCurrentText(Current);
            arr[2] = "Vd = " + getVoltageText(VoltageDiff);
            arr[3] = "P = " + getUnitText(Power, "Вт");
            arr[4] = "Vf = " + getVoltageText(fwdrop);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Прямое падение напряжения @ 1A", fwdrop, 10, 1000);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            fwdrop = ei.value;
            setup();
        }
        internal override bool needsShortcut()
        {
            return this.GetType() == typeof(DiodeElm);
        }
    }

}
   