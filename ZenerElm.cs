using System;

// Zener code contributed by J. Mike Rollins
// http://www.camotruck.net/rollins/simulator.html
class ZenerElm:DiodeElm
{
	override internal int DumpType
	{
		get
		{
			return 'z';
		}
		
	}
	public ZenerElm(int xx, int yy):base(xx, yy)
	{
		zvoltage = default_zvoltage;
		setup();
	}
	public ZenerElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		zvoltage = System.Double.Parse(st.NextToken());
		setup();
	}
	internal override void  setup()
	{
		diode.leakage = 5e-6; // 1N4004 is 5.0 uAmp
		base.setup();
	}
	internal override System.String dump()
	{
		return base.dump() + " " + zvoltage;
	}
	
	//UPGRADE_NOTE: Final was removed from the declaration of 'hs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	new internal int hs = 8;
	new internal System.Drawing.Drawing2D.GraphicsPath poly;
	new internal System.Drawing.Point[] cathode;
	internal System.Drawing.Point[] wing;
	
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(16);
		cathode = newPointArray(2);
		wing = newPointArray(2);
		System.Drawing.Point[] pa = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref pa[0], ref pa[1], 0, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref cathode[0], ref cathode[1], 1, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref cathode[0], ref cathode[1], ref wing[0], - 0.2, - hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref cathode[1], ref cathode[0], ref wing[1], - 0.2, - hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		poly = createPolygon(ref pa[0], ref pa[1], ref lead2);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		
		double v1 = volts[0];
		double v2 = volts[1];
		
		draw2Leads(g);
		
		// draw arrow thingy
		setPowerColor(g, true);
		setVoltageColor(g, v1);
		g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), poly);
		
		// draw thing arrow is pointing to
		setVoltageColor(g, v2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref cathode[0], ref cathode[1]);
		
		// draw wings on cathode
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref wing[0], ref cathode[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref wing[1], ref cathode[1]);
		
		doDots(g);
		drawPosts(g);
	}
	
	//UPGRADE_NOTE: Final was removed from the declaration of 'default_zvoltage '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal double default_zvoltage = 5.6;
	
	internal override void  getInfo(System.String[] arr)
	{
		base.getInfo(arr);
		arr[0] = "Стабилитрон";
		arr[5] = "Vz = " + getVoltageText(zvoltage);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Прямое падение напряжения @ 1A", fwdrop, 10, 1000);
		if (n == 1)
			return new EditInfo("Напряжение стабилизации @ 5mA", zvoltage, 1, 25);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			fwdrop = ei.value_Renamed;
		if (n == 1)
			zvoltage = ei.value_Renamed;
		setup();
	}
}