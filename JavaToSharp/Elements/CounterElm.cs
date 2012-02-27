	namespace JavaToSharp
{
    internal class CounterElm : ChipElm
    {
        internal readonly int FLAG_ENABLE = 2;
        public CounterElm(int xx, int yy) : base(xx, yy)
        {
        }
        public CounterElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal override bool needsBits()
        {
            return true;
        }
        internal override string ChipName
        {
            get
            {
                return "Counter";
            }
        }
        internal override void setupPins()
        {
            sizeX = 2;
            sizeY = bits > 2 ? bits : 2;
            pins = new Pin[PostCount];
            pins[0] = new Pin(0, SIDE_W, "");
            pins[0].clock = true;
            pins[1] = new Pin(sizeY-1, SIDE_W, "R");
            pins[1].bubble = true;
            int i;
            for (i = 0; i != bits; i++)
            {
                int ii = i+2;
                pins[ii] = new Pin(i, SIDE_E, "Q" + (bits-i-1));
                pins[ii].output = pins[ii].state = true;
            }
            if (hasEnable())
                pins[bits+2] = new Pin(sizeY-2, SIDE_W, "En");
            allocNodes();
        }
        internal virtual int PostCount
        {
            get
            {
                if (hasEnable())
                    return bits+3;
                return bits+2;
            }
        }
        internal virtual bool hasEnable()
        {
            return (flags & FLAG_ENABLE) != 0;
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return bits;
            }
        }
        internal override void execute()
        {
            bool en = true;
            if (hasEnable())
                en = pins[bits+2].value;
            if (pins[0].value && !lastClock && en)
            {
                int i;
                for (i = bits-1; i >= 0; i--)
                {
                    int ii = i+2;
                    if (!pins[ii].value)
                    {
                        pins[ii].value = true;
                        break;
                    }
                    pins[ii].value = false;
                }
            }
            if (!pins[1].value)
            {
                int i;
                for (i = 0; i != bits; i++)
                    pins[i+2].value = false;
            }
            lastClock = pins[0].value;
        }
        internal virtual int DumpType
        {
            get
            {
                return 164;
            }
        }
    }
}
