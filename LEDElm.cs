using System;

class LEDElm:DiodeElm
{
	override internal int DumpType
	{
		get
		{
			return 162;
		}
		
	}
	internal double colorR, colorG, colorB;
	public LEDElm(int xx, int yy):base(xx, yy)
	{
		fwdrop = 2.1024259;
		setup();
		colorR = 1; colorG = colorB = 0;
	}
	public LEDElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
		if ((f & FLAG_FWDROP) == 0)
			fwdrop = 2.1024259;
		setup();
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		colorR = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		colorG = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		colorB = System.Double.Parse(st.NextToken());
	}
	internal override System.String dump()
	{
		return base.dump() + " " + colorR + " " + colorG + " " + colorB;
	}
	
	internal System.Drawing.Point ledLead1, ledLead2, ledCenter;
	internal override void  setPoints()
	{
		base.setPoints();
		int cr = 12;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ledLead1 = interpPoint(ref point1, ref point2, .5 - cr / dn);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ledLead2 = interpPoint(ref point1, ref point2, .5 + cr / dn);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		ledCenter = interpPoint(ref point1, ref point2, .5);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		if (needsHighlight() || this == sim.dragElm)
		{
			base.draw(g);
			return ;
		}
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref ledLead1);
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref ledLead2, ref point2);
		
		SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
		int cr = 12;
		drawThickCircle(g, ledCenter.X, ledCenter.Y, cr);
		cr -= 4;
		double w = 255 * current / .01;
		if (w > 255)
			w = 255;
		//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
		System.Drawing.Color cc = System.Drawing.Color.FromArgb((int) (colorR * w), (int) (colorG * w), (int) (colorB * w));
		SupportClass.GraphicsManager.manager.SetColor(g, cc);
		g.FillEllipse(SupportClass.GraphicsManager.manager.GetPaint(g), ledCenter.X - cr, ledCenter.Y - cr, cr * 2, cr * 2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, cr);
		updateDotCount();
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref point1, ref ledLead1, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref point2, ref ledLead2, - curcount);
		drawPosts(g);
	}
	
	internal override void  getInfo(System.String[] arr)
	{
		base.getInfo(arr);
		arr[0] = "светодиод";
	}
	
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return base.getEditInfo(n);
		if (n == 1)
			return new EditInfo("Значение красного (0-1)", colorR, 0, 1).setDimensionless();
		if (n == 2)
			return new EditInfo("Значение зеленого (0-1)", colorG, 0, 1).setDimensionless();
		if (n == 3)
			return new EditInfo("Значение синего (0-1)", colorB, 0, 1).setDimensionless();
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			base.setEditValue(0, ei);
		if (n == 1)
			colorR = ei.value_Renamed;
		if (n == 2)
			colorG = ei.value_Renamed;
		if (n == 3)
			colorB = ei.value_Renamed;
	}
}