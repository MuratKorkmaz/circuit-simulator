using System;

internal interface Editable
{
	EditInfo getEditInfo(int n);
	void  setEditValue(int n, EditInfo ei);
}

[Serializable]
class EditDialog:System.Windows.Forms.Form
{
	virtual internal EditInfo Bar
	{
		set
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int x = (int) (barmax * (value.value_Renamed - value.minval) / (value.maxval - value.minval));
			value.bar.Value = x;
		}
		
	}
	internal Editable elm;
	internal CirSim cframe;
	internal System.Windows.Forms.Button applyButton, okButton;
	internal EditInfo[] einfos;
	internal int einfocount;
	//UPGRADE_NOTE: Final was removed from the declaration of 'barmax '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
    internal const int barmax = 1000;
    internal SupportClass.TextNumberFormat noCommaFormat;
	
	internal EditDialog(Editable ce, CirSim f):base()
	{
		//UPGRADE_TODO: Constructor 'java.awt.Dialog.Dialog' was converted to 'SupportClass.DialogSupport.SetDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogDialog_javaawtFrame_javalangString_boolean'"
		SupportClass.DialogSupport.SetDialog(this, f, "Редактировать компонент");
		cframe = f;
		elm = ce;
		//UPGRADE_ISSUE: Method 'java.awt.Container.setLayout' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtContainersetLayout_javaawtLayoutManager'"
		setLayout(new EditDialogLayout());
		einfos = new EditInfo[10];
		noCommaFormat = SupportClass.TextNumberFormat.getTextNumberInstance();
		noCommaFormat.setMaximumFractionDigits(10);
		noCommaFormat.GroupingUsed = false;
		int i;
		for (i = 0; ; i++)
		{
			einfos[i] = elm.getEditInfo(i);
			if (einfos[i] == null)
				break;
			EditInfo ei = einfos[i];
			System.Windows.Forms.Label temp_Label2;
			temp_Label2 = new System.Windows.Forms.Label();
			temp_Label2.Text = ei.name;
			//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
			System.Windows.Forms.Control temp_Control;
			temp_Control = temp_Label2;
			Controls.Add(temp_Control);
			if (ei.choice != null)
			{
				//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
				Controls.Add(ei.choice);
				ei.choice.SelectedIndexChanged += new System.EventHandler(this.itemStateChanged);
			}
			else if (ei.checkbox != null)
			{
				//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
				Controls.Add(ei.checkbox);
				ei.checkbox.Click += new System.EventHandler(this.itemStateChanged);
			}
			else
			{
				System.Windows.Forms.TextBox temp_TextBox2;
				//UPGRADE_TODO: Constructor 'java.awt.TextField.TextField' was converted to 'System.Windows.Forms.TextBox' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtTextFieldTextField_javalangString_int'"
				temp_TextBox2 = new System.Windows.Forms.TextBox();
				temp_TextBox2.Text = unitString(ei);
				//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
				System.Windows.Forms.Control temp_Control2;
				temp_Control2 = ei.textf = temp_TextBox2;
				Controls.Add(temp_Control2);
				if (ei.text != null)
					ei.textf.Text = ei.text;
				//UPGRADE_TODO: Method 'java.awt.TextField.addActionListener' was converted to 'System.Windows.Forms.TextBox.KeyPress' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtTextFieldaddActionListener_javaawteventActionListener'"
				ei.textf.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.actionPerformed);
				if (ei.text == null)
				{
					//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
					System.Windows.Forms.Control temp_Control3;
					temp_Control3 = ;
					Controls.Add(temp_Control3);
					Bar = ei;
					//UPGRADE_TODO: Method 'java.awt.Scrollbar.addAdjustmentListener' was converted to 'System.Windows.Forms.ScrollEventArgs' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtScrollbaraddAdjustmentListener_javaawteventAdjustmentListener'"
					ei.bar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.adjustmentValueChanged);
				}
			}
		}
		einfocount = i;
		System.Windows.Forms.Button temp_Button2;
		temp_Button2 = new System.Windows.Forms.Button();
		temp_Button2.Text = "Применить";
		//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
		System.Windows.Forms.Control temp_Control4;
		temp_Control4 = applyButton = temp_Button2;
		Controls.Add(temp_Control4);
		applyButton.Click += new System.EventHandler(this.actionPerformed);
		SupportClass.CommandManager.CheckCommand(applyButton);
		System.Windows.Forms.Button temp_Button4;
		temp_Button4 = new System.Windows.Forms.Button();
		temp_Button4.Text = "OK";
		//UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
		System.Windows.Forms.Control temp_Control5;
		temp_Control5 = okButton = temp_Button4;
		Controls.Add(temp_Control5);
		okButton.Click += new System.EventHandler(this.actionPerformed);
		SupportClass.CommandManager.CheckCommand(okButton);
		System.Windows.Forms.Control temp_Control6;
		//UPGRADE_NOTE: Exceptions thrown by the equivalent in .NET of method 'java.awt.Component.getLocationOnScreen' may be different. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1099'"
		temp_Control6 = CirSim.main;
		System.Drawing.Point x = temp_Control6.PointToScreen(temp_Control6.Location);
		System.Drawing.Size d = Size;
		//UPGRADE_TODO: Method 'java.awt.Component.setLocation' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetLocation_int_int'"
		Location = new System.Drawing.Point(x.X + (cframe.winSize.Width - d.Width) / 2, x.Y + (cframe.winSize.Height - d.Height) / 2);
	}
	
	internal virtual System.String unitString(EditInfo ei)
	{
		double v = ei.value_Renamed;
		double va = System.Math.Abs(v);
		if (ei.dimensionless)
			return noCommaFormat.FormatDouble(v);
		if (v == 0)
			return "0";
		if (va < 1e-9)
			return noCommaFormat.FormatDouble(v * 1e12) + "p";
		if (va < 1e-6)
			return noCommaFormat.FormatDouble(v * 1e9) + "n";
		if (va < 1e-3)
			return noCommaFormat.FormatDouble(v * 1e6) + "u";
		if (va < 1 && !ei.forceLargeM)
			return noCommaFormat.FormatDouble(v * 1e3) + "m";
		if (va < 1e3)
			return noCommaFormat.FormatDouble(v);
		if (va < 1e6)
			return noCommaFormat.FormatDouble(v * 1e-3) + "k";
		if (va < 1e9)
			return noCommaFormat.FormatDouble(v * 1e-6) + "M";
		return noCommaFormat.FormatDouble(v * 1e-9) + "G";
	}
	
	internal virtual double parseUnits(EditInfo ei)
	{
		System.String s = ei.textf.Text;
		s = s.Trim();
		int len = s.Length;
		char uc = s[len - 1];
		double mult = 1;
		switch (uc)
		{
			
			case 'p': 
			case 'P':  mult = 1e-12; break;
			
			case 'n': 
			case 'N':  mult = 1e-9; break;
			
			case 'u': 
			case 'U':  mult = 1e-6; break;
				
				// for ohm values, we assume mega for lowercase m, otherwise milli
			
			case 'm':  mult = (ei.forceLargeM)?1e6:1e-3; break;
			
			
			case 'k': 
			case 'K':  mult = 1e3; break;
			
			case 'M':  mult = 1e6; break;
			
			case 'G': 
			case 'g':  mult = 1e9; break;
			}
		if (Math.Abs(mult - 1) > double.Epsilon)
			s = s.Substring(0, (len - 1) - (0)).Trim();
		//UPGRADE_ISSUE: Method 'java.text.NumberFormat.parse' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javatextNumberFormatparse_javalangString'"
		return System.Convert.ToDouble(noCommaFormat.parse(s)) * mult;
	}
	
	internal virtual void  apply()
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
				catch (System.Exception ex)
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
	
	public virtual void  actionPerformed(System.Object event_sender, System.EventArgs e)
	{
		int i;
		System.Object src = event_sender;
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
					catch (System.Exception ex)
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
			//UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
			//UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
			Visible = false;
			CirSim.editDialog = null;
		}
		if (event_sender == applyButton)
			apply();
	}
	
	public virtual void  adjustmentValueChanged(System.Object event_sender, System.Windows.Forms.ScrollEventArgs e)
	{
		System.Object src = event_sender;
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
				ei.value_Renamed = (ei.maxval - ei.minval) * v + ei.minval;
				/*if (ei.maxval-ei.minval > 100)
				ei.value = Math.round(ei.value);
				else
				ei.value = Math.round(ei.value*100)/100.;*/
				//UPGRADE_TODO: Method 'java.lang.Math.round' was converted to 'System.Math.Round' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangMathround_double'"
				ei.value_Renamed = (long) System.Math.Round(ei.value_Renamed / ei.minval) * ei.minval;
				elm.setEditValue(i, ei);
				ei.textf.Text = unitString(ei);
				cframe.needAnalyze();
			}
		}
	}
	
	public virtual void  itemStateChanged(System.Object event_sender, System.EventArgs e)
	{
		if (event_sender is System.Windows.Forms.MenuItem)
			((System.Windows.Forms.MenuItem) event_sender).Checked = !((System.Windows.Forms.MenuItem) event_sender).Checked;
		System.Object src = event_sender;
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
			//UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
			//UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
			Visible = false;
			CirSim.editDialog = new EditDialog(elm, cframe);
			//UPGRADE_TODO: Method 'java.awt.Dialog.show' was converted to 'System.Windows.Forms.Form.ShowDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogshow'"
			CirSim.editDialog.ShowDialog();
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
			CirSim.editDialog = null;
			return true;
		}
		//UPGRADE_ISSUE: Method 'java.awt.Component.handleEvent' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtComponenthandleEvent_javaawtEvent'"
		return base.handleEvent(ev);
	}
}