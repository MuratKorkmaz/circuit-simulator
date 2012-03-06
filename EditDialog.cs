using System;
using System.Drawing;
using System.Windows.Forms;

namespace circuit_emulator
{
    internal interface Editable
    {
        EditInfo getEditInfo(int n);
        void setEditValue(int n, EditInfo ei);
    }

    [Serializable]
    internal class EditDialog : Form
    {
        internal const int barmax = 1000;
        internal Button applyButton;
        internal CirSim cframe;
        internal int einfocount;
        internal EditInfo[] einfos;
        internal Editable elm;
        private FlowLayoutPanel flowLayoutPanel;
        internal SupportClass.TextNumberFormat noCommaFormat;
        internal Button okButton;

        internal EditDialog(Editable ce, CirSim f)
        {
            InitializeComponent();
            SupportClass.DialogSupport.SetDialog(this, f, "Редактировать компонент");
            cframe = f;
            elm = ce;
            einfos = new EditInfo[10];
            noCommaFormat = SupportClass.TextNumberFormat.getTextNumberInstance();
            noCommaFormat.setMaximumFractionDigits(10);
            noCommaFormat.GroupingUsed = false;
            int i;
            for (i = 0;; i++)
            {
                einfos[i] = elm.getEditInfo(i);
                if (einfos[i] == null)
                    break;
                EditInfo ei = einfos[i];
                Label temp_Label2;
                temp_Label2 = new Label();
                temp_Label2.Text = ei.name;
                Control temp_Control;
                temp_Control = temp_Label2;
                temp_Control.Dock = DockStyle.Fill;
                flowLayoutPanel.Controls.Add(temp_Control);
                if (ei.choice != null)
                {
                    flowLayoutPanel.Controls.Add(ei.choice);
                    ei.choice.SelectedIndexChanged += itemStateChanged;
                }
                else if (ei.checkbox != null)
                {
                    flowLayoutPanel.Controls.Add(ei.checkbox);
                    ei.checkbox.Click += itemStateChanged;
                }
                else
                {
                    TextBox temp_TextBox2;
                    temp_TextBox2 = new TextBox();
                    temp_TextBox2.Text = unitString(ei);
                    Control temp_Control2;
                    temp_Control2 = ei.textf = temp_TextBox2;
                    temp_Control2.Dock = DockStyle.Fill;
                    flowLayoutPanel.Controls.Add(temp_Control2);
                    if (ei.text != null)
                        ei.textf.Text = ei.text;
                    ei.textf.KeyPress += actionPerformed;
                    if (ei.text == null)
                    {
                        flowLayoutPanel.Controls.Add(ei.bar);
                        Bar = ei;
                        ei.bar.Scroll += adjustmentValueChanged;
                    }
                }
            }
            einfocount = i;
            Button temp_Button2;
            temp_Button2 = new Button();
            temp_Button2.Text = "Применить";
            Control temp_Control4;
            temp_Control4 = applyButton = temp_Button2;
            temp_Control4.Dock = DockStyle.Fill;
            flowLayoutPanel.Controls.Add(temp_Control4);
            applyButton.Click += actionPerformed;
            SupportClass.CommandManager.CheckCommand(applyButton);
            Button temp_Button4;
            temp_Button4 = new Button();
            temp_Button4.Text = "OK";
            Control temp_Control5;
            temp_Control5 = okButton = temp_Button4;
            temp_Control5.Dock = DockStyle.Fill;
            flowLayoutPanel.Controls.Add(temp_Control5);
            okButton.Click += actionPerformed;
            SupportClass.CommandManager.CheckCommand(okButton);
        }

        internal virtual EditInfo Bar
        {
            set
            {
                var x = (int) (barmax*(value.value_Renamed - value.minval)/(value.maxval - value.minval));
                value.bar.Value = x;
            }
        }


        internal virtual String unitString(EditInfo ei)
        {
            double v = ei.value_Renamed;
            double va = Math.Abs(v);
            if (ei.dimensionless)
                return noCommaFormat.FormatDouble(v);
            if (v == 0)
                return "0";
            if (va < 1e-9)
                return noCommaFormat.FormatDouble(v*1e12) + "p";
            if (va < 1e-6)
                return noCommaFormat.FormatDouble(v*1e9) + "n";
            if (va < 1e-3)
                return noCommaFormat.FormatDouble(v*1e6) + "u";
            if (va < 1 && !ei.forceLargeM)
                return noCommaFormat.FormatDouble(v*1e3) + "m";
            if (va < 1e3)
                return noCommaFormat.FormatDouble(v);
            if (va < 1e6)
                return noCommaFormat.FormatDouble(v*1e-3) + "k";
            if (va < 1e9)
                return noCommaFormat.FormatDouble(v*1e-6) + "M";
            return noCommaFormat.FormatDouble(v*1e-9) + "G";
        }

