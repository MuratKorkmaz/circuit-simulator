using System;

class TransformerElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 'T';
		}
		
	}
	virtual internal bool Trapezoidal
	{
		get
		{
			return (flags & FLAG_BACK_EULER) == 0;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 4;
		}
		
	}
	internal double inductance, ratio, couplingCoef;
	internal System.Drawing.Point[] ptEnds, ptCoil, ptCore;
	new internal double[] current;
	new internal double[] curcount;
	internal int width;
	public const int FLAG_BACK_EULER = 2;
	public TransformerElm(int xx, int yy):base(xx, yy)
	{
		inductance = 4;
		ratio = 1;
		width = 32;
		noDiagonal = true;
		couplingCoef = .999;
		current = new double[2];
		curcount = new double[2];
	}
	public TransformerElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		width = max(32, abs(yb - ya));
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		inductance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		ratio = System.Double.Parse(st.NextToken());
		current = new double[2];
		curcount = new double[2];
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		current[0] = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		current[1] = System.Double.Parse(st.NextToken());
		couplingCoef = .999;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			couplingCoef = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
		noDiagonal = true;
	}
	internal override void  drag(int xx, int yy)
	{
		xx = sim.snapGrid(xx);
		yy = sim.snapGrid(yy);
		width = max(32, abs(yy - y));
		if (xx == x)
			yy = y;
		x2 = xx; y2 = yy;
		setPoints();
	}
	internal override System.String dump()
	{
		return base.dump() + " " + inductance + " " + ratio + " " + current[0] + " " + current[1] + " " + couplingCoef;
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		int i;
		for (i = 0; i != 4; i++)
		{
			setVoltageColor(g, volts[i]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref ptEnds[i], ref ptCoil[i]);
		}
		for (i = 0; i != 2; i++)
		{
			setPowerColor(g, current[i] * (volts[i] - volts[i + 2]));
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawCoil(g, dsign * (i == 1?- 6:6), ref ptCoil[i], ref ptCoil[i + 2], volts[i], volts[i + 2]);
		}
		SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
		for (i = 0; i != 2; i++)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref ptCore[i], ref ptCore[i + 2]);
			curcount[i] = updateDotCount(current[i], curcount[i]);
		}
		for (i = 0; i != 2; i++)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref ptEnds[i], ref ptCoil[i], curcount[i]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref ptCoil[i], ref ptCoil[i + 2], curcount[i]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref ptEnds[i + 2], ref ptCoil[i + 2], - curcount[i]);
		}
		
		drawPosts(g);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref ptEnds[0], ref ptEnds[3], 0);
	}
	
	internal override void  setPoints()
	{
		base.setPoints();
		point2.Y = point1.Y;
		ptEnds = newPointArray(4);
		ptCoil = newPointArray(4);
		ptCore = newPointArray(4);
		ptEnds[0] = point1;
		ptEnds[1] = point2;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref ptEnds[2], 0, (- dsign) * width);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref ptEnds[3], 1, (- dsign) * width);
		double ce = .5 - 12 / dn;
		double cd = .5 - 2 / dn;
		int i;
		for (i = 0; i != 4; i += 2)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref ptEnds[i], ref ptEnds[i + 1], ref ptCoil[i], ce);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref ptEnds[i], ref ptEnds[i + 1], ref ptCoil[i + 1], 1 - ce);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref ptEnds[i], ref ptEnds[i + 1], ref ptCore[i], cd);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref ptEnds[i], ref ptEnds[i + 1], ref ptCore[i + 1], 1 - cd);
		}
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return ptEnds[n];
	}
	internal override void  reset()
	{
		current[0] = current[1] = volts[0] = volts[1] = volts[2] = volts[3] = curcount[0] = curcount[1] = 0;
	}
	internal double a1, a2, a3, a4;
	internal override void  stamp()
	{
		// equations for transformer:
		//   v1 = L1 di1/dt + M  di2/dt
		//   v2 = M  di1/dt + L2 di2/dt
		// we invert that to get:
		//   di1/dt = a1 v1 + a2 v2
		//   di2/dt = a3 v1 + a4 v2
		// integrate di1/dt using trapezoidal approx and we get:
		//   i1(t2) = i1(t1) + dt/2 (i1(t1) + i1(t2))
		//          = i1(t1) + a1 dt/2 v1(t1) + a2 dt/2 v2(t1) +
		//                     a1 dt/2 v1(t2) + a2 dt/2 v2(t2)
		// the norton equivalent of this for i1 is:
		//  a. current source, I = i1(t1) + a1 dt/2 v1(t1) + a2 dt/2 v2(t1)
		//  b. resistor, G = a1 dt/2
		//  c. current source controlled by voltage v2, G = a2 dt/2
		// and for i2:
		//  a. current source, I = i2(t1) + a3 dt/2 v1(t1) + a4 dt/2 v2(t1)
		//  b. resistor, G = a3 dt/2
		//  c. current source controlled by voltage v2, G = a4 dt/2
		//
		// For backward euler,
		//
		//   i1(t2) = i1(t1) + a1 dt v1(t2) + a2 dt v2(t2)
		//
		// So the current source value is just i1(t1) and we use
		// dt instead of dt/2 for the resistor and VCCS.
		//
		// first winding goes from node 0 to 2, second is from 1 to 3
		double l1 = inductance;
		double l2 = inductance * ratio * ratio;
		double m = couplingCoef * System.Math.Sqrt(l1 * l2);
		// build inverted matrix
		double deti = 1 / (l1 * l2 - m * m);
		double ts = Trapezoidal?sim.timeStep / 2:sim.timeStep;
		a1 = l2 * deti * ts; // we multiply dt/2 into a1..a4 here
		a2 = (- m) * deti * ts;
		a3 = (- m) * deti * ts;
		a4 = l1 * deti * ts;
		sim.stampConductance(nodes[0], nodes[2], a1);
		sim.stampVCCurrentSource(nodes[0], nodes[2], nodes[1], nodes[3], a2);
		sim.stampVCCurrentSource(nodes[1], nodes[3], nodes[0], nodes[2], a3);
		sim.stampConductance(nodes[1], nodes[3], a4);
		sim.stampRightSide(nodes[0]);
		sim.stampRightSide(nodes[1]);
		sim.stampRightSide(nodes[2]);
		sim.stampRightSide(nodes[3]);
	}
	internal override void  startIteration()
	{
		double voltdiff1 = volts[0] - volts[2];
		double voltdiff2 = volts[1] - volts[3];
		if (Trapezoidal)
		{
			curSourceValue1 = voltdiff1 * a1 + voltdiff2 * a2 + current[0];
			curSourceValue2 = voltdiff1 * a3 + voltdiff2 * a4 + current[1];
		}
		else
		{
			curSourceValue1 = current[0];
			curSourceValue2 = current[1];
		}
	}
	internal double curSourceValue1, curSourceValue2;
	internal override void  doStep()
	{
		sim.stampCurrentSource(nodes[0], nodes[2], curSourceValue1);
		sim.stampCurrentSource(nodes[1], nodes[3], curSourceValue2);
	}
	internal override void  calculateCurrent()
	{
		double voltdiff1 = volts[0] - volts[2];
		double voltdiff2 = volts[1] - volts[3];
		current[0] = voltdiff1 * a1 + voltdiff2 * a2 + curSourceValue1;
		current[1] = voltdiff1 * a3 + voltdiff2 * a4 + curSourceValue2;
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "трансформатор";
		arr[1] = "L = " + getUnitText(inductance, "Гн");
		arr[2] = "трансформац. = 1:" + ratio;
		arr[3] = "Vd1 = " + getVoltageText(volts[0] - volts[2]);
		arr[4] = "Vd2 = " + getVoltageText(volts[1] - volts[3]);
		arr[5] = "I1 = " + getCurrentText(current[0]);
		arr[6] = "I2 = " + getCurrentText(current[1]);
	}
	internal override bool getConnection(int n1, int n2)
	{
		if (comparePair(n1, n2, 0, 2))
			return true;
		if (comparePair(n1, n2, 1, 3))
			return true;
		return false;
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Индуктивность первичной обмотки (Гн)", inductance, .01, 5);
		if (n == 1)
			return new EditInfo("Коэффициент трансформации", ratio, 1, 10).setDimensionless();
		if (n == 2)
			return new EditInfo("Коэффициент связи", couplingCoef, 0, 1).setDimensionless();
		if (n == 3)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("трапецив. апроксимация", Trapezoidal);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			inductance = ei.value_Renamed;
		if (n == 1)
			ratio = ei.value_Renamed;
		if (n == 2 && ei.value_Renamed > 0 && ei.value_Renamed < 1)
			couplingCoef = ei.value_Renamed;
		if (n == 3)
		{
			if (ei.checkbox.Checked)
				flags &= ~ Inductor.FLAG_BACK_EULER;
			else
				flags |= Inductor.FLAG_BACK_EULER;
		}
	}
}