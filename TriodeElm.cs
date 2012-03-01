namespace circuit_emulator
{
    class TriodeElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 173;
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 3;
            }
		
        }
        override internal double Power
        {
            get
            {
                return (volts[0] - volts[2]) * current;
            }
		
        }
        internal double mu, kg1;
        internal double curcountp, curcountc, curcountg, currentp, currentg, currentc;
        //UPGRADE_NOTE: Final was removed from the declaration of 'gridCurrentR '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal double gridCurrentR = 6000;
        public TriodeElm(int xx, int yy):base(xx, yy)
        {
            mu = 93;
            kg1 = 680;
            setup();
        }
        public TriodeElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            mu = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            kg1 = System.Double.Parse(st.NextToken());
            setup();
        }
        internal virtual void  setup()
        {
            noDiagonal = true;
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void  reset()
        {
            volts[0] = volts[1] = volts[2] = 0;
            curcount = 0;
        }
        internal override System.String dump()
        {
            return base.dump() + " " + mu + " " + kg1;
        }
	
        internal System.Drawing.Point[] plate, grid, cath;
        internal System.Drawing.Point midgrid, midcath;
        internal int circler;
        internal override void  setPoints()
        {
            base.setPoints();
            plate = newPointArray(4);
            grid = newPointArray(8);
            cath = newPointArray(4);
            grid[0] = point1;
            int nearw = 8;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref plate[1], 1, nearw);
            int farw = 32;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref plate[0], 1, farw);
            int platew = 18;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point2, ref plate[1], ref plate[2], ref plate[3], 1, platew);
		
            circler = 24;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point1, ref point2, ref grid[1], (dn - circler) / dn, 0);
            int i;
            for (i = 0; i != 3; i++)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref grid[1], ref point2, ref grid[2 + i * 2], (i * 3 + 1) / 4.5, 0);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref grid[1], ref point2, ref grid[3 + i * 2], (i * 3 + 2) / 4.5, 0);
            }
            midgrid = point2;
		
            int cathw = 16;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            midcath = interpPoint(ref point1, ref point2, 1, - nearw);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point2, ref plate[1], ref cath[1], ref cath[2], - 1, cathw);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point2, ref plate[1], ref cath[3], - 1.2, - cathw);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint(ref point2, ref plate[1], ref cath[0], (- farw) / (double) nearw, cathw);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
            drawThickCircle(g, point2.X, point2.Y, circler);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref plate[0], 16);
            adjustBbox(cath[0].X, cath[1].Y, point2.X + circler, point2.Y + circler);
            setPowerColor(g, true);
            // draw plate
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref plate[0], ref plate[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref plate[2], ref plate[3]);
            // draw grid
            setVoltageColor(g, volts[1]);
            int i;
            for (i = 0; i != 8; i += 2)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref grid[i], ref grid[i + 1]);
            }
            // draw cathode
            setVoltageColor(g, volts[2]);
            for (i = 0; i != 3; i++)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref cath[i], ref cath[i + 1]);
            }
            // draw dots
            curcountp = updateDotCount(currentp, curcountp);
            curcountc = updateDotCount(currentc, curcountc);
            curcountg = updateDotCount(currentg, curcountg);
            if (sim.dragElm != this)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref plate[0], ref midgrid, curcountp);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref midgrid, ref midcath, curcountc);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref midcath, ref cath[1], curcountc + 8);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref cath[1], ref cath[0], curcountc + 8);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref point1, ref midgrid, curcountg);
            }
            drawPosts(g);
        }
        internal override System.Drawing.Point getPost(int n)
        {
            return (n == 0)?plate[0]:((n == 1)?grid[0]:cath[0]);
        }
	
        internal double lastv0, lastv1, lastv2;
        internal override void  doStep()
        {
            double[] vs = new double[3];
            vs[0] = volts[0];
            vs[1] = volts[1];
            vs[2] = volts[2];
            if (vs[1] > lastv1 + .5)
                vs[1] = lastv1 + .5;
            if (vs[1] < lastv1 - .5)
                vs[1] = lastv1 - .5;
            if (vs[2] > lastv2 + .5)
                vs[2] = lastv2 + .5;
            if (vs[2] < lastv2 - .5)
                vs[2] = lastv2 - .5;
            int grid = 1;
            int cath = 2;
            int plate = 0;
            double vgk = vs[grid] - vs[cath];
            double vpk = vs[plate] - vs[cath];
            if (System.Math.Abs(lastv0 - vs[0]) > .01 || System.Math.Abs(lastv1 - vs[1]) > .01 || System.Math.Abs(lastv2 - vs[2]) > .01)
                sim.converged = false;
            lastv0 = vs[0];
            lastv1 = vs[1];
            lastv2 = vs[2];
            double ids = 0;
            double gm = 0;
            double Gds = 0;
            double ival = vgk + vpk / mu;
            currentg = 0;
            if (vgk > .01)
            {
                sim.stampResistor(nodes[grid], nodes[cath], gridCurrentR);
                currentg = vgk / gridCurrentR;
            }
            if (ival < 0)
            {
                // should be all zero, but that causes a singular matrix,
                // so instead we treat it as a large resistor
                Gds = 1e-8;
                ids = vpk * Gds;
            }
            else
            {
                ids = System.Math.Pow(ival, 1.5) / kg1;
                double q = 1.5 * System.Math.Sqrt(ival) / kg1;
                // gm = dids/dgk;
                // Gds = dids/dpk;
                Gds = q;
                gm = q / mu;
            }
            currentp = ids;
            currentc = ids + currentg;
            double rs = - ids + Gds * vpk + gm * vgk;
            sim.stampMatrix(nodes[plate], nodes[plate], Gds);
            sim.stampMatrix(nodes[plate], nodes[cath], - Gds - gm);
            sim.stampMatrix(nodes[plate], nodes[grid], gm);
		
            sim.stampMatrix(nodes[cath], nodes[plate], - Gds);
            sim.stampMatrix(nodes[cath], nodes[cath], Gds + gm);
            sim.stampMatrix(nodes[cath], nodes[grid], - gm);
		
            sim.stampRightSide(nodes[plate], rs);
            sim.stampRightSide(nodes[cath], - rs);
        }
	
        internal override void  stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
            sim.stampNonLinear(nodes[2]);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "триод";
            double vbc = volts[0] - volts[1];
            double vbe = volts[0] - volts[2];
            double vce = volts[1] - volts[2];
            arr[1] = "Vсэ = " + getVoltageText(vbe);
            arr[2] = "Vск = " + getVoltageText(vbc);
            arr[3] = "Vак = " + getVoltageText(vce);
        }
        // grid not connected to other terminals
        internal override bool getConnection(int n1, int n2)
        {
            return !(n1 == 1 || n2 == 1);
        }
    }
}