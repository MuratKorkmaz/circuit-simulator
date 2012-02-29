using System;

abstract class GateElm:CircuitElm
{
	virtual internal bool Inverting
	{
		get
		{
			return false;
		}
		
	}
	virtual internal int Size
	{
		set
		{
			gsize = value;
			gwidth = 7 * value;
			gwidth2 = 14 * value;
			gheight = 8 * value;
			flags = (value == 1)?FLAG_SMALL:0;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return inputCount + 1;
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 1;
		}
		
	}
	internal abstract System.String GateName{get;}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_SMALL '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_SMALL = 1;
	internal int inputCount = 2;
	internal bool lastOutput;
	
	public GateElm(int xx, int yy):base(xx, yy)
	{
		noDiagonal = true;
		inputCount = 2;
		Size = sim.smallGridCheckItem.Checked?1:2;
	}
	public GateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		inputCount = System.Int32.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		lastOutput = System.Double.Parse(st.NextToken()) > 2.5;
		noDiagonal = true;
		Size = (f & FLAG_SMALL) != 0?1:2;
	}
	internal int gsize, gwidth, gwidth2, gheight, hs2;
	internal override System.String dump()
	{
		return base.dump() + " " + inputCount + " " + volts[inputCount];
	}
	internal System.Drawing.Point[] inPosts, inGates;
	internal int ww;
	internal override void  setPoints()
	{
		base.setPoints();
		if (dn > 150 && this == sim.dragElm)
			Size = 2;
		int hs = gheight;
		int i;
		ww = gwidth2; // was 24
		if (ww > dn / 2)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			ww = (int) (dn / 2);
		}
		if (Inverting && ww + 8 > dn / 2)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			ww = (int) (dn / 2 - 8);
		}
		calcLeads(ww * 2);
		inPosts = new System.Drawing.Point[inputCount];
		inGates = new System.Drawing.Point[inputCount];
		allocNodes();
		int i0 = (- inputCount) / 2;
		for (i = 0; i != inputCount; i++, i0++)
		{
			if (i0 == 0 && (inputCount & 1) == 0)
				i0++;
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			inPosts[i] = interpPoint(ref point1, ref point2, 0, hs * i0);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			inGates[i] = interpPoint(ref lead1, ref lead2, 0, hs * i0);
			volts[i] = (lastOutput ^ Inverting)?5:0;
		}
		hs2 = gwidth * (inputCount / 2 + 1);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs2);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		int i;
		for (i = 0; i != inputCount; i++)
		{
			setVoltageColor(g, volts[i]);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref inPosts[i], ref inGates[i]);
		}
		setVoltageColor(g, volts[inputCount]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead2, ref point2);
		SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
		drawThickPolygon(g, gatePoly);
		if (linePoints != null)
			for (i = 0; i != linePoints.Length - 1; i++)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				drawThickLine(g, ref linePoints[i], ref linePoints[i + 1]);
			}
		if (Inverting)
			drawThickCircle(g, pcircle.X, pcircle.Y, 3);
		curcount = updateDotCount(current, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref lead2, ref point2, curcount);
		drawPosts(g);
	}
	internal System.Drawing.Drawing2D.GraphicsPath gatePoly;
	internal System.Drawing.Point pcircle;
	internal System.Drawing.Point[] linePoints;
	internal override System.Drawing.Point getPost(int n)
	{
		if (n == inputCount)
			return point2;
		return inPosts[n];
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = GateName;
		arr[1] = "Vout = " + getVoltageText(volts[inputCount]);
		arr[2] = "Iout = " + getCurrentText(getCurrent());
	}
	internal override void  stamp()
	{
		sim.stampVoltageSource(0, nodes[inputCount], voltSource);
	}
	internal virtual bool getInput(int x)
	{
		return volts[x] > 2.5;
	}
	internal abstract bool calcFunction();
	internal override void  doStep()
	{
		int i;
		bool f = calcFunction();
		if (Inverting)
			f = !f;
		lastOutput = f;
		double res = f?5:0;
		sim.updateVoltageSource(0, nodes[inputCount], voltSource, res);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("количество входов", inputCount, 1, 8).setDimensionless();
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
		inputCount = (int) ei.value_Renamed;
		setPoints();
	}
	// there is no current path through the gate inputs, but there
	// is an indirect path through the output to ground.
	internal override bool getConnection(int n1, int n2)
	{
		return false;
	}
	internal override bool hasGroundConnection(int n1)
	{
		return (n1 == inputCount);
	}
}