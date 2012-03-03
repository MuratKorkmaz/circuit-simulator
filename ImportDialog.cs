using System;
using System.Windows.Forms;

namespace circuit_emulator
{
    [Serializable]
    internal partial class ImportDialog : Form
    {
        internal CirSim cframe;
        internal bool isURL;


        internal ImportDialog(CirSim f, String str, bool url)
        {
            InitializeComponent();
            SupportClass.DialogSupport.SetDialog(this, f, (str.Length > 0) ? "Export" : "Import");
            FormBorderStyle = FormBorderStyle.FixedToolWindow;

            isURL = url;
            cframe = f;
            text.Text = str;


            if (!isURL)
            {
                Controls.Add(importButton);
            }
            importButton.Click += actionPerformed;
            SupportClass.CommandManager.CheckCommand(importButton);
            closeButton.Click += actionPerformed;
            SupportClass.CommandManager.CheckCommand(closeButton);
            if (str.Length > 0)
                text.SelectAll();
        }

        public virtual void actionPerformed(Object event_sender, EventArgs e)
        {
            int i;
            Object src = event_sender;
            if (src == importButton)
            {
                cframe.readSetup(text.Text);
                Visible = false;
            }
            if (src == closeButton)
            {
                Visible = false;
            }
        }
    }
}