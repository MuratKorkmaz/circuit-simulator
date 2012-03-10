namespace circuit_emulator
{
    class ResistorElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'r';
            }
		
        }
        internal double resistance;
        public ResistorElm(int xx, int yy):base(xx, yy)
        {
            resistance = 100;
        }
        public ResistorElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            resistance = System.Double.Parse(st.NextToken());
        }
        internal override System.String dump()
        {
            return base.dump() + " " + resistance;
        }
	
        internal System.Drawing.Point ps3, ps4;
        internal override void  setPoints()
        {
            base.setPoints();
            calcLeads(32);
            ps3 = new System.Drawing.Point(0, 0);
            ps4 = new System.Drawing.Point(0, 0);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            int segments = 16;
            int i;
            int ox = 0;
            int hs = sim.euroResistorCheckItem.Checked?6:8;
            double v1 = volts[0];
            double v2 = volts[1];
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, hs);
            draw2Leads(g);
            setPowerColor(g, true);
            double segf = 1.0 / segments;
            if (!sim.euroResistorCheckItem.Checked)
            {
                // draw zigzag
                for (i = 0; i != segments; i++)
                {
                    int nx = 0;
                    switch (i & 3)
                    {
					
                        case 0:  nx = 1; break;
					
                        case 2:  nx = - 1; break;
					
                        default:  nx = 0; break;
					
                    }
                    double v = v1 + (v2 - v1) * i / segments;
                    setVoltageColor(g, v);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref lead1, ref lead2, ref ps1, i * segf, hs * ox);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref lead1, ref lead2, ref ps2, (i + 1) * segf, hs * nx);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps1, ref ps2);
                    ox = nx;
                }
            }
            else
            {
                // draw rectangle
                setVoltageColor(g, v1);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 0, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
                for (i = 0; i != segments; i++)
                {
                    double v = v1 + (v2 - v1) * i / segments;
                    setVoltageColor(g, v);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, i * segf, hs);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint2(ref lead1, ref lead2, ref ps3, ref ps4, (i + 1) * segf, hs);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps1, ref ps3);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps2, ref ps4);
                }
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 1, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
            }
            if (sim.showValuesCheckItem.Checked)
            {
                System.String s = getShortUnitText(resistance, "");
                drawValues(g, s, hs);
            }
            doDots(g);
            drawPosts(g);
        }
	
        internal override void  calculateCurrent()
        {
            current = (volts[0] - volts[1]) / resistance;
            //System.out.print(this + " res current set to " + current + "\n");
        }
        internal override void  stamp()
        {
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "резистор";
            getBasicInfo(arr);
            arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
            arr[4] = "P = " + getUnitText(Power, "Вт");
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Сопротивление (Ом)", resistance, 0, 0);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (ei.value_Renamed > 0)
                resistance = ei.value_Renamed;
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}