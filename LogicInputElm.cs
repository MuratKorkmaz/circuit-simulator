namespace circuit_emulator
{
    class LogicInputElm:SwitchElm
    {
        virtual internal bool Ternary
        {
            get
            {
                return (flags & FLAG_TERNARY) != 0;
            }
		
        }
        virtual internal bool Numeric
        {
            get
            {
                return (flags & (FLAG_TERNARY | FLAG_NUMERIC)) != 0;
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 'L';
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 1;
            }
		
        }
        override internal int VoltageSourceCount
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
                return volts[0];
            }
		
        }
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_TERNARY '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_TERNARY = 1;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_NUMERIC '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_NUMERIC = 2;
        internal double hiV, loV;
        public LogicInputElm(int xx, int yy):base(xx, yy, false)
        {
            hiV = 5;
            loV = 0;
        }
        public LogicInputElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                hiV = System.Double.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                loV = System.Double.Parse(st.NextToken());
            }
            catch (System.Exception e)
            {
                hiV = 5;
                loV = 0;
            }
            if (Ternary)
                posCount = 3;
        }
        internal override System.String dump()
        {
            return base.dump() + " " + hiV + " " + loV;
        }
        internal override void  setPoints()
        {
            base.setPoints();
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            lead1 = interpPoint(ref point1, ref point2, 1 - 12 / dn);
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            //UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
            System.Drawing.Font f = new System.Drawing.Font("SansSerif", 20, System.Drawing.FontStyle.Bold);
            SupportClass.GraphicsManager.manager.SetFont(g, f);
            SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:whiteColor);
            System.String s = position == 0?"L":"H";
            if (Numeric)
                s = "" + position;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref lead1, 0);
            drawCenteredText(g, s, x2, y2, true);
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref lead1);
            updateDotCount();
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref point1, ref lead1, curcount);
            drawPosts(g);
        }
        internal override void  setCurrent(int vs, double c)
        {
            current = - c;
        }
        internal override void  stamp()
        {
            double v = (position == 0)?loV:hiV;
            if (Ternary)
                v = position * 2.5;
            sim.stampVoltageSource(0, nodes[0], voltSource, v);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "логический вход";
            arr[1] = (position == 0)?"low":"high";
            if (Numeric)
                arr[1] = "" + position;
            arr[1] += (" (" + getVoltageText(volts[0]) + ")");
            arr[2] = "I = " + getCurrentText(getCurrent());
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, 0, 0);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Самовозврат", momentary);
                return ei;
            }
            if (n == 1)
                return new EditInfo("Напряжение высокого уровня", hiV, 10, - 10);
            if (n == 2)
                return new EditInfo("Напряжение низкого уровня", loV, 10, - 10);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                momentary = ei.checkbox.Checked;
            if (n == 1)
                hiV = ei.value_Renamed;
            if (n == 2)
                loV = ei.value_Renamed;
        }
    }
}