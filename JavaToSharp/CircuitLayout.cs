using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class CircuitLayout
    {
        public CircuitLayout()
        {
        }
        public virtual void addLayoutComponent(string name, Component c)
        {
        }
        public virtual void removeLayoutComponent(Component c)
        {
        }
        public virtual Size preferredLayoutSize(Container target)
        {
            return new Size(500, 500);
        }
        public virtual Size minimumLayoutSize(Container target)
        {
            return new Size(100,100);
        }
        public virtual void layoutContainer(Container target)
        {
            //Insets insets = target.insets();
         //   int targetw = target.size().width - insets.left - insets.right;
          //  int cw = targetw* 8/10;
         //   int targeth = target.size().height - (insets.top+insets.bottom);
          //  target.getComponent(0).move(insets.left, insets.top);
          //  target.getComponent(0).resize(cw, targeth);
         //   int barwidth = targetw - cw;
         //   cw += insets.left;
            int i;
         //   int h = insets.top;
          //  for (i = 1; i < target.ComponentCount; i++)
           // {
           //     Component m = target.getComponent(i);
           //     if (m.Visible)
            //    {
            //        Dimension d = m.PreferredSize;
             //       if (m is Scrollbar)
                //        d.width = barwidth;
                //    if (m is Choice && d.width > barwidth)
                  //      d.width = barwidth;
                 //   if (m is Label)
                 //   {
                  //      h += d.height/5;
                 //       d.width = barwidth;
                 //   }
                 //   m.move(cw, h);
                 //   m.resize(d.width, d.height);
                 //   h += d.height;
                }
            }
        }
    //}
//}
