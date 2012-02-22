using System;
using System.Drawing;
using JavaToSharp;


internal class InverterElm : CircuitElm
	{
	internal double slewRate; // V/ns
	public InverterElm(int xx, int yy) : base(xx, yy)
	{
		noDiagonal = true;
		slewRate =.5;
	}
	public InverterElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
		noDiagonal = true;
		try
		{
		    string sSlewRate = st.nextToken();
		    bool isParsedSlewRate = double.TryParse(sSlewRate, out slewRate);
		    if (!isParsedSlewRate)
		    {
		        throw new Exception("Не удалось привести к типу double");
		    }
		}
		catch (Exception e)
		{
		slewRate =0.5;
		}
	}
	internal override string dump()
	{
		return base.dump() + " " + slewRate;
	}

	internal override int DumpType
	{
		get
		{
			return 'I';
		}
	}
	internal override void draw(Graphics g)
	{
		drawPosts(g);
		draw2Leads(g);
		g.Color = needsHighlight() ? selectColor : lightGrayColor;
		drawThickPolygon(g, gatePoly);
		drawThickCircle(g, pcircle.X, pcircle.Y, 3);
		curcount = updateDotCount(current, curcount);
		drawDots(g, lead2, point2, curcount);
	}
	internal Polygon gatePoly;
	internal Point pcircle;
	internal override void setPoints()
	{
		base.setPoints();
		int hs = 16;
		int ww = 16;
		if (ww > dn/2)
		ww = (int)(dn/2);
		lead1 = interpPoint(point1, point2,.5-ww/dn);
		lead2 = interpPoint(point1, point2,.5+(ww+2)/dn);
		pcircle = interpPoint(point1, point2,.5+(ww-2)/dn);
		Point[] triPoints = newPointArray(3);
		interpPoint2(lead1, lead2, triPoints[0], triPoints[1], 0, hs);
		triPoints[2] = interpPoint(point1, point2,.5+(ww-5)/dn);
		gatePoly = createPolygon(triPoints);
		setBbox(point1, point2, hs);
	}
	internal override int VoltageSourceCount
	{
		get
		{
			return 1;
		}
	}
	internal override void stamp()
	{
		sim.stampVoltageSource(0, nodes[1], voltSource);
	}
	internal override void doStep()
	{
		double v0 = volts[1];
		double @out = volts[0] > 2.5 ? 0 : 5;
		double maxStep = slewRate * sim.timeStep * 1e9;
		@out = Math.Max(Math.Min(v0+maxStep, @out), v0-maxStep);
		sim.updateVoltageSource(0, nodes[1], voltSource, @out);
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
		arr[0] = "инвертор";
		arr[1] = "Vi = " + getVoltageText(volts[0]);
		arr[2] = "Vo = " + getVoltageText(volts[1]);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		return new EditInfo("Скорость нарастания (В/нс)", slewRate, 0, 0);
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		slewRate = ei.value;
	}
	// there is no current path through the inverter input, but there
	// is an indirect path through the output to ground.
	internal override bool getConnection(int n1, int n2)
	{
		return false;
	}
	internal override bool hasGroundConnection(int n1)
	{
		return (n1 == 1);
	}
	}
