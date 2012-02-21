	using JavaToSharp;
namespace JavaToSharp
{
    internal class LEDElm : DiodeElm
    {
        internal double colorR, colorG, colorB;
        public LEDElm(int xx, int yy)
            : base(xx, yy)
        {
            fwdrop = 2.1024259;
            setup();
            colorR = 1;
            colorG = colorB = 0;
        }
        public LEDElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st)
            : base(xa, ya, xb, yb, f, st)
	{
		if ((f & FLAG_FWDROP) == 0)
		fwdrop = 2.1024259;
		setup();
		colorR = new (double)double?(st.nextToken());
		colorG = new (double)double?(st.nextToken());
		colorB = new (double)double?(st.nextToken());
	}
        internal override int DumpType
        {
            get
            {
                return 162;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + colorR + " " + colorG + " " + colorB;
        }

        internal Point ledLead1, ledLead2, ledCenter;
        internal override void setPoints()
        {
            base.setPoints();
            int cr = 12;
            ledLead1 = interpPoint(point1, point2, .5 - cr / dn);
            ledLead2 = interpPoint(point1, point2, .5 + cr / dn);
            ledCenter = interpPoint(point1, point2, .5);
        }

        internal override void draw(Graphics g)
        {
            if (needsHighlight() || this == sim.dragElm)
            {
                base.draw(g);
                return;
            }
            setVoltageColor(g, volts[0]);
            drawThickLine(g, point1, ledLead1);
            setVoltageColor(g, volts[1]);
            drawThickLine(g, ledLead2, point2);

            g.Color = Color.gray;
            int cr = 12;
            drawThickCircle(g, ledCenter.x, ledCenter.y, cr);
            cr -= 4;
            double w = 255 * current / .01;
            if (w > 255)
                w = 255;
            Color cc = new Color((int)(colorR * w), (int)(colorG * w), (int)(colorB * w));
            g.Color = cc;
            g.fillOval(ledCenter.x - cr, ledCenter.y - cr, cr * 2, cr * 2);
            setBbox(point1, point2, cr);
            updateDotCount();
            drawDots(g, point1, ledLead1, curcount);
            drawDots(g, point2, ledLead2, -curcount);
            drawPosts(g);
        }

        internal override void getInfo(string[] arr)
        {
            base.getInfo(arr);
            arr[0] = "светодиод";
        }

        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return base.getEditInfo(n);
            if (n == 1)
                return new EditInfo("Значение красного (0-1)", colorR, 0, 1).setDimensionless();
            if (n == 2)
                return new EditInfo("Значение зеленого (0-1)", colorG, 0, 1).setDimensionless();
            if (n == 3)
                return new EditInfo("Значение синего (0-1)", colorB, 0, 1).setDimensionless();
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                base.setEditValue(0, ei);
            if (n == 1)
                colorR = ei.value;
            if (n == 2)
                colorG = ei.value;
            if (n == 3)
                colorB = ei.value;
        }
    }

}
