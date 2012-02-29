using System;

class CurrentElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'i';
		}
		
	}
	override internal double VoltageDiff
	{
		get
		{
			return volts[1] - volts[0];
		}
		
	}
	internal double currentValue;
	public CurrentElm(int xx, int yy):base(xx, yy)
	{
		currentValue = .01;
	}
	public CurrentElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			currentValue = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
			currentValue = .01;
		}
	}
	internal override System.String dump()
	{
		return base.dump() + " " + currentValue;
	}
	
	internal System.Drawing.Drawing2D.GraphicsPath arrow;
	internal System.Drawing.Point ashaft1, ashaft2, center;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(26);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ashaft1 = interpPoint(ref lead1, ref lead2, .25);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ashaft2 = interpPoint(ref lead1, ref lead2, .6);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		center = interpPoint(ref lead1, ref lead2, .5);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		System.Drawing.Point p2 = interpPoint(ref lead1, ref lead2, .75);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		arrow = calcArrow(ref center, ref p2, 4, 4);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		int cr = 12;
		draw2Leads(g);
		setVoltageColor(g, (volts[0] + volts[1]) / 2);
		setPowerColor(g, false);
		
		drawThickCircle(g, center.X, center.Y, cr);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref ashaft1, ref ashaft2);
		
		g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrow);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, cr);
		doDots(g);
		if (sim.showValuesCheckItem.Checked)
		{
			System.String s = getShortUnitText(currentValue, "A");
			if (dx == 0 || dy == 0)
				drawValues(g, s, cr);
		}
		drawPosts(g);
	}
	internal override void  stamp()
	{
		current = currentValue;
		sim.stampCurrentSource(nodes[0], nodes[1], current);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Ток (A)", currentValue, 0, .1);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		currentValue = ei.value_Renamed;
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "источник тока";
		getBasicInfo(arr);
	}
}