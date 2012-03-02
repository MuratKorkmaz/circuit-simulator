// Circuit.java (c) 2005,2008 by Paul Falstad, www.falstad.com
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
namespace circuit_emulator
{
	
	//UPGRADE_TODO: Class 'java.applet.Applet' was converted to 'System.Windows.Forms.UserControl' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaappletApplet'"
	[Serializable]
	public class Circuit:UserControl
	{

		public Circuit()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			Load += Circuit_StartEventHandler;
			Disposed += Circuit_StopEventHandler;
		}
		public bool isActiveVar = true;
		internal static CirSim ogf;
		internal virtual void  destroyFrame()
		{
			if (ogf != null)
				ogf.Dispose();
			ogf = null;
			//UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
			Refresh();
		}
		internal bool started = false;
		//UPGRADE_TODO: Commented code was moved to the 'InitializeComponent' method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1228'"
		public void  init()
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
		public static void  Main()
		{
		    Application.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			Application.Run(new CirSim(null));
		}
		
		internal virtual void  showFrame()
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
		
		public virtual void  toggleSwitch(int x)
		{
			ogf.toggleSwitch(x);
		}
		
		protected override void  OnPaint(System.Windows.Forms.PaintEventArgs g_EventArg)
		{
			System.Drawing.Graphics g = null;
			if (g_EventArg != null)
				g = g_EventArg.Graphics;
			System.String s = "Апплет запущен в отдельном окне.";
			if (!started)
				s = "Апплет запускается.";
			else if (ogf == null)
				s = "Апплет завершен.";
			else if (ogf.useFrame)
				ogf.triggerShow();
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(s, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), 10, 30 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
		}
		
		public virtual void  componentHidden(System.Object event_sender, System.EventArgs e)
		{
		}
		public virtual void  componentMoved(System.Object event_sender, System.EventArgs e)
		{
		}
		public virtual void  componentShown(System.Object event_sender, System.EventArgs e)
		{
			showFrame();
		}
		public virtual void  componentResized(System.Object event_sender, System.EventArgs e)
		{
			if (ogf != null)
				ogf.componentResized(event_sender, e);
		}
		
		//UPGRADE_TODO: The equivalent of the 'java.applet.Applet.destroy' method needs to be modified in order to work properly. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1178'"
		//UPGRADE_NOTE: Since the declaration of the following entity is not virtual in .NET the modifier new was added. References to it may have been changed to InvokeMethodAsVirtual, GetPropertyAsVirtual or SetPropertyAsVirtual. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1195'"
		new virtual public  void  Dispose()
		{
			if (ogf != null)
				ogf.Dispose();
			ogf = null;
			//UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
			Refresh();
		}
		public void  ResizeControl(System.Drawing.Size p)
		{
			this.Width = p.Width;
			this.Height = p.Height;
		}
		public void  ResizeControl(int p2, int p3)
		{
			this.Width = p2;
			this.Height = p3;
		}
		public System.String GetUserControlInfo()
		{
			return null;
		}
		public System.String[][] GetParameterInfo()
		{
			return null;
		}
		public System.String  TempDocumentBaseVar = "";
		public virtual System.Uri DocumentBase
		{
			get
			{
				if (TempDocumentBaseVar == "")
					return new System.Uri("http://127.0.0.1");
				else
					return new System.Uri(TempDocumentBaseVar);
			}
			
		}
		public System.Drawing.Image getImage(System.Uri p4)
		{
			Bitmap TemporalyBitmap = new Bitmap(p4.AbsolutePath);
			return (Image) TemporalyBitmap;
		}
		public System.Drawing.Image getImage(System.Uri p5, System.String p6)
		{
			Bitmap TemporalyBitmap = new Bitmap(p5.AbsolutePath + p6);
			return (Image) TemporalyBitmap;
		}
		public virtual System.Boolean isActive()
		{
			return isActiveVar;
		}
		public virtual void  start()
		{
			isActiveVar = true;
		}
		public virtual void  stop()
		{
			isActiveVar = false;
		}
		private void  Circuit_StartEventHandler(System.Object sender, System.EventArgs e)
		{
			init();
			start();
		}
		private void  Circuit_StopEventHandler(System.Object sender, System.EventArgs e)
		{
			stop();
		}
		public virtual String getParameter(System.String paramName)
		{
			return null;
		}
		#region Windows Form Designer generated code
		private void  InitializeComponent()
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
		private Circuit Circuit_GeneratedVar;
		public void  Circuit_VisibleChanged(System.Object event_sender, System.EventArgs e)
		{
			Circuit_GeneratedVar.componentHidden(event_sender, e);
		}
		public void  Circuit_Move(System.Object event_sender, System.EventArgs e)
		{
			Circuit_GeneratedVar.componentMoved(event_sender, e);
		}
		public void  Circuit_Resize(System.Object event_sender, System.EventArgs e)
		{
			Circuit_GeneratedVar.componentResized(event_sender, e);
		}
		public void  Circuit_VisibleChanged2(System.Object event_sender, System.EventArgs e)
		{
			Circuit_GeneratedVar.componentShown(event_sender, e);
		}
	}
	
}