using System.Collections.Generic;

namespace circuit_emulator
{
    class TextElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'x';
            }
		
        }
        override internal bool CenteredText
        {
            get
            {
                return (flags & FLAG_CENTER) != 0;
            }
		
        }
        override internal int PostCount
        {
            get
            {
                return 0;
            }
		
        }
        internal System.String text;
        private List<string> lines;
        internal int size;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_CENTER '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_CENTER = 1;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_BAR '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_BAR = 2;
        public TextElm(int xx, int yy):base(xx, yy)
        {
            text = "hello";
            lines = new List<string>();
            lines.Add(text);
            size = 24;
        }
        public TextElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            size = System.Int32.Parse(st.NextToken());
            text = st.NextToken();
            while (st.HasMoreTokens())
                text += (' ' + st.NextToken());
            split();
        }
        internal virtual void  split()
        {
            int i;
            lines = new List<string>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text);
            for (i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                if (c == '\\')
                {
                    sb.Remove(i, 1);
                    c = sb[i];
                    if (c == 'n')
                    {
                        lines.Add(sb.ToString(0, i));
                        sb.Remove(0, i + 1 - 0);
                        i = - 1;
                        continue;
                    }
                }
            }
            lines.Add(sb.ToString());
        }
        internal override System.String dump()
        {
            return base.dump() + " " + size + " " + text;
        }
        internal override void  drag(int xx, int yy)
        {
            x = xx;
            y = yy;
            x2 = xx + 16;
            y2 = yy;
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
            //UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
            System.Drawing.Font f = new System.Drawing.Font("SansSerif", size, System.Drawing.FontStyle.Regular);
            SupportClass.GraphicsManager.manager.SetFont(g, f);
            System.Drawing.Font fm = SupportClass.GraphicsManager.manager.GetFont(g);
            int i;
            int maxw = - 1;
            for (i = 0; i != lines.Count; i++)
            {
                //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
			
                int w = (int)g.MeasureString(lines[i], f).Width;
                if (w > maxw)
                    maxw = w;
            }
            int cury = y;
            setBbox(x, y, x, y);
            for (i = 0; i != lines.Count; i++)
            {
                string s = (string)(lines[i]);
                int w = (int)g.MeasureString(lines[i], f).Width;
                if ((flags & FLAG_CENTER) != 0)
                {
                    //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
				

                    x = (sim.winSize.Width - w) / 2;
                
                }
                //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, cury - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
                if ((flags & FLAG_BAR) != 0)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    int by = cury - SupportClass.GetAscent(fm);
                    //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
                    g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), x, by, x + w - 1, by);
                }
                //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
                //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getDescent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                adjustBbox(x, cury - SupportClass.GetAscent(fm), x + w, cury + SupportClass.GetDescent(fm));
                //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getHeight' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                cury += (int) fm.GetHeight();
            }
            x2 = boundingBox.X + boundingBox.Width;
            y2 = boundingBox.Y + boundingBox.Height;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("Text", 0, - 1, - 1);
                ei.text = text;
                return ei;
            }
            if (n == 1)
                return new EditInfo("Размер", size, 5, 100);
            if (n == 2)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Center", (flags & FLAG_CENTER) != 0);
                return ei;
            }
            if (n == 3)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Draw Bar On Top", (flags & FLAG_BAR) != 0);
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                text = ei.textf.Text;
                split();
            }
            if (n == 1)
            {
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                size = (int) ei.value_Renamed;
            }
            if (n == 3)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_BAR;
                else
                    flags &= ~ FLAG_BAR;
            }
            if (n == 2)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_CENTER;
                else
                    flags &= ~ FLAG_CENTER;
            }
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = text;
        }
    }
}