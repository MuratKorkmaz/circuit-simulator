using System;

class MemristorElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'm';
		}
		
	}
	internal double r_on, r_off, dopeWidth, totalWidth, mobility, resistance;
	public MemristorElm(int xx, int yy):base(xx, yy)
	{
		r_on = 100;
		r_off = 160 * r_on;
		dopeWidth = 0;
		totalWidth = 10e-9; // meters
		mobility = 1e-10; // m^2/sV
		resistance = 100;
	}
	public MemristorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		r_on = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		r_off = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		dopeWidth = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		totalWidth = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		mobility = System.Double.Parse(st.NextToken());
		resistance = 100;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + r_on + " " + r_off + " " + dopeWidth + " " + totalWidth + " " + mobility;
	}
	
	internal System.Drawing.Point ps3, ps4;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		ps3 = new System.Drawing.Point(0, 0);
		ps4 = new System.Drawing.Point(0, 0);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int segments = 6;
		int i;
		int ox = 0;
		double v1 = volts[0];
		double v2 = volts[1];
		//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
		int hs = 2 + (int) (8 * (1 - dopeWidth / totalWidth));
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		draw2Leads(g);
		setPowerColor(g, true);
		double segf = 1.0 / segments;
		
		// draw zigzag
		for (i = 0; i <= segments; i++)
		{
			int nx = (i & 1) == 0?1:- 1;
			if (i == segments)
				nx = 0;
			double v = v1 + (v2 - v1) * i / segments;
			setVoltageColor(g, v);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref ps1, i * segf, hs * ox);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref ps2, i * segf, hs * nx);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref ps1, ref ps2);
			if (i == segments)
				break;
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref ps1, (i + 1) * segf, hs * nx);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref ps1, ref ps2);
			ox = nx;
		}
		
		doDots(g);
		drawPosts(g);
	}
	
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void  calculateCurrent()
	{
		current = (volts[0] - volts[1]) / resistance;
	}
	internal override void  reset()
	{
		dopeWidth = 0;
	}
	internal override void  startIteration()
	{
		double wd = dopeWidth / totalWidth;
		dopeWidth += sim.timeStep * mobility * r_on * current / totalWidth;
		if (dopeWidth < 0)
			dopeWidth = 0;
		if (dopeWidth > totalWidth)
			dopeWidth = totalWidth;
		resistance = r_on * wd + r_off * (1 - wd);
	}
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
	}
	internal override void  doStep()
	{
		sim.stampResistor(nodes[0], nodes[1], resistance);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "мемристор";
		getBasicInfo(arr);
		arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
		arr[4] = "P = " + getUnitText(Power, "Вт");
	}
	internal override double getScopeValue(int x)
	{
		return (x == 2)?resistance:((x == 1)?Power:VoltageDiff);
	}
	internal override System.String getScopeUnits(int x)
	{
		return (x == 2)?CirSim.ohmString:((x == 1)?"W":"V");
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Максимальное сопртивление (Ом)", r_on, 0, 0);
		if (n == 1)
			return new EditInfo("Минимальное сопротивление (Ом)", r_off, 0, 0);
		if (n == 2)
			return new EditInfo("Width of Doped Region (nm)", dopeWidth * 1e9, 0, 0);
		if (n == 3)
			return new EditInfo("Total Width (nm)", totalWidth * 1e9, 0, 0);
		if (n == 4)
			return new EditInfo("Mobility (um^2/(s*V))", mobility * 1e12, 0, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			r_on = ei.value_Renamed;
		if (n == 1)
			r_off = ei.value_Renamed;
		if (n == 2)
			dopeWidth = ei.value_Renamed * 1e-9;
		if (n == 3)
			totalWidth = ei.value_Renamed * 1e-9;
		if (n == 4)
			mobility = ei.value_Renamed * 1e-12;
	}
}