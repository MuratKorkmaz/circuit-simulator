using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace circuit_emulator
{
    public abstract class CircuitElm : Editable
    {
        internal const double pi = 3.14159265358979323846;
        internal static double voltageRange = 5;
        internal static int colorScaleCount = 32;
        internal static Color[] colorScale;
        internal static double currentMult;
        internal static double powerMult;
        internal static Point ps1;
        internal static Point ps2;
        internal static CirSim sim;
        internal static Color whiteColor;
        internal static Color selectColor;
        internal static Color lightGrayColor;
        internal static Font unitsFont;

        public static SupportClass.TextNumberFormat showFormat;
        public static SupportClass.TextNumberFormat shortFormat;
        public static SupportClass.TextNumberFormat noCommaFormat;
        internal Rectangle boundingBox;
        internal double curcount;
        internal double current;
        internal double dn, dpx1, dpy1;
        internal int dsign;
        internal int dx, dy;
        internal int flags;
        internal Point lead1, lead2;
        internal bool noDiagonal;
        internal int[] nodes;
        internal Point point1, point2;
        public bool selected;
        internal int voltSource;
        internal double[] volts;
        internal int x;
        internal int x2;
        internal int y;
        internal int y2;

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

        internal virtual int DumpType
        {
            get { return 0; }
        }

        internal virtual Type DumpClass
        {
            get { return GetType(); }
        }

        internal virtual int DefaultFlags
        {
            get { return 0; }
        }

        internal virtual int VoltageSourceCount
        {
            get { return 0; }
        }

        internal virtual int InternalNodeCount
        {
            get { return 0; }
        }

        internal virtual double VoltageDiff
        {
            get { return volts[0] - volts[1]; }
        }

        internal virtual int PostCount
        {
            get { return 2; }
        }

        internal virtual bool CenteredText
        {
            get { return false; }
        }

        internal virtual double Power
        {
            get { return VoltageDiff*current; }
        }

        internal virtual bool Wire
        {
            get { return false; }
        }

        internal virtual bool Selected
        {
            get { return selected; }

            set { selected = value; }
        }

        internal virtual Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

        #region Editable Members

        public virtual EditInfo getEditInfo(int n)
        {
            return null;
        }

        public virtual void setEditValue(int n, EditInfo ei)
        {
        }

        #endregion

        internal static void initClass(CirSim s)
        {
            //UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
            unitsFont = new Font("SansSerif", 10, FontStyle.Regular);
            sim = s;

            colorScale = new Color[colorScaleCount];
            int i;
            for (i = 0; i != colorScaleCount; i++)
            {
                double v = i*2.0/colorScaleCount - 1;
                if (v < 0)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    int n1 = (int) (128*(- v)) + 127;
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    var n2 = (int) (127*(1 + v));
                    colorScale[i] = Color.FromArgb(n1, n2, n2);
                }
                else
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    int n1 = (int) (128*v) + 127;
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    var n2 = (int) (127*(1 - v));
                    colorScale[i] = Color.FromArgb(n2, n1, n2);
                }
            }

            ps1 = new Point(0, 0);
            ps2 = new Point(0, 0);

            showFormat = SupportClass.TextNumberFormat.getTextNumberInstance();
            showFormat.setMaximumFractionDigits(2);
            shortFormat = SupportClass.TextNumberFormat.getTextNumberInstance();
            shortFormat.setMaximumFractionDigits(1);
            noCommaFormat = SupportClass.TextNumberFormat.getTextNumberInstance();
            noCommaFormat.setMaximumFractionDigits(10);
            noCommaFormat.GroupingUsed = false;
        }

        internal virtual void initBoundingBox()
        {
            boundingBox = new Rectangle();
            SupportClass.RectangleSupport.ReshapeRectangle(ref boundingBox, min(x, x2), min(y, y2), abs(x2 - x) + 1,
                                                           abs(y2 - y) + 1);
        }

        internal virtual void allocNodes()
        {
            nodes = new int[PostCount + InternalNodeCount];
            volts = new double[PostCount + InternalNodeCount];
        }

        internal virtual String dump()
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

        internal virtual void setCurrent(int x, double c)
        {
            current = c;
        }

        internal virtual double getCurrent()
        {
            return current;
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

        internal virtual double getPostVoltage(int x)
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
            dpy1 = (- dx)/dn;
            dsign = (dy == 0) ? sign(dx) : sign(dy);
            point1 = new Point(x, y);
            point2 = new Point(x2, y2);
        }

        internal virtual void calcLeads(int len)
        {
            if (dn < len || len == 0)
            {
                lead1 = point1;
                lead2 = point2;
                return;
            }
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            lead1 = interpPoint(ref point1, ref point2, (dn - len)/(2*dn));
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            lead2 = interpPoint(ref point1, ref point2, (dn + len)/(2*dn));
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual Point interpPoint(ref Point a, ref Point b, double f)
        {
            var p = new Point(0, 0);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref a, ref b, ref p, f);
            return p;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void interpPoint(ref Point a, ref Point b, ref Point c, double f)
        {
            int xpd = b.X - a.X;
            int ypd = b.Y - a.Y;
            /*double q = (a.x*(1-f)+b.x*f+.48);
		System.out.println(q + " " + (int) q);*/
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + .48);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + .48);
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void interpPoint(ref Point a, ref Point b, ref Point c, double f, double g)
        {
            int xpd = b.X - a.X;
            int ypd = b.Y - a.Y;
            int gx = b.Y - a.Y;
            int gy = a.X - b.X;
            g /= Math.Sqrt(gx*gx + gy*gy);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + g*gx + .48);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + g*gy + .48);
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual Point interpPoint(ref Point a, ref Point b, double f, double g)
        {
            var p = new Point(0, 0);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref a, ref b, ref p, f, g);
            return p;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void interpPoint2(ref Point a, ref Point b, ref Point c, ref Point d, double f, double g)
        {
            int xpd = b.X - a.X;
            int ypd = b.Y - a.Y;
            int gx = b.Y - a.Y;
            int gy = a.X - b.X;
            g /= Math.Sqrt(gx*gx + gy*gy);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.X = (int) Math.Floor(a.X*(1 - f) + b.X*f + g*gx + .48);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            c.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f + g*gy + .48);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            d.X = (int) Math.Floor(a.X*(1 - f) + b.X*f - g*gx + .48);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            d.Y = (int) Math.Floor(a.Y*(1 - f) + b.Y*f - g*gy + .48);
        }

        internal virtual void draw2Leads(Graphics g)
        {
            // draw first lead
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref lead1);

            // draw second lead
            setVoltageColor(g, volts[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref lead2, ref point2);
        }

        internal virtual Point[] newPointArray(int n)
        {
            var a = new Point[n];
            while (n > 0)
                a[--n] = new Point(0, 0);
            return a;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void drawDots(Graphics g, ref Point pa, ref Point pb, double pos)
        {
            if (sim.stoppedCheck.Checked || pos == 0 || !sim.dotsCheckItem.Checked)
                return;
            int dx = pb.X - pa.X;
            int dy = pb.Y - pa.Y;
            double dn = Math.Sqrt(dx*dx + dy*dy);
            SupportClass.GraphicsManager.manager.SetColor(g, Color.Yellow);
            int ds = 16;
            pos %= ds;
            if (pos < 0)
                pos += ds;
            double di = 0;
            for (di = pos; di < dn; di += ds)
            {
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                var x0 = (int) (pa.X + di*dx/dn);
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                var y0 = (int) (pa.Y + di*dy/dn);
                g.FillRectangle(SupportClass.GraphicsManager.manager.GetPaint(g), x0 - 1, y0 - 1, 4, 4);
            }
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual GraphicsPath calcArrow(ref Point a, ref Point b, double al, double aw)
        {
            var poly = new GraphicsPath();
            var p1 = new Point(0, 0);
            var p2 = new Point(0, 0);
            int adx = b.X - a.X;
            int ady = b.Y - a.Y;
            double l = Math.Sqrt(adx*adx + ady*ady);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(poly, b.X, b.Y);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref a, ref b, ref p1, ref p2, 1 - al/l, aw);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(poly, p1.X, p1.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(poly, p2.X, p2.Y);
            return poly;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual GraphicsPath createPolygon(ref Point a, ref Point b, ref Point c)
        {
            var p = new GraphicsPath();
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, a.X, a.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, b.X, b.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, c.X, c.Y);
            return p;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual GraphicsPath createPolygon(ref Point a, ref Point b, ref Point c, ref Point d)
        {
            var p = new GraphicsPath();
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, a.X, a.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, b.X, b.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, c.X, c.Y);
            //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
            SupportClass.AddPointToGraphicsPath(p, d.X, d.Y);
            return p;
        }

        internal virtual GraphicsPath createPolygon(Point[] a)
        {
            var p = new GraphicsPath();
            int i;
            for (i = 0; i != a.Length; i++)
            {
                //UPGRADE_TODO: Method 'java.awt.Polygon.addPoint' was converted to 'SupportClass.AddPointToGraphicsPath' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPolygonaddPoint_int_int'"
                SupportClass.AddPointToGraphicsPath(p, a[i].X, a[i].Y);
            }
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

        internal virtual void move(int dx, int dy)
        {
            x += dx;
            y += dy;
            x2 += dx;
            y2 += dy;
            boundingBox.Location = new Point(dx, dy);
            setPoints();
        }

        // determine if moving this element by (dx,dy) will put it on top of another element
        internal virtual bool allowMove(int dx, int dy)
        {
            int nx = x + dx;
            int ny = y + dy;
            int nx2 = x2 + dx;
            int ny2 = y2 + dy;
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

        internal virtual void movePoint(int n, int dx, int dy)
        {
            if (n == 0)
            {
                x += dx;
                y += dy;
            }
            else
            {
                x2 += dx;
                y2 += dy;
            }
            setPoints();
        }

        internal virtual void drawPosts(Graphics g)
        {
            int i;
            for (i = 0; i != PostCount; i++)
            {
                Point p = getPost(i);
                drawPost(g, p.X, p.Y, nodes[i]);
            }
        }

        internal virtual void stamp()
        {
        }

        internal virtual void setNode(int p, int n)
        {
            nodes[p] = n;
        }

        internal virtual void setVoltageSource(int n, int v)
        {
            voltSource = v;
        }

        internal virtual int getVoltageSource()
        {
            return voltSource;
        }

        internal virtual bool nonLinear()
        {
            return false;
        }

        internal virtual int getNode(int n)
        {
            return nodes[n];
        }

        internal virtual Point getPost(int n)
        {
            return (n == 0) ? point1 : ((n == 1) ? point2 : Point.Empty);
        }

        internal virtual void drawPost(Graphics g, int x0, int y0, int n)
        {
            if (sim.dragElm == null && !needsHighlight() && sim.getCircuitNode(n).links.Count == 2)
                return;
            if (sim.mouseMode == CirSim.MODE_DRAG_ROW || sim.mouseMode == CirSim.MODE_DRAG_COLUMN)
                return;
            drawPost(g, x0, y0);
        }

        internal virtual void drawPost(Graphics g, int x0, int y0)
        {
            SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
            g.FillEllipse(SupportClass.GraphicsManager.manager.GetPaint(g), x0 - 3, y0 - 3, 7, 7);
        }

        internal virtual void setBbox(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2)
            {
                int q = x1;
                x1 = x2;
                x2 = q;
            }
            if (y1 > y2)
            {
                int q = y1;
                y1 = y2;
                y2 = q;
            }
            SupportClass.RectangleSupport.ReshapeRectangle(ref boundingBox, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void setBbox(ref Point p1, ref Point p2, double w)
        {
            setBbox(p1.X, p1.Y, p2.X, p2.Y);
            int gx = p2.Y - p1.Y;
            int gy = p1.X - p2.X;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var dpx = (int) (dpx1*w);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var dpy = (int) (dpy1*w);
            adjustBbox(p1.X + dpx, p1.Y + dpy, p1.X - dpx, p1.Y - dpy);
        }

        internal virtual void adjustBbox(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2)
            {
                int q = x1;
                x1 = x2;
                x2 = q;
            }
            if (y1 > y2)
            {
                int q = y1;
                y1 = y2;
                y2 = q;
            }
            x1 = min(boundingBox.X, x1);
            y1 = min(boundingBox.Y, y1);
            x2 = max(boundingBox.X + boundingBox.Width - 1, x2);
            y2 = max(boundingBox.Y + boundingBox.Height - 1, y2);
            SupportClass.RectangleSupport.ReshapeRectangle(ref boundingBox, x1, y1, x2 - x1, y2 - y1);
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void adjustBbox(ref Point p1, ref Point p2)
        {
            adjustBbox(p1.X, p1.Y, p2.X, p2.Y);
        }

        internal virtual void drawCenteredText(Graphics g, String s, int x, int y, bool cx)
        {
            Font fm = SupportClass.GraphicsManager.manager.GetFont(g);
            //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
            var w = (int) g.MeasureString(s, fm).Width;
            if (cx)
                x -= w/2;
            //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g),
                         SupportClass.GraphicsManager.manager.GetBrush(g), x,
                         y + SupportClass.GetAscent(fm)/2 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getDescent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            adjustBbox(x, y - SupportClass.GetAscent(fm)/2, x + w,
                       y + SupportClass.GetAscent(fm)/2 + SupportClass.GetDescent(fm));
        }

        internal virtual void drawValues(Graphics g, String s, double hs)
        {
            if (s == null)
                return;
            SupportClass.GraphicsManager.manager.SetFont(g, unitsFont);
            Font fm = SupportClass.GraphicsManager.manager.GetFont(g);
            //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
            var w = (int) g.MeasureString(s, fm).Width;
            SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            int ya = SupportClass.GetAscent(fm)/2;
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
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var dpx = (int) (dpx1*hs);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var dpy = (int) (dpy1*hs);
            if (dpx == 0)
            {
                //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g),
                             SupportClass.GraphicsManager.manager.GetBrush(g), xc - w/2,
                             yc - abs(dpy) - 2 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
            }
            else
            {
                int xx = xc + abs(dpx) + 2;
                if (this is VoltageElm || (x < x2 && y > y2))
                    xx = xc - (w + abs(dpx) + 2);
                //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g),
                             SupportClass.GraphicsManager.manager.GetBrush(g), xx,
                             yc + dpy + ya - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
            }
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void drawCoil(Graphics g, int hs, ref Point p1, ref Point p2, double v1, double v2)
        {
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            double len = distance(ref p1, ref p2);
            int segments = 30; // 10*(int) (len/10);
            int i;
            double segf = 1.0/segments;

            ps1 = p1;
            for (i = 0; i != segments; i++)
            {
                double cx = (((i + 1)*6.0*segf)%2) - 1;
                double hsx = Math.Sqrt(1 - cx*cx);
                if (hsx < 0)
                    hsx = - hsx;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref p1, ref p2, ref ps2, i*segf, hsx*hs);
                double v = v1 + (v2 - v1)*i/segments;
                setVoltageColor(g, v);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
                ps1 = ps2;
            }
        }

        internal static void drawThickLine(Graphics g, int x, int y, int x2, int y2)
        {
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), x, y, x2, y2);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), x + 1, y, x2 + 1, y2);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), x, y + 1, x2, y2 + 1);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), x + 1, y + 1, x2 + 1, y2 + 1);
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal static void drawThickLine(Graphics g, ref Point pa, ref Point pb)
        {
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), pa.X, pa.Y, pb.X, pb.Y);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), pa.X + 1, pa.Y, pb.X + 1, pb.Y);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), pa.X, pa.Y + 1, pb.X, pb.Y + 1);
            g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), pa.X + 1, pa.Y + 1, pb.X + 1, pb.Y + 1);
        }

        internal static void drawThickPolygon(Graphics g, int[] xs, int[] ys, int c)
        {
            int i;
            for (i = 0; i != c - 1; i++)
                drawThickLine(g, xs[i], ys[i], xs[i + 1], ys[i + 1]);
            drawThickLine(g, xs[i], ys[i], xs[0], ys[0]);
        }

        internal static void drawThickPolygon(Graphics g, GraphicsPath p)
        {
            drawThickPolygon(g, SupportClass.GetXPoints(p), SupportClass.GetYPoints(p), p.PointCount);
        }

        internal static void drawThickCircle(Graphics g, int cx, int cy, int ri)
        {
            int a;
            double m = pi/180;
            double r = ri*.98;
            for (a = 0; a != 360; a += 20)
            {
                double ax = Math.Cos(a*m)*r + cx;
                double ay = Math.Sin(a*m)*r + cy;
                double bx = Math.Cos((a + 20)*m)*r + cx;
                double by = Math.Sin((a + 20)*m)*r + cy;
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                drawThickLine(g, (int) ax, (int) ay, (int) bx, (int) by);
            }
        }

        internal static String getVoltageDText(double v)
        {
            return getUnitText(Math.Abs(v), "В");
        }

        internal static String getVoltageText(double v)
        {
            return getUnitText(v, "В");
        }

        internal static String getUnitText(double v, String u)
        {
            double va = Math.Abs(v);
            if (va < 1e-14)
                return "0 " + u;
            if (va < 1e-9)
                return showFormat.FormatDouble(v*1e12) + " п" + u;
            if (va < 1e-6)
                return showFormat.FormatDouble(v*1e9) + " н" + u;
            if (va < 1e-3)
                return showFormat.FormatDouble(v*1e6) + " " + CirSim.muString + u;
            if (va < 1)
                return showFormat.FormatDouble(v*1e3) + " м" + u;
            if (va < 1e3)
                return showFormat.FormatDouble(v) + " " + u;
            if (va < 1e6)
                return showFormat.FormatDouble(v*1e-3) + " к" + u;
            if (va < 1e9)
                return showFormat.FormatDouble(v*1e-6) + " М" + u;
            return showFormat.FormatDouble(v*1e-9) + " Г" + u;
        }

        internal static String getShortUnitText(double v, String u)
        {
            double va = Math.Abs(v);
            if (va < 1e-13)
                return null;
            if (va < 1e-9)
                return shortFormat.FormatDouble(v*1e12) + "п" + u;
            if (va < 1e-6)
                return shortFormat.FormatDouble(v*1e9) + "н" + u;
            if (va < 1e-3)
                return shortFormat.FormatDouble(v*1e6) + CirSim.muString + u;
            if (va < 1)
                return shortFormat.FormatDouble(v*1e3) + "м" + u;
            if (va < 1e3)
                return shortFormat.FormatDouble(v) + u;
            if (va < 1e6)
                return shortFormat.FormatDouble(v*1e-3) + "к" + u;
            if (va < 1e9)
                return shortFormat.FormatDouble(v*1e-6) + "М" + u;
            return shortFormat.FormatDouble(v*1e-9) + "Г" + u;
        }

        internal static String getCurrentText(double i)
        {
            return getUnitText(i, "A");
        }

        internal static String getCurrentDText(double i)
        {
            return getUnitText(Math.Abs(i), "A");
        }

        internal virtual void updateDotCount()
        {
            curcount = updateDotCount(current, curcount);
        }

        internal virtual double updateDotCount(double cur, double cc)
        {
            if (sim.stoppedCheck.Checked)
                return cc;
            double cadd = cur*currentMult;
            /*if (cur != 0 && cadd <= .05 && cadd >= -.05)
		cadd = (cadd < 0) ? -.05 : .05;*/
            cadd %= 8;
            /*if (cadd > 8)
		cadd = 8;
		if (cadd < -8)
		cadd = -8;*/
            return cc + cadd;
        }

        internal virtual void doDots(Graphics g)
        {
            updateDotCount();
            if (sim.dragElm != this)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref point1, ref point2, curcount);
            }
        }

        internal virtual void doAdjust()
        {
        }

        internal virtual void setupAdjust()
        {
        }

        internal virtual void getInfo(String[] arr)
        {
        }

        internal virtual int getBasicInfo(String[] arr)
        {
            arr[1] = "I = " + getCurrentDText(getCurrent());
            arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
            return 3;
        }

        internal virtual void setVoltageColor(Graphics g, double volts)
        {
            if (needsHighlight())
            {
                SupportClass.GraphicsManager.manager.SetColor(g, selectColor);
                return;
            }
            if (!sim.voltsCheckItem.Checked)
            {
                if (!sim.powerCheckItem.Checked)
                    // && !conductanceCheckItem.getState())
                    SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
                return;
            }
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var c = (int) ((volts + voltageRange)*(colorScaleCount - 1)/(voltageRange*2));
            if (c < 0)
                c = 0;
            if (c >= colorScaleCount)
                c = colorScaleCount - 1;
            SupportClass.GraphicsManager.manager.SetColor(g, colorScale[c]);
        }

        internal virtual void setPowerColor(Graphics g, bool yellow)
        {
            /*if (conductanceCheckItem.getState()) {
		setConductanceColor(g, current/getVoltageDiff());
		return;
		}*/
            if (!sim.powerCheckItem.Checked)
                return;
            setPowerColor(g, Power);
        }

        internal virtual void setPowerColor(Graphics g, double w0)
        {
            w0 *= powerMult;
            //System.out.println(w);
            double w = (w0 < 0) ? - w0 : w0;
            if (w > 1)
                w = 1;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int rg = 128 + (int) (w*127);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var b = (int) (128*(1 - w));
            /*if (yellow)
		g.setColor(new Color(rg, rg, b));
		else */
            if (w0 > 0)
                SupportClass.GraphicsManager.manager.SetColor(g, Color.FromArgb(rg, b, b));
            else
                SupportClass.GraphicsManager.manager.SetColor(g, Color.FromArgb(b, rg, b));
        }

        internal virtual void setConductanceColor(Graphics g, double w0)
        {
            w0 *= powerMult;
            //System.out.println(w);
            double w = (w0 < 0) ? - w0 : w0;
            if (w > 1)
                w = 1;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            var rg = (int) (w*255);
            SupportClass.GraphicsManager.manager.SetColor(g, Color.FromArgb(rg, rg, rg));
        }

        internal virtual double getScopeValue(int x)
        {
            return (x == 1) ? Power : VoltageDiff;
        }

        internal virtual String getScopeUnits(int x)
        {
            return (x == 1) ? "Вт" : "В";
        }

        internal virtual bool getConnection(int n1, int n2)
        {
            return true;
        }

        internal virtual bool hasGroundConnection(int n1)
        {
            return false;
        }

        internal virtual bool canViewInScope()
        {
            return PostCount <= 2;
        }

        internal virtual bool comparePair(int x1, int x2, int y1, int y2)
        {
            return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
        }

        internal virtual bool needsHighlight()
        {
            return sim.mouseElm == this || selected;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void selectRect(ref Rectangle r)
        {
            selected = r.IntersectsWith(boundingBox);
        }

        internal static int abs(int x)
        {
            return x < 0 ? - x : x;
        }

        internal static int sign(int x)
        {
            return (x < 0) ? - 1 : ((x == 0) ? 0 : 1);
        }

        internal static int min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        internal static int max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal static double distance(ref Point p1, ref Point p2)
        {
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            return Math.Sqrt(x*x + y*y);
        }

        internal virtual bool needsShortcut()
        {
            return false;
        }
    }
}