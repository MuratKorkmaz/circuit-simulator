using System;

class CounterElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "Counter";
		}
		
	}
	override internal int PostCount
	{
		get
		{
			if (hasEnable())
				return bits + 3;
			return bits + 2;
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return bits;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 164;
		}
		
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_ENABLE '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_ENABLE = 2;
	public CounterElm(int xx, int yy):base(xx, yy)
	{
	}
	public CounterElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
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
		pins[0] = new Pin(this, 0, SIDE_W, "");
		pins[0].clock = true;
		pins[1] = new Pin(this, sizeY - 1, SIDE_W, "R");
		pins[1].bubble = true;
		int i;
		for (i = 0; i != bits; i++)
		{
			int ii = i + 2;
			pins[ii] = new Pin(this, i, SIDE_E, "Q" + (bits - i - 1));
			pins[ii].output = pins[ii].state = true;
		}
		if (hasEnable())
			pins[bits + 2] = new Pin(this, sizeY - 2, SIDE_W, "En");
		allocNodes();
	}
	internal virtual bool hasEnable()
	{
		return (flags & FLAG_ENABLE) != 0;
	}
	internal override void  execute()
	{
		bool en = true;
		if (hasEnable())
			en = pins[bits + 2].value_Renamed;
		if (pins[0].value_Renamed && !lastClock && en)
		{
			int i;
			for (i = bits - 1; i >= 0; i--)
			{
				int ii = i + 2;
				if (!pins[ii].value_Renamed)
				{
					pins[ii].value_Renamed = true;
					break;
				}
				pins[ii].value_Renamed = false;
			}
		}
		if (!pins[1].value_Renamed)
		{
			int i;
			for (i = 0; i != bits; i++)
				pins[i + 2].value_Renamed = false;
		}
		lastClock = pins[0].value_Renamed;
	}
}