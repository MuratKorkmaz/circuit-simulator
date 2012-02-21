using System;
using JavaToSharp.Elements;

internal class NJfetElm : JfetElm
	{
	public NJfetElm(int xx, int yy) : base(xx, yy, false)
	{
	}
	internal virtual Type DumpClass
	{
		get
		{
			return typeof(JfetElm);
		}
	}
	}

	internal class PJfetElm : JfetElm
	{
	public PJfetElm(int xx, int yy) : base(xx, yy, true)
	{
	}
	internal virtual Type DumpClass
	{
		get
		{
			return typeof(JfetElm);
		}
	}
	}

