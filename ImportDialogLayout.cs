//UPGRADE_ISSUE: Interface 'java.awt.LayoutManager' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtLayoutManager'"
namespace circuit_emulator
{
    class ImportDialogLayout
    {
        public ImportDialogLayout()
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
            int pw = 300;
            if ((int) target.Controls.Count == 0)
                return ;
            System.Windows.Forms.Control cl = target.Controls[(int) target.Controls.Count - 1];
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Component.getPreferredSize' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Drawing.Size dl = cl is CircuitCanvas?((CircuitCanvas) cl).getPreferredSize():cl.Size;
            //UPGRADE_TODO: Method 'java.awt.Component.move' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentmove_int_int'"
            target.Controls[0].Location = new System.Drawing.Point(insets[1], insets[0]);
            int cw = target.Size.Width - insets[1] - insets[2];
            int ch = target.Size.Height - insets[0] - insets[3] - dl.Height;
            target.Controls[0].Size = new System.Drawing.Size(cw, ch);
            int h = ch + insets[0];
            int x = 0;
            for (i = 1; i < (int) target.Controls.Count; i++)
            {
                System.Windows.Forms.Control m = target.Controls[i];
                //UPGRADE_TODO: Method 'java.awt.Component.isVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentisVisible'"
                if (m.Visible)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Component.getPreferredSize' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    System.Drawing.Size d = m is CircuitCanvas?((CircuitCanvas) m).getPreferredSize():m.Size;
                    //UPGRADE_TODO: Method 'java.awt.Component.move' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentmove_int_int'"
                    m.Location = new System.Drawing.Point(insets[1] + x, h);
                    m.Size = new System.Drawing.Size(d.Width, d.Height);
                    x += d.Width;
                }
            }
        }
    }
}
