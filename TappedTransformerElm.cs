namespace circuit_emulator
{
    class TappedTransformerElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 169;
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 5;
            }
		
        }
        internal double inductance, ratio;
        internal System.Drawing.Point[] ptEnds, ptCoil, ptCore;
        new internal double[] current;
        new internal double[] curcount;
        public TappedTransformerElm(int xx, int yy):base(xx, yy)
        {
            inductance = 4;
            ratio = 1;
            noDiagonal = true;
            current = new double[4];
            curcount = new double[4];
        }
        public TappedTransformerElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            inductance = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            ratio = System.Double.Parse(st.NextToken());
            current = new double[4];
            curcount = new double[4];
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            current[0] = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            current[1] = System.Double.Parse(st.NextToken());
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                current[2] = System.Double.Parse(st.NextToken());
            }
            catch (System.Exception e)
            {
            }
            noDiagonal = true;
        }
        internal override System.String dump()
        {
            return base.dump() + " " + inductance + " " + ratio + " " + current[0] + " " + current[1] + " " + current[2];
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            int i;
            for (i = 0; i != 5; i++)
            {
                setVoltageColor(g, volts[i]);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ptEnds[i], ref ptCoil[i]);
            }
            for (i = 0; i != 4; i++)
            {
                if (i == 1)
                    continue;
                setPowerColor(g, current[i] * (volts[i] - volts[i + 1]));
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawCoil(g, i > 1?- 6:6, ref ptCoil[i], ref ptCoil[i + 1], volts[i], volts[i + 1]);
            }
            SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
            for (i = 0; i != 4; i += 2)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ptCore[i], ref ptCore[i + 1]);
            }
            // calc current of tap wire
            current[3] = current[1] - current[2];
            for (i = 0; i != 4; i++)
                curcount[i] = updateDotCount(current[i], curcount[i]);
		
            // primary dots
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptEnds[0], ref ptCoil[0], curcount[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[0], ref ptCoil[1], curcount[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[1], ref ptEnds[1], curcount[0]);
		
            // secondary dots
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptEnds[2], ref ptCoil[2], curcount[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[2], ref ptCoil[3], curcount[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[3], ref ptEnds[3], curcount[3]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[3], ref ptCoil[4], curcount[2]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawDots(g, ref ptCoil[4], ref ptEnds[4], curcount[2]);
		
            drawPosts(g);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref ptEnds[0], ref ptEnds[4], 0);
        }
	
        internal override void  setPoints()
        {
            base.setPoints();
            int hs = 32;
            ptEnds = newPointArray(5);
            ptCoil = newPointArray(5);
            ptCore = newPointArray(4);
            ptEnds[0] = point1;
            ptEnds[2] = point2;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref ptEnds[1], 0, (- hs) * 2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref ptEnds[3], 1, - hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref ptEnds[4], 1, (- hs) * 2);
            double ce = .5 - 12 / dn;
            double cd = .5 - 2 / dn;
            int i;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCoil[0], ce);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCoil[1], ce, (- hs) * 2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCoil[2], 1 - ce);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCoil[3], 1 - ce, - hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCoil[4], 1 - ce, (- hs) * 2);
            for (i = 0; i != 2; i++)
            {
                int b = (- hs) * i * 2;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCore[i], cd, b);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref ptEnds[0], ref ptEnds[2], ref ptCore[i + 2], 1 - cd, b);
            }
        }
        internal override System.Drawing.Point getPost(int n)
        {
            return ptEnds[n];
        }
        internal override void  reset()
        {
            current[0] = current[1] = volts[0] = volts[1] = volts[2] = volts[3] = curcount[0] = curcount[1] = 0;
        }
        internal double[] a;
        internal override void  stamp()
        {
            // equations for transformer:
            //   v1 = L1 di1/dt + M1 di2/dt + M1 di3/dt
            //   v2 = M1 di1/dt + L2 di2/dt + M2 di3/dt
            //   v3 = M1 di1/dt + M2 di2/dt + L2 di3/dt
            // we invert that to get:
            //   di1/dt = a1 v1 + a2 v2 + a3 v3
            //   di2/dt = a4 v1 + a5 v2 + a6 v3
            //   di3/dt = a7 v1 + a8 v2 + a9 v3
            // integrate di1/dt using trapezoidal approx and we get:
            //   i1(t2) = i1(t1) + dt/2 (i1(t1) + i1(t2))
            //          = i1(t1) + a1 dt/2 v1(t1)+a2 dt/2 v2(t1)+a3 dt/2 v3(t3) +
            //                     a1 dt/2 v1(t2)+a2 dt/2 v2(t2)+a3 dt/2 v3(t3)
            // the norton equivalent of this for i1 is:
            //  a. current source, I = i1(t1) + a1 dt/2 v1(t1) + a2 dt/2 v2(t1)
            //                                + a3 dt/2 v3(t1)
            //  b. resistor, G = a1 dt/2
            //  c. current source controlled by voltage v2, G = a2 dt/2
            //  d. current source controlled by voltage v3, G = a3 dt/2
            // and similarly for i2
            // 
            // first winding goes from node 0 to 1, second is from 2 to 3 to 4
            double l1 = inductance;
            // second winding is split in half, so each part has half the turns;
            // we square the 1/2 to divide by 4
            double l2 = inductance * ratio * ratio / 4;
            double cc = .99;
            //double m1 = .999*Math.sqrt(l1*l2);
            // mutual inductance between two halves of the second winding
            // is equal to self-inductance of either half (slightly less
            // because the coupling is not perfect)
            //double m2 = .999*l2;
            a = new double[9];
            // load pre-inverted matrix
            a[0] = (1 + cc) / (l1 * (1 + cc - 2 * cc * cc));
            a[1] = a[2] = a[3] = a[6] = 2 * cc / ((2 * cc * cc - cc - 1) * inductance * ratio);
            a[4] = a[8] = (- 4) * (1 + cc) / ((2 * cc * cc - cc - 1) * l1 * ratio * ratio);
            a[5] = a[7] = 4 * cc / ((2 * cc * cc - cc - 1) * l1 * ratio * ratio);
            int i;
            for (i = 0; i != 9; i++)
                a[i] *= sim.timeStep / 2;
            sim.stampConductance(nodes[0], nodes[1], a[0]);
            sim.stampVCCurrentSource(nodes[0], nodes[1], nodes[2], nodes[3], a[1]);
            sim.stampVCCurrentSource(nodes[0], nodes[1], nodes[3], nodes[4], a[2]);
		
            sim.stampVCCurrentSource(nodes[2], nodes[3], nodes[0], nodes[1], a[3]);
            sim.stampConductance(nodes[2], nodes[3], a[4]);
            sim.stampVCCurrentSource(nodes[2], nodes[3], nodes[3], nodes[4], a[5]);
		
            sim.stampVCCurrentSource(nodes[3], nodes[4], nodes[0], nodes[1], a[6]);
            sim.stampVCCurrentSource(nodes[3], nodes[4], nodes[2], nodes[3], a[7]);
            sim.stampConductance(nodes[3], nodes[4], a[8]);
		
            for (i = 0; i != 5; i++)
                sim.stampRightSide(nodes[i]);
            voltdiff = new double[3];
            curSourceValue = new double[3];
        }
        internal override void  startIteration()
        {
            voltdiff[0] = volts[0] - volts[1];
            voltdiff[1] = volts[2] - volts[3];
            voltdiff[2] = volts[3] - volts[4];
            int i, j;
            for (i = 0; i != 3; i++)
            {
                curSourceValue[i] = current[i];
                for (j = 0; j != 3; j++)
                    curSourceValue[i] += a[i * 3 + j] * voltdiff[j];
            }
        }
        internal double[] curSourceValue, voltdiff;
        internal override void  doStep()
        {
            sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue[0]);
            sim.stampCurrentSource(nodes[2], nodes[3], curSourceValue[1]);
            sim.stampCurrentSource(nodes[3], nodes[4], curSourceValue[2]);
        }
        internal override void  calculateCurrent()
        {
            voltdiff[0] = volts[0] - volts[1];
            voltdiff[1] = volts[2] - volts[3];
            voltdiff[2] = volts[3] - volts[4];
            int i, j;
            for (i = 0; i != 3; i++)
            {
                current[i] = curSourceValue[i];
                for (j = 0; j != 3; j++)
                    current[i] += a[i * 3 + j] * voltdiff[j];
            }
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "трансформатор";
            arr[1] = "L = " + getUnitText(inductance, "Гн");
            arr[2] = "трансформац. = " + ratio;
            //arr[3] = "I1 = " + getCurrentText(current1);
            arr[3] = "Vd1 = " + getVoltageText(volts[0] - volts[2]);
            //arr[5] = "I2 = " + getCurrentText(current2);
            arr[4] = "Vd2 = " + getVoltageText(volts[1] - volts[3]);
        }
        internal override bool getConnection(int n1, int n2)
        {
            if (comparePair(n1, n2, 0, 1))
                return true;
            if (comparePair(n1, n2, 2, 3))
                return true;
            if (comparePair(n1, n2, 3, 4))
                return true;
            if (comparePair(n1, n2, 2, 4))
                return true;
            return false;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Индуктивность первичной обмотки (Гн)", inductance, .01, 5);
            if (n == 1)
                return new EditInfo("Коэффициент трансформации", ratio, 1, 10).setDimensionless();
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                inductance = ei.value_Renamed;
            if (n == 1)
                ratio = ei.value_Renamed;
        }
    }
}