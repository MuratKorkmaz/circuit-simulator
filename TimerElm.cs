using System;

class TimerElm:ChipElm
{
	override internal int DefaultFlags
	{
		get
		{
			return FLAG_RESET;
		}
		
	}
	override internal System.String ChipName
	{
		get
		{
			return "555 таймер";
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return hasReset()?7:6;
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
			return 165;
		}
		
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_RESET '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_RESET = 2;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_DIS '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_DIS = 0;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_TRIG '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_TRIG = 1;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_THRES '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_THRES = 2;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_VIN '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_VIN = 3;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_CTL '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_CTL = 4;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_OUT '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_OUT = 5;
	//UPGRADE_NOTE: Final was removed from the declaration of 'N_RST '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int N_RST = 6;
	public TimerElm(int xx, int yy):base(xx, yy)
	{
	}
	public TimerElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	internal override void  setupPins()
	{
		sizeX = 3;
		sizeY = 5;
		pins = new Pin[7];
		pins[N_DIS] = new Pin(this, 1, SIDE_W, "dis");
		pins[N_TRIG] = new Pin(this, 3, SIDE_W, "tr");
		pins[N_TRIG].lineOver = true;
		pins[N_THRES] = new Pin(this, 4, SIDE_W, "th");
		pins[N_VIN] = new Pin(this, 1, SIDE_N, "Vin");
		pins[N_CTL] = new Pin(this, 1, SIDE_S, "ctl");
		pins[N_OUT] = new Pin(this, 2, SIDE_E, "out");
		pins[N_OUT].output = pins[N_OUT].state = true;
		pins[N_RST] = new Pin(this, 1, SIDE_E, "rst");
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal virtual bool hasReset()
	{
		return (flags & FLAG_RESET) != 0;
	}
	internal override void  stamp()
	{
		// stamp voltage divider to put ctl pin at 2/3 V
		sim.stampResistor(nodes[N_VIN], nodes[N_CTL], 5000);
		sim.stampResistor(nodes[N_CTL], 0, 10000);
		// output pin
		sim.stampVoltageSource(0, nodes[N_OUT], pins[N_OUT].voltSource);
		// discharge pin
		sim.stampNonLinear(nodes[N_DIS]);
	}
	internal override void  calculateCurrent()
	{
		// need current for V, discharge, control; output current is
		// calculated for us, and other pins have no current
		pins[N_VIN].current = (volts[N_CTL] - volts[N_VIN]) / 5000;
		pins[N_CTL].current = (- volts[N_CTL]) / 10000 - pins[N_VIN].current;
		pins[N_DIS].current = (!out_Renamed && !setOut)?(- volts[N_DIS]) / 10:0;
	}
	internal bool setOut, out_Renamed;
	internal override void  startIteration()
	{
		out_Renamed = volts[N_OUT] > volts[N_VIN] / 2;
		setOut = false;
		// check comparators
		if (volts[N_CTL] / 2 > volts[N_TRIG])
			setOut = out_Renamed = true;
		if (volts[N_THRES] > volts[N_CTL] || (hasReset() && volts[N_RST] < .7))
			out_Renamed = false;
	}
	internal override void  doStep()
	{
		// if output is low, discharge pin 0.  we use a small
		// resistor because it's easier, and sometimes people tie
		// the discharge pin to the trigger and threshold pins.
		// We check setOut to properly emulate the case where
		// trigger is low and threshold is high.
		if (!out_Renamed && !setOut)
			sim.stampResistor(nodes[N_DIS], 0, 10);
		// output
		sim.updateVoltageSource(0, nodes[N_OUT], pins[N_OUT].voltSource, out_Renamed?volts[N_VIN]:0);
	}
}