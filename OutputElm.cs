using System;

class OutputElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'O';
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
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_VALUE '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_VALUE = 1;
	public OutputElm(int xx, int yy):base(xx, yy)
	{
	}
	public OutputElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
	}
	internal override void  setPoints()
	{
		base.setPoints();
		lead1 = new System.Drawing.Point(0, 0);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		bool selected = (needsHighlight() || sim.plotYElm == this);
		//UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
		System.Drawing.Font f = new System.Drawing.Font("SansSerif", 14, (System.Drawing.FontStyle) (selected?(int) System.Drawing.FontStyle.Bold:0));
		SupportClass.GraphicsManager.manager.SetFont(g, f);
		SupportClass.GraphicsManager.manager.SetColor(g, selected?selectColor:whiteColor);
		System.String s = (flags & FLAG_VALUE) != 0?getVoltageText(volts[0]):"out";
		System.Drawing.Font fm = SupportClass.GraphicsManager.manager.GetFont(g);
		if (this == sim.plotXElm)
			s = "X";
		if (this == sim.plotYElm)
			s = "Y";
        int w = (int)g.MeasureString(s, f).Width;
		//UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref lead1, 1 - (w / 2 + 8) / dn);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref lead1, 0);
		drawCenteredText(g, s, x2, y2, true);
		setVoltageColor(g, volts[0]);
		if (selected)
			SupportClass.GraphicsManager.manager.SetColor(g, selectColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		drawPosts(g);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "выход";
		arr[1] = "V = " + getVoltageText(volts[0]);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Показывать напряжение", (flags & FLAG_VALUE) != 0);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			flags = (ei.checkbox.Checked)?(flags | FLAG_VALUE):(flags & ~ FLAG_VALUE);
	}
}