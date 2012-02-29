using System;

//UPGRADE_ISSUE: Interface 'java.awt.LayoutManager' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtLayoutManager'"
class EditDialogLayout : LayoutManager
{
	public EditDialogLayout()
	{
	}
	public virtual void  addLayoutComponent(System.String name, System.Windows.Forms.Control c)
	{
	}
	public virtual void  removeLayoutComponent(System.Windows.Forms.Control c)
	{
	}
	public virtual System.Drawing.Size preferredLayoutSize(System.Windows.Forms.Control target)
	{
		return new System.Drawing.Size(500, 500);
	}
	public virtual System.Drawing.Size minimumLayoutSize(System.Windows.Forms.Control target)
	{
		return new System.Drawing.Size(100, 100);
	}
	public virtual void  layoutContainer(System.Windows.Forms.Control target)
	{
		System.Int32[] insets = SupportClass.GetInsets(target);
		int targetw = target.Size.Width - insets[1] - insets[2];
		int targeth = target.Size.Height - (insets[0] + insets[3]);
		int i;
		int h = insets[0];
		int pw = 300;
		int x = 0;
		for (i = 0; i < (int) target.Controls.Count; i++)
		{
			System.Windows.Forms.Control m = target.Controls[i];
			bool newline = true;
			//UPGRADE_TODO: Method 'java.awt.Component.isVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentisVisible'"
			if (m.Visible)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Component.getPreferredSize' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Drawing.Size d = m is CircuitCanvas?((CircuitCanvas) m).getPreferredSize():m.Size;
				if (pw < d.Width)
					pw = d.Width;
				//UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
				if (m is System.Windows.Forms.ScrollBar)
				{
					h += 10;
					d.Width = targetw - x;
				}
				//UPGRADE_TODO: Class 'java.awt.Choice' was converted to 'System.Windows.Forms.ComboBox' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtChoice'"
				if (m is System.Windows.Forms.ComboBox && d.Width > targetw)
					d.Width = targetw - x;
				if (m is System.Windows.Forms.Label)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Component.getPreferredSize' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
					System.Drawing.Size d2 = target.Controls[i + 1].Size;
					if (d.Height < d2.Height)
						d.Height = d2.Height;
					h += d.Height / 5;
					newline = false;
				}
				if (m is System.Windows.Forms.Button)
				{
					if (x == 0)
						h += 20;
					if (i != (int) target.Controls.Count - 1)
						newline = false;
				}
				//UPGRADE_TODO: Method 'java.awt.Component.move' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentmove_int_int'"
				m.Location = new System.Drawing.Point(insets[1] + x, h);
				m.Size = new System.Drawing.Size(d.Width, d.Height);
				if (newline)
				{
					h += d.Height;
					x = 0;
				}
				else
					x += d.Width;
			}
		}
		if (target.Size.Height < h)
			target.Size = new System.Drawing.Size(pw + insets[2], h + insets[3]);
	}
}
