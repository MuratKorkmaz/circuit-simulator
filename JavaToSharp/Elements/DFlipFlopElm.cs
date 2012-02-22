	using System.Windows.Forms;
	using JavaToSharp;

internal class DFlipFlopElm : ChipElm
	{
	internal readonly int FLAG_RESET = 2;
	internal virtual bool hasReset()
	{
		return (flags & FLAG_RESET) != 0;
	}
	public DFlipFlopElm(int xx, int yy) : base(xx, yy)
	{
	}
	public DFlipFlopElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
	{
		pins[2].value = !pins[1].value;
	}
	internal override string ChipName
	{
		get
		{
			return "D триггер";
		}
	}
	internal override void setupPins()
	{
		sizeX = 2;
		sizeY = 3;
		pins = new Pin[PostCount];
		pins[0] = new Pin(0, SIDE_W, "D");
		pins[1] = new Pin(0, SIDE_E, "Q");
		pins[1].output = pins[1].state = true;
		pins[2] = new Pin(2, SIDE_E, "Q");
		pins[2].output = true;
		pins[2].lineOver = true;
		pins[3] = new Pin(1, SIDE_W, "");
		pins[3].clock = true;
		if (hasReset())
		pins[4] = new Pin(2, SIDE_W, "R");
	}
	internal virtual int PostCount
	{
		get
		{
			return hasReset() ? 5 : 4;
		}
	}
	internal override int VoltageSourceCount
	{
		get
		{
			return 2;
		}
	}
	internal override void execute()
	{
		if (pins[3].value && !lastClock)
		{
		pins[1].value = pins[0].value;
		pins[2].value = !pins[0].value;
		}
		if (pins.Length > 4 && pins[4].value)
		{
		pins[1].value = false;
		pins[2].value = true;
		}
		lastClock = pins[3].value;
	}
	internal virtual int DumpType
	{
		get
		{
			return 155;
		}
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 2)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new CheckBox();
		    ei.checkbox.Text = "Reset Pin";
		    if (hasReset())
		    {
		        return ei;
		    }
		
		}
		return base.getEditInfo(n);
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 2)
		{
		if (ei.checkbox.Checked)
			flags |= FLAG_RESET;
		else
			flags &= ~FLAG_RESET;
		setupPins();
		allocNodes();
		setPoints();
		}
		base.setEditValue(n, ei);
	}
	}
