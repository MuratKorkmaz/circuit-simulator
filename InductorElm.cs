namespace circuit_emulator
{
    class InductorElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'l';
            }
		
        }
        internal Inductor ind;
        internal double inductance;
        public InductorElm(int xx, int yy):base(xx, yy)
        {
            ind = new Inductor(sim);
            inductance = 1;
            ind.setup(inductance, current, flags);
        }
        public InductorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            ind = new Inductor(sim);
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            inductance = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            current = System.Double.Parse(st.NextToken());
            ind.setup(inductance, current, flags);
        }
        internal override System.String dump()
        {
            return base.dump() + " " + inductance + " " + current;
        }
        internal override void  setPoints()
        {
            base.setPoints();
            calcLeads(32);
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            double v1 = volts[0];
            double v2 = volts[1];
            int i;
            int hs = 8;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, hs);
            draw2Leads(g);
            setPowerColor(g, false);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawCoil(g, 8, ref lead1, ref lead2, v1, v2);
            if (sim.showValuesCheckItem.Checked)
            {
                System.String s = getShortUnitText(inductance, "Гн");
                drawValues(g, s, hs);
            }
            doDots(g);
            drawPosts(g);
        }
        internal override void  reset()
        {
            current = volts[0] = volts[1] = curcount = 0;
            ind.reset();
        }
        internal override void  stamp()
        {
            ind.stamp(nodes[0], nodes[1]);
        }
        internal override void  startIteration()
        {
            ind.startIteration(volts[0] - volts[1]);
        }
        internal override bool nonLinear()
        {
            return ind.nonLinear();
        }
        internal override void  calculateCurrent()
        {
            double voltdiff = volts[0] - volts[1];
            current = ind.calculateCurrent(voltdiff);
        }
        internal override void  doStep()
        {
            double voltdiff = volts[0] - volts[1];
            ind.doStep(voltdiff);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "индуктивность";
            getBasicInfo(arr);
            arr[3] = "L = " + getUnitText(inductance, "Гн");
            arr[4] = "P = " + getUnitText(Power, "Вт");
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Индутивность (Гн)", inductance, 0, 0);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Трапецив. апроксимация", ind.Trapezoidal);
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                inductance = ei.value_Renamed;
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags &= ~ Inductor.FLAG_BACK_EULER;
                else
                    flags |= Inductor.FLAG_BACK_EULER;
            }
            ind.setup(inductance, current, flags);
        }
    }
}