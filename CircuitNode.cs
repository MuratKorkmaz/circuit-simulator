using System;

//UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
public class CircuitNode
{
	internal int x, y;
	internal System.Collections.ArrayList links;
	internal bool internal_Renamed;
	internal CircuitNode()
	{
		links = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
	}
}