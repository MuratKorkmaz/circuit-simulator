using System;

class AnalogSwitch2Elm:AnalogSwitchElm
{
	override internal int PostCount
	{
		get
		{
			return 4;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 160;
		}
		
	}
	public AnalogSwitch2Elm(int xx, int yy):base(xx, yy)
	{
	}
	public AnalogSwitch2Elm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	
	//UPGRADE_NOTE: Final was removed from the declaration of 'openhs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int openhs = 16;
	internal System.Drawing.Point[] swposts, swpoles;
	internal System.Drawing.Point ctlPoint;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		swposts = newPointArray(2);
		swpoles = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref swpoles[0], ref swpoles[1], 1, openhs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref swposts[0], ref swposts[1], 1, openhs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ctlPoint = interpPoint(ref point1, ref point2, .5, openhs);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, openhs);
		
		// draw first lead
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		
		// draw second lead
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref swpoles[0], ref swposts[0]);
		
		// draw third lead
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref swpoles[1], ref swposts[1]);
		
		// draw switch
		SupportClass.GraphicsManager.manager.SetColor(g, lightGrayColor);
		int position = (open)?1:0;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead1, ref swpoles[position]);
		
		updateDotCount();
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref point1, ref lead1, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref swpoles[position], ref swposts[position], curcount);
		drawPosts(g);
	}
	
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:((n == 3)?ctlPoint:swposts[n - 1]);
	}
	
	internal override void  calculateCurrent()
	{
		if (open)
			current = (volts[0] - volts[2]) / r_on;
		else
			current = (volts[0] - volts[1]) / r_on;
	}
	
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
		sim.stampNonLinear(nodes[2]);
	}
	internal override void  doStep()
	{
		open = (volts[3] < 2.5);
		if ((flags & FLAG_INVERT) != 0)
			open = !open;
		if (open)
		{
			sim.stampResistor(nodes[0], nodes[2], r_on);
			sim.stampResistor(nodes[0], nodes[1], r_off);
		}
		else
		{
			sim.stampResistor(nodes[0], nodes[1], r_on);
			sim.stampResistor(nodes[0], nodes[2], r_off);
		}
	}
	
	internal override bool getConnection(int n1, int n2)
	{
		if (n1 == 3 || n2 == 3)
			return false;
		return true;
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "аналоговый выключатель (SPDT)";
		arr[1] = "I = " + getCurrentDText(getCurrent());
	}
}