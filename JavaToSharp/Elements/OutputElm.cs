	using System.Drawing;
	using JavaToSharp;

internal class OutputElm : CircuitElm
	{
	internal readonly int FLAG_VALUE = 1;
	public OutputElm(int xx, int yy) : base(xx, yy)
	{
	}
	public OutputElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
	}
	internal override int DumpType
	{
		get
		{
			return 'O';
		}
	}
	internal override int PostCount
	{
		get
		{
			return 1;
		}
	}
	internal override void setPoints()
	{
		base.setPoints();
		lead1 = new Point();
	}
	internal override void draw(Graphics g)
	{
		bool selected = (needsHighlight() || sim.plotYElm == this);
		Font f = new Font("SansSerif", selected ? Font.BOLD : 0, 14);
		g.Font = f;
		g.Color = selected ? selectColor : whiteColor;
		string s = (flags & FLAG_VALUE) != 0 ? getVoltageText(volts[0]) : "out";
		FontMetrics fm = g.FontMetrics;
		if (this == sim.plotXElm)
		s = "X";
		if (this == sim.plotYElm)
		s = "Y";
		interpPoint(point1, point2, lead1, 1-(fm.stringWidth(s)/2+8)/dn);
		setBbox(point1, lead1, 0);
		drawCenteredText(g, s, x2, y2, true);
		setVoltageColor(g, volts[0]);
		if (selected)
		g.Color = selectColor;
		drawThickLine(g, point1, lead1);
		drawPosts(g);
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
		arr[0] = "выход";
		arr[1] = "V = " + getVoltageText(volts[0]);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new Checkbox("Показывать напряжение", (flags & FLAG_VALUE) != 0);
		return ei;
		}
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		flags = (ei.checkbox.State) ? (flags | FLAG_VALUE) : (flags & ~FLAG_VALUE);
	}
	}
