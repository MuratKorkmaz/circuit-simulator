using System.ComponentModel;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class EditDialogLayout : LayoutManager
    {
        public EditDialogLayout()
        {
        }
        public virtual void addLayoutComponent(string name, Component c)
        {
        }
        public virtual void removeLayoutComponent(Component c)
        {
        }
        public virtual Dimension preferredLayoutSize(Container target)
        {
            return new Dimension(500, 500);
        }
        public virtual Dimension minimumLayoutSize(Container target)
        {
            return new Dimension(100,100);
        }
        public virtual void layoutContainer(Container target)
        {
            Insets insets = target.Insets();
            int targetw = target.Size().width - insets.left - insets.right;
            int targeth = target.Size().height - (insets.top+insets.bottom);
            int i;
            int h = insets.top;
            int pw = 300;
            int x = 0;
            for (i = 0; i < target.ComponentCount; i++)
            {
                Component m = target.getComponent(i);
                bool newline = true;
                if (m.Visible)
                {
                    Dimension d = m.PreferredSize;
                    if (pw < d.width)
                        pw = d.width;
                    if (m is Scrollbar)
                    {
                        h += 10;
                        d.width = targetw-x;
                    }
                    if (m is Choice && d.width > targetw)
                        d.width = targetw-x;
                    if (m is Label)
                    {
                        Dimension d2 = target.getComponent(i+1).PreferredSize;
                        if (d.height < d2.height)
                            d.height = d2.height;
                        h += d.height/5;
                        newline = false;
                    }
                    if (m is Button)
                    {
                        if (x == 0)
                            h += 20;
                        if (i != target.ComponentCount-1)
                            newline = false;
                    }
                    m.move(insets.left+x, h);
                    m.resize(d.width, d.height);
                    if (newline)
                    {
                        h += d.height;
                        x = 0;
                    }
                    else
                        x += d.width;
                }
            }
            if (target.size().height < h)
                target.resize(pw + insets.right, h + insets.bottom);
        }
    }
}

