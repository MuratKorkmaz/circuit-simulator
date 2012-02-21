using System;
using System.Drawing;
using JavaToSharp;

// stub implementation of DiacElm, based on SparkGapElm
// FIXME need to add DiacElm.java to srclist
// FIXME need to uncomment DiacElm line from CirSim.java


internal class DiacElm : CircuitElm
{
	internal double onresistance, offresistance, breakdown, holdcurrent;
	internal bool state;
	public DiacElm(int xx, int yy) : base(xx, yy)
	{
	// FIXME need to adjust defaults to make sense for diac
	offresistance = 1e9;
	onresistance = 1e3;
	breakdown = 1e3;
	holdcurrent = 0.001;
	state = false;
	}
	public DiacElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
	onresistance = new (double)double?(st.nextToken());
	offresistance = new (double)double?(st.nextToken());
	breakdown = new (double)double?(st.nextToken());
	holdcurrent = new (double)double?(st.nextToken());
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override int DumpType
	{
		get
		{
			return 185;
		}
	}
	internal override string dump()
	{
	return base.dump() + " " + onresistance + " " + offresistance + " " + breakdown + " " + holdcurrent;
	}
	internal Point ps3, ps4;
	internal override void setPoints()
	{
	base.setPoints();
	calcLeads(32);
	ps3 = new Point();
	ps4 = new Point();
	}

	internal override void draw(Graphics g)
	{
	// FIXME need to draw Diac
	int i;
	double v1 = volts[0];
	double v2 = volts[1];
	setBbox(point1, point2, 6);
	draw2Leads(g);
	setPowerColor(g, true);
	doDots(g);
	drawPosts(g);
	}

    protected override void calculateCurrent()
	{
	double vd = volts[0] - volts[1];
	if(state)
		current = vd/onresistance;
	else
		current = vd/offresistance;
	}
	internal override void startIteration()
	{
	double vd = volts[0] - volts[1];
	if(Math.Abs(current) < holdcurrent)
		state = false;
	if(Math.Abs(vd) > breakdown)
		state = true;
	//System.out.print(this + " res current set to " + current + "\n");
	}
	internal override void doStep()
	{
	if(state)
		sim.stampResistor(nodes[0], nodes[1], onresistance);
	else
		sim.stampResistor(nodes[0], nodes[1], offresistance);
	}
	internal override void stamp()
	{
	sim.stampNonLinear(nodes[0]);
	sim.stampNonLinear(nodes[1]);
	}
	internal override void getInfo(string[] arr)
	{
	// FIXME
	arr[0] = "spark gap";
	getBasicInfo(arr);
	arr[3] = state ? "on" : "off";
	arr[4] = "Ron = " + getUnitText(onresistance, sim.ohmString);
	arr[5] = "Roff = " + getUnitText(offresistance, sim.ohmString);
	arr[6] = "Vbrkdn = " + getUnitText(breakdown, "V");
	arr[7] = "Ihold = " + getUnitText(holdcurrent, "A");
	}
	public override EditInfo getEditInfo(int n)
	{
	if (n == 0)
		return new EditInfo("On resistance (ohms)", onresistance, 0, 0);
	if (n == 1)
		return new EditInfo("Off resistance (ohms)", offresistance, 0, 0);
	if (n == 2)
		return new EditInfo("Breakdown voltage (volts)", breakdown, 0, 0);
	if (n == 3)
		return new EditInfo("Hold current (amps)", holdcurrent, 0, 0);
	return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
	if (ei.value > 0 && n == 0)
		onresistance = ei.value;
	if (ei.value > 0 && n == 1)
		offresistance = ei.value;
	if (ei.value > 0 && n == 2)
		breakdown = ei.value;
	if (ei.value > 0 && n == 3)
		holdcurrent = ei.value;
	}
	internal override bool needsShortcut()
	{
		return false;
	}
}

