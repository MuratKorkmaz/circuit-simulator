using System;

class InverterElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'I';
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 1;
		}
		
	}
	override internal double VoltageDiff
	{
		get
		{
			return volts[0];
		}
		
	}
	internal double slewRate; // V/ns
	public InverterElm(int xx, int yy):base(xx, yy)
	{
		noDiagonal = true;
		slewRate = .5;
	}
	public InverterElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		noDiagonal = true;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			slewRate = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
			slewRate = .5;
		}
	}
	internal override System.String dump()
	{
		return base.dump() + " " + slewRate;
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		drawPosts(g);
		draw2Leads(g);
		SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
		drawThickPolygon(g, gatePoly);
		drawThickCircle(g, pcircle.X, pcircle.Y, 3);
		curcount = updateDotCount(current, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref lead2, ref point2, curcount);
	}
	internal System.Drawing.Drawing2D.GraphicsPath gatePoly;
	internal System.Drawing.Point pcircle;
	internal override void  setPoints()
	{
		base.setPoints();
		int hs = 16;
		int ww = 16;
		if (ww > dn / 2)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			ww = (int) (dn / 2);
		}
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead1 = interpPoint(ref point1, ref point2, .5 - ww / dn);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead2 = interpPoint(ref point1, ref point2, .5 + (ww + 2) / dn);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		pcircle = interpPoint(ref point1, ref point2, .5 + (ww - 2) / dn);
		System.Drawing.Point[] triPoints = newPointArray(3);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref triPoints[0], ref triPoints[1], 0, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		triPoints[2] = interpPoint(ref point1, ref point2, .5 + (ww - 5) / dn);
		gatePoly = createPolygon(triPoints);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
	}
	internal override void  stamp()
	{
		sim.stampVoltageSource(0, nodes[1], voltSource);
	}
	internal override void  doStep()
	{
		double v0 = volts[1];
		double out_Renamed = volts[0] > 2.5?0:5;
		double maxStep = slewRate * sim.timeStep * 1e9;
		out_Renamed = System.Math.Max(System.Math.Min(v0 + maxStep, out_Renamed), v0 - maxStep);
		sim.updateVoltageSource(0, nodes[1], voltSource, out_Renamed);
	}
	internal override void  getInfo(System.String[] arr)
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
	public override void  setEditValue(int n, EditInfo ei)
	{
		slewRate = ei.value_Renamed;
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