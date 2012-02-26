using System;
using System.Drawing;
using JavaToSharp;
using System.Globalization;

public abstract class CircuitElm : Editable
{
	internal static double voltageRange = 5;
    private const int colorScaleCount = 32;
    private static Color[] colorScale;
	internal static double currentMult, powerMult;
	internal static Point ps1, ps2;
	internal static CirSim sim;
	internal static Color whiteColor, selectColor, lightGrayColor, voltageColor;
	internal static Font unitsFont;
    internal SolidBrush myBrush;
    public static NumberFormatInfo showFormat;
    private static NumberFormatInfo shortFormat;
    private static NumberFormatInfo noCommaFormat;
    internal const double pi = Math.PI;
    internal float ascentPixel, descentPixel, heightPixel;
    internal int ascent, descent, height;
	internal int x; 
    internal int y; 
    internal int x2; 
    internal int y2; 
    internal static int flags; 
    internal int[] nodes; 
    internal int voltSource;
	internal int dx, dy, dsign;
    internal double dn;
    private double dpx1;
    private double dpy1;
    internal Point point1, point2, lead1, lead2;
    internal Pen myPen;
	internal double[] volts;
	internal double current, curcount;
	internal Rectangle boundingBox;
	internal bool noDiagonal;
	public bool selected;

    internal virtual int DumpType
    {
        get { return 0; }
    }

    internal virtual Type DumpClass
    {
        get { return this.GetType(); }
    }

    protected virtual int DefaultFlags
    {
        get { return 0; }
    }

    internal static void initClass(CirSim s)
	{
	    unitsFont = new Font("SansSerif", 10f);
	    sim = s;

	    colorScale = new Color[colorScaleCount];
	    int i;
	    for (i = 0; i != colorScaleCount; i++)
	    {
	        double v = i*2.0/colorScaleCount - 1;
	        if (v < 0)
	        {
	            int n1 = (int) (128*-v) + 127;
	            int n2 = (int) (127*(1 + v));
	            colorScale[i] = Color.FromArgb(n1, n2, n2);
	        }
	        else
	        {
	            int n1 = (int) (128*v) + 127;
	            int n2 = (int) (127*(1 - v));
	            colorScale[i] = Color.FromArgb(n2, n1, n2);
	        }
	    }

	    ps1 = new Point();
	    ps2 = new Point();

	}

    internal CircuitElm(int xx, int yy)
    {
        x = x2 = xx;
        y = y2 = yy;
        flags = DefaultFlags;
        allocNodes();
        initBoundingBox();
    }

    internal CircuitElm(int xa, int ya, int xb, int yb, int f)
    {
        x = xa;
        y = ya;
        x2 = xb;
        y2 = yb;
        flags = f;
        allocNodes();
        initBoundingBox();
    }

    private void initBoundingBox()
    {
        boundingBox = new Rectangle(min(x, x2), min(y, y2), abs(x2 - x) + 1, abs(y2 - y) + 1);
    }

    internal void allocNodes()
    {
        nodes = new int[PostCount + InternalNodeCount];
        volts = new double[PostCount + InternalNodeCount];
    }

    internal virtual string dump()
    {
        int t = DumpType;
        return (t < 127 ? ((char) t) + " " : t + " ") + x + " " + y + " " + x2 + " " + y2 + " " + flags;
    }

    internal virtual void reset()
    {
        int i;
        for (i = 0; i != PostCount + InternalNodeCount; i++)
            volts[i] = 0;
        curcount = 0;
    }

    internal virtual void draw(Graphics g)
    {
    }

    internal virtual void setCurrent(int xp, double c)
    {
        current = c;
    }

    internal virtual double Current
    {
        get { return current; }
    }

    internal virtual void doStep()
    {
    }

    internal virtual void delete()
    {
    }

    internal virtual void startIteration()
    {
    }

    internal double getPostVoltage(int x)
    {
        return volts[x];
    }

