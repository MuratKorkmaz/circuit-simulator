namespace circuit_emulator
{
    class LogicOutputElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'M';
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 1;
            }
		
        }
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
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_PULLDOWN '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_PULLDOWN = 4;
        internal double threshold;
        internal System.String value_Renamed;
        public LogicOutputElm(int xx, int yy):base(xx, yy)
        {
            threshold = 2.5;
        }
        public LogicOutputElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                threshold = System.Double.Parse(st.NextToken());
            }
            catch (System.Exception e)
            {
                threshold = 2.5;
            }
        }
        internal override System.String dump()
        {
            return base.dump() + " " + threshold;
        }
        internal virtual bool needsPullDown()
        {
            return (flags & FLAG_PULLDOWN) != 0;
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
            //g.setColor(needsHighlight() ? selectColor : lightGrayColor);
            SupportClass.GraphicsManager.manager.SetColor(g, lightGrayColor);
            System.String s = (volts[0] < threshold)?"L":"H";
            if (Ternary)
            {
                if (volts[0] > 3.75)
                    s = "2";
                else if (volts[0] > 1.25)
                    s = "1";
                else
                    s = "0";
            }
            else if (Numeric)
                s = (volts[0] < threshold)?"0":"1";
            value_Renamed = s;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref lead1, 0);
            drawCenteredText(g, s, x2, y2, true);
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref lead1);
            drawPosts(g);
        }
        internal override void  stamp()
        {
            if (needsPullDown())
                sim.stampResistor(nodes[0], 0, 1e6);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "логический выход";
            arr[1] = (volts[0] < threshold)?"low":"high";
            if (Numeric)
                arr[1] = value_Renamed;
            arr[2] = "V = " + getVoltageText(volts[0]);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Порог", threshold, 10, - 10);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Требуется подтяжка (pullUp)", needsPullDown());
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                threshold = ei.value_Renamed;
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags = FLAG_PULLDOWN;
                else
                    flags &= ~ FLAG_PULLDOWN;
            }
        }
    }
}