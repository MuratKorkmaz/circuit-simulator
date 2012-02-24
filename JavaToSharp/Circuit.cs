// Circuit.java (c) 2005,2008 by Paul Falstad, www.falstad.com


using System.Drawing;

namespace JavaToSharp
{
    public class Circuit : Applet, ComponentListener
    {
        private static CirSim ogf;
        internal virtual void destroyFrame()
        {
            if (ogf != null)
                ogf.Dispose();
            ogf = null;
            repaint();
        }

        private bool started;
        public virtual void init()
        {
            addComponentListener(this);
        }

        static void SMain(string[] args)
        {
            ogf = new CirSim(null);
            ogf.init();
        }

        protected virtual void showFrame()
        {
            if (ogf == null)
            {
                started = true;
                ogf = new CirSim(this);
                ogf.init();
                repaint();
            }
        }

        public virtual void toggleSwitch(int x)
        {
            ogf.toggleSwitch(x);
        }

        public virtual void paint(Graphics g)
        {
            string s = "Апплет запущен в отдельном окне.";
            if (!started)
                s = "Апплет запускается.";
            else if (ogf == null)
                s = "Апплет завершен.";
            else if (ogf.useFrame)
                ogf.triggerShow();
            g.DrawString(s, SystemFonts.DefaultFont, Brushes.White, 10, 30);
        }

        public virtual void componentHidden(ComponentEvent e)
        {
        }
        public virtual void componentMoved(ComponentEvent e)
        {
        }
        public virtual void componentShown(ComponentEvent e)
        {
            showFrame();
        }
        public virtual void componentResized(ComponentEvent e)
        {
            if (ogf != null)
                ogf.componentResized(e);
        }

        public virtual void destroy()
        {
            if (ogf != null)
                ogf.Dispose();
            ogf = null;
            repaint();
        }
    }
}

