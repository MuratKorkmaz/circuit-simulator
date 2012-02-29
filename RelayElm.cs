using System;

// 0 = switch
// 1 = switch end 1
// 2 = switch end 2
// ...
// 3n   = coil
// 3n+1 = coil
// 3n+2 = end of coil resistor

class RelayElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 178;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 2 + poleCount * 3;
		}
		
	}
	override internal int InternalNodeCount
	{
		get
		{
			return 1;
		}
		
	}
	internal double inductance;
	internal Inductor ind;
	internal double r_on, r_off, onCurrent;
	internal System.Drawing.Point[] coilPosts, coilLeads;
	internal System.Drawing.Point[][] swposts, swpoles;
	internal System.Drawing.Point[] ptSwitch;
	internal System.Drawing.Point[] lines;
	internal double coilCurrent;
	internal double[] switchCurrent;
	internal double coilCurCount;
	internal double[] switchCurCount;
	internal double d_position, coilR;
	internal int i_position;
	internal int poleCount;
	internal int openhs;
	//UPGRADE_NOTE: Final was removed from the declaration of 'nSwitch0 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int nSwitch0 = 0;
	//UPGRADE_NOTE: Final was removed from the declaration of 'nSwitch1 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int nSwitch1 = 1;
	//UPGRADE_NOTE: Final was removed from the declaration of 'nSwitch2 '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int nSwitch2 = 2;
	internal int nCoil1, nCoil2, nCoil3;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_SWAP_COIL '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_SWAP_COIL = 1;
	
	public RelayElm(int xx, int yy):base(xx, yy)
	{
		ind = new Inductor(sim);
		inductance = .2;
		ind.setup(inductance, 0, Inductor.FLAG_BACK_EULER);
		noDiagonal = true;
		onCurrent = .02;
		r_on = .05;
		r_off = 1e6;
		coilR = 20;
		coilCurrent = coilCurCount = 0;
		poleCount = 1;
		setupPoles();
	}
	public RelayElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		poleCount = System.Int32.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		inductance = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		coilCurrent = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		r_on = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		r_off = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		onCurrent = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		coilR = System.Double.Parse(st.NextToken());
		noDiagonal = true;
		ind = new Inductor(sim);
		ind.setup(inductance, coilCurrent, Inductor.FLAG_BACK_EULER);
		setupPoles();
	}
	
	internal virtual void  setupPoles()
	{
		nCoil1 = 3 * poleCount;
		nCoil2 = nCoil1 + 1;
		nCoil3 = nCoil1 + 2;
		if (switchCurrent == null || switchCurrent.Length != poleCount)
		{
			switchCurrent = new double[poleCount];
			switchCurCount = new double[poleCount];
		}
	}
	
	internal override System.String dump()
	{
		return base.dump() + " " + poleCount + " " + inductance + " " + coilCurrent + " " + r_on + " " + r_off + " " + onCurrent + " " + coilR;
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		int i, p;
		for (i = 0; i != 2; i++)
		{
			setVoltageColor(g, volts[nCoil1 + i]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref coilLeads[i], ref coilPosts[i]);
		}
		int x = ((flags & FLAG_SWAP_COIL) != 0)?1:0;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawCoil(g, dsign * 6, ref coilLeads[x], ref coilLeads[1 - x], volts[nCoil1 + x], volts[nCoil2 - x]);
		
		// draw lines
		SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.DarkGray);
		for (i = 0; i != poleCount; i++)
		{
			if (i == 0)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				interpPoint(ref point1, ref point2, ref lines[i * 2], .5, openhs * 2 + 5 * dsign - i * openhs * 3);
			}
			else
			{
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				interpPoint(ref point1, ref point2, ref lines[i * 2], .5, (int) (openhs * ((- i) * 3 + 3 - .5 + d_position)) + 5 * dsign);
			}
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref point1, ref point2, ref lines[i * 2 + 1], .5, (int) (openhs * ((- i) * 3 - .5 + d_position)) - 5 * dsign);
			g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), lines[i * 2].X, lines[i * 2].Y, lines[i * 2 + 1].X, lines[i * 2 + 1].Y);
		}
		
		for (p = 0; p != poleCount; p++)
		{
			int po = p * 3;
			for (i = 0; i != 3; i++)
			{
				// draw lead
				setVoltageColor(g, volts[nSwitch0 + po + i]);
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				drawThickLine(g, ref swposts[p][i], ref swpoles[p][i]);
			}
			
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref swpoles[p][1], ref swpoles[p][2], ref ptSwitch[p], d_position);
			//setVoltageColor(g, volts[nSwitch0]);
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.LightGray);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref swpoles[p][0], ref ptSwitch[p]);
			switchCurCount[p] = updateDotCount(switchCurrent[p], switchCurCount[p]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref swposts[p][0], ref swpoles[p][0], switchCurCount[p]);
			
			if (i_position != 2)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				drawDots(g, ref swpoles[p][i_position + 1], ref swposts[p][i_position + 1], switchCurCount[p]);
			}
		}
		
		coilCurCount = updateDotCount(coilCurrent, coilCurCount);
		
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref coilPosts[0], ref coilLeads[0], coilCurCount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref coilLeads[0], ref coilLeads[1], coilCurCount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref coilLeads[1], ref coilPosts[1], coilCurCount);
		
		drawPosts(g);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref coilPosts[0], ref coilLeads[1], 0);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		adjustBbox(ref swpoles[poleCount - 1][0], ref swposts[poleCount - 1][1]); // XXX
	}
	
	internal override void  setPoints()
	{
		base.setPoints();
		setupPoles();
		allocNodes();
		openhs = (- dsign) * 16;
		
		// switch
		calcLeads(32);
		swposts = new System.Drawing.Point[poleCount][];
		for (int i = 0; i < poleCount; i++)
		{
			swposts[i] = new System.Drawing.Point[3];
		}
		swpoles = new System.Drawing.Point[poleCount][];
		for (int i2 = 0; i2 < poleCount; i2++)
		{
			swpoles[i2] = new System.Drawing.Point[3];
		}
		int i3, j;
		for (i3 = 0; i3 != poleCount; i3++)
		{
			for (j = 0; j != 3; j++)
			{
				swposts[i3][j] = new System.Drawing.Point(0, 0);
				swpoles[i3][j] = new System.Drawing.Point(0, 0);
			}
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref swpoles[i3][0], 0, (- openhs) * 3 * i3);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref swpoles[i3][1], 1, (- openhs) * 3 * i3 - openhs);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref swpoles[i3][2], 1, (- openhs) * 3 * i3 + openhs);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref point1, ref point2, ref swposts[i3][0], 0, (- openhs) * 3 * i3);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref point1, ref point2, ref swposts[i3][1], 1, (- openhs) * 3 * i3 - openhs);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref point1, ref point2, ref swposts[i3][2], 1, (- openhs) * 3 * i3 + openhs);
		}
		
		// coil
		coilPosts = newPointArray(2);
		coilLeads = newPointArray(2);
		ptSwitch = newPointArray(poleCount);
		
		int x = ((flags & FLAG_SWAP_COIL) != 0)?1:0;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref coilPosts[0], x, openhs * 2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref coilPosts[1], x, openhs * 3);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref coilLeads[0], .5, openhs * 2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref coilLeads[1], .5, openhs * 3);
		
		// lines
		lines = newPointArray(poleCount * 2);
	}
	internal override System.Drawing.Point getPost(int n)
	{
		if (n < 3 * poleCount)
			return swposts[n / 3][n % 3];
		return coilPosts[n - 3 * poleCount];
	}
	internal override void  reset()
	{
		base.reset();
		ind.reset();
		coilCurrent = coilCurCount = 0;
		int i;
		for (i = 0; i != poleCount; i++)
			switchCurrent[i] = switchCurCount[i] = 0;
	}
	internal double a1, a2, a3, a4;
	internal override void  stamp()
	{
		// inductor from coil post 1 to internal node
		ind.stamp(nodes[nCoil1], nodes[nCoil3]);
		// resistor from internal node to coil post 2
		sim.stampResistor(nodes[nCoil3], nodes[nCoil2], coilR);
		
		int i;
		for (i = 0; i != poleCount * 3; i++)
			sim.stampNonLinear(nodes[nSwitch0 + i]);
	}
	internal override void  startIteration()
	{
		ind.startIteration(volts[nCoil1] - volts[nCoil3]);
		
		// magic value to balance operate speed with reset speed semi-realistically
		double magic = 1.3;
		double pmult = System.Math.Sqrt(magic + 1);
		double p = coilCurrent * pmult / onCurrent;
		d_position = System.Math.Abs(p * p) - 1.3;
		if (d_position < 0)
			d_position = 0;
		if (d_position > 1)
			d_position = 1;
		if (d_position < .1)
			i_position = 0;
		else if (d_position > .9)
			i_position = 1;
		else
			i_position = 2;
		//System.out.println("ind " + this + " " + current + " " + voltdiff);
	}
	
	// we need this to be able to change the matrix for each step
	internal override bool nonLinear()
	{
		return true;
	}
	
	internal override void  doStep()
	{
		double voltdiff = volts[nCoil1] - volts[nCoil3];
		ind.doStep(voltdiff);
		int p;
		for (p = 0; p != poleCount * 3; p += 3)
		{
			sim.stampResistor(nodes[nSwitch0 + p], nodes[nSwitch1 + p], i_position == 0?r_on:r_off);
			sim.stampResistor(nodes[nSwitch0 + p], nodes[nSwitch2 + p], i_position == 1?r_on:r_off);
		}
	}
	internal override void  calculateCurrent()
	{
		double voltdiff = volts[nCoil1] - volts[nCoil3];
		coilCurrent = ind.calculateCurrent(voltdiff);
		
		// actually this isn't correct, since there is a small amount
		// of current through the switch when off
		int p;
		for (p = 0; p != poleCount; p++)
		{
			if (i_position == 2)
				switchCurrent[p] = 0;
			else
				switchCurrent[p] = (volts[nSwitch0 + p * 3] - volts[nSwitch1 + p * 3 + i_position]) / r_on;
		}
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = i_position == 0?"реле (выкл)":(i_position == 1?"реле (вкл)":"реле");
		int i;
		int ln = 1;
		for (i = 0; i != poleCount; i++)
			arr[ln++] = "I" + (i + 1) + " = " + getCurrentDText(switchCurrent[i]);
		arr[ln++] = "обмотка I = " + getCurrentDText(coilCurrent);
		arr[ln++] = "обмотка Vd = " + getVoltageDText(volts[nCoil1] - volts[nCoil2]);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Индуктивность (Гн)", inductance, 0, 0);
		if (n == 1)
			return new EditInfo("Сопротивление вкл (Ом)", r_on, 0, 0);
		if (n == 2)
			return new EditInfo("Сопротивление выкл (Ом)", r_off, 0, 0);
		if (n == 3)
			return new EditInfo("Ток вкл (A)", onCurrent, 0, 0);
		if (n == 4)
			return new EditInfo("Количество контактных групп", poleCount, 1, 4).setDimensionless();
		if (n == 5)
			return new EditInfo("Сопротивление обмотки (Ом)", coilR, 0, 0);
		if (n == 6)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Поменять направление обмотки", (flags & FLAG_SWAP_COIL) != 0);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0 && ei.value_Renamed > 0)
		{
			inductance = ei.value_Renamed;
			ind.setup(inductance, coilCurrent, Inductor.FLAG_BACK_EULER);
		}
		if (n == 1 && ei.value_Renamed > 0)
			r_on = ei.value_Renamed;
		if (n == 2 && ei.value_Renamed > 0)
			r_off = ei.value_Renamed;
		if (n == 3 && ei.value_Renamed > 0)
			onCurrent = ei.value_Renamed;
		if (n == 4 && ei.value_Renamed >= 1)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			poleCount = (int) ei.value_Renamed;
			setPoints();
		}
		if (n == 5 && ei.value_Renamed > 0)
			coilR = ei.value_Renamed;
		if (n == 6)
		{
			if (ei.checkbox.Checked)
				flags |= FLAG_SWAP_COIL;
			else
				flags &= ~ FLAG_SWAP_COIL;
			setPoints();
		}
	}
	internal override bool getConnection(int n1, int n2)
	{
		return (n1 / 3 == n2 / 3);
	}
}