namespace circuit_emulator
{
    class VarRailElm:RailElm
    {
        override internal int DumpType
        {
            get
            {
                return 172;
            }
		
        }
        override internal double Voltage
        {
            get
            {
                frequency = slider.Value * (maxVoltage - bias) / 100.0 + bias;
                return frequency;
            }
		
        }
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar slider;
        internal System.Windows.Forms.Label label;
        internal System.String sliderText;
        public VarRailElm(int xx, int yy):base(xx, yy, WF_VAR)
        {
            sliderText = "Voltage";
            frequency = maxVoltage;
            createSlider();
        }
        public VarRailElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
            sliderText = st.NextToken();
            while (st.HasMoreTokens())
                sliderText += (' ' + st.NextToken());
            createSlider();
        }
        internal override System.String dump()
        {
            return base.dump() + " " + sliderText;
        }
        internal virtual void  createSlider()
        {
            waveform = WF_VAR;
            System.Windows.Forms.Label temp_Label2;
            //UPGRADE_TODO: The equivalent in .NET for field 'java.awt.Label.CENTER' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            temp_Label2 = new System.Windows.Forms.Label();
            temp_Label2.Text = sliderText;
            temp_Label2.TextAlign = (System.Drawing.ContentAlignment) System.Drawing.ContentAlignment.MiddleCenter;
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control;
            temp_Control = label = temp_Label2;
            CirSim.main.Controls.Add(temp_Control);
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int value_Renamed = (int) ((frequency - bias) * 100 / (maxVoltage - bias));
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control2;
            temp_Control2 = ;
            CirSim.main.Controls.Add(temp_Control2);
            CirSim.main.Invalidate();
        }
        internal override void  delete()
        {
            CirSim.main.Controls.Remove(label);
            CirSim.main.Controls.Remove(slider);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Мин. напряжение", bias, - 20, 20);
            if (n == 1)
                return new EditInfo("Макс. напряжение", maxVoltage, - 20, 20);
            if (n == 2)
            {
                EditInfo ei = new EditInfo("Slider Text", 0, - 1, - 1);
                ei.text = sliderText;
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                bias = ei.value_Renamed;
            if (n == 1)
                maxVoltage = ei.value_Renamed;
            if (n == 2)
            {
                sliderText = ei.textf.Text;
                label.Text = sliderText;
            }
        }
    }
}