using System;
using System.Drawing;
using System.Windows.Forms;
using JavaToSharp;
using JavaToSharp.Elements;


internal abstract class ChipElm : CircuitElm
	{
    internal static int csize;
    internal static int cspc;
    internal static int cspc2;
	internal int bits;
	internal readonly int FLAG_SMALL = 1;
	internal static readonly int FLAG_FLIP_X = 1024;
	internal static readonly int FLAG_FLIP_Y = 2048;
	public ChipElm(int xx, int yy) : base(xx, yy)
	{
		if (needsBits())
		bits = (this is DecadeElm) ? 10 : 4;
		noDiagonal = true;
		setupPins();
	}
	public ChipElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
	    string sBits = null;
	    if (needsBits())
	    sBits = st.nextToken();
	    bool isParsedBits = int.TryParse(sBits, out bits);
	    if (!isParsedBits)
	    {
	        throw new Exception("Не удалось привести к типу int");
	    }
	    noDiagonal = true;
		setupPins();
		Size = (f & FLAG_SMALL) != 0 ? 1 : 2;
		int i;
		for (i = 0; i != PostCount; i++)
		{
		if (pins[i].state)
		{
		    string sVolts = st.nextToken();
		    bool isParsedVolts = double.TryParse(sVolts,out volts[i]);
		    if (!isParsedVolts)
		    {
		         throw new Exception("Не удалось привести к типу double");
		    }
		    pins[i].value = volts[i] > 2.5;
		}
		}
	}
	internal virtual bool needsBits()
	{
		return false;
	}
	internal virtual int Size
	{
		set
		{
			csize = value;
			cspc = 8*value;
			cspc2 = cspc*2;
			flags &= ~FLAG_SMALL;
			flags |= (value == 1) ? FLAG_SMALL : 0;
		}
	}
	internal abstract void setupPins();
	internal override void draw(Graphics g)
	{
		drawChip(g);
	}
	internal virtual void drawChip(Graphics g)
	{
		int i;
		Font f = new Font("SansSerif", 10*csize, FontStyle.Bold );
       
        FontFamily fontFamily = new FontFamily("SansSerif");
        Font font = new Font(fontFamily, 10*csize, FontStyle.Bold, GraphicsUnit.Pixel);
        ascent = fontFamily.GetCellAscent(FontStyle.Bold);
        descent = fontFamily.GetCellDescent(FontStyle.Bold);
        ascentPixel = font.Size * ascent / fontFamily.GetEmHeight(FontStyle.Bold);
        descentPixel = font.Size * descent / fontFamily.GetEmHeight(FontStyle.Bold);
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
	    voltageColor =	setVoltageColor(g, volts[i]);
        myBrush = new SolidBrush(voltageColor);
		myPen = new Pen(voltageColor);
		Point a = p.post;
		Point b = p.stub;
		drawThickLine(g, myPen,a, b);
		p.curcount = updateDotCount(p.current, p.curcount);
		drawDots(g, b, a, p.curcount);
		if (p.bubble)
		{
			drawThickCircle(g, p.bubbleX, p.bubbleY, 1);
			g.GetNearestColor(lightGrayColor);
			drawThickCircle(g, p.bubbleX, p.bubbleY, 3);
		}
		g.GetNearestColor(whiteColor);
        myBrush = new SolidBrush(whiteColor);
        int sw = (int)g.MeasureString(p.text, font).Width;
		g.DrawString(p.text, font,myBrush,p.textloc.X-sw/2, p.textloc.Y+(int)ascentPixel/2);
		if (p.lineOver)
		{
			int ya = p.textloc.Y-(int)ascentPixel/2;
			g.DrawLine(myPen,p.textloc.X-sw/2, ya, p.textloc.X+sw/2, ya);
		}
		}

	    myPen = new Pen(g.GetNearestColor(needsHighlight() ? selectColor : lightGrayColor));
		drawThickPolygon(g, rectPointsX, rectPointsY, 4);
        if (ClockPoints != null)
            g.DrawLines(myPen ,ClockPoints);
		for (i = 0; i != PostCount; i++)
		drawPost(g, pins[i].post.X, pins[i].post.Y, nodes[i]);
	}
	internal int[] rectPointsX; internal int[] rectPointsY;
    internal static Point[] ClockPoints;
	internal Pin[] pins;
	internal static int sizeX;
    internal static int sizeY;
    internal bool lastClock;
	internal override void drag(int xx, int yy)
	{
		yy = sim.snapGrid(yy);
		if (xx < x)
		{
		xx = x;
		yy = y;
		}
		else
		{
		y = y2 = yy;
		x2 = sim.snapGrid(xx);
		}
		setPoints();
	}
	internal override void setPoints()
	{
		if (x2-x > sizeX*cspc2 && this == sim.dragElm)
		Size = 2;
		int hs = cspc;
		int x0 = x+cspc2;
		int y0 = y;
		int xr = x0-cspc;
		int yr = y0-cspc;
		int xs = sizeX*cspc2;
		int ys = sizeY*cspc2;
		rectPointsX = new int[] { xr, xr+xs, xr+xs, xr };
		rectPointsY = new int[] { yr, yr, yr+ys, yr+ys };
		setBbox(xr, yr, rectPointsX[2], rectPointsY[2]);
		int i;
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		switch (p.side)
		{
		case SIDE_N:
			p.setPoint(x0, y0, 1, 0, 0, -1, 0, 0);
			break;
		case SIDE_S:
			p.setPoint(x0, y0, 1, 0, 0, 1, 0, ys-cspc2);
			break;
		case SIDE_W:
			p.setPoint(x0, y0, 0, 1, -1, 0, 0, 0);
			break;
		case SIDE_E:
			p.setPoint(x0, y0, 0, 1, 1, 0, xs-cspc2, 0);
			break;
		}
		}
	}
	internal override Point getPost(int n)
	{
		return pins[n].post;
	}
	internal abstract int VoltageSourceCount {get;}
	internal override void setVoltageSource(int j, int vs)
	{
		int i;
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		if (p.output && j-- == 0)
		{
			p.voltSource = vs;
			return;
		}
		}
		Console.WriteLine("setVoltageSource failed for " + this);
	}
	internal override void stamp()
	{
		int i;
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		if (p.output)
			sim.stampVoltageSource(0, nodes[i], p.voltSource);
		}
	}
	internal virtual void execute()
	{
	}
	internal override void doStep()
	{
		int i;
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		if (!p.output)
			p.value = volts[i] > 2.5;
		}
		execute();
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		if (p.output)
			sim.updateVoltageSource(0, nodes[i], p.voltSource, p.value ? 5 : 0);
		}
	}
	internal override void reset()
	{
		int i;
		for (i = 0; i != PostCount; i++)
		{
		pins[i].value = false;
		pins[i].curcount = 0;
		volts[i] = 0;
		}
		lastClock = false;
	}

	internal override string dump()
	{
		int t = DumpType;
		string s = base.dump();
		if (needsBits())
		s += " " + bits;
		int i;
		for (i = 0; i != PostCount; i++)
		{
		if (pins[i].state)
			s += " " + volts[i];
		}
		return s;
	}

	internal override void getInfo(string[] arr)
	{
		arr[0] = ChipName;
		int i , a = 1;
		for (i = 0; i != PostCount; i++)
		{
		Pin p = pins[i];
		if (arr[a] != null)
			arr[a] += "; ";
		else
			arr[a] = "";
		string t = p.text;
		if (p.lineOver)
			t += '\'';
		if (p.clock)
			t = "Clk";
		arr[a] += t + " = " + getVoltageText(volts[i]);
		if (i % 2 == 1)
			a++;
		}
	}
	internal override void setCurrent(int xp, double c)
	{
		int i;
		for (i = 0; i != PostCount; i++)
		if (pins[i].output && pins[i].voltSource == xp)
			pins[i].current = c;
	}
	internal virtual string ChipName
	{
		get
		{
			return "chip";
		}
	}
	internal override bool getConnection(int n1, int n2)
	{
		return false;
	}
	internal override bool hasGroundConnection(int n1)
	{
		return pins[n1].output;
	}

	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new CheckBox();
		    if ((flags & FLAG_FLIP_X) != 0)
		    {
                ei.checkbox.Text = "Flip X";
		    }
		return ei;
		}
		if (n == 1)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new CheckBox();
		    
		    if ((flags & FLAG_FLIP_Y) != 0)
		    {
                ei.checkbox.Text = "Flip Y";
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
			flags |= FLAG_FLIP_X;
		else
			flags &= ~FLAG_FLIP_X;
		setPoints();
		}
		if (n == 1)
		{
		if (ei.checkbox.Checked)
			flags |= FLAG_FLIP_Y;
		else
			flags &= ~FLAG_FLIP_Y;
		setPoints();
		}
	}

	internal  const  int  SIDE_N = 0;
	internal  const int SIDE_S = 1;
	internal  const int SIDE_W = 2;
	internal  const int SIDE_E = 3;
	internal class Pin
	{
		internal Pin(int p, int s, string t)
		{
		pos = p;
		side = s;
		text = t;
		}
		internal Point post, stub;
		internal Point textloc;
		internal int pos, side, voltSource, bubbleX, bubbleY;
		internal string text;
		internal bool lineOver, bubble, clock, output, value, state;
		internal double curcount, current;
		internal virtual void setPoint(int px, int py, int dx, int dy, int dax, int day, int sx, int sy)
		{
		if ((flags & FLAG_FLIP_X) != 0)
		{
			dx = -dx;
			dax = -dax;
			px += cspc2*(sizeX-1);
			sx = -sx;
		}
		if ((flags & FLAG_FLIP_Y) != 0)
		{
			dy = -dy;
			day = -day;
			py += cspc2*(sizeY-1);
			sy = -sy;
		}
		int xa = px+cspc2*dx*pos+sx;
		int ya = py+cspc2*dy*pos+sy;
		post = new Point(xa+dax*cspc2, ya+day*cspc2);
		stub = new Point(xa+dax*cspc, ya+day*cspc);
		textloc = new Point(xa, ya);
		if (bubble)
		{
			bubbleX = xa+dax*10*csize;
			bubbleY = ya+day*10*csize;
		}
		if (clock)
		{
		    ClockPoints = new Point[3];

		    
		    
		   
			ClockPoints[0].X = xa+dax*cspc-dx*cspc/2;
			ClockPoints[0].Y = ya+day*cspc-dy*cspc/2;
			ClockPoints[1].X = xa;
			ClockPoints[1].Y = ya;
			ClockPoints[2].X = xa+dax*cspc+dx*cspc/2;
			ClockPoints[2].Y = ya+day*cspc+dy*cspc/2;
		}
		}
	}
	}

