using System;

class PhaseCompElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "фазовый компаратор";
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
	override internal int DumpType
	{
		get
		{
			return 161;
		}
		
	}
	public PhaseCompElm(int xx, int yy):base(xx, yy)
	{
	}
	public PhaseCompElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	internal override void  setupPins()
	{
		sizeX = 2;
		sizeY = 2;
		pins = new Pin[3];
		pins[0] = new Pin(this, 0, SIDE_W, "I1");
		pins[1] = new Pin(this, 1, SIDE_W, "I2");
		pins[2] = new Pin(this, 0, SIDE_E, "O");
		pins[2].output = true;
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void  stamp()
	{
		int vn = sim.nodeList.Count + pins[2].voltSource;
		sim.stampNonLinear(vn);
		sim.stampNonLinear(0);
		sim.stampNonLinear(nodes[2]);
	}
	internal bool ff1, ff2;
	internal override void  doStep()
	{
		bool v1 = volts[0] > 2.5;
		bool v2 = volts[1] > 2.5;
		if (v1 && !pins[0].value_Renamed)
			ff1 = true;
		if (v2 && !pins[1].value_Renamed)
			ff2 = true;
		if (ff1 && ff2)
			ff1 = ff2 = false;
		double out_Renamed = (ff1)?5:((ff2)?0:- 1);
		//System.out.println(out + " " + v1 + " " + v2);
		if (out_Renamed != - 1)
			sim.stampVoltageSource(0, nodes[2], pins[2].voltSource, out_Renamed);
		else
		{
			// tie current through output pin to 0
			int vn = sim.nodeList.Count + pins[2].voltSource;
			sim.stampMatrix(vn, vn, 1);
		}
		pins[0].value_Renamed = v1;
		pins[1].value_Renamed = v2;
	}
}