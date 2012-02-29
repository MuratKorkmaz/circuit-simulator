using System;
class PTransistorElm:TransistorElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(TransistorElm);
		}
		
	}
	public PTransistorElm(int xx, int yy):base(xx, yy, true)
	{
	}
}