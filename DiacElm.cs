// stub implementation of DiacElm, based on SparkGapElm
// FIXME need to add DiacElm.java to srclist
// FIXME need to uncomment DiacElm line from CirSim.java
using System;

class DiacElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 185;
		}
		
	}
	internal double onresistance, offresistance, breakdown, holdcurrent;
	internal bool state;
	public DiacElm(int xx, int yy):base(xx, yy)
	{
		// FIXME need to adjust defaults to make sense for diac
		offresistance = 1e9;
		onresistance = 1e3;
		breakdown = 1e3;
		holdcurrent = 0.001;
		state = false;
	}
	public DiacElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		onresistance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		offresistance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		breakdown = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		holdcurrent = System.Double.Parse(st.NextToken());
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + onresistance + " " + offresistance + " " + breakdown + " " + holdcurrent;
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
		// FIXME need to draw Diac
		int i;
		double v1 = volts[0];
		double v2 = volts[1];
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, 6);
		draw2Leads(g);
		setPowerColor(g, true);
		doDots(g);
		drawPosts(g);
	}
	
	internal override void  calculateCurrent()
	{
		double vd = volts[0] - volts[1];
		if (state)
			current = vd / onresistance;
		else
			current = vd / offresistance;
	}
	internal override void  startIteration()
	{
		double vd = volts[0] - volts[1];
		if (System.Math.Abs(current) < holdcurrent)
			state = false;
		if (System.Math.Abs(vd) > breakdown)
			state = true;
		//System.out.print(this + " res current set to " + current + "\n");
	}
	internal override void  doStep()
	{
		if (state)
			sim.stampResistor(nodes[0], nodes[1], onresistance);
		else
			sim.stampResistor(nodes[0], nodes[1], offresistance);
	}
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
	}
	internal override void  getInfo(System.String[] arr)
	{
		// FIXME
		arr[0] = "spark gap";
		getBasicInfo(arr);
		arr[3] = state?"on":"off";
		arr[4] = "Ron = " + getUnitText(onresistance, CirSim.ohmString);
		arr[5] = "Roff = " + getUnitText(offresistance, CirSim.ohmString);
		arr[6] = "Vbrkdn = " + getUnitText(breakdown, "V");
		arr[7] = "Ihold = " + getUnitText(holdcurrent, "A");
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("On resistance (ohms)", onresistance, 0, 0);
		if (n == 1)
			return new EditInfo("Off resistance (ohms)", offresistance, 0, 0);
		if (n == 2)
			return new EditInfo("Breakdown voltage (volts)", breakdown, 0, 0);
		if (n == 3)
			return new EditInfo("Hold current (amps)", holdcurrent, 0, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (ei.value_Renamed > 0 && n == 0)
			onresistance = ei.value_Renamed;
		if (ei.value_Renamed > 0 && n == 1)
			offresistance = ei.value_Renamed;
		if (ei.value_Renamed > 0 && n == 2)
			breakdown = ei.value_Renamed;
		if (ei.value_Renamed > 0 && n == 3)
			holdcurrent = ei.value_Renamed;
	}
	internal override bool needsShortcut()
	{
		return false;
	}
}