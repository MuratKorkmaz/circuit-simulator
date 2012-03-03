namespace circuit_emulator
{
    class JfetElm:MosfetElm
    {
        override internal int DumpType
        {
            get
            {
                return 'j';
            }
		
        }
        override internal double DefaultThreshold
        {
            // these values are taken from Hayes+Horowitz p155
		
            get
            {
                return - 4;
            }
		
        }
        override internal double Beta
        {
            get
            {
                return .00125;
            }
		
        }
        internal JfetElm(int xx, int yy, bool pnpflag):base(xx, yy, pnpflag)
        {
            noDiagonal = true;
        }
        public JfetElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
            noDiagonal = true;
        }
	
        internal System.Drawing.Drawing2D.GraphicsPath gatePoly;
        new internal System.Drawing.Drawing2D.GraphicsPath arrowPoly;
        internal System.Drawing.Point gatePt;
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, hs);
            setVoltageColor(g, volts[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref src[0], ref src[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref src[1], ref src[2]);
            setVoltageColor(g, volts[2]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref drn[0], ref drn[1]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref drn[1], ref drn[2]);
            setVoltageColor(g, volts[0]);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref point1, ref gatePt);
            g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), arrowPoly);
            setPowerColor(g, true);
            g.FillPath(SupportClass.GraphicsManager.manager.GetPaint(g), gatePoly);
            curcount = updateDotCount(- ids, curcount);
            if (curcount != 0)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref src[0], ref src[1], curcount);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref src[1], ref src[2], curcount + 8);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref drn[0], ref drn[1], - curcount);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref drn[1], ref drn[2], - (curcount + 8));
            }
            drawPosts(g);
        }
        internal override void  setPoints()
        {
            base.setPoints();
		
            // find the coordinates of the various points we need to draw
            // the JFET.
            int hs2 = hs * dsign;
            src = newPointArray(3);
            drn = newPointArray(3);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point1, ref point2, ref src[0], ref drn[0], 1, hs2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point1, ref point2, ref src[1], ref drn[1], 1, hs2 / 2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point1, ref point2, ref src[2], ref drn[2], 1 - 10 / dn, hs2 / 2);
		
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            gatePt = interpPoint(ref point1, ref point2, 1 - 14 / dn);
		
            System.Drawing.Point[] ra = newPointArray(4);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point1, ref point2, ref ra[0], ref ra[1], 1 - 13 / dn, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref point1, ref point2, ref ra[2], ref ra[3], 1 - 10 / dn, hs);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            gatePoly = createPolygon(ref ra[0], ref ra[1], ref ra[3], ref ra[2]);
            if (pnp == - 1)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                System.Drawing.Point x = interpPoint(ref gatePt, ref point1, 18 / dn);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                arrowPoly = calcArrow(ref gatePt, ref x, 8, 3);
            }
            else
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                arrowPoly = calcArrow(ref point1, ref gatePt, 8, 3);
            }
        }
        internal override void  getInfo(System.String[] arr)
        {
            getFetInfo(arr, "JFET");
        }
    }
}