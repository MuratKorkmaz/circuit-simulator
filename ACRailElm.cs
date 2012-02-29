using System;
class ACRailElm:RailElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(RailElm);
		}
		
	}
	public ACRailElm(int xx, int yy):base(xx, yy, WF_AC)
	{
	}
}