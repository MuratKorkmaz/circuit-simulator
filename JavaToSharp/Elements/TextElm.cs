using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class TextElm : CircuitElm
    {
        private string text;
        private List<string> lines;
        private int size;
        private const int FLAG_CENTER = 1;
        private const int FLAG_BAR = 2;

        public TextElm(int xx, int yy) : base(xx, yy)
        {
            text = "hello";
            lines = new List<string>();
            lines.Add(text);
            size = 24;
        }
        public TextElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sSize = st.nextToken();
            bool isParsedSize = int.TryParse(sSize, out size);
            if (!isParsedSize)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            text = st.nextToken();
            while (st.hasMoreTokens())
                text += ' ' + st.nextToken();
            Split();
        }
        internal virtual void split()
        {
            int i;
            lines = new List<string>();
            StringBuilder sb = new StringBuilder(text);
            for (i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                if (c == '\\')
                {
                    sb.Remove(i, 1);
                    c = sb[i];
                    if (c == 'n')
                    {
                        lines.Add(sb.Substring(0, i));
                        sb.Remove(0, i+1);
                        i = -1;
                    }
                }
            }
            lines.Add(sb.ToString());
        }
        internal override string dump()
        {
            return base.dump() + " " + size + " " + text;
        }
        internal override int DumpType
        {
            get
            {
                return 'x';
            }
        }
        internal override void drag(int xx, int yy)
        {
            x = xx;
            y = yy;
            x2 = xx+16;
            y2 = yy;
        }
        internal override void draw(Graphics g)
        {

            myPen = new Pen(g.GetNearestColor(needsHighlight() ? selectColor : lightGrayColor));
            myBrush = new SolidBrush(g.GetNearestColor(needsHighlight() ? selectColor : lightGrayColor));
            FontFamily fontFamily = new FontFamily("SansSerif");
            Font font = new Font(fontFamily, 16,FontStyle.Regular,GraphicsUnit.Pixel);
            ascent = fontFamily.GetCellAscent(FontStyle.Regular);
            descent = fontFamily.GetCellDescent(FontStyle.Regular);
            ascentPixel = font.Size * ascent / fontFamily.GetEmHeight(FontStyle.Regular);
            descentPixel = font.Size * descent / fontFamily.GetEmHeight(FontStyle.Regular);
            
           
            int i;
            int maxw = -1;
            for (i = 0; i != lines.Count; i++)
            {
                int w = (int)g.MeasureString(lines[i], font).Width;
                if (w > maxw)
                    maxw = w;
            }
            int cury = y;
            setBbox(x, y, x, y);
            for (i = 0; i != lines.Count; i++)
            {
                string s = (string)(lines[i]);
                int w = (int)g.MeasureString(lines[i], font).Width;
                if ((flags & FLAG_CENTER) != 0)
                    x = (sim.winSize.Width-w)/2;
                g.DrawString(s, font,myBrush,x, cury);
                if ((flags & FLAG_BAR) != 0)
                {
                    int by = cury-(int)ascentPixel;
                    g.DrawLine(myPen ,x, by, x+w-1, by);
                }
                adjustBbox(x, cury-(int)ascentPixel , x+w, cury+(int)descentPixel);
                cury +=Convert.ToInt32(ascentPixel + 2*descentPixel);
            }
            x2 = boundingBox.X + boundingBox.Width;
            y2 = boundingBox.Y + boundingBox.Height;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("Text", 0, -1, -1);
                ei.text = text;
                return ei;
            }
            if (n == 1)
                return new EditInfo("Размер", size, 5, 100);
            if (n == 2)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if ((flags & FLAG_CENTER) != 0)
                {
                    ei.checkbox.Text = "Center";
                    
                }
                return ei;
            }
            if (n == 3)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if ((flags & FLAG_BAR) != 0)
                {
                    ei.checkbox.Text = "Draw Bar On Top";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                text = ei.textf.Text;
                Split();
            }
            if (n == 1)
                size = (int) ei.value;
            if (n == 3)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_BAR;
                else
                    flags &= ~FLAG_BAR;
            }
            if (n == 2)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_CENTER;
                else
                    flags &= ~FLAG_CENTER;
            }
        }
        internal new bool isCenteredText
        {
            get
            {
                return (flags & FLAG_CENTER) != 0;
            }
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = text;
        }
        internal override int PostCount
        {
            get
            {
                return 0;
            }
        }
    }
}

