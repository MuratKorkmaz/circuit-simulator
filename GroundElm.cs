namespace circuit_emulator
{
    class GroundElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'g';
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 1;
            }
		
        }
        override internal double VoltageDiff
        {
            get
            {
                return 0;
            }
		
        }
        override internal int VoltageSourceCount
        {
            get
            {
                return 1;
            }
		
        }
        public GroundElm(int xx, int yy):base(xx, yy)
        {
        }
        public GroundElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            setVoltageColor(g, 0);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref point2);
            int i;
            for (i = 0; i != 3; i++)
            {
                int a = 10 - i * 4;
                int b = i * 5; // -10;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref point1, ref point2, ref ps1, ref ps2, 1 + b / dn, a);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
            }
            doDots(g);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref ps2, 1 + 11.0 / dn);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref ps2, 11);
            drawPost(g, x, y, nodes[0]);
        }
        internal override void  setCurrent(int x, double c)
        {
            current = - c;
        }
        internal override void  stamp()
        {
            sim.stampVoltageSource(0, nodes[0], voltSource, 0);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "заземление";
            arr[1] = "I = " + getCurrentText(getCurrent());
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}