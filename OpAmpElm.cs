using System;

class OpAmpElm:CircuitElm
{
	override internal double Power
	{
		get
		{
			return volts[2] * current;
		}
		
	}
	virtual internal int Size
	{
		set
		{
			opsize = value;
			opheight = 8 * value;
			opwidth = 13 * value;
			flags = (flags & ~ FLAG_SMALL) | ((value == 1)?FLAG_SMALL:0);
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	override internal int VoltageSourceCount
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
			return volts[2] - volts[1];
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 'a';
		}
		
	}
	internal int opsize, opheight, opwidth, opaddtext;
	internal double maxOut, minOut, gain, gbw;
	internal bool reset;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_SWAP '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_SWAP = 1;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_SMALL '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_SMALL = 2;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_LOWGAIN '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_LOWGAIN = 4;
	public OpAmpElm(int xx, int yy):base(xx, yy)
	{
		noDiagonal = true;
		maxOut = 15;
		minOut = - 15;
		gbw = 1e6;
		Size = sim.smallGridCheckItem.Checked?1:2;
		setGain();
	}
	public OpAmpElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		maxOut = 15;
		minOut = - 15;
		// GBW has no effect in this version of the simulator, but we
		// retain it to keep the file format the same
		gbw = 1e6;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			maxOut = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			minOut = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			gbw = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
		noDiagonal = true;
		Size = (f & FLAG_SMALL) != 0?1:2;
		setGain();
	}
	internal virtual void  setGain()
	{
		// gain of 100000 breaks e-amp-dfdx.txt
		// gain was 1000, but it broke amp-schmitt.txt
		gain = ((flags & FLAG_LOWGAIN) != 0)?1000:100000;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + maxOut + " " + minOut + " " + gbw;
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, opheight * 2);
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref in1p[0], ref in1p[1]);
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref in2p[0], ref in2p[1]);
		SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
		setPowerColor(g, true);
		drawThickPolygon(g, triangle);
		SupportClass.GraphicsManager.manager.SetFont(g, plusFont);
		drawCenteredText(g, "-", textp[0].X, textp[0].Y - 2, true);
		drawCenteredText(g, "+", textp[1].X, textp[1].Y, true);
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead2, ref point2);
		curcount = updateDotCount(current, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref point2, ref lead2, curcount);
		drawPosts(g);
	}
	internal System.Drawing.Point[] in1p, in2p, textp;
	internal System.Drawing.Drawing2D.GraphicsPath triangle;
	internal System.Drawing.Font plusFont;
	internal override void  setPoints()
	{
		base.setPoints();
		if (dn > 150 && this == sim.dragElm)
			Size = 2;
		int ww = opwidth;
		if (ww > dn / 2)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			ww = (int) (dn / 2);
		}
		calcLeads(ww * 2);
		int hs = opheight * dsign;
		if ((flags & FLAG_SWAP) != 0)
			hs = - hs;
		in1p = newPointArray(2);
		in2p = newPointArray(2);
		textp = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref in1p[0], ref in2p[0], 0, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref in1p[1], ref in2p[1], 0, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref textp[0], ref textp[1], .2, hs);
		System.Drawing.Point[] tris = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref tris[0], ref tris[1], 0, hs * 2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		triangle = createPolygon(ref tris[0], ref tris[1], ref lead2);
		//UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
		plusFont = new System.Drawing.Font("SansSerif", opsize == 2?14:10, System.Drawing.FontStyle.Regular);
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?in1p[0]:((n == 1)?in2p[0]:point2);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "операц. усилитель";
		arr[1] = "V+ = " + getVoltageText(volts[1]);
		arr[2] = "V- = " + getVoltageText(volts[0]);
		// sometimes the voltage goes slightly outside range, to make
		// convergence easier.  so we hide that here.
		double vo = System.Math.Max(System.Math.Min(volts[2], maxOut), minOut);
		arr[3] = "Vвых = " + getVoltageText(vo);
		arr[4] = "Iвых = " + getCurrentText(getCurrent());
		arr[5] = "диапазон = " + getVoltageText(minOut) + " до " + getVoltageText(maxOut);
	}
	
	internal double lastvd;
	
	internal override void  stamp()
	{
		int vn = sim.nodeList.Count + voltSource;
		sim.stampNonLinear(vn);
		sim.stampMatrix(nodes[2], vn, 1);
	}
	internal override void  doStep()
	{
		double vd = volts[1] - volts[0];
		if (System.Math.Abs(lastvd - vd) > .1)
			sim.converged = false;
		else if (volts[2] > maxOut + .1 || volts[2] < minOut - .1)
			sim.converged = false;
		double x = 0;
		int vn = sim.nodeList.Count + voltSource;
		double dx = 0;
		if (vd >= maxOut / gain && (lastvd >= 0 || sim.getrand(4) == 1))
		{
			dx = 1e-4;
			x = maxOut - dx * maxOut / gain;
		}
		else if (vd <= minOut / gain && (lastvd <= 0 || sim.getrand(4) == 1))
		{
			dx = 1e-4;
			x = minOut - dx * minOut / gain;
		}
		else
			dx = gain;
		//System.out.println("opamp " + vd + " " + volts[2] + " " + dx + " "  + x + " " + lastvd + " " + sim.converged);
		
		// newton-raphson
		sim.stampMatrix(vn, nodes[0], dx);
		sim.stampMatrix(vn, nodes[1], - dx);
		sim.stampMatrix(vn, nodes[2], 1);
		sim.stampRightSide(vn, x);
		
		lastvd = vd;
		/*if (sim.converged)
		System.out.println((volts[1]-volts[0]) + " " + volts[2] + " " + initvd);*/
	}
	// there is no current path through the op-amp inputs, but there
	// is an indirect path through the output to ground.
	internal override bool getConnection(int n1, int n2)
	{
		return false;
	}
	internal override bool hasGroundConnection(int n1)
	{
		return (n1 == 2);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Максимальное вых. напряжение (В)", maxOut, 1, 20);
		if (n == 1)
			return new EditInfo("Минимальное вых. напряженеие (В)", minOut, - 20, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			maxOut = ei.value_Renamed;
		if (n == 1)
			minOut = ei.value_Renamed;
	}
}