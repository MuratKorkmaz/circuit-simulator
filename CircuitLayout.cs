//UPGRADE_ISSUE: Interface 'java.awt.LayoutManager' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtLayoutManager'"
namespace circuit_emulator
{
    class CircuitLayout
    {
        public CircuitLayout()
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
            int cw = targetw * 8 / 10;
            int targeth = target.Size.Height - (insets[0] + insets[3]);
            //UPGRADE_TODO: Method 'java.awt.Component.move' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentmove_int_int'"
            target.Controls[0].Location = new System.Drawing.Point(insets[1], insets[0]);
            target.Controls[0].Size = new System.Drawing.Size(cw, targeth);
            int barwidth = targetw - cw;
            cw += insets[1];
            int i;
            int h = insets[0];
            for (i = 1; i < (int) target.Controls.Count; i++)
            {
                System.Windows.Forms.Control m = target.Controls[i];
                //UPGRADE_TODO: Method 'java.awt.Component.isVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentisVisible'"
                if (m.Visible)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Component.getPreferredSize' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    System.Drawing.Size d = m is CircuitCanvas?((CircuitCanvas) m).getPreferredSize():m.Size;
                    //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
                    if (m is System.Windows.Forms.ScrollBar)
                        d.Width = barwidth;
                    //UPGRADE_TODO: Class 'java.awt.Choice' was converted to 'System.Windows.Forms.ComboBox' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtChoice'"
                    if (m is System.Windows.Forms.ComboBox && d.Width > barwidth)
                        d.Width = barwidth;
                    if (m is System.Windows.Forms.Label)
                    {
                        h += d.Height / 5;
                        d.Width = barwidth;
                    }
                    //UPGRADE_TODO: Method 'java.awt.Component.move' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentmove_int_int'"
                    m.Location = new System.Drawing.Point(cw, h);
                    m.Size = new System.Drawing.Size(d.Width, d.Height);
                    h += d.Height;
                }
            }
        }
    }
}
