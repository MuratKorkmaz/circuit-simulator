using System;
class PMosfetElm:MosfetElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(MosfetElm);
		}
		
	}
	public PMosfetElm(int xx, int yy):base(xx, yy, true)
	{
	}
}