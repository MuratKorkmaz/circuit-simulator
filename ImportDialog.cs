using System;

namespace circuit_emulator
{
    [Serializable]
    class ImportDialog:System.Windows.Forms.Form
    {
        internal CirSim cframe;
        internal System.Windows.Forms.Button importButton, closeButton;
        internal System.Windows.Forms.TextBox text;
        internal bool isURL;
	
        internal ImportDialog(CirSim f, System.String str, bool url):base()
        {
            //UPGRADE_TODO: Constructor 'java.awt.Dialog.Dialog' was converted to 'SupportClass.DialogSupport.SetDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogDialog_javaawtFrame_javalangString_boolean'"
            SupportClass.DialogSupport.SetDialog(this, f, (str.Length > 0)?"Export":"Import");
            isURL = url;
            cframe = f;
            //UPGRADE_ISSUE: Method 'java.awt.Container.setLayout' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtContainersetLayout_javaawtLayoutManager'"
            setLayout(new ImportDialogLayout());
            System.Windows.Forms.TextBox temp_TextBox2;
            //UPGRADE_TODO: The equivalent in .NET for field 'java.awt.TextArea.SCROLLBARS_BOTH' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'";;
            temp_TextBox2 = new System.Windows.Forms.TextBox();
            temp_TextBox2.Text = str;
            temp_TextBox2.WordWrap = false;
            temp_TextBox2.ScrollBars = (System.Windows.Forms.ScrollBars) System.Windows.Forms.ScrollBars.Both;
            temp_TextBox2.Multiline = true;
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control;
            temp_Control = text = temp_TextBox2;
            Controls.Add(temp_Control);
            System.Windows.Forms.Button temp_Button;
            temp_Button = new System.Windows.Forms.Button();
            temp_Button.Text = "Import";
            importButton = temp_Button;
            if (!isURL)
            {
                //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
                Controls.Add(importButton);
            }
            importButton.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(importButton);
            System.Windows.Forms.Button temp_Button3;
            temp_Button3 = new System.Windows.Forms.Button();
            temp_Button3.Text = "Close";
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control2;
            temp_Control2 = closeButton = temp_Button3;
            Controls.Add(temp_Control2);
            closeButton.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(closeButton);
            System.Windows.Forms.Control temp_Control3;
            //UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.awt.Component.getLocationOnScreen' may be different. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1099'"
            temp_Control3 = CirSim.main;
            System.Drawing.Point x = temp_Control3.PointToScreen(temp_Control3.Location);
            Size = new System.Drawing.Size(400, 300);
            System.Drawing.Size d = Size;
            //UPGRADE_TODO: Method 'java.awt.Component.setLocation' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetLocation_int_int'"
            Location = new System.Drawing.Point(x.X + (cframe.winSize.Width - d.Width) / 2, x.Y + (cframe.winSize.Height - d.Height) / 2);
            //UPGRADE_TODO: Method 'java.awt.Dialog.show' was converted to 'System.Windows.Forms.Form.ShowDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogshow'"
            ShowDialog();
            if (str.Length > 0)
                text.SelectAll();
        }
	
        public virtual void  actionPerformed(System.Object event_sender, System.EventArgs e)
        {
            int i;
            System.Object src = event_sender;
            if (src == importButton)
            {
                cframe.readSetup(text.Text);
                //UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                Visible = false;
            }
            if (src == closeButton)
            {
                //UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                Visible = false;
            }
        }
	
        //UPGRADE_NOTE: The equivalent of method 'java.awt.Component.handleEvent' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //UPGRADE_ISSUE: Class 'java.awt.Event' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
        public bool handleEvent(Event ev)
        {
            //UPGRADE_ISSUE: Field 'java.awt.Event.id' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
            //UPGRADE_ISSUE: Field 'java.awt.Event.WINDOW_DESTROY' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
            if (ev.id == Event.WINDOW_DESTROY)
            {
                CirSim.main.Focus();
                //UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                Visible = false;
                CirSim.impDialog = null;
                return true;
            }
            //UPGRADE_ISSUE: Method 'java.awt.Component.handleEvent' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtComponenthandleEvent_javaawtEvent'"
            return base.handleEvent(ev);
        }
    }
}