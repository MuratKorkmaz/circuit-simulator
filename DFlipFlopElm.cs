namespace circuit_emulator
{
    class DFlipFlopElm:ChipElm
    {
        override internal System.String ChipName
        {
            get
            {
                return "D триггер";
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return hasReset()?5:4;
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
                return 155;
            }
		
        }
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_RESET '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_RESET = 2;
        internal virtual bool hasReset()
        {
            return (flags & FLAG_RESET) != 0;
        }
        public DFlipFlopElm(int xx, int yy):base(xx, yy)
        {
        }
        public DFlipFlopElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
            pins[2].value_Renamed = !pins[1].value_Renamed;
        }
        internal override void  setupPins()
        {
            sizeX = 2;
            sizeY = 3;
            pins = new Pin[PostCount];
            pins[0] = new Pin(this, 0, SIDE_W, "D");
            pins[1] = new Pin(this, 0, SIDE_E, "Q");
            pins[1].output = pins[1].state = true;
            pins[2] = new Pin(this, 2, SIDE_E, "Q");
            pins[2].output = true;
            pins[2].lineOver = true;
            pins[3] = new Pin(this, 1, SIDE_W, "");
            pins[3].clock = true;
            if (hasReset())
                pins[4] = new Pin(this, 2, SIDE_W, "R");
        }
        internal override void  execute()
        {
            if (pins[3].value_Renamed && !lastClock)
            {
                pins[1].value_Renamed = pins[0].value_Renamed;
                pins[2].value_Renamed = !pins[0].value_Renamed;
            }
            if (pins.Length > 4 && pins[4].value_Renamed)
            {
                pins[1].value_Renamed = false;
                pins[2].value_Renamed = true;
            }
            lastClock = pins[3].value_Renamed;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 2)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Reset Pin", hasReset());
                return ei;
            }
            return base.getEditInfo(n);
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 2)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_RESET;
                else
                    flags &= ~ FLAG_RESET;
                setupPins();
                allocNodes();
                setPoints();
            }
            base.setEditValue(n, ei);
        }
    }
}