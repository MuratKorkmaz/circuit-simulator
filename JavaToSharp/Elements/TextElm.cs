using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace JavaToSharp
{
    internal class TextElm : CircuitElm
    {
        private string text;
        private ArrayList lines;
        private int size;
        private const int FLAG_CENTER = 1;
        private const int FLAG_BAR = 2;

        public TextElm(int xx, int yy) : base(xx, yy)
        {
            text = "hello";
            lines = new ArrayList();
            lines.Add(text);
            size = 24;
        }
        public TextElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            size = new (int)int?
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
            lines = new ArrayList();
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
            g.Color = needsHighlight() ? selectColor : lightGrayColor;
            Font f = new Font("SansSerif", 0, size);
            g.Font = f;
            FontMetrics fm = g.FontMetrics;
            int i;
            int maxw = -1;
            for (i = 0; i != lines.Count; i++)
            {
                int w = fm.stringWidth((string)(lines[i]));
                if (w > maxw)
                    maxw = w;
            }
            int cury = y;
            setBbox(x, y, x, y);
            for (i = 0; i != lines.Count; i++)
            {
                string s = (string)(lines[i]);
                if ((flags & FLAG_CENTER) != 0)
                    x = (sim.winSize.Width-fm.stringWidth(s))/2;
                g.drawString(s, x, cury);
                if ((flags & FLAG_BAR) != 0)
                {
                    int by = cury-fm.Ascent;
                    g.drawLine(x, by, x+fm.stringWidth(s)-1, by);
                }
                adjustBbox(x, cury-fm.Ascent, x+fm.stringWidth(s), cury+fm.Descent);
                cury += fm.Height;
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
                ei.checkbox = new Checkbox("Center", (flags & FLAG_CENTER) != 0);
                return ei;
            }
            if (n == 3)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new Checkbox("Draw Bar On Top", (flags & FLAG_BAR) != 0);
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
                if (ei.checkbox.State)
                    flags |= FLAG_BAR;
                else
                    flags &= ~FLAG_BAR;
            }
            if (n == 2)
            {
                if (ei.checkbox.State)
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

