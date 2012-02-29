using System;
class DCVoltageElm:VoltageElm
{
	override internal System.Type DumpClass
	{
		get
		{
			return typeof(VoltageElm);
		}
		
	}
	public DCVoltageElm(int xx, int yy):base(xx, yy, WF_DC)
	{
	}
}