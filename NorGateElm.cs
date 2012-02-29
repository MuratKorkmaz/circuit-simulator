using System;

class NorGateElm:OrGateElm
{
	override internal System.String GateName
	{
		get
		{
			return "элемент ИЛИ-НЕ";
		}
		
	}
	override internal bool Inverting
	{
		get
		{
			return true;
		}
		
	}
	override internal int DumpType
	{
		get
		{
			return 153;
		}
		
	}
	public NorGateElm(int xx, int yy):base(xx, yy)
	{
	}
	public NorGateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
	}
}