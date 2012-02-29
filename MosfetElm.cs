using System;

class MosfetElm:CircuitElm
{
	virtual internal double DefaultThreshold
	{
		get
		{
			return 1.5;
		}
		
	}
	virtual internal double Beta
	{
		get
		{
			return .02;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 'f';
		}
		
	}
	override internal double Power
	{
		get
		{
			return ids * (volts[2] - volts[1]);
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	override internal double VoltageDiff
	{
		get
		{
			return volts[2] - volts[1];
		}
		
	}
	internal int pnp;
	internal int FLAG_PNP = 1;
	internal int FLAG_SHOWVT = 2;
	internal int FLAG_DIGITAL = 4;
	internal double vt;
	internal MosfetElm(int xx, int yy, bool pnpflag):base(xx, yy)
	{
		pnp = (pnpflag)?- 1:1;
		flags = (pnpflag)?FLAG_PNP:0;
		noDiagonal = true;
		vt = DefaultThreshold;
	}
	public MosfetElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		pnp = ((f & FLAG_PNP) != 0)?- 1:1;
		noDiagonal = true;
		vt = DefaultThreshold;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			vt = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal virtual bool drawDigital()
	{
		return (flags & FLAG_DIGITAL) != 0;
	}
	internal override void  reset()
	{
		lastv1 = lastv2 = volts[0] = volts[1] = volts[2] = curcount = 0;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + vt;
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'hs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int hs = 16;
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref src[0], ref src[1]);
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref drn[0], ref drn[1]);
		int segments = 6;
		int i;
		setPowerColor(g, true);
		double segf = 1.0 / segments;
		for (i = 0; i != segments; i++)
		{
			double v = volts[1] + (volts[2] - volts[1]) * i / segments;
			setVoltageColor(g, v);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref src[1], ref drn[1], ref ps1, i * segf);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref src[1], ref drn[1], ref ps2, (i + 1) * segf);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawThickLine(g, ref ps1, ref ps2);
		}
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref src[1], ref src[2]);
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref drn[1], ref drn[2]);
		if (!drawDigital())
		{
			setVoltageColor(g, pnp == 1?volts[1]:volts[2]);
			g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrowPoly);
		}
		if (sim.powerCheckItem.Checked)
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref gate[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref gate[0], ref gate[2]);
		if (drawDigital() && pnp == - 1)
			drawThickCircle(g, pcircle.X, pcircle.Y, pcircler);
		if ((flags & FLAG_SHOWVT) != 0)
		{
			System.String s = "" + (vt * pnp);
			SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
			SupportClass.GraphicsManager.manager.SetFont(g, unitsFont);
			drawCenteredText(g, s, x2 + 2, y2, false);
		}
		if ((needsHighlight() || sim.dragElm == this) && dy == 0)
		{
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.White);
			SupportClass.GraphicsManager.manager.SetFont(g, unitsFont);
			int ds = sign(dx);
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString("G", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), gate[1].X - 10 * ds, gate[1].Y - 5 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(pnp == - 1?"D":"S", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), src[0].X - 3 + 9 * ds, src[0].Y + 4 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight()); // x+6 if ds=1, -12 if -1
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(pnp == - 1?"S":"D", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), drn[0].X - 3 + 9 * ds, drn[0].Y + 4 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
		}
		curcount = updateDotCount(- ids, curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref src[0], ref src[1], curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref src[1], ref drn[1], curcount);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref drn[1], ref drn[0], curcount);
		drawPosts(g);
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:((n == 1)?src[0]:drn[0]);
	}
	internal override double getCurrent()
	{
		return ids;
	}
	
	internal int pcircler;
	internal System.Drawing.Point[] src, drn, gate;
	internal System.Drawing.Point pcircle;
	internal System.Drawing.Drawing2D.GraphicsPath arrowPoly;
	
	internal override void  setPoints()
	{
		base.setPoints();
		
		// find the coordinates of the various points we need to draw
		// the MOSFET.
		int hs2 = hs * dsign;
		src = newPointArray(3);
		drn = newPointArray(3);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref src[0], ref drn[0], 1, - hs2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref src[1], ref drn[1], 1 - 22 / dn, - hs2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref src[2], ref drn[2], 1 - 22 / dn, (- hs2) * 4 / 3);
		
		gate = newPointArray(3);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref gate[0], ref gate[2], 1 - 28 / dn, hs2 / 2); // was 1-20/dn
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref gate[0], ref gate[2], ref gate[1], .5);
		
		if (!drawDigital())
		{
			if (pnp == 1)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				arrowPoly = calcArrow(ref src[1], ref src[0], 10, 4);
			}
			else
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				arrowPoly = calcArrow(ref drn[0], ref drn[1], 12, 5);
			}
		}
		else if (pnp == - 1)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref point1, ref point2, ref gate[1], 1 - 36 / dn);
			int dist = (dsign < 0)?32:31;
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			pcircle = interpPoint(ref point1, ref point2, 1 - dist / dn);
			pcircler = 3;
		}
	}
	
	internal double lastv1, lastv2;
	internal double ids;
	internal int mode = 0;
	internal double gm = 0;
	
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[1]);
		sim.stampNonLinear(nodes[2]);
	}
	internal override void  doStep()
	{
		double[] vs = new double[3];
		vs[0] = volts[0];
		vs[1] = volts[1];
		vs[2] = volts[2];
		if (vs[1] > lastv1 + .5)
			vs[1] = lastv1 + .5;
		if (vs[1] < lastv1 - .5)
			vs[1] = lastv1 - .5;
		if (vs[2] > lastv2 + .5)
			vs[2] = lastv2 + .5;
		if (vs[2] < lastv2 - .5)
			vs[2] = lastv2 - .5;
		int source = 1;
		int drain = 2;
		if (pnp * vs[1] > pnp * vs[2])
		{
			source = 2;
			drain = 1;
		}
		int gate = 0;
		double vgs = vs[gate] - vs[source];
		double vds = vs[drain] - vs[source];
		if (System.Math.Abs(lastv1 - vs[1]) > .01 || System.Math.Abs(lastv2 - vs[2]) > .01)
			sim.converged = false;
		lastv1 = vs[1];
		lastv2 = vs[2];
		double realvgs = vgs;
		double realvds = vds;
		vgs *= pnp;
		vds *= pnp;
		ids = 0;
		gm = 0;
		double Gds = 0;
		double beta = Beta;
		if (vgs > .5 && this is JfetElm)
		{
			sim.stop("JFET is reverse biased!", this);
			return ;
		}
		if (vgs < vt)
		{
			// should be all zero, but that causes a singular matrix,
			// so instead we treat it as a large resistor
			Gds = 1e-8;
			ids = vds * Gds;
			mode = 0;
		}
		else if (vds < vgs - vt)
		{
			// linear
			ids = beta * ((vgs - vt) * vds - vds * vds * .5);
			gm = beta * vds;
			Gds = beta * (vgs - vds - vt);
			mode = 1;
		}
		else
		{
			// saturation; Gds = 0
			gm = beta * (vgs - vt);
			// use very small Gds to avoid nonconvergence
			Gds = 1e-8;
			ids = .5 * beta * (vgs - vt) * (vgs - vt) + (vds - (vgs - vt)) * Gds;
			mode = 2;
		}
		double rs = (- pnp) * ids + Gds * realvds + gm * realvgs;
		//System.out.println("M " + vds + " " + vgs + " " + ids + " " + gm + " "+ Gds + " " + volts[0] + " " + volts[1] + " " + volts[2] + " " + source + " " + rs + " " + this);
		sim.stampMatrix(nodes[drain], nodes[drain], Gds);
		sim.stampMatrix(nodes[drain], nodes[source], - Gds - gm);
		sim.stampMatrix(nodes[drain], nodes[gate], gm);
		
		sim.stampMatrix(nodes[source], nodes[drain], - Gds);
		sim.stampMatrix(nodes[source], nodes[source], Gds + gm);
		sim.stampMatrix(nodes[source], nodes[gate], - gm);
		
		sim.stampRightSide(nodes[drain], rs);
		sim.stampRightSide(nodes[source], - rs);
		if (source == 2 && pnp == 1 || source == 1 && pnp == - 1)
			ids = - ids;
	}
	internal virtual void  getFetInfo(System.String[] arr, System.String n)
	{
		arr[0] = ((pnp == - 1)?"p-":"n-") + n;
		arr[0] += (" (Vt = " + getVoltageText(pnp * vt) + ")");
		arr[1] = ((pnp == 1)?"Ids = ":"Isd = ") + getCurrentText(ids);
		arr[2] = "Vgs = " + getVoltageText(volts[0] - volts[pnp == - 1?2:1]);
		arr[3] = ((pnp == 1)?"Vds = ":"Vsd = ") + getVoltageText(volts[2] - volts[1]);
		arr[4] = (mode == 0)?"off":((mode == 1)?"linear":"saturation");
		arr[5] = "gm = " + getUnitText(gm, "A/В");
	}
	internal override void  getInfo(System.String[] arr)
	{
		getFetInfo(arr, "MOSFET");
	}
	internal override bool canViewInScope()
	{
		return true;
	}
	internal override bool getConnection(int n1, int n2)
	{
		return !(n1 == 0 || n2 == 0);
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Пороговое напряжение", pnp * vt, .01, 5);
		if (n == 1)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Цифровой символ", drawDigital());
			return ei;
		}
		
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
			vt = pnp * ei.value_Renamed;
		if (n == 1)
		{
			flags = (ei.checkbox.Checked)?(flags | FLAG_DIGITAL):(flags & ~ FLAG_DIGITAL);
			setPoints();
		}
	}
}