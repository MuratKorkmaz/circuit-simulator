using System;


	internal class OpAmpElm : CircuitElm
	{
	internal int opsize, opheight, opwidth, opaddtext;
	internal double maxOut, minOut, gain, gbw;
	internal bool reset;
	internal readonly int FLAG_SWAP = 1;
	internal readonly int FLAG_SMALL = 2;
	internal readonly int FLAG_LOWGAIN = 4;
	public OpAmpElm(int xx, int yy) : base(xx, yy)
	{
		noDiagonal = true;
		maxOut = 15;
		minOut = -15;
		gbw = 1e6;
		Size = sim.smallGridCheckItem.State ? 1 : 2;
		setGain();
	}
	public OpAmpElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
		maxOut = 15;
		minOut = -15;
		// GBW has no effect in this version of the simulator, but we
		// retain it to keep the file format the same
		gbw = 1e6;
		try
		{
		maxOut = new (double)double?(st.nextToken());
		minOut = new (double)double?(st.nextToken());
		gbw = new (double)double?(st.nextToken());
		}
		catch (Exception e)
		{
		}
		noDiagonal = true;
		Size = (f & FLAG_SMALL) != 0 ? 1 : 2;
		setGain();
	}
	internal virtual void setGain()
	{
		// gain of 100000 breaks e-amp-dfdx.txt
		// gain was 1000, but it broke amp-schmitt.txt
		gain = ((flags & FLAG_LOWGAIN) != 0) ? 1000 : 100000;

	}
	internal override string dump()
	{
		return base.dump() + " " + maxOut + " " + minOut + " " + gbw;
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void draw(Graphics g)
	{
		setBbox(point1, point2, opheight*2);
		setVoltageColor(g, volts[0]);
		drawThickLine(g, in1p[0], in1p[1]);
		setVoltageColor(g, volts[1]);
		drawThickLine(g, in2p[0], in2p[1]);
		g.Color = needsHighlight() ? selectColor : lightGrayColor;
		setPowerColor(g, true);
		drawThickPolygon(g, triangle);
		g.Font = plusFont;
		drawCenteredText(g, "-", textp[0].x, textp[0].y-2, true);
		drawCenteredText(g, "+", textp[1].x, textp[1].y, true);
		setVoltageColor(g, volts[2]);
		drawThickLine(g, lead2, point2);
		curcount = updateDotCount(current, curcount);
		drawDots(g, point2, lead2, curcount);
		drawPosts(g);
	}
	internal override double Power
	{
		get
		{
			return volts[2]*current;
		}
	}
	internal Point[] in1p; internal Point[] in2p; internal Point[] textp;
	internal Polygon triangle;
	internal Font plusFont;
	internal virtual int Size
	{
		set
		{
			opsize = value;
			opheight = 8*value;
			opwidth = 13*value;
			flags = (flags & ~FLAG_SMALL) | ((value == 1) ? FLAG_SMALL : 0);
		}
	}
	internal override void setPoints()
	{
		base.setPoints();
		if (dn > 150 && this == sim.dragElm)
		Size = 2;
		int ww = opwidth;
		if (ww > dn/2)
		ww = (int)(dn/2);
		calcLeads(ww*2);
		int hs = opheight*dsign;
		if ((flags & FLAG_SWAP) != 0)
		hs = -hs;
		in1p = newPointArray(2);
		in2p = newPointArray(2);
		textp = newPointArray(2);
		interpPoint2(point1, point2, in1p[0], in2p[0], 0, hs);
		interpPoint2(lead1, lead2, in1p[1], in2p[1], 0, hs);
		interpPoint2(lead1, lead2, textp[0], textp[1],.2, hs);
		Point[] tris = newPointArray(2);
		interpPoint2(lead1, lead2, tris[0], tris[1], 0, hs*2);
		triangle = createPolygon(tris[0], tris[1], lead2);
		plusFont = new Font("SansSerif", 0, opsize == 2 ? 14 : 10);
	}
	internal override int PostCount
	{
		get
		{
			return 3;
		}
	}
	internal override Point getPost(int n)
	{
		return (n == 0) ? in1p[0] : (n == 1) ? in2p[0] : point2;
	}
	internal override int VoltageSourceCount
	{
		get
		{
			return 1;
		}
	}
	internal override void getInfo(string[] arr)
	{
		arr[0] = "операц. усилитель";
		arr[1] = "V+ = " + getVoltageText(volts[1]);
		arr[2] = "V- = " + getVoltageText(volts[0]);
		// sometimes the voltage goes slightly outside range, to make
		// convergence easier.  so we hide that here.
		double vo = Math.Max(Math.Min(volts[2], maxOut), minOut);
		arr[3] = "Vвых = " + getVoltageText(vo);
		arr[4] = "Iвых = " + getCurrentText(Current);
		arr[5] = "диапазон = " + getVoltageText(minOut) + " до " + getVoltageText(maxOut);
	}

	internal double lastvd;

	internal override void stamp()
	{
		int vn = sim.nodeList.Count+voltSource;
		sim.stampNonLinear(vn);
		sim.stampMatrix(nodes[2], vn, 1);
	}
	internal override void doStep()
	{
		double vd = volts[1] - volts[0];
		if (Math.Abs(lastvd-vd) >.1)
		sim.converged = false;
		else if (volts[2] > maxOut+.1 || volts[2] < minOut-.1)
		sim.converged = false;
		double x = 0;
		int vn = sim.nodeList.Count+voltSource;
		double dx = 0;
		if (vd >= maxOut/gain && (lastvd >= 0 || sim.getrand(4) == 1))
		{
		dx = 1e-4;
		x = maxOut - dx*maxOut/gain;
		}
		else if (vd <= minOut/gain && (lastvd <= 0 || sim.getrand(4) == 1))
		{
		dx = 1e-4;
		x = minOut - dx*minOut/gain;
		}
		else
		dx = gain;
		//System.out.println("opamp " + vd + " " + volts[2] + " " + dx + " "  + x + " " + lastvd + " " + sim.converged);

		// newton-raphson
		sim.stampMatrix(vn, nodes[0], dx);
		sim.stampMatrix(vn, nodes[1], -dx);
		sim.stampMatrix(vn, nodes[2], 1);
		sim.stampRightSide(vn, x);

		lastvd = vd;
//	    if (sim.converged)
//	      System.out.println((volts[1]-volts[0]) + " " + volts[2] + " " + initvd);
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
	internal override double VoltageDiff
	{
		get
		{
			return volts[2] - volts[1];
		}
	}
	internal override int DumpType
	{
		get
		{
			return 'a';
		}
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		return new EditInfo("Максимальное вых. напряжение (В)", maxOut, 1, 20);
		if (n == 1)
		return new EditInfo("Минимальное вых. напряженеие (В)", minOut, -20, 0);
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		maxOut = ei.value;
		if (n == 1)
		minOut = ei.value;
	}
	}
