using System;

class SwitchElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 's';
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return (position == 1)?0:1;
		}
		
	}
	override internal bool Wire
	{
		get
		{
			return true;
		}
		
	}
	internal bool momentary;
	// position 0 == closed, position 1 == open
	internal int position, posCount;
	public SwitchElm(int xx, int yy):base(xx, yy)
	{
		momentary = false;
		position = 0;
		posCount = 2;
	}
	internal SwitchElm(int xx, int yy, bool mm):base(xx, yy)
	{
		position = (mm)?1:0;
		momentary = mm;
		posCount = 2;
	}
	public SwitchElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		System.String str = st.NextToken();
		if (String.CompareOrdinal(str, "true") == 0)
			position = (this is LogicInputElm)?0:1;
		else if (String.CompareOrdinal(str, "false") == 0)
			position = (this is LogicInputElm)?1:0;
		else
			position = System.Int32.Parse(str);
		momentary = st.NextToken().ToUpper().Equals("TRUE");
		posCount = 2;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + position + " " + momentary;
	}
	
	internal System.Drawing.Point ps;
	new internal System.Drawing.Point ps2;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		ps = new System.Drawing.Point(0, 0);
		ps2 = new System.Drawing.Point(0, 0);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int openhs = 16;
		int hs1 = (position == 1)?0:2;
		int hs2 = (position == 1)?openhs:2;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, openhs);
		
		draw2Leads(g);
		
		if (position == 0)
			doDots(g);
		
		if (!needsHighlight())
			SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref lead1, ref lead2, ref ps, 0, hs1);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref lead1, ref lead2, ref ps2, 1, hs2);
		
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref ps, ref ps2);
		drawPosts(g);
	}
	internal override void  calculateCurrent()
	{
		if (position == 1)
			current = 0;
	}
	internal override void  stamp()
	{
		if (position == 0)
			sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
	}
	internal virtual void  mouseUp()
	{
		if (momentary)
			toggle();
	}
	internal virtual void  toggle()
	{
		position++;
		if (position >= posCount)
			position = 0;
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = (momentary)?"Кнопка":"Выключатель";
		if (position == 1)
		{
			arr[1] = "разомкн.";
			arr[2] = "Vd = " + getVoltageDText(VoltageDiff);
		}
		else
		{
			arr[1] = "замкн.";
			arr[2] = "V = " + getVoltageText(volts[0]);
			arr[3] = "I = " + getCurrentDText(getCurrent());
		}
	}
	internal override bool getConnection(int n1, int n2)
	{
		return position == 0;
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Самовозврат", momentary);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			momentary = ei.checkbox.Checked;
	}
}