using System;

	internal class PMosfetElm : MosfetElm
	{
	public PMosfetElm(int xx, int yy) : base(xx, yy, true)
	{
	}
	internal override Type DumpClass
	{
		get
		{
			return typeof(MosfetElm);
		}
	}
	}
