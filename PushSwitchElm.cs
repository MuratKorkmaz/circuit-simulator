using System;
class PushSwitchElm:SwitchElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(SwitchElm);
		}
		
	}
	public PushSwitchElm(int xx, int yy):base(xx, yy, true)
	{
	}
}