using System;

class AnalogSwitchElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 159;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_INVERT '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_INVERT = 1;
	internal double resistance, r_on, r_off;
	public AnalogSwitchElm(int xx, int yy):base(xx, yy)
	{
		r_on = 20;
		r_off = 1e10;
	}
	public AnalogSwitchElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		r_on = 20;
		r_off = 1e10;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			r_on = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			r_off = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
	}
	internal override System.String dump()
	{
		return base.dump() + " " + r_on + " " + r_off;
	}
	internal bool open;
	
	internal System.Drawing.Point ps, point3, lead3;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		ps = new System.Drawing.Point(0, 0);
		int openhs = 16;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		point3 = interpPoint(ref point1, ref point2, .5, - openhs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead3 = interpPoint(ref point1, ref point2, .5, (- openhs) / 2);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int openhs = 16;
		int hs = (open)?openhs:0;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, openhs);
		
		draw2Leads(g);
		
		SupportClass.GraphicsManager.manager.SetColor(g, lightGrayColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref lead1, ref lead2, ref ps, 1, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead1, ref ps);
		
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point3, ref lead3);
		
		if (!open)
			doDots(g);
		drawPosts(g);
	}
	internal override void  calculateCurrent()
	{
		current = (volts[0] - volts[1]) / resistance;
	}
	
	// we need this to be able to change the matrix for each step
	internal override bool nonLinear()
	{
		return true;
	}
	
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
	}
	internal override void  doStep()
	{
		open = (volts[2] < 2.5);
		if ((flags & FLAG_INVERT) != 0)
			open = !open;
		resistance = (open)?r_off:r_on;
		sim.stampResistor(nodes[0], nodes[1], resistance);
	}
	internal override void  drag(int xx, int yy)
	{
		xx = sim.snapGrid(xx);
		yy = sim.snapGrid(yy);
		if (abs(x - xx) < abs(y - yy))
			xx = x;
		else
			yy = y;
		int q1 = abs(x - xx) + abs(y - yy);
		int q2 = (q1 / 2) % sim.gridSize;
		if (q2 != 0)
			return ;
		x2 = xx; y2 = yy;
		setPoints();
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:((n == 1)?point2:point3);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "аналоговый выключатель";
		arr[1] = open?"разомкн.":"замкн.";
		arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
		arr[3] = "I = " + getCurrentDText(getCurrent());
		arr[4] = "Vc = " + getVoltageText(volts[2]);
	}
	// we have to just assume current will flow either way, even though that
	// might cause singular matrix errors
	internal override bool getConnection(int n1, int n2)
	{
		if (n1 == 2 || n2 == 2)
			return false;
		return true;
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Нормально замкнутый", (flags & FLAG_INVERT) != 0);
			return ei;
		}
		if (n == 1)
			return new EditInfo("Сопротивление во вкл. состоянии (Ом)", r_on, 0, 0);
		if (n == 2)
			return new EditInfo("Сопротивление в выкл. состоянии (Ом)", r_off, 0, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			flags = (ei.checkbox.Checked)?(flags | FLAG_INVERT):(flags & ~ FLAG_INVERT);
		if (n == 1 && ei.value_Renamed > 0)
			r_on = ei.value_Renamed;
		if (n == 2 && ei.value_Renamed > 0)
			r_off = ei.value_Renamed;
	}
}