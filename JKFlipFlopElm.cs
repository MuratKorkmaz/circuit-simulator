using System;

class JKFlipFlopElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "JK триггер";
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 5;
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 2;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 156;
		}
		
	}
	public JKFlipFlopElm(int xx, int yy):base(xx, yy)
	{
	}
	public JKFlipFlopElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
		pins[4].value_Renamed = !pins[3].value_Renamed;
	}
	internal override void  setupPins()
	{
		sizeX = 2;
		sizeY = 3;
		pins = new Pin[5];
		pins[0] = new Pin(this, 0, SIDE_W, "J");
		pins[1] = new Pin(this, 1, SIDE_W, "");
		pins[1].clock = true;
		pins[1].bubble = true;
		pins[2] = new Pin(this, 2, SIDE_W, "K");
		pins[3] = new Pin(this, 0, SIDE_E, "Q");
		pins[3].output = pins[3].state = true;
		pins[4] = new Pin(this, 2, SIDE_E, "Q");
		pins[4].output = true;
		pins[4].lineOver = true;
	}
	internal override void  execute()
	{
		if (!pins[1].value_Renamed && lastClock)
		{
			bool q = pins[3].value_Renamed;
			if (pins[0].value_Renamed)
			{
				if (pins[2].value_Renamed)
					q = !q;
				else
					q = true;
			}
			else if (pins[2].value_Renamed)
				q = false;
			pins[3].value_Renamed = q;
			pins[4].value_Renamed = !q;
		}
		lastClock = pins[1].value_Renamed;
	}
}