using System;

class OrGateElm:GateElm
{
	override internal System.String GateName
	{
		get
		{
			return "элемент ИЛИ";
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 152;
		}
		
	}
	public OrGateElm(int xx, int yy):base(xx, yy)
	{
	}
	public OrGateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
	internal override void  setPoints()
	{
		base.setPoints();
		
		// 0-15 = top curve, 16 = right, 17-32=bottom curve,
		// 33-37 = left curve
		System.Drawing.Point[] triPoints = newPointArray(38);
		if (this is XorGateElm)
			linePoints = new System.Drawing.Point[5];
		int i;
		for (i = 0; i != 16; i++)
		{
			double a = i / 16.0;
			double b = 1 - a * a;
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint2(ref lead1, ref lead2, ref triPoints[i], ref triPoints[32 - i], .5 + a / 2, b * hs2);
		}
		double ww2 = (ww == 0)?dn * 2:ww * 2;
		for (i = 0; i != 5; i++)
		{
			double a = (i - 2) / 2.0;
			double b = 4 * (1 - a * a) - 2;
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			interpPoint(ref lead1, ref lead2, ref triPoints[33 + i], b / (ww2), a * hs2);
			if (this is XorGateElm)
			{
				//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
				linePoints[i] = interpPoint(ref lead1, ref lead2, (b - 5) / (ww2), a * hs2);
			}
		}
		triPoints[16] = new System.Drawing.Point(new System.Drawing.Size(lead2));
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
		bool f = false;
		for (i = 0; i != inputCount; i++)
			f |= getInput(i);
		return f;
	}
}