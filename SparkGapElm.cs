namespace circuit_emulator
{
    class SparkGapElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 187;
            }
		
        }
        internal double resistance, onresistance, offresistance, breakdown, holdcurrent;
        internal bool state;
        public SparkGapElm(int xx, int yy):base(xx, yy)
        {
            offresistance = 1e9;
            onresistance = 1e3;
            breakdown = 1e3;
            holdcurrent = 0.001;
            state = false;
        }
        public SparkGapElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            onresistance = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            offresistance = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            breakdown = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            holdcurrent = System.Double.Parse(st.NextToken());
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override System.String dump()
        {
            return base.dump() + " " + onresistance + " " + offresistance + " " + breakdown + " " + holdcurrent;
        }
        internal System.Drawing.Drawing2D.GraphicsPath arrow1, arrow2;
        internal override void  setPoints()
        {
            base.setPoints();
            int dist = 16;
            int alen = 8;
            calcLeads(dist + alen);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p1 = interpPoint(ref point1, ref point2, (dn - alen) / (2 * dn));
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            arrow1 = calcArrow(ref point1, ref p1, alen, alen);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            p1 = interpPoint(ref point1, ref point2, (dn + alen) / (2 * dn));
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            arrow2 = calcArrow(ref point2, ref p1, alen, alen);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            int i;
            double v1 = volts[0];
            double v2 = volts[1];
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, 8);
            draw2Leads(g);
            setPowerColor(g, true);
            setVoltageColor(g, volts[0]);
            g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrow1);
            setVoltageColor(g, volts[1]);
            g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrow2);
            if (state)
                doDots(g);
            drawPosts(g);
        }
	
        internal override void  calculateCurrent()
        {
            double vd = volts[0] - volts[1];
            current = vd / resistance;
        }
	
        internal override void  reset()
        {
            base.reset();
            state = false;
        }
	
        internal override void  startIteration()
        {
            if (System.Math.Abs(current) < holdcurrent)
                state = false;
            double vd = volts[0] - volts[1];
            if (System.Math.Abs(vd) > breakdown)
                state = true;
        }
	
        internal override void  doStep()
        {
            resistance = (state)?onresistance:offresistance;
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void  stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "искровой промежуток";
            getBasicInfo(arr);
            arr[3] = state?"on":"off";
            arr[4] = "Ron = " + getUnitText(onresistance, CirSim.ohmString);
            arr[5] = "Roff = " + getUnitText(offresistance, CirSim.ohmString);
            arr[6] = "Vbreakdown = " + getUnitText(breakdown, "В");
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Сопротивление вкл. (Ом)", onresistance, 0, 0);
            if (n == 1)
                return new EditInfo("Сопротивление выкл. (Ом)", offresistance, 0, 0);
            if (n == 2)
                return new EditInfo("Напряжение пробоя", breakdown, 0, 0);
            if (n == 3)
                return new EditInfo("Ток удержания (A)", holdcurrent, 0, 0);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (ei.value_Renamed > 0 && n == 0)
                onresistance = ei.value_Renamed;
            if (ei.value_Renamed > 0 && n == 1)
                offresistance = ei.value_Renamed;
            if (ei.value_Renamed > 0 && n == 2)
                breakdown = ei.value_Renamed;
            if (ei.value_Renamed > 0 && n == 3)
                holdcurrent = ei.value_Renamed;
        }
        internal override bool needsShortcut()
        {
            return false;
        }
    }
}