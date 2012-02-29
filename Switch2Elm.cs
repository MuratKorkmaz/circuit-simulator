using System;

class Switch2Elm:SwitchElm
{
	override internal int DumpType
	{
		get
		{
			return 'S';
		}
		
	}
	override internal int PostCount
	{
		get
		{
			return 3;
		}
		
	}
	override internal int VoltageSourceCount
	{
		get
		{
			return (position == 2)?0:1;
		}
		
	}
	internal int link;
	internal const int FLAG_CENTER_OFF = 1;
	
	public Switch2Elm(int xx, int yy):base(xx, yy, false)
	{
		noDiagonal = true;
	}
	internal Switch2Elm(int xx, int yy, bool mm):base(xx, yy, mm)
	{
		noDiagonal = true;
	}
	public Switch2Elm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
	{
		link = System.Int32.Parse(st.NextToken());
		noDiagonal = true;
	}
	internal override System.String dump()
	{
		return base.dump() + " " + link;
	}
	
	//UPGRADE_NOTE: Final was removed from the declaration of 'openhs '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int openhs = 16;
	internal System.Drawing.Point[] swposts, swpoles;
	internal override void  setPoints()
	{
		base.setPoints();
		calcLeads(32);
		swposts = newPointArray(2);
		swpoles = newPointArray(3);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref lead1, ref lead2, ref swpoles[0], ref swpoles[1], 1, openhs);
		swpoles[2] = lead2;
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		interpPoint2(ref point1, ref point2, ref swposts[0], ref swposts[1], 1, openhs);
		posCount = hasCenterOff()?3:2;
	}
	
	internal override void  draw(System.Drawing.Graphics g)
	{
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		setBbox(ref point1, ref point2, openhs);
		
		// draw first lead
		setVoltageColor(g, volts[0]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref point1, ref lead1);
		
		// draw second lead
		setVoltageColor(g, volts[1]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref swpoles[0], ref swposts[0]);
		
		// draw third lead
		setVoltageColor(g, volts[2]);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref swpoles[1], ref swposts[1]);
		
		// draw switch
		if (!needsHighlight())
			SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawThickLine(g, ref lead1, ref swpoles[position]);
		
		updateDotCount();
		//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
		drawDots(g, ref point1, ref lead1, curcount);
		if (position != 2)
		{
			//UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
			drawDots(g, ref swpoles[position], ref swposts[position], curcount);
		}
		drawPosts(g);
	}
	internal override System.Drawing.Point getPost(int n)
	{
		return (n == 0)?point1:swposts[n - 1];
	}
	internal override void  calculateCurrent()
	{
		if (position == 2)
			current = 0;
	}
	internal override void  stamp()
	{
		if (position == 2)
		// in center?
			return ;
		sim.stampVoltageSource(nodes[0], nodes[position + 1], voltSource, 0);
	}
	internal override void  toggle()
	{
		base.toggle();
		if (link != 0)
		{
			int i;
			for (i = 0; i != sim.elmList.Count; i++)
			{
				System.Object o = sim.elmList[i];
				if (o is Switch2Elm)
				{
					Switch2Elm s2 = (Switch2Elm) o;
					if (s2.link == link)
						s2.position = position;
				}
			}
		}
	}
	internal override bool getConnection(int n1, int n2)
	{
		if (position == 2)
			return false;
		return comparePair(n1, n2, 0, 1 + position);
	}
	internal override void  getInfo(System.String[] arr)
	{
		arr[0] = (link == 0)?"switch (SPDT)":"switch (DPDT)";
		arr[1] = "I = " + getCurrentDText(getCurrent());
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 1)
		{
			EditInfo ei = new EditInfo("", 0, - 1, - 1);
			ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Center Off", hasCenterOff());
			return ei;
		}
		return base.getEditInfo(n);
	}
	public override void  setEditValue(int n, EditInfo ei)
	{
		if (n == 1)
		{
			flags &= ~ FLAG_CENTER_OFF;
			if (ei.checkbox.Checked)
				flags |= FLAG_CENTER_OFF;
			if (hasCenterOff())
				momentary = false;
			setPoints();
		}
		else
			base.setEditValue(n, ei);
	}
	internal virtual bool hasCenterOff()
	{
		return (flags & FLAG_CENTER_OFF) != 0;
	}
}