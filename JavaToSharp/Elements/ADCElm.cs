namespace JavaToSharp
{
    internal class ADCElm : ChipElm
    {
        public ADCElm(int xx, int yy) : base(xx, yy)
        {
        }
        public ADCElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal override string ChipName
        {
            get 
            {
                return "АЦП";
            }
        }
        internal override bool needsBits()
        {
            return true;
        }
        internal override void setupPins()
        {
            sizeX = 2;
            sizeY = bits > 2 ? bits : 2;
            pins = new Pin[PostCount];
            int i;
            for (i = 0; i != bits; i++)
            {
                pins[i] = new Pin(bits-1-i, SIDE_E, "D" + i);
                pins[i].output = true;
            }
            pins[bits] = new Pin(0, SIDE_W, "Вх.");
            pins[bits+1] = new Pin(sizeY-1, SIDE_W, "V+");
            allocNodes();
        }
        internal override void execute()
        {
            int imax = (1<<bits)-1;
            // if we round, the half-flash doesn't work
            double val = imax*volts[bits]/volts[bits+1]; // +.5;
            int ival = (int) val;
            ival = min(imax, max(0, ival));
            int i;
            for (i = 0; i != bits; i++)
                pins[i].value = ((ival & (1<<i)) != 0);
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return bits;
            }
        }

        internal override int PostCount
        {
            get
            {
                return bits+2;
            }
        }
        internal override int DumpType
        {
            get
            {
                return 167;
            }
        }
    }
}

