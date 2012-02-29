using System;

class ProbeElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'p';
		}
		
	}
	internal const int FLAG_SHOWVOLTAGE = 1;
	public ProbeElm(int xx, int yy):base(xx, yy)
	{
	}
	public ProbeElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
	}
	
	internal System.Drawing.Point center;
	internal override void  setPoints()
	{
		base.setPoints();
		// swap points so that we subtract higher from lower
		if (point2.Y < point1.Y)
		{
			System.Drawing.Point x = point1;
			point1 = point2;
			point2 = x;
		}
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		center = interpPoint(ref point1, ref point2, .5);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		int hs = 8;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		bool selected = (needsHighlight() || sim.plotYElm == this);
		double len = (selected || sim.dragElm == this)?16:dn - 32;
		//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
		calcLeads((int) len);
		setVoltageColor(g, volts[0]);
		if (selected)
			SupportClass.GraphicsManager.manager.SetColor(g, selectColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		setVoltageColor(g, volts[1]);
		if (selected)
			SupportClass.GraphicsManager.manager.SetColor(g, selectColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead2, ref point2);
		//UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
		System.Drawing.Font f = new System.Drawing.Font("SansSerif", 14, System.Drawing.FontStyle.Bold);
		SupportClass.GraphicsManager.manager.SetFont(g, f);
		if (this == sim.plotXElm)
			drawCenteredText(g, "X", center.X, center.Y, true);
		if (this == sim.plotYElm)
			drawCenteredText(g, "Y", center.X, center.Y, true);
		if (mustShowVoltage())
		{
			System.String s = getShortUnitText(volts[0], "В");
			drawValues(g, s, 4);
		}
		drawPosts(g);
	}
	
	internal virtual bool mustShowVoltage()
	{
		return (flags & FLAG_SHOWVOLTAGE) != 0;
	}
	
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "проба осциллографа";
		arr[1] = "Vd = " + getVoltageText(VoltageDiff);
	}
	internal override bool getConnection(int n1, int n2)
	{
		return false;
	}
	
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Показывать напряжение", mustShowVoltage());
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		{
			if (ei.checkbox.Checked)
				flags = FLAG_SHOWVOLTAGE;
			else
				flags &= ~ FLAG_SHOWVOLTAGE;
		}
	}
}