internal class LatchElm : ChipElm
{
	public LatchElm(int xx, int yy) : base(xx, yy)
	{
	}
	public LatchElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
	{
	}
	internal override string ChipName
	{
		get
		{
			return "Защелка";
		}
	}
	internal override bool needsBits()
	{
		return true;
	}
	internal int loadPin;
	internal override void setupPins()
	{
	sizeX = 2;
	sizeY = bits+1;
	pins = new Pin[PostCount];
	int i;
	for (i = 0; i != bits; i++)
		pins[i] = new Pin(bits-1-i, SIDE_W, "I" + i);
	for (i = 0; i != bits; i++)
	{
		pins[i+bits] = new Pin(bits-1-i, SIDE_E, "O");
		pins[i+bits].output = true;
	}
	pins[loadPin = bits*2] = new Pin(bits, SIDE_W, "Ld");
	allocNodes();
	}
	internal bool lastLoad = false;
	internal override void execute()
	{
	int i;
	if (pins[loadPin].value && !lastLoad)
		for (i = 0; i != bits; i++)
		pins[i+bits].value = pins[i].value;
	lastLoad = pins[loadPin].value;
	}
	internal override int VoltageSourceCount
	{
		get
		{
			return bits;
		}
	}
	internal virtual int PostCount
	{
		get
		{
			return bits*2+1;
		}
	}
	internal virtual int DumpType
	{
		get
		{
			return 168;
		}
	}
}

