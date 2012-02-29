// stub ThermistorElm based on SparkGapElm
// FIXME need to uncomment ThermistorElm line from CirSim.java
// FIXME need to add ThermistorElm.java to srclist
using System;

class ThermistorElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 188;
		}
		
	}
	internal double minresistance, maxresistance;
	internal double resistance;
	//UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
	internal System.Windows.Forms.ScrollBar slider;
	internal System.Windows.Forms.Label label;
	public ThermistorElm(int xx, int yy):base(xx, yy)
	{
		maxresistance = 1e9;
		minresistance = 1e3;
		createSlider();
	}
	public ThermistorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		minresistance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		maxresistance = System.Double.Parse(st.NextToken());
		createSlider();
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + minresistance + " " + maxresistance;
	}
	internal System.Drawing.Point ps3, ps4;
	internal virtual void  createSlider()
	{
		System.Windows.Forms.Label temp_Label2;
		//UPGRADE_TODO: The equivalent in .NET for field 'java.awt.Label.CENTER' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
		temp_Label2 = new System.Windows.Forms.Label();
		temp_Label2.Text = "Температура";
		temp_Label2.TextAlign = (System.Drawing.ContentAlignment) System.Drawing.ContentAlignment.MiddleCenter;
		//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
		System.Windows.Forms.Control temp_Control;
		temp_Control = label = temp_Label2;
		CirSim.main.Controls.Add(temp_Control);
		int value_Renamed = 50;
		//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
		System.Windows.Forms.Control temp_Control2;
		temp_Control2 = ;
		CirSim.main.Controls.Add(temp_Control2);
		CirSim.main.Invalidate();
	}
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		ps3 = new System.Drawing.Point(0, 0);
		ps4 = new System.Drawing.Point(0, 0);
	}
	internal override void  delete()
	{
		CirSim.main.Controls.Remove(label);
		CirSim.main.Controls.Remove(slider);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int i;
		double v1 = volts[0];
		double v2 = volts[1];
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, 6);
		draw2Leads(g);
		// FIXME need to draw properly, see ResistorElm.java
		setPowerColor(g, true);
		doDots(g);
		drawPosts(g);
	}
	
	internal override void  calculateCurrent()
	{
		double vd = volts[0] - volts[1];
		current = vd / resistance;
	}
	internal override void  startIteration()
	{
		double vd = volts[0] - volts[1];
		// FIXME set resistance as appropriate, using slider.getValue()
		resistance = minresistance;
		//System.out.print(this + " res current set to " + current + "\n");
	}
	internal override void  doStep()
	{
		sim.stampResistor(nodes[0], nodes[1], resistance);
	}
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
	}
	internal override void  getInfo(System.String[] arr)
	{
		// FIXME
		arr[0] = "термистор";
		getBasicInfo(arr);
		arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
		arr[4] = "Ron = " + getUnitText(minresistance, CirSim.ohmString);
		arr[5] = "Roff = " + getUnitText(maxresistance, CirSim.ohmString);
	}
	public override EditInfo getEditInfo(int n)
	{
		// ohmString doesn't work here on linux
		if (n == 0)
			return new EditInfo("Мин. сопротивление (Ом)", minresistance, 0, 0);
		if (n == 1)
			return new EditInfo("Макс. сопротивление (Ом)", maxresistance, 0, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (ei.value_Renamed > 0 && n == 0)
			minresistance = ei.value_Renamed;
		if (ei.value_Renamed > 0 && n == 1)
			maxresistance = ei.value_Renamed;
	}
}