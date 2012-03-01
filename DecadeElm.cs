namespace circuit_emulator
{
    class DecadeElm:ChipElm
    {
        override internal System.String ChipName
        {
            get
            {
                return "декадный счетчик";
            }
		
        }
        override internal int PostCount
        {
            get
            {
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
                return 163;
            }
		
        }
        public DecadeElm(int xx, int yy):base(xx, yy)
        {
        }
        public DecadeElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
        }
        internal override bool needsBits()
        {
            return true;
        }
        internal override void  setupPins()
        {
            sizeX = bits > 2?bits:2;
            sizeY = 2;
            pins = new Pin[PostCount];
            pins[0] = new Pin(this, 1, SIDE_W, "");
            pins[0].clock = true;
            pins[1] = new Pin(this, sizeX - 1, SIDE_S, "R");
            pins[1].bubble = true;
            int i;
            for (i = 0; i != bits; i++)
            {
                int ii = i + 2;
                pins[ii] = new Pin(this, i, SIDE_N, "Q" + i);
                pins[ii].output = pins[ii].state = true;
            }
            allocNodes();
        }
        internal override void  execute()
        {
            int i;
            if (pins[0].value_Renamed && !lastClock)
            {
                for (i = 0; i != bits; i++)
                    if (pins[i + 2].value_Renamed)
                        break;
                if (i < bits)
                    pins[i++ + 2].value_Renamed = false;
                i %= bits;
                pins[i + 2].value_Renamed = true;
            }
            if (!pins[1].value_Renamed)
            {
                for (i = 1; i != bits; i++)
                    pins[i + 2].value_Renamed = false;
                pins[2].value_Renamed = true;
            }
            lastClock = pins[0].value_Renamed;
        }
    }
}