        internal virtual double parseUnits(EditInfo ei)
        {
            String s = ei.textf.Text;
            s = s.Trim();
            int len = s.Length;
            char uc = s[len - 1];
            double mult = 1;
            switch (uc)
            {
                case 'p':
                case 'P':
                    mult = 1e-12;
                    break;

                case 'n':
                case 'N':
                    mult = 1e-9;
                    break;

                case 'u':
                case 'U':
                    mult = 1e-6;
                    break;

                    // for ohm values, we assume mega for lowercase m, otherwise milli

                case 'm':
                    mult = (ei.forceLargeM) ? 1e6 : 1e-3;
                    break;


                case 'k':
                case 'K':
                    mult = 1e3;
                    break;

                case 'M':
                    mult = 1e6;
                    break;

                case 'G':
                case 'g':
                    mult = 1e9;
                    break;
            }
            if (Math.Abs(mult - 1) > double.Epsilon)
                s = s.Substring(0, (len - 1) - (0)).Trim();
            //UPGRADE_ISSUE: Method 'java.text.NumberFormat.parse' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javatextNumberFormatparse_javalangString'"
            //todo return System.Convert.ToDouble(noCommaFormat.Parse(s)) * mult;
            return Convert.ToDouble(s)*mult;
        }

        internal virtual void apply()
        {
            int i;
            for (i = 0; i != einfocount; i++)
            {
                EditInfo ei = einfos[i];
                if (ei.textf == null)
                    continue;
                if (ei.text == null)
                {
                    try
                    {
                        double d = parseUnits(ei);
                        ei.value_Renamed = d;
                    }
                    catch (Exception ex)
                    {
                        /* ignored */
                    }
                }
                elm.setEditValue(i, ei);
                if (ei.text == null)
                    Bar = ei;
            }
            cframe.needAnalyze();
        }

        public virtual void actionPerformed(Object event_sender, EventArgs e)
        {
            int i;
            Object src = event_sender;
            for (i = 0; i != einfocount; i++)
            {
                EditInfo ei = einfos[i];
                if (src == ei.textf)
                {
                    if (ei.text == null)
                    {
                        try
                        {
                            double d = parseUnits(ei);
                            ei.value_Renamed = d;
                        }
                        catch (Exception ex)
                        {
                            /* ignored */
                        }
                    }
                    elm.setEditValue(i, ei);
                    if (ei.text == null)
                        Bar = ei;
                    cframe.needAnalyze();
                }
            }
            if (event_sender == okButton)
            {
                apply();
                CirSim.main.Focus();
                Visible = false;
                CirSim.editDialog = null;
            }
            if (event_sender == applyButton)
                apply();
        }

        public virtual void adjustmentValueChanged(Object event_sender, ScrollEventArgs e)
        {
            Object src = event_sender;
            int i;
            for (i = 0; i != einfocount; i++)
            {
                EditInfo ei = einfos[i];
                if (ei.bar == src)
                {
                    double v = ei.bar.Value/1000.0;
                    if (v < 0)
                        v = 0;
                    if (v > 1)
                        v = 1;
                    ei.value_Renamed = (ei.maxval - ei.minval)*v + ei.minval;
                    /*if (ei.maxval-ei.minval > 100)
				ei.value = Math.round(ei.value);
				else
				ei.value = Math.round(ei.value*100)/100.;*/
                    //UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangMathround_double'"
                    ei.value_Renamed = (long) Math.Round(ei.value_Renamed/ei.minval)*ei.minval;
                    elm.setEditValue(i, ei);
                    ei.textf.Text = unitString(ei);
                    cframe.needAnalyze();
                }
            }
        }

        public virtual void itemStateChanged(Object event_sender, EventArgs e)
        {
            if (event_sender is MenuItem)
                ((MenuItem) event_sender).Checked = !((MenuItem) event_sender).Checked;
            Object src = event_sender;
            int i;
            bool changed = false;
            for (i = 0; i != einfocount; i++)
            {
                EditInfo ei = einfos[i];
                if (ei.choice == src || ei.checkbox == src)
                {
                    elm.setEditValue(i, ei);
                    if (ei.newDialog)
                        changed = true;
                    cframe.needAnalyze();
                }
            }
            if (changed)
            {
                Visible = false;
                CirSim.editDialog = new EditDialog(elm, cframe);
                CirSim.editDialog.ShowDialog();
            }
        }

        private void InitializeComponent()
        {
            flowLayoutPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Location = new Point(0, 0);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(265, 188);
            flowLayoutPanel.TabIndex = 0;
            // 
            // EditDialog
            // 
            ClientSize = new Size(265, 188);
            Controls.Add(flowLayoutPanel);
            Name = "EditDialog";
            ResumeLayout(false);
        }
    }
}