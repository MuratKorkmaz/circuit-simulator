using System;

class SweepElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 170;
		}
		
	}
	override internal int PostCount
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
			return volts[0];
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 1;
		}
		
	}
	internal double maxV, maxF, minF, sweepTime, frequency;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_LOG '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_LOG = 1;
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_BIDIR '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_BIDIR = 2;
	
	public SweepElm(int xx, int yy):base(xx, yy)
	{
		minF = 20; maxF = 4000;
		maxV = 5;
		sweepTime = .1;
		flags = FLAG_BIDIR;
		reset();
	}
	public SweepElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		minF = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		maxF = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		maxV = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		sweepTime = System.Double.Parse(st.NextToken());
		reset();
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'circleSize '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int circleSize = 17;
	
	internal override System.String dump()
	{
		return base.dump() + " " + minF + " " + maxF + " " + maxV + " " + sweepTime;
	}
	internal override void  setPoints()
	{
		base.setPoints();
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		lead1 = interpPoint(ref point1, ref point2, 1 - circleSize / dn);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, circleSize);
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:System.Drawing.Color.Gray);
		setPowerColor(g, false);
		int xc = point2.X; int yc = point2.Y;
		drawThickCircle(g, xc, yc, circleSize);
		int wl = 8;
		adjustBbox(xc - circleSize, yc - circleSize, xc + circleSize, yc + circleSize);
		int i;
		int xl = 10;
		int ox = - 1, oy = - 1;
		long tm = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
		//double w = (this == mouseElm ? 3 : 2);
		tm %= 2000;
		if (tm > 1000)
			tm = 2000 - tm;
		double w = 1 + tm * .002;
		if (!sim.stoppedCheck.Checked)
			w = 1 + 2 * (frequency - minF) / (maxF - minF);
		for (i = - xl; i <= xl; i++)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int yy = yc + (int) (.95 * System.Math.Sin(i * pi * w / xl) * wl);
			if (ox != - 1)
				drawThickLine(g, ox, oy, xc + i, yy);
			ox = xc + i; oy = yy;
		}
		if (sim.showValuesCheckItem.Checked)
		{
			System.String s = getShortUnitText(frequency, "Hz");
			if (dx == 0 || dy == 0)
				drawValues(g, s, circleSize);
		}
		
		drawPosts(g);
		curcount = updateDotCount(- current, curcount);
		if (sim.dragElm != this)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point1, ref lead1, curcount);
		}
	}
	
	internal override void  stamp()
	{
		sim.stampVoltageSource(0, nodes[0], voltSource);
	}
	internal double fadd, fmul, freqTime, savedTimeStep;
	internal int dir = 1;
	internal virtual void  setParams()
	{
		if (frequency < minF || frequency > maxF)
		{
			frequency = minF;
			freqTime = 0;
			dir = 1;
		}
		if ((flags & FLAG_LOG) == 0)
		{
			fadd = dir * sim.timeStep * (maxF - minF) / sweepTime;
			fmul = 1;
		}
		else
		{
			fadd = 0;
			fmul = System.Math.Pow(maxF / minF, dir * sim.timeStep / sweepTime);
		}
		savedTimeStep = sim.timeStep;
	}
	internal override void  reset()
	{
		frequency = minF;
		freqTime = 0;
		dir = 1;
		setParams();
	}
	internal double v;
	internal override void  startIteration()
	{
		// has timestep been changed?
		if (sim.timeStep != savedTimeStep)
			setParams();
		v = System.Math.Sin(freqTime) * maxV;
		freqTime += frequency * 2 * pi * sim.timeStep;
		frequency = frequency * fmul + fadd;
		if (frequency >= maxF && dir == 1)
		{
			if ((flags & FLAG_BIDIR) != 0)
			{
				fadd = - fadd;
				fmul = 1 / fmul;
				dir = - 1;
			}
			else
				frequency = minF;
		}
		if (frequency <= minF && dir == - 1)
		{
			fadd = - fadd;
			fmul = 1 / fmul;
			dir = 1;
		}
	}
	internal override void  doStep()
	{
		sim.updateVoltageSource(0, nodes[0], voltSource, v);
	}
	internal override bool hasGroundConnection(int n1)
	{
		return true;
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "свип " + (((flags & FLAG_LOG) == 0)?"(линейн.)":"(логарифм.)");
		arr[1] = "I = " + getCurrentDText(getCurrent());
		arr[2] = "V = " + getVoltageText(volts[0]);
		arr[3] = "f = " + getUnitText(frequency, "Гц");
		arr[4] = "диап. = " + getUnitText(minF, "Гц") + " .. " + getUnitText(maxF, "Гц");
		arr[5] = "время = " + getUnitText(sweepTime, "с");
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
			return new EditInfo("Мин. частота (Гц)", minF, 0, 0);
		if (n == 1)
			return new EditInfo("макс. частота (Гц)", maxF, 0, 0);
		if (n == 2)
			return new EditInfo("Период свипа (с)", sweepTime, 0, 0);
		if (n == 3)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Логарифмич.", (flags & FLAG_LOG) != 0);
			return ei;
		}
		if (n == 4)
			return new EditInfo("Макс. напряжение", maxV, 0, 0);
		if (n == 5)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Двунаправленн.", (flags & FLAG_BIDIR) != 0);
			return ei;
		}
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		double maxfreq = 1 / (8 * sim.timeStep);
		if (n == 0)
		{
			minF = ei.value_Renamed;
			if (minF > maxfreq)
				minF = maxfreq;
		}
		if (n == 1)
		{
			maxF = ei.value_Renamed;
			if (maxF > maxfreq)
				maxF = maxfreq;
		}
		if (n == 2)
			sweepTime = ei.value_Renamed;
		if (n == 3)
		{
			flags &= ~ FLAG_LOG;
			if (ei.checkbox.Checked)
				flags |= FLAG_LOG;
		}
		if (n == 4)
			maxV = ei.value_Renamed;
		if (n == 5)
		{
			flags &= ~ FLAG_BIDIR;
			if (ei.checkbox.Checked)
				flags |= FLAG_BIDIR;
		}
		setParams();
	}
}