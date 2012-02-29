using System;

class ADCElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "АЦП";
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return bits;
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
			return 167;
		}
		
	}
	public ADCElm(int xx, int yy):base(xx, yy)
	{
	}
	public ADCElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
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
		{
			pins[i] = new Pin(this, bits - 1 - i, SIDE_E, "D" + i);
			pins[i].output = true;
		}
		pins[bits] = new Pin(this, 0, SIDE_W, "Вх.");
		pins[bits + 1] = new Pin(this, sizeY - 1, SIDE_W, "V+");
		allocNodes();
	}
	internal override void  execute()
	{
		int imax = (1 << bits) - 1;
		// if we round, the half-flash doesn't work
		double val = imax * volts[bits] / volts[bits + 1]; // + .5;
		//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
		int ival = (int) val;
		ival = min(imax, max(0, ival));
		int i;
		for (i = 0; i != bits; i++)
			pins[i].value_Renamed = ((ival & (1 << i)) != 0);
	}
}