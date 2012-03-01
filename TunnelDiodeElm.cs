namespace circuit_emulator
{
    class TunnelDiodeElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 175;
            }
		
        }
        public TunnelDiodeElm(int xx, int yy):base(xx, yy)
        {
            setup();
        }
        public TunnelDiodeElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            setup();
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal virtual void  setup()
        {
        }
	
        //UPGRADE_NOTE: Final was removed from the declaration of 'hs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int hs = 8;
        internal System.Drawing.Drawing2D.GraphicsPath poly;
        internal System.Drawing.Point[] cathode;
	
        internal override void  setPoints()
        {
            base.setPoints();
            calcLeads(16);
            cathode = newPointArray(4);
            System.Drawing.Point[] pa = newPointArray(2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref pa[0], ref pa[1], 0, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref cathode[0], ref cathode[1], 1, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref cathode[2], ref cathode[3], .8, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            poly = createPolygon(ref pa[0], ref pa[1], ref lead2);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
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
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref cathode[2], ref cathode[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref cathode[3], ref cathode[1]);
		
            doDots(g);
            drawPosts(g);
        }
	
        internal override void  reset()
        {
            lastvoltdiff = volts[0] = volts[1] = curcount = 0;
        }
	
        internal double lastvoltdiff;
        internal virtual double limitStep(double vnew, double vold)
        {
            // Prevent voltage changes of more than 1V when iterating.  Wow, I thought it would be
            // much harder than this to prevent convergence problems.
            if (vnew > vold + 1)
                return vold + 1;
            if (vnew < vold - 1)
                return vold - 1;
            return vnew;
        }
        internal override void  stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal const double pvp = .1;
        internal const double pip = 4.7e-3;
        internal const double pvv = .37;
        internal const double pvt = .026;
        internal const double pvpp = .525;
        internal const double piv = 370e-6;
        internal override void  doStep()
        {
            double voltdiff = volts[0] - volts[1];
            if (System.Math.Abs(voltdiff - lastvoltdiff) > .01)
                sim.converged = false;
            //System.out.println(voltdiff + " " + lastvoltdiff + " " + Math.abs(voltdiff-lastvoltdiff));
            voltdiff = limitStep(voltdiff, lastvoltdiff);
            lastvoltdiff = voltdiff;
		
            double i = pip * System.Math.Exp((- pvpp) / pvt) * (System.Math.Exp(voltdiff / pvt) - 1) + pip * (voltdiff / pvp) * System.Math.Exp(1 - voltdiff / pvp) + piv * System.Math.Exp(voltdiff - pvv);
		
            double geq = pip * System.Math.Exp((- pvpp) / pvt) * System.Math.Exp(voltdiff / pvt) / pvt + pip * System.Math.Exp(1 - voltdiff / pvp) / pvp - System.Math.Exp(1 - voltdiff / pvp) * pip * voltdiff / (pvp * pvp) + System.Math.Exp(voltdiff - pvv) * piv;
            double nc = i - geq * voltdiff;
            sim.stampConductance(nodes[0], nodes[1], geq);
            sim.stampCurrentSource(nodes[0], nodes[1], nc);
        }
        internal override void  calculateCurrent()
        {
            double voltdiff = volts[0] - volts[1];
            current = pip * System.Math.Exp((- pvpp) / pvt) * (System.Math.Exp(voltdiff / pvt) - 1) + pip * (voltdiff / pvp) * System.Math.Exp(1 - voltdiff / pvp) + piv * System.Math.Exp(voltdiff - pvv);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "туннельный диод";
            arr[1] = "I = " + getCurrentText(getCurrent());
            arr[2] = "Vd = " + getVoltageText(VoltageDiff);
            arr[3] = "P = " + getUnitText(Power, "Вт");
        }
    }
}