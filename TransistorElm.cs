using System;

class TransistorElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 't';
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	override internal double Power
	{
		get
		{
			return (volts[0] - volts[2]) * ib + (volts[1] - volts[2]) * ic;
		}
		
	}
	internal int pnp;
	internal double beta;
	internal double fgain;
	internal double gmin;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_FLIP '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_FLIP = 1;
	internal TransistorElm(int xx, int yy, bool pnpflag):base(xx, yy)
	{
		pnp = (pnpflag)?- 1:1;
		beta = 100;
		setup();
	}
	public TransistorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		pnp = System.Int32.Parse(st.NextToken());
		beta = 100;
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			lastvbe = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			lastvbc = System.Double.Parse(st.NextToken());
			volts[0] = 0;
			volts[1] = - lastvbe;
			volts[2] = - lastvbc;
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			beta = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
		setup();
	}
	internal virtual void  setup()
	{
		vcrit = vt * System.Math.Log(vt / (System.Math.Sqrt(2) * leakage));
		fgain = beta / (beta + 1);
		noDiagonal = true;
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void  reset()
	{
		volts[0] = volts[1] = volts[2] = 0;
		lastvbc = lastvbe = curcount_c = curcount_e = curcount_b = 0;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + pnp + " " + (volts[0] - volts[1]) + " " + (volts[0] - volts[2]) + " " + beta;
	}
	internal double ic, ie, ib, curcount_c, curcount_e, curcount_b;
	internal System.Drawing.Drawing2D.GraphicsPath rectPoly, arrowPoly;
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, 16);
		setPowerColor(g, true);
		// draw collector
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref coll[0], ref coll[1]);
		// draw emitter
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref emit[0], ref emit[1]);
		// draw arrow
		SupportClass.GraphicsManager.manager.SetColor(g, lightGrayColor);
		g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrowPoly);
		// draw base
		setVoltageColor(g, volts[0]);
		if (sim.powerCheckItem.Checked)
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref base_Renamed);
		// draw dots
		curcount_b = updateDotCount(- ib, curcount_b);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref base_Renamed, ref point1, curcount_b);
		curcount_c = updateDotCount(- ic, curcount_c);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref coll[1], ref coll[0], curcount_c);
		curcount_e = updateDotCount(- ie, curcount_e);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref emit[1], ref emit[0], curcount_e);
		// draw base rectangle
		setVoltageColor(g, volts[0]);
		setPowerColor(g, true);
		g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), rectPoly);
		
		if ((needsHighlight() || sim.dragElm == this) && dy == 0)
		{
			SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.White);
			SupportClass.GraphicsManager.manager.SetFont(g, unitsFont);
			int ds = sign(dx);
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString("Б", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), base_Renamed.X - 10 * ds, base_Renamed.Y - 5 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString("К", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), coll[0].X - 3 + 9 * ds, coll[0].Y + 4 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight()); // x+6 if ds=1, -12 if -1
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString("Э", SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), emit[0].X - 3 + 9 * ds, emit[0].Y + 4 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
		}
		drawPosts(g);
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:((n == 1)?coll[0]:emit[0]);
	}
	
	internal System.Drawing.Point[] rect, coll, emit;
	internal System.Drawing.Point base_Renamed;
	internal override void  setPoints()
	{
		base.setPoints();
		int hs = 16;
		if ((flags & FLAG_FLIP) != 0)
			dsign = - dsign;
		int hs2 = hs * dsign * pnp;
		// calc collector, emitter posts
		coll = newPointArray(2);
		emit = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref coll[0], ref emit[0], 1, hs2);
		// calc rectangle edges
		rect = newPointArray(4);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref rect[0], ref rect[1], 1 - 16 / dn, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref rect[2], ref rect[3], 1 - 13 / dn, hs);
		// calc points where collector/emitter leads contact rectangle
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref coll[1], ref emit[1], 1 - 13 / dn, 6 * dsign * pnp);
		// calc point where base lead contacts rectangle
		base_Renamed = new System.Drawing.Point(0, 0);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref point1, ref point2, ref base_Renamed, 1 - 16 / dn);
		
		// rectangle
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		rectPoly = createPolygon(ref rect[0], ref rect[2], ref rect[3], ref rect[1]);
		
		// arrow
		if (pnp == 1)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			arrowPoly = calcArrow(ref emit[1], ref emit[0], 8, 4);
		}
		else
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			System.Drawing.Point pt = interpPoint(ref point1, ref point2, 1 - 11 / dn, (- 5) * dsign * pnp);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			arrowPoly = calcArrow(ref emit[0], ref pt, 8, 4);
		}
	}
	
	internal const double leakage = 1e-13; // 1e-6;
	internal const double vt = .025;
	//UPGRADE_NOTE: Final was removed from the declaration of 'vdcoef '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal static readonly double vdcoef = 1 / vt;
	internal const double rgain = .5;
	internal double vcrit;
	internal double lastvbc, lastvbe;
	internal virtual double limitStep(double vnew, double vold)
	{
		double arg;
		double oo = vnew;
		
		if (vnew > vcrit && System.Math.Abs(vnew - vold) > (vt + vt))
		{
			if (vold > 0)
			{
				arg = 1 + (vnew - vold) / vt;
				if (arg > 0)
				{
					vnew = vold + vt * System.Math.Log(arg);
				}
				else
				{
					vnew = vcrit;
				}
			}
			else
			{
				vnew = vt * System.Math.Log(vnew / vt);
			}
			sim.converged = false;
			//System.out.println(vnew + " " + oo + " " + vold);
		}
		return (vnew);
	}
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
		sim.stampNonLinear(nodes[2]);
	}
	internal override void  doStep()
	{
		double vbc = volts[0] - volts[1]; // typically negative
		double vbe = volts[0] - volts[2]; // typically positive
		if (System.Math.Abs(vbc - lastvbc) > .01 || System.Math.Abs(vbe - lastvbe) > .01)
			sim.converged = false;
		gmin = 0;
		if (sim.subIterations > 100)
		{
			// if we have trouble converging, put a conductance in parallel with all P-N junctions.
			// Gradually increase the conductance value for each iteration.
			gmin = System.Math.Exp((- 9) * System.Math.Log(10) * (1 - sim.subIterations / 3000.0));
			if (gmin > .1)
				gmin = .1;
		}
		//System.out.print("T " + vbc + " " + vbe + "\n");
		vbc = pnp * limitStep(pnp * vbc, pnp * lastvbc);
		vbe = pnp * limitStep(pnp * vbe, pnp * lastvbe);
		lastvbc = vbc;
		lastvbe = vbe;
		double pcoef = vdcoef * pnp;
		double expbc = System.Math.Exp(vbc * pcoef);
		/*if (expbc > 1e13 || Double.isInfinite(expbc))
		expbc = 1e13;*/
		double expbe = System.Math.Exp(vbe * pcoef);
		if (expbe < 1)
			expbe = 1;
		/*if (expbe > 1e13 || Double.isInfinite(expbe))
		expbe = 1e13;*/
		ie = pnp * leakage * (- (expbe - 1) + rgain * (expbc - 1));
		ic = pnp * leakage * (fgain * (expbe - 1) - (expbc - 1));
		ib = - (ie + ic);
		//System.out.println("gain " + ic/ib);
		//System.out.print("T " + vbc + " " + vbe + " " + ie + " " + ic + "\n");
		double gee = (- leakage) * vdcoef * expbe;
		double gec = rgain * leakage * vdcoef * expbc;
		double gce = (- gee) * fgain;
		double gcc = (- gec) * (1 / rgain);
		
		/*System.out.print("gee = " + gee + "\n");
		System.out.print("gec = " + gec + "\n");
		System.out.print("gce = " + gce + "\n");
		System.out.print("gcc = " + gcc + "\n");
		System.out.print("gce+gcc = " + (gce+gcc) + "\n");
		System.out.print("gee+gec = " + (gee+gec) + "\n");*/
		
		// stamps from page 302 of Pillage.  Node 0 is the base,
		// node 1 the collector, node 2 the emitter.  Also stamp
		// minimum conductance (gmin) between b,e and b,c
		sim.stampMatrix(nodes[0], nodes[0], - gee - gec - gce - gcc + gmin * 2);
		sim.stampMatrix(nodes[0], nodes[1], gec + gcc - gmin);
		sim.stampMatrix(nodes[0], nodes[2], gee + gce - gmin);
		sim.stampMatrix(nodes[1], nodes[0], gce + gcc - gmin);
		sim.stampMatrix(nodes[1], nodes[1], - gcc + gmin);
		sim.stampMatrix(nodes[1], nodes[2], - gce);
		sim.stampMatrix(nodes[2], nodes[0], gee + gec - gmin);
		sim.stampMatrix(nodes[2], nodes[1], - gec);
		sim.stampMatrix(nodes[2], nodes[2], - gee + gmin);
		
		// we are solving for v(k+1), not delta v, so we use formula
		// 10.5.13, multiplying J by v(k)
		sim.stampRightSide(nodes[0], - ib - (gec + gcc) * vbc - (gee + gce) * vbe);
		sim.stampRightSide(nodes[1], - ic + gce * vbe + gcc * vbc);
		sim.stampRightSide(nodes[2], - ie + gee * vbe + gec * vbc);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "транзистор (" + ((pnp == - 1)?"PNP)":"NPN)") + " beta=" + showFormat.FormatDouble(beta);
		double vbc = volts[0] - volts[1];
		double vbe = volts[0] - volts[2];
		double vce = volts[1] - volts[2];
		if (vbc * pnp > .2)
			arr[1] = vbe * pnp > .2?"saturation":"reverse active";
		else
			arr[1] = vbe * pnp > .2?"fwd active":"cutoff";
		arr[2] = "Iк = " + getCurrentText(ic);
		arr[3] = "Iб = " + getCurrentText(ib);
		arr[4] = "Vбэ = " + getVoltageText(vbe);
		arr[5] = "Vбк = " + getVoltageText(vbc);
		arr[6] = "Vкэ = " + getVoltageText(vce);
	}
	internal override double getScopeValue(int x)
	{
		switch (x)
		{
			
			case Scope.VAL_IB:  return ib;
			
			case Scope.VAL_IC:  return ic;
			
			case Scope.VAL_IE:  return ie;
			
			case Scope.VAL_VBE:  return volts[0] - volts[2];
			
			case Scope.VAL_VBC:  return volts[0] - volts[1];
			
			case Scope.VAL_VCE:  return volts[1] - volts[2];
			}
		return 0;
	}
	internal override System.String getScopeUnits(int x)
	{
		switch (x)
		{
			
			case Scope.VAL_IB: 
			case Scope.VAL_IC: 
			case Scope.VAL_IE:  return "A";
			
			default:  return "V";
			
		}
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Beta/hFE", beta, 10, 1000).setDimensionless();
		if (n == 1)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Поменять местами Э/К", (flags & FLAG_FLIP) != 0);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		{
			beta = ei.value_Renamed;
			setup();
		}
		if (n == 1)
		{
			if (ei.checkbox.Checked)
				flags |= FLAG_FLIP;
			else
				flags &= ~ FLAG_FLIP;
			setPoints();
		}
	}
	internal override bool canViewInScope()
	{
		return true;
	}
}