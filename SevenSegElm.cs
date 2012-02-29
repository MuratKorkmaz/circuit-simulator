using System;

class SevenSegElm:ChipElm
{
	override internal System.String ChipName
	{
		get
		{
			return "7ми сегментный индикатор";
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 7;
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return 0;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 157;
		}
		
	}
	public SevenSegElm(int xx, int yy):base(xx, yy)
	{
	}
	public SevenSegElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	internal System.Drawing.Color darkred;
	internal override void  setupPins()
	{
		darkred = System.Drawing.Color.FromArgb(30, 0, 0);
		sizeX = 4;
		sizeY = 4;
		pins = new Pin[7];
		pins[0] = new Pin(this, 0, SIDE_W, "a");
		pins[1] = new Pin(this, 1, SIDE_W, "b");
		pins[2] = new Pin(this, 2, SIDE_W, "c");
		pins[3] = new Pin(this, 3, SIDE_W, "d");
		pins[4] = new Pin(this, 1, SIDE_S, "e");
		pins[5] = new Pin(this, 2, SIDE_S, "f");
		pins[6] = new Pin(this, 3, SIDE_S, "g");
	}
	internal override void  draw(System.Drawing.Graphics g)
	{
		drawChip(g);
		SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Red);
		int xl = x + cspc * 5;
		int yl = y + cspc;
		setColor(g, 0);
		drawThickLine(g, xl, yl, xl + cspc, yl);
		setColor(g, 1);
		drawThickLine(g, xl + cspc, yl, xl + cspc, yl + cspc);
		setColor(g, 2);
		drawThickLine(g, xl + cspc, yl + cspc, xl + cspc, yl + cspc2);
		setColor(g, 3);
		drawThickLine(g, xl, yl + cspc2, xl + cspc, yl + cspc2);
		setColor(g, 4);
		drawThickLine(g, xl, yl + cspc, xl, yl + cspc2);
		setColor(g, 5);
		drawThickLine(g, xl, yl, xl, yl + cspc);
		setColor(g, 6);
		drawThickLine(g, xl, yl + cspc, xl + cspc, yl + cspc);
	}
	internal virtual void  setColor(System.Drawing.Graphics g, int p)
	{
		SupportClass.GraphicsManager.manager.SetColor(g, pins[p].value_Renamed?System.Drawing.Color.Red:(sim.printableCheckItem.Checked?System.Drawing.Color.White:darkred));
	}
}