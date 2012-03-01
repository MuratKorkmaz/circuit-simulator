namespace circuit_emulator
{
    class LatchElm:ChipElm
    {
        override internal System.String ChipName
        {
            get
            {
                return "Защелка";
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
                return bits * 2 + 1;
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 168;
            }
		
        }
        public LatchElm(int xx, int yy):base(xx, yy)
        {
        }
        public LatchElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
        }
        internal override bool needsBits()
        {
            return true;
        }
        internal int loadPin;
        internal override void  setupPins()
        {
            sizeX = 2;
            sizeY = bits + 1;
            pins = new Pin[PostCount];
            int i;
            for (i = 0; i != bits; i++)
                pins[i] = new Pin(this, bits - 1 - i, SIDE_W, "I" + i);
            for (i = 0; i != bits; i++)
            {
                pins[i + bits] = new Pin(this, bits - 1 - i, SIDE_E, "O");
                pins[i + bits].output = true;
            }
            pins[loadPin = bits * 2] = new Pin(this, bits, SIDE_W, "Ld");
            allocNodes();
        }
        internal bool lastLoad = false;
        internal override void  execute()
        {
            int i;
            if (pins[loadPin].value_Renamed && !lastLoad)
                for (i = 0; i != bits; i++)
                    pins[i + bits].value_Renamed = pins[i].value_Renamed;
            lastLoad = pins[loadPin].value_Renamed;
        }
    }
}