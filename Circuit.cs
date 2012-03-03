// Circuit.java (c) 2005,2008 by Paul Falstad, www.falstad.com

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace circuit_emulator
{
    //UPGRADE_TODO: Class 'java.applet.Applet' was converted to 'System.Windows.Forms.UserControl' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaappletApplet'"
    [Serializable]
    public class Circuit : UserControl
    {
        internal static CirSim ogf;
        private Circuit Circuit_GeneratedVar;
        public String TempDocumentBaseVar = "";
        public bool isActiveVar = true;
        internal bool started;

        public Circuit()
        {
            InitBlock();
        }

        public virtual Uri DocumentBase
        {
            get
            {
                if (TempDocumentBaseVar == "")
                    return new Uri("http://127.0.0.1");
                else
                    return new Uri(TempDocumentBaseVar);
            }
        }

        private void InitBlock()
        {
            Load += Circuit_StartEventHandler;
            Disposed += Circuit_StopEventHandler;
        }

        internal virtual void destroyFrame()
        {
            if (ogf != null)
                ogf.Dispose();
            ogf = null;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            Refresh();
        }

        //UPGRADE_TODO: Commented code was moved to the 'InitializeComponent' method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1228'"
        public void init()
        {
            InitializeComponent();

            Circuit_GeneratedVar = this;
            /*
			//UPGRADE_WARNING: Extra logic should be included into componentHidden to know if the Component is hidden. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1144'"
			VisibleChanged += new System.EventHandler(this.Circuit_VisibleChanged);
			Move += new System.EventHandler(this.Circuit_Move);
			Resize += new System.EventHandler(this.Circuit_Resize);
			//UPGRADE_WARNING: Extra logic should be included into componentShown to know if the Component is visible. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1145'"
			VisibleChanged += new System.EventHandler(this.Circuit_VisibleChanged2);*/
        }

        [STAThread]
        public static void Main()
        {
            Application.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Application.Run(new CirSim(null));
        }

        internal virtual void showFrame()
        {
            if (ogf == null)
            {
                started = true;
                ogf = new CirSim(this);
                ogf.init();
                //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                Refresh();
            }
        }

        public virtual void toggleSwitch(int x)
        {
            ogf.toggleSwitch(x);
        }

        protected override void OnPaint(PaintEventArgs g_EventArg)
        {
            Graphics g = null;
            if (g_EventArg != null)
                g = g_EventArg.Graphics;
            String s = "Апплет запущен в отдельном окне.";
            if (!started)
                s = "Апплет запускается.";
            else if (ogf == null)
                s = "Апплет завершен.";
            else if (ogf.useFrame)
                ogf.triggerShow();
            //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
            g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g),
                         SupportClass.GraphicsManager.manager.GetBrush(g), 10,
                         30 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
        }

        public virtual void componentHidden(Object event_sender, EventArgs e)
        {
        }

        public virtual void componentMoved(Object event_sender, EventArgs e)
        {
        }

        public virtual void componentShown(Object event_sender, EventArgs e)
        {
            showFrame();
        }

        public virtual void componentResized(Object event_sender, EventArgs e)
        {
            if (ogf != null)
                ogf.componentResized(event_sender, e);
        }

        //UPGRADE_TODO: The equivalent of the 'java.applet.Applet.destroy' method needs to be modified in order to work properly. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1178'"
        //UPGRADE_NOTE: Since the declaration of the following entity is not virtual in .NET the modifier new was added. References to it may have been changed to InvokeMethodAsVirtual, GetPropertyAsVirtual or SetPropertyAsVirtual. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1195'"
        public new virtual void Dispose()
        {
            if (ogf != null)
                ogf.Dispose();
            ogf = null;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            Refresh();
        }

        public void ResizeControl(Size p)
        {
            Width = p.Width;
            Height = p.Height;
        }

        public void ResizeControl(int p2, int p3)
        {
            Width = p2;
            Height = p3;
        }

        public String GetUserControlInfo()
        {
            return null;
        }

        public String[][] GetParameterInfo()
        {
            return null;
        }

        public Image getImage(Uri p4)
        {
            var TemporalyBitmap = new Bitmap(p4.AbsolutePath);
            return TemporalyBitmap;
        }

        public Image getImage(Uri p5, String p6)
        {
            var TemporalyBitmap = new Bitmap(p5.AbsolutePath + p6);
            return TemporalyBitmap;
        }

        public virtual Boolean isActive()
        {
            return isActiveVar;
        }

        public virtual void start()
        {
            isActiveVar = true;
        }

        public virtual void stop()
        {
            isActiveVar = false;
        }

        private void Circuit_StartEventHandler(Object sender, EventArgs e)
        {
            init();
            start();
        }

        private void Circuit_StopEventHandler(Object sender, EventArgs e)
        {
            stop();
        }

        public virtual String getParameter(String paramName)
        {
            return null;
        }

        public void Circuit_VisibleChanged(Object event_sender, EventArgs e)
        {
            Circuit_GeneratedVar.componentHidden(event_sender, e);
        }

        public void Circuit_Move(Object event_sender, EventArgs e)
        {
            Circuit_GeneratedVar.componentMoved(event_sender, e);
        }

        public void Circuit_Resize(Object event_sender, EventArgs e)
        {
            Circuit_GeneratedVar.componentResized(event_sender, e);
        }

        public void Circuit_VisibleChanged2(Object event_sender, EventArgs e)
        {
            Circuit_GeneratedVar.componentShown(event_sender, e);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Circuit
            // 
            this.BackColor = System.Drawing.Color.LightGray;
            this.Name = "Circuit";
            this.Size = new System.Drawing.Size(157, 156);
            this.VisibleChanged += new System.EventHandler(this.Circuit_VisibleChanged2);
            this.Move += new System.EventHandler(this.Circuit_Move);
            this.Resize += new System.EventHandler(this.Circuit_Resize);
            this.ResumeLayout(false);
        }

        #endregion
    }
}