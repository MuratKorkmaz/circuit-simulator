namespace circuit_emulator
{
    class TransLineElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 171;
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 4;
            }
		
        }
        override internal int InternalNodeCount
        {
            get
            {
                return 2;
            }
		
        }
        override internal int VoltageSourceCount
        {
            //double getVoltageDiff() { return volts[0]; }
		
            get
            {
                return 2;
            }
		
        }
        internal double delay, imped;
        internal double[] voltageL, voltageR;
        internal int lenSteps, ptr, width;
        public TransLineElm(int xx, int yy):base(xx, yy)
        {
            delay = 1000 * sim.timeStep;
            imped = 75;
            noDiagonal = true;
            reset();
        }
        public TransLineElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            delay = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            imped = System.Double.Parse(st.NextToken());
            width = System.Int32.Parse(st.NextToken());
            // next slot is for resistance (losses), which is not implemented
            st.NextToken();
            noDiagonal = true;
            reset();
        }
        internal override System.String dump()
        {
            return base.dump() + " " + delay + " " + imped + " " + width + " " + 0.0;
        }
        internal override void  drag(int xx, int yy)
        {
            xx = sim.snapGrid(xx);
            yy = sim.snapGrid(yy);
            int w1 = max(sim.gridSize, abs(yy - y));
            int w2 = max(sim.gridSize, abs(xx - x));
            if (w1 > w2)
            {
                xx = x;
                width = w2;
            }
            else
            {
                yy = y;
                width = w1;
            }
            x2 = xx; y2 = yy;
            setPoints();
        }
	
        internal System.Drawing.Point[] posts, inner;
	
        internal override void  reset()
        {
            if (sim.timeStep == 0)
                return ;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            lenSteps = (int) (delay / sim.timeStep);
            System.Console.Out.WriteLine(lenSteps + " steps");
            if (lenSteps > 100000)
                voltageL = voltageR = null;
            else
            {
                voltageL = new double[lenSteps];
                voltageR = new double[lenSteps];
            }
            ptr = 0;
            base.reset();
        }
        internal override void  setPoints()
        {
            base.setPoints();
            int ds = (dy == 0)?sign(dx):- sign(dy);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p3 = interpPoint(ref point1, ref point2, 0, (- width) * ds);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p4 = interpPoint(ref point1, ref point2, 1, (- width) * ds);
            int sep = sim.gridSize / 2;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p5 = interpPoint(ref point1, ref point2, 0, (- (width / 2 - sep)) * ds);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p6 = interpPoint(ref point1, ref point2, 1, (- (width / 2 - sep)) * ds);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p7 = interpPoint(ref point1, ref point2, 0, (- (width / 2 + sep)) * ds);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            System.Drawing.Point p8 = interpPoint(ref point1, ref point2, 1, (- (width / 2 + sep)) * ds);
		
            // we number the posts like this because we want the lower-numbered
            // points to be on the bottom, so that if some of them are unconnected
            // (which is often true) then the bottom ones will get automatically
            // attached to ground.
            posts = new System.Drawing.Point[]{p3, p4, point1, point2};
            inner = new System.Drawing.Point[]{p7, p8, p5, p6};
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref posts[0], ref posts[3], 0);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int segments = (int) (dn / 2);
            int ix0 = ptr - 1 + lenSteps;
            double segf = 1.0 / segments;
            int i;
            SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.DarkGray);
            g.FillRectangle(SupportClass.GraphicsManager.manager.GetPaint(g), inner[2].X, inner[2].Y, inner[1].X - inner[2].X + 2, inner[1].Y - inner[2].Y + 2);
            for (i = 0; i != 4; i++)
            {
                setVoltageColor(g, volts[i]);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref posts[i], ref inner[i]);
            }
            if (voltageL != null)
            {
                for (i = 0; i != segments; i++)
                {
                    int ix1 = (ix0 - lenSteps * i / segments) % lenSteps;
                    int ix2 = (ix0 - lenSteps * (segments - 1 - i) / segments) % lenSteps;
                    double v = (voltageL[ix1] + voltageR[ix2]) / 2;
                    setVoltageColor(g, v);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref inner[0], ref inner[1], ref ps1, i * segf);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref inner[2], ref inner[3], ref ps2, i * segf);
                    g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), ps1.X, ps1.Y, ps2.X, ps2.Y);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref inner[2], ref inner[3], ref ps1, (i + 1) * segf);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps1, ref ps2);
                }
            }
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref inner[0], ref inner[1]);
            drawPosts(g);
		
            curCount1 = updateDotCount(- current1, curCount1);
            curCount2 = updateDotCount(current2, curCount2);
            if (sim.dragElm != this)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref posts[0], ref inner[0], curCount1);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref posts[2], ref inner[2], - curCount1);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref posts[1], ref inner[1], - curCount2);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref posts[3], ref inner[3], curCount2);
            }
        }
	
        internal int voltSource1, voltSource2;
        internal double current1, current2, curCount1, curCount2;
        internal override void  setVoltageSource(int n, int v)
        {
            if (n == 0)
                voltSource1 = v;
            else
                voltSource2 = v;
        }
        internal override void  setCurrent(int v, double c)
        {
            if (v == voltSource1)
                current1 = c;
            else
                current2 = c;
        }
	
        internal override void  stamp()
        {
            sim.stampVoltageSource(nodes[4], nodes[0], voltSource1);
            sim.stampVoltageSource(nodes[5], nodes[1], voltSource2);
            sim.stampResistor(nodes[2], nodes[4], imped);
            sim.stampResistor(nodes[3], nodes[5], imped);
        }
	
        internal override void  startIteration()
        {
            // calculate voltages, currents sent over wire
            if (voltageL == null)
            {
                sim.stop("Задержка в линии передачи слишком большая!", this);
                return ;
            }
            voltageL[ptr] = volts[2] - volts[0] + volts[2] - volts[4];
            voltageR[ptr] = volts[3] - volts[1] + volts[3] - volts[5];
            //System.out.println(volts[2] + " " + volts[0] + " " + (volts[2]-volts[0]) + " " + (imped*current1) + " " + voltageL[ptr]);
            /*System.out.println("sending fwd  " + currentL[ptr] + " " + current1);
		System.out.println("sending back " + currentR[ptr] + " " + current2);*/
            //System.out.println("sending back " + voltageR[ptr]);
            ptr = (ptr + 1) % lenSteps;
        }
        internal override void  doStep()
        {
            if (voltageL == null)
            {
                sim.stop("Задержка в линии передачи слишком большая!", this);
                return ;
            }
            sim.updateVoltageSource(nodes[4], nodes[0], voltSource1, - voltageR[ptr]);
            sim.updateVoltageSource(nodes[5], nodes[1], voltSource2, - voltageL[ptr]);
            if (System.Math.Abs(volts[0]) > 1e-5 || System.Math.Abs(volts[1]) > 1e-5)
            {
                sim.stop("Необходимо заземлить линию передачи!", this);
                return ;
            }
        }
	
        internal override System.Drawing.Point getPost(int n)
        {
            return posts[n];
        }
        internal override bool hasGroundConnection(int n1)
        {
            return false;
        }
        internal override bool getConnection(int n1, int n2)
        {
            return false;
            /*if (comparePair(n1, n2, 0, 1))
		return true;
		if (comparePair(n1, n2, 2, 3))
		return true;
		return false;*/
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "линия передачи";
            arr[1] = getUnitText(imped, CirSim.ohmString);
            arr[2] = "длинна = " + getUnitText(2.9979e8 * delay, "м");
            arr[3] = "задержка = " + getUnitText(delay, "с");
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Задержка (с)", delay, 0, 0);
            if (n == 1)
                return new EditInfo("Импенданс (Ом)", imped, 0, 0);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                delay = ei.value_Renamed;
                reset();
            }
            if (n == 1)
            {
                imped = ei.value_Renamed;
                reset();
            }
        }
    }
}