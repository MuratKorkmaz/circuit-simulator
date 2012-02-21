using System;
using JavaToSharp;


internal class LogicOutputElm : CircuitElm
	{
	internal readonly int FLAG_TERNARY = 1;
	internal readonly int FLAG_NUMERIC = 2;
	internal readonly int FLAG_PULLDOWN = 4;
	internal double threshold;
	internal string value;
	public LogicOutputElm(int xx, int yy) : base(xx, yy)
	{
		threshold = 2.5;
	}
	public LogicOutputElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
		try
		{
		threshold = new (double)double?(st.nextToken());
		}
		catch (Exception e)
		{
		threshold = 2.5;
		}
	}
	internal override string dump()
	{
		return base.dump() + " " + threshold;
	}
	internal override int DumpType
	{
		get
		{
			return 'M';
		}
	}
	internal override int PostCount
	{
		get
		{
			return 1;
		}
	}
	internal virtual bool isTernary()
	{
		get
		{
			return (flags & FLAG_TERNARY) != 0;
		}
	}
	internal virtual bool isNumeric()
	{
		get
		{
			return (flags & (FLAG_TERNARY|FLAG_NUMERIC)) != 0;
		}
	}
	internal virtual bool needsPullDown()
	{
		return (flags & FLAG_PULLDOWN) != 0;
	}
	internal override void setPoints()
	{
		base.setPoints();
		lead1 = interpPoint(point1, point2, 1-12/dn);
	}
	internal override void draw(Graphics g)
	{
		Font f = new Font("SansSerif", Font.BOLD, 20);
		g.Font = f;
		//g.setColor(needsHighlight() ? selectColor : lightGrayColor);
		g.Color = lightGrayColor;
		string s = (volts[0] < threshold) ? "L" : "H";
		if (Ternary)
		{
		if (volts[0] > 3.75)
			s = "2";
		else if (volts[0] > 1.25)
			s = "1";
		else
			s = "0";
		}
		else if (Numeric)
		s = (volts[0] < threshold) ? "0" : "1";
		value = s;
		setBbox(point1, lead1, 0);
		drawCenteredText(g, s, x2, y2, true);
		setVoltageColor(g, volts[0]);
		drawThickLine(g, point1, lead1);
		drawPosts(g);
	}
	internal override void stamp()
	{
		if (needsPullDown())
		sim.stampResistor(nodes[0], 0, 1e6);
	}
	internal override double VoltageDiff
	{
		get
		{
			return volts[0];
		}
	}
	internal override void getInfo(string[] arr)
	{
		arr[0] = "логический выход";
		arr[1] = (volts[0] < threshold) ? "low" : "high";
		if (Numeric)
		arr[1] = value;
		arr[2] = "V = " + getVoltageText(volts[0]);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		return new EditInfo("Порог", threshold, 10, -10);
		if (n == 1)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new Checkbox("Требуется подтяжка (pullUp)", needsPullDown());
		return ei;
		}
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		threshold = ei.value;
		if (n == 1)
		{
		if (ei.checkbox.State)
			flags = FLAG_PULLDOWN;
		else
			flags &= ~FLAG_PULLDOWN;
		}
	}
	}
