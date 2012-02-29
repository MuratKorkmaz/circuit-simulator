using System;

class CC2Elm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "CC2";
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
			return 179;
		}
		
	}
	internal double gain;
	public CC2Elm(int xx, int yy):base(xx, yy)
	{
		gain = 1;
	}
	public CC2Elm(int xx, int yy, int g):base(xx, yy)
	{
		gain = g;
	}
	public CC2Elm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		gain = System.Double.Parse(st.NextToken());
	}
	internal override System.String dump()
	{
		return base.dump() + " " + gain;
	}
	internal override void  setupPins()
	{
		sizeX = 2;
		sizeY = 3;
		pins = new Pin[3];
		pins[0] = new Pin(this, 0, SIDE_W, "X");
		pins[0].output = true;
		pins[1] = new Pin(this, 2, SIDE_W, "Y");
		pins[2] = new Pin(this, 1, SIDE_E, "Z");
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = (gain == 1)?"CCII+":"CCII-";
		arr[1] = "X,Y = " + getVoltageText(volts[0]);
		arr[2] = "Z = " + getVoltageText(volts[2]);
		arr[3] = "I = " + getCurrentText(pins[0].current);
	}
	//boolean nonLinear() { return true; }
	internal override void  stamp()
	{
		// X voltage = Y voltage
		sim.stampVoltageSource(0, nodes[0], pins[0].voltSource);
		sim.stampVCVS(0, nodes[1], 1, pins[0].voltSource);
		// Z current = gain * X current
		sim.stampCCCS(0, nodes[2], pins[0].voltSource, gain);
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		pins[2].current = pins[0].current * gain;
		drawChip(g);
	}
}

class CC2NegElm:CC2Elm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(CC2Elm);
		}
		
	}
	public CC2NegElm(int xx, int yy):base(xx, yy, - 1)
	{
	}
}