    internal virtual void setNodeVoltage(int n, double c)
    {
        volts[n] = c;
        calculateCurrent();
    }

    internal virtual void calculateCurrent()
    {
    }

    internal virtual void setPoints()
    {
        dx = x2 - x;
        dy = y2 - y;
        dn = Math.Sqrt(dx*dx + dy*dy);
        dpx1 = dy/dn;
        dpy1 = -dx/dn;
        dsign = (dy == 0) ? sign(dx) : sign(dy);
        point1 = new Point(x, y);
        point2 = new Point(x2, y2);
    }

    internal void calcLeads(int len)
    {
        if (dn < len || len == 0)
        {
            lead1 = point1;
            lead2 = point2;
            return;
        }
        lead1 = interpPoint(point1, point2, (dn - len)/(2*dn));
        lead2 = interpPoint(point1, point2, (dn + len)/(2*dn));
    }

    internal Point interpPoint(Point a, Point b, double f)
    {
        var p = new Point();
        interpPoint(a, b, p, f);
        return p;
    }

    internal void interpPoint(Point a, Point b, Point c, double f)
    {
        c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + .48);
        c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + .48);
    }

    internal void interpPoint(Point a, Point b, Point c, double f, double g)
    {
        int gx = b.Y - a.Y;
        int gy = a.X - b.X;
        g /= Math.Sqrt(gx*gx + gy*gy);
        c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + g*gx + .48);
        c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + g*gy + .48);
    }

    internal Point interpPoint(Point a, Point b, double f, double g)
    {
        var p = new Point();
        interpPoint(a, b, p, f, g);
        return p;
    }

    internal void interpPoint2(Point a, Point b, Point c, Point d, double f, double g)
    {
        int gx = b.Y - a.Y;
        int gy = a.X - b.X;
        g /= Math.Sqrt(gx*gx + gy*gy);
        c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + g*gx + .48);
        c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + g*gy + .48);
        d.X = (int) Math.Floor(a.X*(1 - f) + b.X*f - g*gx + .48);
        d.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f - g*gy + .48);
    }

    internal void draw2Leads(Graphics g)
    {
        // draw first lead
        voltageColor = setVoltageColor(g, volts[0]);
        myPen = new Pen(voltageColor);
        drawThickLine(g, myPen,point1, lead1);

        // draw second lead
        voltageColor = setVoltageColor(g, volts[1]);
        myPen = new Pen(voltageColor);
        drawThickLine(g, myPen,lead2, point2);
    }

    internal Point[] newPointArray(int n)
    {
        var a = new Point[n];
        while (n > 0)
            a[--n] = new Point();
        return a;
    }

    internal void drawDots(Graphics g, Point pa, Point pb, double pos)
    {
        if (sim.stoppedCheck.Checked || Math.Abs(pos - 0) < double.Epsilon)
            return;
        int dxp = pb.X - pa.X;
        int dyp = pb.Y - pa.Y;
        double dnp = Math.Sqrt(dxp*dxp + dyp*dyp);
        var brush = Brushes.Yellow;
        const int ds = 16;
        pos %= ds;
        if (pos < 0)
            pos += ds;
        for (double di = pos; di < dnp; di += ds)
        {
            int x0 = (int) (pa.X + di*dxp/dnp);
            int y0 = (int) (pa.X + di*dyp/dnp);
            g.FillRectangle(brush, x0 - 1, y0 - 1, 4, 4);
        }
    }

    internal Polygon calcArrow(Point a, Point b, double al, double aw)
    {
        var poly = new Polygon();
        var p1 = new Point();
        var p2 = new Point();
        int adx = b.X - a.X;
        int ady = b.Y - a.Y;
        double l = Math.Sqrt(adx*adx + ady*ady);
        poly.addPoint(b.X, b.Y);
        interpPoint2(a, b, p1, p2, 1 - al/l, aw);
        poly.addPoint(p1.X, p1.Y);
        poly.addPoint(p2.X, p2.Y);
        return poly;
    }

    internal Polygon createPolygon(Point a, Point b, Point c)
    {
        var p = new Polygon();
        p.addPoint(a.X, a.Y);
        p.addPoint(b.X, b.Y);
        p.addPoint(c.X, c.Y);
        return p;
    }

    internal Polygon createPolygon(Point a, Point b, Point c, Point d)
    {
        var p = new Polygon();
        p.addPoint(a.X, a.Y);
        p.addPoint(b.X, b.Y);
        p.addPoint(c.X, c.Y);
        p.addPoint(d.X, d.Y);
        return p;
    }

    internal Polygon createPolygon(Point[] a)
    {
        var p = new Polygon();
        for (int i = 0; i != a.Length; i++)
            p.addPoint(a[i].X, a[i].Y);
        return p;
    }

    internal virtual void drag(int xx, int yy)
    {
        xx = sim.snapGrid(xx);
        yy = sim.snapGrid(yy);
        if (noDiagonal)
        {
            if (Math.Abs(x - xx) < Math.Abs(y - yy))
            {
                xx = x;
            }
            else
            {
                yy = y;
            }
        }
        x2 = xx;
        y2 = yy;
        setPoints();
    }

    internal void move(int dxp, int dyp)
    {
        x += dxp;
        y += dyp;
        x2 += dxp;
        y2 += dyp;
        boundingBox.Offset(dxp, dyp);
        setPoints();
    }

    // determine if moving this element by (dxp,dyp) will put it on top of another element
    internal bool allowMove(int dxp, int dyp)
    {
        int nx = x + dxp;
        int ny = y + dyp;
        int nx2 = x2 + dxp;
        int ny2 = y2 + dyp;
        int i;
        for (i = 0; i != sim.elmList.Count; i++)
        {
            CircuitElm ce = sim.getElm(i);
            if (ce.x == nx && ce.y == ny && ce.x2 == nx2 && ce.y2 == ny2)
                return false;
            if (ce.x == nx2 && ce.y == ny2 && ce.x2 == nx && ce.y2 == ny)
                return false;
        }
        return true;
    }

    internal void movePoint(int n, int dxp, int dyp)
    {
        if (n == 0)
        {
            x += dxp;
            y += dyp;
        }
        else
        {
            x2 += dxp;
            y2 += dyp;
        }
        setPoints();
    }

    internal void drawPosts(Graphics g)
    {
        int i;
        for (i = 0; i != PostCount; i++)
        {
            Point p = getPost(i);
            drawPost(g, p.X, p.X, nodes[i]);
        }
    }

    internal virtual void stamp()
    {
    }

    internal virtual int VoltageSourceCount
    {
        get { return 0; }
    }

    internal virtual int InternalNodeCount
    {
        get { return 0; }
    }

    internal void setNode(int p, int n)
    {
        nodes[p] = n;
    }

    internal virtual void setVoltageSource(int n, int v)
    {
        voltSource = v;
    }

    internal int VoltageSource
    {
        get { return voltSource; }
    }

    internal virtual double VoltageDiff
    {
        get { return volts[0] - volts[1]; }
    }

    internal virtual bool nonLinear()
    {
        return false;
    }

    internal virtual int PostCount
    {
        get { return 2; }
    }

    internal int getNode(int n)
    {
        return nodes[n];
    }

    internal virtual Point getPost(int n)
    {
        return (n == 0) ? point1 : (n == 1) ? point2 : Point.Empty;
    }

    internal void drawPost(Graphics g, int x0, int y0, int n)
    {
        if (sim.dragElm == null && !needsHighlight() && sim.getCircuitNode(n).links.Count == 2)
            return;
        if (CirSim.mouseMode == CirSim.MODE_DRAG_ROW || CirSim.mouseMode == CirSim.MODE_DRAG_COLUMN)
            return;
        drawPost(g, x0, y0);
    }

    internal void drawPost(Graphics g, int x0, int y0)
    {
        var brush = new SolidBrush(whiteColor);
        g.FillEllipse(brush, x0 - 3, y0 - 3, 7, 7);
    }

    internal void setBbox(int x1, int y1, int x2p, int y2p)
    {
        if (x1 > x2p)
        {
            int q = x1;
            x1 = x2p;
            x2p = q;
        }
        if (y1 > y2p)
        {
            int q = y1;
            y1 = y2p;
            y2p = q;
        }
        boundingBox = new Rectangle(x1, y1, x2p - x1 + 1, y2p - y1 + 1);
    }

    internal void setBbox(Point p1, Point p2, double w)
    {
        setBbox(p1.X, p1.Y, p2.X, p2.Y);
        int dpx = (int) (dpx1*w);
        int dpy = (int) (dpy1*w);
        adjustBbox(p1.X + dpx, p1.Y + dpy, p1.X - dpx, p1.Y - dpy);
    }

    internal void adjustBbox(int x1, int y1, int x2p, int y2p)
    {
        if (x1 > x2p)
        {
            int q = x1;
            x1 = x2p;
            x2p = q;
        }
        if (y1 > y2p)
        {
            int q = y1;
            y1 = y2p;
            y2p = q;
        }
        x1 = min(boundingBox.X, x1);
        y1 = min(boundingBox.Y, y1);
        x2p = max(boundingBox.X + boundingBox.Width - 1, x2p);
        y2p = max(boundingBox.Y + boundingBox.Height - 1, y2p);
        boundingBox = new Rectangle(x1, y1, x2p - x1, y2p - y1);
    }

    internal void adjustBbox(Point p1, Point p2)
    {
        adjustBbox(p1.X, p1.Y, p2.X, p2.Y);
    }

    internal bool isCenteredText
    {
        get { return false; }
    }

    internal void drawCenteredText(Graphics g,Font o , string s, int xp, int yp, bool cx)
    {
        FontFamily fontFamily = new FontFamily("SansSerif");
         myBrush = new SolidBrush(Color.Empty);
         o = new Font(fontFamily, 16, FontStyle.Regular, GraphicsUnit.Pixel);
        ascent = fontFamily.GetCellAscent(FontStyle.Regular);
        descent = fontFamily.GetCellDescent(FontStyle.Regular);
        ascentPixel = o.Size * ascent / fontFamily.GetEmHeight(FontStyle.Regular);
        descentPixel = o.Size * descent / fontFamily.GetEmHeight(FontStyle.Regular);
        int w = (int)g.MeasureString(s, o).Width;
       
        if (cx)
            xp -= w/2;
        g.DrawString(s,o,myBrush,xp, yp + (int)ascentPixel/2);
        adjustBbox(xp, yp - (int)ascentPixel/2, xp + w, yp + (int)ascentPixel/2 + (int)descentPixel);
    }

    internal void drawValues(Graphics g, string s, double hs)
    {
        if (s == null)
            return;

        FontFamily fontFamily = new FontFamily("SansSerif");
        myBrush = new SolidBrush(Color.Empty);
        Font o =  new Font(fontFamily, 16,FontStyle.Regular,GraphicsUnit.Pixel);
        ascent = fontFamily.GetCellAscent(FontStyle.Regular);
        descent = fontFamily.GetCellDescent(FontStyle.Regular);
        ascentPixel = o.Size * ascent / fontFamily.GetEmHeight(FontStyle.Regular);
        descentPixel = o.Size * descent / fontFamily.GetEmHeight(FontStyle.Regular);
        int w = (int)g.MeasureString(s, o).Width;
        
        g.GetNearestColor(whiteColor);
        int ya = (int)ascentPixel/2;
        int xc, yc;
        if (this is RailElm || this is SweepElm)
        {
            xc = x2;
            yc = y2;
        }
        else
        {
            xc = (x2 + x)/2;
            yc = (y2 + y)/2;
        }
        int dpx = (int) (dpx1*hs);
        int dpy = (int) (dpy1*hs);
        if (dpx == 0)
        {
            g.DrawString(s, o,myBrush,xc - w/2, yc - abs(dpy) - 2);
        }
        else
        {
            int xx = xc + abs(dpx) + 2;
            if (this is VoltageElm || (x < x2 && y > y2))
                xx = xc - (w + abs(dpx) + 2);
            g.DrawString(s, o,myBrush ,xx, yc + dpy + ya);
        }
    }

    internal void drawCoil(Graphics g, int hs, Point p1, Point p2, double v1, double v2)
	{
	    const int segments = 30; // 10*(int) (len/10);
	    const double segf = 1.0/segments;
	    ps1.X = p1.X;
	    ps1.Y = p1.Y;
	    for (int i = 0; i != segments; i++)
	    {
	        double cx = (((i + 1)*6.0*segf)%2) - 1;
	        double hsx = Math.Sqrt(1 - cx*cx);
            if (hsx < 0)
                hsx = -hsx;
	        interpPoint(p1, p2, ps2, i*segf, hsx*hs);
	        double v = v1 + (v2 - v1)*i/segments;
	        voltageColor= setVoltageColor(g, v);
            myPen = new Pen(voltageColor);
	        drawThickLine(g, myPen,ps1, ps2);
	        ps1.X = ps2.X;
	        ps1.Y = ps2.Y;
	    }
	}

    internal static void drawThickLine(Graphics g, int x, int y, int x2, int y2)
    {
        var pen = new Pen(lightGrayColor);
        g.DrawLine(pen, x, y, x2, y2);
        g.DrawLine(pen, x + 1, y, x2 + 1, y2);
        g.DrawLine(pen, x, y + 1, x2, y2 + 1);
        g.DrawLine(pen, x + 1, y + 1, x2 + 1, y2 + 1);
    }

    internal static void drawThickLine(Graphics g, Pen pen, Point pa, Point pb)
    {
        g.DrawLine(pen, pa.X, pa.Y, pb.X, pb.Y);
        g.DrawLine(pen, pa.X + 1, pa.Y, pb.X + 1, pb.Y);
        g.DrawLine(pen, pa.X, pa.Y + 1, pb.X, pb.Y + 1);
        g.DrawLine(pen, pa.X + 1, pa.Y + 1, pb.X + 1, pb.Y + 1);
    }

    internal static void drawThickPolygon(Graphics g, int[] xs, int[] ys, int c)
    {
        int i;
        for (i = 0; i != c - 1; i++)
            drawThickLine(g, xs[i], ys[i], xs[i + 1], ys[i + 1]);
        drawThickLine(g, xs[i], ys[i], xs[0], ys[0]);
    }

    internal static void drawThickPolygon(Graphics g, Polygon p)
    {
        drawThickPolygon(g, p.xpoints.ToArray(), p.ypoints.ToArray(), p.npoints);
    }

    internal static void drawThickCircle(Graphics g, int cx, int cy, int ri)
    {
        const double m = pi/180;
        double r = ri*.98;
        for (int i = 0; i != 360; i += 20)
        {
            double ax = Math.Cos(i*m)*r + cx;
            double ay = Math.Sin(i*m)*r + cy;
            double bx = Math.Cos((i + 20)*m)*r + cx;
            double by = Math.Sin((i + 20)*m)*r + cy;
            drawThickLine(g, (int) ax, (int) ay, (int) bx, (int) by);
        }
    }

    internal   string getVoltageDText(double v)
    {
        return getUnitText2(Math.Abs(v), "В");
    }

    internal string getVoltageText(double v)
    {
        return getUnitText(v, "В");
    }

    public  static  string getUnitText2(double v, string u)
    {
        double va = Math.Abs(v);
        if (va < 1e-14)
            return "0 " + u;
        string value;
        if (va < 1e-9)
        {
            value = ((v * 1e12).ToString("F", CultureInfo.InvariantCulture));
            return value + "п" + u;
        }
            
        if (va < 1e-6)
        {
            value = ((v * 1e9).ToString("F", CultureInfo.InvariantCulture));
            return value  + " н" + u;
        }
         
        if (va < 1e-3)
        {
            value = ((v * 1e6).ToString("F", CultureInfo.InvariantCulture));
            return value + " " + CirSim.muString + u;
        }
          
        if (va < 1)
        {
            value = ((v * 1e3).ToString("F", CultureInfo.InvariantCulture));
            return value + " м" + u;
        }
        if (va < 1e3)
        {
            value = ((v).ToString("F", CultureInfo.InvariantCulture));
            return value + " " + u;
        }
            
        if (va < 1e6)
        {
            value = ((v*1e-3).ToString("F", CultureInfo.InvariantCulture));
            return value + " к" + u;
        }
        if (va < 1e9)
        {
            value = ((v * 1e-6).ToString("F", CultureInfo.InvariantCulture));
            return value + " М" + u;
        }
        value = ((v * 1e-9).ToString("F", CultureInfo.InvariantCulture));
        return value  + " Г" + u;
    }
    public  string getUnitText(double v, string u)
    {
        double va = Math.Abs(v);
        if (va < 1e-14)
            return "0 " + u;
        string value;
        if (va < 1e-9)
        {
            value = ((v * 1e12).ToString("F", CultureInfo.InvariantCulture));
            return value + "п" + u;
        }

        if (va < 1e-6)
        {
            value = ((v * 1e9).ToString("F", CultureInfo.InvariantCulture));
            return value + " н" + u;
        }

        if (va < 1e-3)
        {
            value = ((v * 1e6).ToString("F", CultureInfo.InvariantCulture));
            return value + " " + CirSim.muString + u;
        }

        if (va < 1)
        {
            value = ((v * 1e3).ToString("F", CultureInfo.InvariantCulture));
            return value + " м" + u;
        }
        if (va < 1e3)
        {
            value = ((v).ToString("F", CultureInfo.InvariantCulture));
            return value + " " + u;
        }

        if (va < 1e6)
        {
            value = ((v * 1e-3).ToString("F", CultureInfo.InvariantCulture));
            return value + " к" + u;
        }
        if (va < 1e9)
        {
            value = ((v * 1e-6).ToString("F", CultureInfo.InvariantCulture));
            return value + " М" + u;
        }
        value = ((v * 1e-9).ToString("F", CultureInfo.InvariantCulture));
        return value + " Г" + u;
    }

    internal static string getShortUnitText(double v, string u)
    {
        double va = Math.Abs(v);
        string value;
        if (va < 1e-13)
            return null;
        if (va < 1e-9)
        {
            value = (v*1e12).ToString("F", CultureInfo.InvariantCulture);
            return value  + "п" + u;
        }
            
        if (va < 1e-6)
        {
            value = (v*1e9).ToString("F", CultureInfo.InvariantCulture);
            return value + "н" + u;
        }
        if (va < 1e-3)
        {
            value = (v * 1e6).ToString("F", CultureInfo.InvariantCulture);
            return value + CirSim.muString + u;
        }
            
        if (va < 1)
        {
            value = (v * 1e3).ToString("F", CultureInfo.InvariantCulture);
            return value + "м" + u;
        }
        if (va < 1e3)
        {
            value = v.ToString("F", CultureInfo.InvariantCulture);
            return value + u;
        }
        if (va < 1e6)
        {
            value = (v * 1e-3).ToString("F", CultureInfo.InvariantCulture);
            return value + "к" + u;
        }
           
        if (va < 1e9)
        {
            value = (v * 1e-6).ToString("F", CultureInfo.InvariantCulture);
            return value + "М" + u;
        }
        value = (v * 1e-9).ToString("F", CultureInfo.InvariantCulture);
        return value  + "Г" + u;
    }

    internal string getCurrentText(double i)
    {
        return getUnitText(i, "A");
    }

    internal   string getCurrentDText(double i)
    {
        return getUnitText(Math.Abs(i), "A");
    }

    internal void updateDotCount()
    {
        curcount = updateDotCount(current, curcount);
    }

    internal virtual double updateDotCount(double cur, double cc)
    {
        if (sim.stoppedCheck.Checked)
            return cc;
        double cadd = cur*currentMult;
        //	if (cur != 0 && cadd <= .05 && cadd >= -.05)
        //	  cadd = (cadd < 0) ? -.05 : .05;
        cadd %= 8;
        //	if (cadd > 8)
        //	  cadd = 8;
        //	  if (cadd < -8)
        //	  cadd = -8;
        return cc + cadd;
    }

    internal void doDots(Graphics g)
    {
        updateDotCount();
        if (sim.dragElm != this)
            drawDots(g, point1, point2, curcount);
    }

    internal void doAdjust()
    {
    }

    internal void setupAdjust()
    {
    }

    internal virtual void getInfo(string[] arr)
    {
    }

    internal int getBasicInfo(string[] arr)
    {
        arr[1] = "I = " + getCurrentDText(Current);
        arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
        return 3;
    }

    internal Color setVoltageColor(Graphics g, double voltsValue)
    {
        if (needsHighlight())
        {
            return selectColor;
            
        }
        
       
        int c = (int) ((voltsValue + voltageRange)*(colorScaleCount - 1)/(voltageRange*2));
        if (c < 0)
            c = 0;
        if (c >= colorScaleCount)
            c = colorScaleCount - 1;
        return colorScale[c];
    }

   

    internal Color setConductanceColor(Graphics g, double w0)
    {
        w0 *= powerMult;
        //System.out.println(w);
        double w = (w0 < 0) ? -w0 : w0;
        if (w > 1)
            w = 1;
        int rg = (int) (w*255);
        return Color.FromArgb(rg, rg, rg);
    }

    internal virtual double Power
    {
        get { return VoltageDiff*current; }
    }

    internal virtual double getScopeValue(int x)
    {
        return (x == 1) ? Power : VoltageDiff;
    }

    internal virtual string getScopeUnits(int x)
    {
        return (x == 1) ? "Вт" : "В";
    }

    public virtual EditInfo getEditInfo(int n)
    {
        return null;
    }

    public virtual void setEditValue(int n, EditInfo ei)
    {
    }

    internal virtual bool getConnection(int n1, int n2)
    {
        return true;
    }

    internal virtual bool hasGroundConnection(int n1)
    {
        return false;
    }

    internal virtual bool isWire
    {
        get { return false; }
    }

    internal virtual bool canViewInScope()
    {
        return PostCount <= 2;
    }

    internal virtual bool comparePair(int x1, int x2p, int y1, int y2p)
    {
        return ((x1 == y1 && x2p == y2p) || (x1 == y2p && x2p == y1));
    }

    internal virtual bool needsHighlight()
    {
        return sim.mouseElm == this || selected;
    }

    internal virtual bool isSelected
    {
        get { return selected; }
        set { selected = value; }
    }

    internal void selectRect(Rectangle r)
    {
        selected = r.intersects(boundingBox);
    }

    internal static int abs(int x)
    {
        return x < 0 ? -x : x;
    }

    internal static int sign(int x)
    {
        return (x < 0) ? -1 : (x == 0) ? 0 : 1;
    }

    internal static int min(int a, int b)
    {
        return (a < b) ? a : b;
    }

    internal static int max(int a, int b)
    {
        return (a > b) ? a : b;
    }

    internal static double distance(Point p1, Point p2)
    {
        double x = p1.X - p2.X;
        double y = p1.Y - p2.Y;
        return Math.Sqrt(x*x + y*y);
    }

    internal Rectangle BoundingBox
    {
        get { return boundingBox; }
    }

    internal virtual bool needsShortcut()
    {
        return false;
    }
}
