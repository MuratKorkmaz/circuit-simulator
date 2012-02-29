using System;

class DACElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "ЦАП";
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 1;
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return bits + 2;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 166;
		}
		
	}
	public DACElm(int xx, int yy):base(xx, yy)
	{
	}
	public DACElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	internal override bool needsBits()
	{
		return true;
	}
	internal override void  setupPins()
	{
		sizeX = 2;
		sizeY = bits > 2?bits:2;
		pins = new Pin[PostCount];
		int i;
		for (i = 0; i != bits; i++)
			pins[i] = new Pin(this, bits - 1 - i, SIDE_W, "D" + i);
		pins[bits] = new Pin(this, 0, SIDE_E, "O");
		pins[bits].output = true;
		pins[bits + 1] = new Pin(this, sizeY - 1, SIDE_E, "V+");
		allocNodes();
	}
	internal override void  doStep()
	{
		int ival = 0;
		int i;
		for (i = 0; i != bits; i++)
			if (volts[i] > 2.5)
				ival |= 1 << i;
		int ivalmax = (1 << bits) - 1;
		double v = ival * volts[bits + 1] / ivalmax;
		sim.updateVoltageSource(0, nodes[bits], pins[bits].voltSource, v);
	}
}