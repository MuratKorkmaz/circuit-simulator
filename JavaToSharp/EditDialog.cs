using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal interface Editable
    {
        EditInfo getEditInfo(int n);
        void setEditValue(int n, EditInfo ei);
    }

    internal class EditDialog
    {
        internal Editable elm;
        internal CirSim cframe;
        internal Button applyButton, okButton;
        internal EditInfo[] einfos;
        internal int einfocount;
        internal readonly int barmax = 1000;
        internal NumberFormatInfo noCommaFormat;

        internal EditDialog(Editable ce, CirSim f) : base(f, "Редактировать компонент", false)
        {
            cframe = f;
            elm = ce;
            einfos = new EditInfo[10];
           
            int i;
            for (i = 0; ; i++)
            {
                einfos[i] = elm.getEditInfo(i);
                if (einfos[i] == null)
                    break;
                EditInfo ei = einfos[i];
                add(new Label(ei.name));
                if (ei.choice != null)
                {
                    add(ei.choice);
                    ei.choice.addItemListener(this);
                }
                else if (ei.checkbox != null)
                {
                    add(ei.checkbox);
                    ei.checkbox.addItemListener(this);
                }
                else
                {
                    add(ei.textf = new TextField(unitString(ei), 10));
                    if (ei.text != null)
                        ei.textf.Text = ei.text;
                    ei.textf.addActionListener(this);
                    if (ei.text == null)
                    {
                        add(ei.bar = new Scrollbar(Scrollbar.HORIZONTAL, 50, 10, 0, barmax+2));
                        Bar = ei;
                        ei.bar.addAdjustmentListener(this);
                    }
                }
            }
            einfocount = i;
            add(applyButton = new Button("Применить"));
            applyButton.addActionListener(this);
            add(okButton = new Button("OK"));
            okButton.addActionListener(this);
            Point x = new Point(0, 0);
            Dimension d = Size;
            setLocation(x.X + (cframe.winSize.Width-d.width)/2, x.Y + (cframe.winSize.Height-d.height)/2);
        }

        internal virtual string unitString(EditInfo ei)
        {
            double v = ei.value;
            double va = Math.Abs(v);
            if (ei.dimensionless)
            return v.ToString("F", CultureInfo.InvariantCulture);
            if (v == 0)
                return "0";
            string value;
            if (va < 1e-9)
            {
                value = ((v*1e12).ToString("F", CultureInfo.InvariantCulture));
                return value + "p";
            }

            if (va < 1e-6)
            {
                 value = ((v * 1e9).ToString("F", CultureInfo.InvariantCulture));
                return value + "n";
            }
              

            if (va < 1e-3)
            {
                string value = ((v * 1e6).ToString("F", CultureInfo.InvariantCulture));
                return value +"u";
            }
                
            if (va < 1 && !ei.forceLargeM)
            {
                value = ((v * 1e3).ToString("F", CultureInfo.InvariantCulture));
                return value  + "m";
            }

            if (va < 1e3)
            return v.ToString("F", CultureInfo.InvariantCulture);
            if (va < 1e6)
            {
                value = ((v * 1e-3).ToString("F", CultureInfo.InvariantCulture));
                return value + "k";
            }

            if (va < 1e9)
            {
                value = ((v * 1e-6).ToString("F", CultureInfo.InvariantCulture));
                return value + "M";
            }
            value = ((v * 1e-9).ToString("F", CultureInfo.InvariantCulture));
            return value + "G";
           
        }

//JAVA TO VB & C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double parseUnits(EditInfo ei) throws java.text.ParseException
        internal virtual double parseUnits(EditInfo ei)
        {
            string s = ei.textf.Text;
            s = s.Trim();
            int len = s.Length;
            char uc = s[len-1];
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
            if (mult != 1)
                s = s.Substring(0, len-1).Trim();
            return (double)noCommaFormat.parse(s) * mult;
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
                        ei.value = d;
                    } // ignored
                    catch (Exception ex)
                    {
                    }
                }
                elm.setEditValue(i, ei);
                if (ei.text == null)
                    Bar = ei;
            }
            cframe.needAnalyze();
        }

        public virtual void actionPerformed(ActionEvent e)
        {
            int i;
            object src = e.Source;
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
                            ei.value = d;
                        } // ignored
                        catch (Exception ex)
                        {
                        }
                    }
                    elm.setEditValue(i, ei);
                    if (ei.text == null)
                        Bar = ei;
                    cframe.needAnalyze();
                }
            }
            if (e.Source == okButton)
            {
                apply();
                //cframe.main.requestFocus();
                Visible = false;
                cframe.editDialog = null;
            }
            if (e.Source == applyButton)
                apply();
        }

        public virtual void adjustmentValueChanged(AdjustmentEvent e)
        {
            object src = e.Source;
            int i;
            for (i = 0; i != einfocount; i++)
            {
                EditInfo ei = einfos[i];
                if (ei.bar == src)
                {
                    double v = ei.bar.Value / 1000.0;
                    if (v < 0)
                        v = 0;
                    if (v > 1)
                        v = 1;
                    ei.value = (ei.maxval-ei.minval)*v + ei.minval;
//		if (ei.maxval-ei.minval > 100)
//		    ei.value = Math.round(ei.value);
//		else
//		ei.value = Math.round(ei.value*100)/100.;
                    ei.value = Math.Round(ei.value/ei.minval)*ei.minval;
                    elm.setEditValue(i, ei);
                    ei.textf.Text = unitString(ei);
                    cframe.needAnalyze();
                }
            }
        }

        public virtual void itemStateChanged(ItemEvent e)
        {
            object src = e.ItemSelectable;
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
                cframe.editDialog = new EditDialog(elm, cframe);
                cframe.editDialog.show();
            }
        }

        public virtual bool handleEvent(Event ev)
        {
            if (ev.id == Event.WINDOW_DESTROY)
            {
                //cframe.main.requestFocus();
                Visible = false;
                cframe.editDialog = null;
                return true;
            }
            return base.handleEvent(ev);
        }

        internal virtual EditInfo Bar
        {
            set
            {
                int x = (int)(barmax*(value.value-value.minval)/(value.maxval-value.minval));
                value.bar.Value = x;
            }
        }
    }
}