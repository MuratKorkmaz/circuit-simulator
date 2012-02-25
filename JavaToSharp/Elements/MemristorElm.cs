using System;
using System.Drawing;
using JavaToSharp;

internal class MemristorElm : CircuitElm
{
	internal double r_on, r_off, dopeWidth, totalWidth, mobility, resistance;
	public MemristorElm(int xx, int yy) : base(xx, yy)
	{
	r_on = 100;
	r_off = 160*r_on;
	dopeWidth = 0;
	totalWidth = 10e-9; // meters
	mobility = 1e-10; // m^2/sV
	resistance = 100;
	}
	public MemristorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
	    string sR_on = st.nextToken();
	    bool isParsedR_on = double.TryParse(sR_on , out r_on);
	    if (!isParsedR_on)
	    {
	        throw new Exception("Не удалось привести к типу double");
	    }
	    string sR_off = st.nextToken();
	    bool isParsedR_off = double.TryParse(sR_off, out r_off);
	    if (!isParsedR_off)
	    {
	       throw new Exception("Не удалось привести к типу double"); 
	    }
	    string sDopeWidth = st.nextToken();
	    bool isParsedDopeWidth = double.TryParse(sDopeWidth,out dopeWidth);
	    string sTotalWidth = st.nextToken();
	    bool isParsedTotalWidth = double.TryParse(sTotalWidth, out totalWidth);
	    if (!isParsedTotalWidth)
	    {
	         throw new Exception("Не удалось привести к типу double"); 
	    }
	    string sMobility = st.nextToken();
	    bool isParsedMobility = double.TryParse(sMobility , out mobility);
	    if (!isParsedMobility)
	    {
	         throw new Exception("Не удалось привести к типу double"); 
	    }
	    resistance = 100;
	}
	internal override int DumpType
	{
		get
		{
			return 'm';
		}
	}
	internal override string dump()
	{
	return base.dump() + " " + r_on + " " + r_off + " " + dopeWidth + " " + totalWidth + " " + mobility;
	}

	internal Point ps3, ps4;
	internal override void setPoints()
	{
	base.setPoints();
	calcLeads(32);
	ps3 = new Point();
	ps4 = new Point();
	}

	internal override void draw(Graphics g)
	{
	int segments = 6;
	int i;
	int ox = 0;
	double v1 = volts[0];
	double v2 = volts[1];
	int hs = 2+(int)(8*(1-dopeWidth/totalWidth));
	setBbox(point1, point2, hs);
	draw2Leads(g);
	double segf = 1.0/segments;

	// draw zigzag
	for (i = 0; i <= segments; i++)
	{
		int nx = (i & 1) == 0 ? 1 : -1;
		if (i == segments)
		nx = 0;
		double v = v1+(v2-v1)*i/segments;
	    voltageColor=	setVoltageColor(g, v);
		interpPoint(lead1, lead2, ps1, i*segf, hs*ox);
		interpPoint(lead1, lead2, ps2, i*segf, hs*nx);
	    myPen = new Pen(voltageColor);
		drawThickLine(g,myPen, ps1, ps2);
		if (i == segments)
		break;
		interpPoint(lead1, lead2, ps1, (i+1)*segf, hs*nx);
		drawThickLine(g, myPen,ps1, ps2);
		ox = nx;
	}

	doDots(g);
	drawPosts(g);
	}

	internal override bool nonLinear()
	{
		return true;
	}

    internal override void calculateCurrent()
	{
	current = (volts[0]-volts[1])/resistance;
	}
	internal override void reset()
	{
	dopeWidth = 0;
	}
	internal override void startIteration()
	{
	double wd = dopeWidth/totalWidth;
	dopeWidth += sim.timeStep*mobility*r_on*current/totalWidth;
	if (dopeWidth < 0)
		dopeWidth = 0;
	if (dopeWidth > totalWidth)
		dopeWidth = totalWidth;
	resistance = r_on * wd + r_off * (1-wd);
	}
	internal override void stamp()
	{
	sim.stampNonLinear(nodes[0]);
	sim.stampNonLinear(nodes[1]);
	}
	internal override void doStep()
	{
	sim.stampResistor(nodes[0], nodes[1], resistance);
	}
	internal override void getInfo(string[] arr)
	{
	arr[0] = "мемристор";
	getBasicInfo(arr);
	arr[3] = "R = " + getUnitText(resistance, "Ом");
	arr[4] = "P = " + getUnitText(Power, "Вт");
	}
	internal override double getScopeValue(int x)
	{
	return (x == 2) ? resistance : (x == 1) ? Power : VoltageDiff;
	}
	internal override string getScopeUnits(int x)
	{
	return (x == 2) ? "Ом" : (x == 1) ? "W" : "V";
	}
	public override EditInfo getEditInfo(int n)
	{
	if (n == 0)
		return new EditInfo("Максимальное сопртивление (Ом)", r_on, 0, 0);
	if (n == 1)
		return new EditInfo("Минимальное сопротивление (Ом)", r_off, 0, 0);
	if (n == 2)
		return new EditInfo("Width of Doped Region (nm)", dopeWidth*1e9, 0, 0);
	if (n == 3)
		return new EditInfo("Total Width (nm)", totalWidth*1e9, 0, 0);
	if (n == 4)
		return new EditInfo("Mobility (um^2/(s*V))", mobility*1e12, 0, 0);
	return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
	if (n == 0)
		r_on = ei.value;
	if (n == 1)
		r_off = ei.value;
	if (n == 2)
		dopeWidth = ei.value*1e-9;
	if (n == 3)
		totalWidth = ei.value*1e-9;
	if (n == 4)
		mobility = ei.value*1e-12;
	}
}

