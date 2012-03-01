namespace circuit_emulator
{
    class DiodeElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'd';
            }
		
        }
        internal Diode diode;
        internal const int FLAG_FWDROP = 1;
        //UPGRADE_NOTE: Final was removed from the declaration of 'defaultdrop '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal double defaultdrop = .805904783;
        internal double fwdrop, zvoltage;
	
        public DiodeElm(int xx, int yy):base(xx, yy)
        {
            diode = new Diode(sim);
            fwdrop = defaultdrop;
            zvoltage = 0;
            setup();
        }
        public DiodeElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            diode = new Diode(sim);
            fwdrop = defaultdrop;
            zvoltage = 0;
            if ((f & FLAG_FWDROP) > 0)
            {
                try
                {
                    //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                    fwdrop = System.Double.Parse(st.NextToken());
                }
                catch (System.Exception e)
                {
                }
            }
            setup();
        }
        internal override bool nonLinear()
        {
            return true;
        }
	
        internal virtual void  setup()
        {
            diode.setup(fwdrop, zvoltage);
        }
        internal override System.String dump()
        {
            flags |= FLAG_FWDROP;
            return base.dump() + " " + fwdrop;
        }
	
	
        //UPGRADE_NOTE: Final was removed from the declaration of 'hs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int hs = 8;
        internal System.Drawing.Drawing2D.GraphicsPath poly;
        internal System.Drawing.Point[] cathode;
	
        internal override void  setPoints()
        {
            base.setPoints();
            calcLeads(16);
            cathode = newPointArray(2);
            System.Drawing.Point[] pa = newPointArray(2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref pa[0], ref pa[1], 0, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref cathode[0], ref cathode[1], 1, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            poly = createPolygon(ref pa[0], ref pa[1], ref lead2);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            drawDiode(g);
            doDots(g);
            drawPosts(g);
        }
	
        internal override void  reset()
        {
            diode.reset();
            volts[0] = volts[1] = curcount = 0;
        }
	
        internal virtual void  drawDiode(System.Drawing.Graphics g)
        {
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, hs);
		
            double v1 = volts[0];
            double v2 = volts[1];
		
            draw2Leads(g);
		
            // draw arrow thingy
            setPowerColor(g, true);
            setVoltageColor(g, v1);
            g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), poly);
		
            // draw thing arrow is pointing to
            setVoltageColor(g, v2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref cathode[0], ref cathode[1]);
        }
	
        internal override void  stamp()
        {
            diode.stamp(nodes[0], nodes[1]);
        }
        internal override void  doStep()
        {
            diode.doStep(volts[0] - volts[1]);
        }
        internal override void  calculateCurrent()
        {
            current = diode.calculateCurrent(volts[0] - volts[1]);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "диод";
            arr[1] = "I = " + getCurrentText(getCurrent());
            arr[2] = "Vd = " + getVoltageText(VoltageDiff);
            arr[3] = "P = " + getUnitText(Power, "Вт");
            arr[4] = "Vf = " + getVoltageText(fwdrop);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Прямое падение напряжения @ 1A", fwdrop, 10, 1000);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            fwdrop = ei.value_Renamed;
            setup();
        }
        internal override bool needsShortcut()
        {
            return GetType() == typeof(DiodeElm);
        }
    }
}