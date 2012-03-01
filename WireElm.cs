namespace circuit_emulator
{
    class WireElm:CircuitElm
    {
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
                return 'w';
            }
		
        }
        override internal double Power
        {
            get
            {
                return 0;
            }
		
        }
        override internal double VoltageDiff
        {
            get
            {
                return volts[0];
            }
		
        }
        override internal bool Wire
        {
            get
            {
                return true;
            }
		
        }
        public WireElm(int xx, int yy):base(xx, yy)
        {
        }
        public WireElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
        }
        internal const int FLAG_SHOWCURRENT = 1;
        internal const int FLAG_SHOWVOLTAGE = 2;
        internal override void  draw(System.Drawing.Graphics g)
        {
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref point2);
            doDots(g);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, 3);
            if (mustShowCurrent())
            {
                System.String s = getShortUnitText(System.Math.Abs(getCurrent()), "A");
                drawValues(g, s, 4);
            }
            else if (mustShowVoltage())
            {
                System.String s = getShortUnitText(volts[0], "В");
                drawValues(g, s, 4);
            }
            drawPosts(g);
        }
        internal override void  stamp()
        {
            sim.stampVoltageSource(nodes[0], nodes[1], voltSource, 0);
        }
        internal virtual bool mustShowCurrent()
        {
            return (flags & FLAG_SHOWCURRENT) != 0;
        }
        internal virtual bool mustShowVoltage()
        {
            return (flags & FLAG_SHOWVOLTAGE) != 0;
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "провод";
            arr[1] = "I = " + getCurrentDText(getCurrent());
            arr[2] = "V = " + getVoltageText(volts[0]);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Показывать ток", mustShowCurrent());
                return ei;
            }
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Показывать напряжение", mustShowVoltage());
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_SHOWCURRENT;
                else
                    flags &= ~ FLAG_SHOWCURRENT;
            }
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_SHOWVOLTAGE;
                else
                    flags &= ~ FLAG_SHOWVOLTAGE;
            }
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}