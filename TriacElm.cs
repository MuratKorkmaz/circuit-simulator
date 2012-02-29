// stub implementation of TriacElm, based on SCRElm
// FIXME need to add TriacElm to srclist
// FIXME need to uncomment TriacElm line from CirSim.java
using System;

// Silicon-Controlled Rectifier
// 3 nodes, 1 internal node
// 0 = anode, 1 = cathode, 2 = gate
// 0, 3 = variable resistor
// 3, 2 = diode
// 2, 1 = 50 ohm resistor

class TriacElm:CircuitElm
{
	override internal int DumpType
	{
		get
		{
			return 183;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	override internal int InternalNodeCount
	{
		get
		{
			return 1;
		}
		
	}
	override internal double Power
	{
		get
		{
			return (volts[anode] - volts[gnode]) * ia + (volts[cnode] - volts[gnode]) * ic;
		}
		
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'anode '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int anode = 0;
	//UPGRADE_NOTE: Final was removed from the declaration of 'cnode '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int cnode = 1;
	//UPGRADE_NOTE: Final was removed from the declaration of 'gnode '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int gnode = 2;
	//UPGRADE_NOTE: Final was removed from the declaration of 'inode '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int inode = 3;
	internal Diode diode;
	public TriacElm(int xx, int yy):base(xx, yy)
	{
		setDefaults();
		setup();
	}
	public TriacElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
	{
		setDefaults();
		try
		{
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			lastvac = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			lastvag = System.Double.Parse(st.NextToken());
			volts[anode] = 0;
			volts[cnode] = - lastvac;
			volts[gnode] = - lastvag;
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			triggerI = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			holdingI = System.Double.Parse(st.NextToken());
			//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			cresistance = System.Double.Parse(st.NextToken());
		}
		catch (System.Exception e)
		{
		}
		setup();
	}
	internal virtual void  setDefaults()
	{
		cresistance = 50;
		holdingI = .0082;
		triggerI = .01;
	}
	internal virtual void  setup()
	{
		diode = new Diode(sim);
		diode.setup(.8, 0);
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void  reset()
	{
		volts[anode] = volts[cnode] = volts[gnode] = 0;
		diode.reset();
		lastvag = lastvac = curcount_a = curcount_c = curcount_g = 0;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + (volts[anode] - volts[cnode]) + " " + (volts[anode] - volts[gnode]) + " " + triggerI + " " + holdingI + " " + cresistance;
	}
	internal double ia, ic, ig, curcount_a, curcount_c, curcount_g;
	internal double lastvac, lastvag;
	internal double cresistance, triggerI, holdingI;
	
	//UPGRADE_NOTE: Final was removed from the declaration of 'hs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int hs = 8;
	internal System.Drawing.Drawing2D.GraphicsPath poly;
	internal System.Drawing.Point[] cathode, gate;
	
	internal override void  setPoints()
	{
		base.setPoints();
		int dir = 0;
		if (abs(dx) > abs(dy))
		{
			dir = (- sign(dx)) * sign(dy);
			point2.Y = point1.Y;
		}
		else
		{
			dir = sign(dy) * sign(dx);
			point2.X = point1.X;
		}
		if (dir == 0)
			dir = 1;
		calcLeads(16);
		cathode = newPointArray(2);
		System.Drawing.Point[] pa = newPointArray(2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref pa[0], ref pa[1], 0, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref cathode[0], ref cathode[1], 1, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		poly = createPolygon(ref pa[0], ref pa[1], ref lead2);
		
		gate = newPointArray(2);
		double leadlen = (dn - 16) / 2;
		int gatelen = sim.gridSize;
		gatelen = (int) (gatelen + leadlen % sim.gridSize);
		if (leadlen < gatelen)
		{
			x2 = x; y2 = y;
			return ;
		}
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref lead2, ref point2, ref gate[0], gatelen / leadlen, gatelen * dir);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint(ref lead2, ref point2, ref gate[1], gatelen / leadlen, sim.gridSize * 2 * dir);
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, hs);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		adjustBbox(ref gate[0], ref gate[1]);
		
		double v1 = volts[anode];
		double v2 = volts[cnode];
		
		draw2Leads(g);
		
		// draw arrow thingy
		setPowerColor(g, true);
		setVoltageColor(g, v1);
		g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), poly);
		
		// draw thing arrow is pointing to
		setVoltageColor(g, v2);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref cathode[0], ref cathode[1]);
		
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead2, ref gate[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref gate[0], ref gate[1]);
		
		curcount_a = updateDotCount(ia, curcount_a);
		curcount_c = updateDotCount(ic, curcount_c);
		curcount_g = updateDotCount(ig, curcount_g);
		if (sim.dragElm != this)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point1, ref lead2, curcount_a);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref point2, ref lead2, curcount_c);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref gate[1], ref gate[0], curcount_g);
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref gate[0], ref lead2, curcount_g + distance(ref gate[1], ref gate[0]));
		}
		drawPosts(g);
	}
	
	
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:((n == 1)?point2:gate[1]);
	}
	
	internal double aresistance;
	internal override void  stamp()
	{
		sim.stampNonLinear(nodes[anode]);
		sim.stampNonLinear(nodes[cnode]);
		sim.stampNonLinear(nodes[gnode]);
		sim.stampNonLinear(nodes[inode]);
		sim.stampResistor(nodes[gnode], nodes[cnode], cresistance);
		diode.stamp(nodes[inode], nodes[gnode]);
	}
	
	internal override void  doStep()
	{
		double vac = volts[anode] - volts[cnode]; // typically negative
		double vag = volts[anode] - volts[gnode]; // typically positive
		if (System.Math.Abs(vac - lastvac) > .01 || System.Math.Abs(vag - lastvag) > .01)
			sim.converged = false;
		lastvac = vac;
		lastvag = vag;
		diode.doStep(volts[inode] - volts[gnode]);
		double icmult = 1 / triggerI;
		double iamult = 1 / holdingI - icmult;
		//System.out.println(icmult + " " + iamult);
		aresistance = ((- icmult) * ic + ia * iamult > 1)?.0105:10e5;
		//System.out.println(vac + " " + vag + " " + sim.converged + " " + ic + " " + ia + " " + aresistance + " " + volts[inode] + " " + volts[gnode] + " " + volts[anode]);
		sim.stampResistor(nodes[anode], nodes[inode], aresistance);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = "SCR";
		double vac = volts[anode] - volts[cnode];
		double vag = volts[anode] - volts[gnode];
		double vgc = volts[gnode] - volts[cnode];
		arr[1] = "Ia = " + getCurrentText(ia);
		arr[2] = "Ig = " + getCurrentText(ig);
		arr[3] = "Vac = " + getVoltageText(vac);
		arr[4] = "Vag = " + getVoltageText(vag);
		arr[5] = "Vgc = " + getVoltageText(vgc);
	}
	internal override void  calculateCurrent()
	{
		ic = (volts[cnode] - volts[gnode]) / cresistance;
		ia = (volts[anode] - volts[inode]) / aresistance;
		ig = - ic - ia;
	}
	public override EditInfo getEditInfo(int n)
	{
		// ohmString doesn't work here on linux
		if (n == 0)
			return new EditInfo("Trigger Current (A)", triggerI, 0, 0);
		if (n == 1)
			return new EditInfo("Holding Current (A)", holdingI, 0, 0);
		if (n == 2)
			return new EditInfo("Gate-Cathode Resistance (ohms)", cresistance, 0, 0);
		return null;
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 0 && ei.value_Renamed > 0)
			triggerI = ei.value_Renamed;
		if (n == 1 && ei.value_Renamed > 0)
			holdingI = ei.value_Renamed;
		if (n == 2 && ei.value_Renamed > 0)
			cresistance = ei.value_Renamed;
	}
}