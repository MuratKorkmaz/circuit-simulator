using System;

class RailElm:VoltageElm
{
	override internal int DumpType
	{
		get
		{
			return 'R';
		}
		
	}
	override internal int PostCount
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
	public RailElm(int xx, int yy):base(xx, yy, WF_DC)
	{
	}
	internal RailElm(int xx, int yy, int wf):base(xx, yy, wf)
	{
	}
	public RailElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_CLOCK '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_CLOCK = 1;
	
	internal override void  setPoints()
	{
		base.setPoints();
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead1 = interpPoint(ref point1, ref point2, 1 - circleSize / dn);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, circleSize);
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		bool clock = waveform == WF_SQUARE && (flags & FLAG_CLOCK) != 0;
		if (waveform == WF_DC || waveform == WF_VAR || clock)
		{
			//UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
			System.Drawing.Font f = new System.Drawing.Font("SansSerif", 12, System.Drawing.FontStyle.Regular);
			SupportClass.GraphicsManager.manager.SetFont(g, f);
			SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:whiteColor);
			setPowerColor(g, false);
			double v = Voltage;
			System.String s = getShortUnitText(v, "В");
			if (System.Math.Abs(v) < 1)
				s = showFormat.FormatDouble(v) + "В";
			if (Voltage > 0)
				s = "+" + s;
			if (this is AntennaElm)
				s = "Ant";
			if (clock)
				s = "CLK";
			drawCenteredText(g, s, x2, y2, true);
		}
		else
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawWaveform(g, ref point2);
		}
		drawPosts(g);
		curcount = updateDotCount(- current, curcount);
		if (sim.dragElm != this)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point1, ref lead1, curcount);
		}
	}
	internal override void  stamp()
	{
		if (waveform == WF_DC)
			sim.stampVoltageSource(0, nodes[0], voltSource, Voltage);
		else
			sim.stampVoltageSource(0, nodes[0], voltSource);
	}
	internal override void  doStep()
	{
		if (waveform != WF_DC)
			sim.updateVoltageSource(0, nodes[0], voltSource, Voltage);
	}
	internal override bool hasGroundConnection(int n1)
	{
		return true;
	}
}