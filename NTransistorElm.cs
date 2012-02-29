using System;
class NTransistorElm:TransistorElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(TransistorElm);
		}
		
	}
	public NTransistorElm(int xx, int yy):base(xx, yy, false)
	{
	}
}