namespace circuit_emulator
{
    class AndGateElm:GateElm
    {
        override internal System.String GateName
        {
            get
            {
                return "элемент И";
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 150;
            }
		
        }
        public AndGateElm(int xx, int yy):base(xx, yy)
        {
        }
        public AndGateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
        }
        internal override void  setPoints()
        {
            base.setPoints();
		
            // 0=topleft, 1-10 = top curve, 11 = right, 12-21=bottom curve,
            // 22 = bottom left
            System.Drawing.Point[] triPoints = newPointArray(23);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref lead1, ref lead2, ref triPoints[0], ref triPoints[22], 0, hs2);
            int i;
            for (i = 0; i != 10; i++)
            {
                double a = i * .1;
                double b = System.Math.Sqrt(1 - a * a);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref triPoints[i + 1], ref triPoints[21 - i], .5 + a / 2, b * hs2);
            }
            triPoints[11] = new System.Drawing.Point(new System.Drawing.Size(lead2));
            if (Inverting)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                pcircle = interpPoint(ref point1, ref point2, .5 + (ww + 4) / dn);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                lead2 = interpPoint(ref point1, ref point2, .5 + (ww + 8) / dn);
            }
            gatePoly = createPolygon(triPoints);
        }
        internal override bool calcFunction()
        {
            int i;
            bool f = true;
            for (i = 0; i != inputCount; i++)
                f &= getInput(i);
            return f;
        }
    }
}