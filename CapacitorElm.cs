using System;

class CapacitorElm:CircuitElm
{
	virtual internal bool Trapezoidal
	{
		get
		{
			return (flags & FLAG_BACK_EULER) == 0;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 'c';
		}
		
	}
	internal double capacitance;
	internal double compResistance, voltdiff;
	internal System.Drawing.Point[] plate1, plate2;
	public const int FLAG_BACK_EULER = 2;
	public CapacitorElm(int xx, int yy):base(xx, yy)
	{
		capacitance = 1e-5;
	}
	public CapacitorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		capacitance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		voltdiff = System.Double.Parse(st.NextToken());
	}
	internal override void  setNodeVoltage(int n, double c)
	{
		base.setNodeVoltage(n, c);
		voltdiff = volts[0] - volts[1];
	}
	internal override void  reset()
	{
		current = curcount = 0;
		// put small charge on caps when reset to start oscillators
		voltdiff = 1e-3;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + capacitance + " " + voltdiff;
	}
	internal override void  setPoints()
	{
		base.setPoints();
		double f = (dn / 2 - 4) / dn;
		// calc leads
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead1 = interpPoint(ref point1, ref point2, f);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead2 = interpPoint(ref point1, ref point2, 1 - f);
		// calc plates
		plate1 = newPointArray(2);
		plate2 = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref plate1[0], ref plate1[1], f, 12);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref plate2[0], ref plate2[1], 1 - f, 12);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int hs = 12;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		
		// draw first lead and plate
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		setPowerColor(g, false);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref plate1[0], ref plate1[1]);
		if (sim.powerCheckItem.Checked)
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
		
		// draw second lead and plate
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point2, ref lead2);
		setPowerColor(g, false);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref plate2[0], ref plate2[1]);
		
		updateDotCount();
		if (sim.dragElm != this)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point1, ref lead1, curcount);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point2, ref lead2, - curcount);
		}
		drawPosts(g);
		if (sim.showValuesCheckItem.Checked)
		{
			System.String s = getShortUnitText(capacitance, "Ф");
			drawValues(g, s, hs);
		}
	}
	internal override void  stamp()
	{
		// capacitor companion model using trapezoidal approximation
		// (Norton equivalent) consists of a current source in
		// parallel with a resistor.  Trapezoidal is more accurate
		// than backward euler but can cause oscillatory behavior
		// if RC is small relative to the timestep.
		if (Trapezoidal)
			compResistance = sim.timeStep / (2 * capacitance);
		else
			compResistance = sim.timeStep / capacitance;
		sim.stampResistor(nodes[0], nodes[1], compResistance);
		sim.stampRightSide(nodes[0]);
		sim.stampRightSide(nodes[1]);
	}
	internal override void  startIteration()
	{
		if (Trapezoidal)
			curSourceValue = (- voltdiff) / compResistance - current;
		else
			curSourceValue = (- voltdiff) / compResistance;
		//System.out.println("cap " + compResistance + " " + curSourceValue + " " + current + " " + voltdiff);
	}
	internal override void  calculateCurrent()
	{
		double voltdiff = volts[0] - volts[1];
		// we check compResistance because this might get called
		// before stamp(), which sets compResistance, causing
		// infinite current
		if (compResistance > 0)
			current = voltdiff / compResistance + curSourceValue;
	}
	internal double curSourceValue;
	internal override void  doStep()
	{
		sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "конденсатор";
		getBasicInfo(arr);
		arr[3] = "C = " + getUnitText(capacitance, "Ф");
		arr[4] = "P = " + getUnitText(Power, "Вт");
		//double v = getVoltageDiff();
		//arr[4] = "U = " + getUnitText(.5*capacitance*v*v, "J");
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Емкость (Ф)", capacitance, 0, 0);
		if (n == 1)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Трапец. апроксимация", Trapezoidal);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0 && ei.value_Renamed > 0)
			capacitance = ei.value_Renamed;
		if (n == 1)
		{
			if (ei.checkbox.Checked)
				flags &= ~ FLAG_BACK_EULER;
			else
				flags |= FLAG_BACK_EULER;
		}
	}
	internal override bool needsShortcut()
	{
		return true;
	}
}