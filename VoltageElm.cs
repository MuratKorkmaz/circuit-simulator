namespace circuit_emulator
{
    class VoltageElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 'v';
            }
		
        }
        virtual internal double Voltage
        {
            get
            {
                double w = 2 * pi * (sim.t - freqTimeZero) * frequency + phaseShift;
                switch (waveform)
                {
				
                    case WF_DC:  return maxVoltage + bias;
				
                    case WF_AC:  return System.Math.Sin(w) * maxVoltage + bias;
				
                    case WF_SQUARE: 
                        return bias + ((w % (2 * pi) > (2 * pi * dutyCycle))?- maxVoltage:maxVoltage);
				
                    case WF_TRIANGLE: 
                        return bias + triangleFunc(w % (2 * pi)) * maxVoltage;
				
                    case WF_SAWTOOTH: 
                        return bias + (w % (2 * pi)) * (maxVoltage / pi) - maxVoltage;
				
                    case WF_PULSE: 
                        return ((w % (2 * pi)) < 1)?maxVoltage + bias:bias;
				
                    default:  return 0;
				
                }
            }
		
        }
        override internal int VoltageSourceCount
        {
            get
            {
                return 1;
            }
		
        }
        override internal double Power
        {
            get
            {
                return (- VoltageDiff) * current;
            }
		
        }
        override internal double VoltageDiff
        {
            get
            {
                return volts[1] - volts[0];
            }
		
        }
        internal const int FLAG_COS = 2;
        internal int waveform;
        internal const int WF_DC = 0;
        internal const int WF_AC = 1;
        internal const int WF_SQUARE = 2;
        internal const int WF_TRIANGLE = 3;
        internal const int WF_SAWTOOTH = 4;
        internal const int WF_PULSE = 5;
        internal const int WF_VAR = 6;
        internal double frequency, maxVoltage, freqTimeZero, bias, phaseShift, dutyCycle;
        internal VoltageElm(int xx, int yy, int wf):base(xx, yy)
        {
            waveform = wf;
            maxVoltage = 5;
            frequency = 40;
            dutyCycle = .5;
            reset();
        }
        public VoltageElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            maxVoltage = 5;
            frequency = 40;
            waveform = WF_DC;
            dutyCycle = .5;
            try
            {
                waveform = System.Int32.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                frequency = System.Double.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                maxVoltage = System.Double.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                bias = System.Double.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                phaseShift = System.Double.Parse(st.NextToken());
                //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                dutyCycle = System.Double.Parse(st.NextToken());
            }
            catch (System.Exception e)
            {
            }
            if ((flags & FLAG_COS) != 0)
            {
                flags &= ~ FLAG_COS;
                phaseShift = pi / 2;
            }
            reset();
        }
        internal override System.String dump()
        {
            return base.dump() + " " + waveform + " " + frequency + " " + maxVoltage + " " + bias + " " + phaseShift + " " + dutyCycle;
        }
        /*void setCurrent(double c) {
	current = c;
	System.out.print("v current set to " + c + "\n");
	}*/
	
        internal override void  reset()
        {
            freqTimeZero = 0;
            curcount = 0;
        }
        internal virtual double triangleFunc(double x)
        {
            if (x < pi)
                return x * (2 / pi) - 1;
            return 1 - (x - pi) * (2 / pi);
        }
        internal override void  stamp()
        {
            if (waveform == WF_DC)
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource, Voltage);
            else
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
        }
        internal override void  doStep()
        {
            if (waveform != WF_DC)
                sim.updateVoltageSource(nodes[0], nodes[1], voltSource, Voltage);
        }
        //UPGRADE_NOTE: Final was removed from the declaration of 'circleSize '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int circleSize = 17;
        internal override void  setPoints()
        {
            base.setPoints();
            calcLeads((waveform == WF_DC || waveform == WF_VAR)?8:circleSize * 2);
        }
        internal override void  draw(System.Drawing.Graphics g)
        {
            setBbox(x, y, x2, y2);
            draw2Leads(g);
            if (waveform == WF_DC)
            {
                setPowerColor(g, false);
                setVoltageColor(g, volts[0]);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 0, 10);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
                setVoltageColor(g, volts[1]);
                int hs = 16;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                setBbox(ref point1, ref point2, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 1, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
            }
            else
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                setBbox(ref point1, ref point2, circleSize);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint(ref lead1, ref lead2, ref ps1, .5);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawWaveform(g, ref ps1);
            }
            updateDotCount();
            if (sim.dragElm != this)
            {
                if (waveform == WF_DC)
                {
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawDots(g, ref point1, ref point2, curcount);
                }
                else
                {
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawDots(g, ref point1, ref lead1, curcount);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawDots(g, ref point2, ref lead2, - curcount);
                }
            }
            drawPosts(g);
        }
	
        //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
        internal virtual void  drawWaveform(System.Drawing.Graphics g, ref System.Drawing.Point center)
        {
            SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:System.Drawing.Color.Gray);
            setPowerColor(g, false);
            int xc = center.X; int yc = center.Y;
            drawThickCircle(g, xc, yc, circleSize);
            int wl = 8;
            adjustBbox(xc - circleSize, yc - circleSize, xc + circleSize, yc + circleSize);
            int xc2;
            switch (waveform)
            {
			
                case WF_DC: 
                    {
                        break;
                    }
			
                case WF_SQUARE: 
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    xc2 = (int) (wl * 2 * dutyCycle - wl + xc);
                    xc2 = max(xc - wl + 3, min(xc + wl - 3, xc2));
                    drawThickLine(g, xc - wl, yc - wl, xc - wl, yc);
                    drawThickLine(g, xc - wl, yc - wl, xc2, yc - wl);
                    drawThickLine(g, xc2, yc - wl, xc2, yc + wl);
                    drawThickLine(g, xc + wl, yc + wl, xc2, yc + wl);
                    drawThickLine(g, xc + wl, yc, xc + wl, yc + wl);
                    break;
			
                case WF_PULSE: 
                    yc += wl / 2;
                    drawThickLine(g, xc - wl, yc - wl, xc - wl, yc);
                    drawThickLine(g, xc - wl, yc - wl, xc - wl / 2, yc - wl);
                    drawThickLine(g, xc - wl / 2, yc - wl, xc - wl / 2, yc);
                    drawThickLine(g, xc - wl / 2, yc, xc + wl, yc);
                    break;
			
                case WF_SAWTOOTH: 
                    drawThickLine(g, xc, yc - wl, xc - wl, yc);
                    drawThickLine(g, xc, yc - wl, xc, yc + wl);
                    drawThickLine(g, xc, yc + wl, xc + wl, yc);
                    break;
			
                case WF_TRIANGLE: 
                    {
                        int xl = 5;
                        drawThickLine(g, xc - xl * 2, yc, xc - xl, yc - wl);
                        drawThickLine(g, xc - xl, yc - wl, xc, yc);
                        drawThickLine(g, xc, yc, xc + xl, yc + wl);
                        drawThickLine(g, xc + xl, yc + wl, xc + xl * 2, yc);
                        break;
                    }
			
                case WF_AC: 
                    {
                        int i;
                        int xl = 10;
                        int ox = - 1, oy = - 1;
                        for (i = - xl; i <= xl; i++)
                        {
                            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                            int yy = yc + (int) (.95 * System.Math.Sin(i * pi / xl) * wl);
                            if (ox != - 1)
                                drawThickLine(g, ox, oy, xc + i, yy);
                            ox = xc + i; oy = yy;
                        }
                        break;
                    }
            }
            if (sim.showValuesCheckItem.Checked)
            {
                System.String s = getShortUnitText(frequency, "Гц");
                if (dx == 0 || dy == 0)
                    drawValues(g, s, circleSize);
            }
        }
        internal override void  getInfo(System.String[] arr)
        {
            switch (waveform)
            {
			
                case WF_DC: 
                case WF_VAR: 
                    arr[0] = "источник напряжения"; break;
			
                case WF_AC:  arr[0] = "ген. перем. тока"; break;
			
                case WF_SQUARE:  arr[0] = "ген. прямоуг."; break;
			
                case WF_PULSE:  arr[0] = "ген. импульсов"; break;
			
                case WF_SAWTOOTH:  arr[0] = "ген. пилы"; break;
			
                case WF_TRIANGLE:  arr[0] = "ген. треугольн."; break;
            }
            arr[1] = "I = " + getCurrentText(getCurrent());
            arr[2] = ((this is RailElm)?"V = ":"Vd = ") + getVoltageText(VoltageDiff);
            if (waveform != WF_DC && waveform != WF_VAR)
            {
                arr[3] = "f = " + getUnitText(frequency, "Гц");
                arr[4] = "Vmax = " + getVoltageText(maxVoltage);
                int i = 5;
                if (bias != 0)
                    arr[i++] = "Voff = " + getVoltageText(bias);
                else if (frequency > 500)
                    arr[i++] = "длинна волны = " + getUnitText(2.9979e8 / frequency, "м");
                arr[i++] = "P = " + getUnitText(Power, "Вт");
            }
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo(waveform == WF_DC?"Напряжение":"максимальное напряжение", maxVoltage, - 20, 20);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("Форма напряжения", waveform, - 1, - 1);
                System.Windows.Forms.ComboBox temp_ComboBox;
                temp_ComboBox = new System.Windows.Forms.ComboBox();
                temp_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                ei.choice = temp_ComboBox;
                ei.choice.Items.Add("Постоянный ток");
                ei.choice.Items.Add("Переменный ток");
                ei.choice.Items.Add("Прямоугольн.");
                ei.choice.Items.Add("Треугольн.");
                ei.choice.Items.Add("Пилообразн.");
                ei.choice.Items.Add("Импульсн.");
                ei.choice.SelectedIndex = waveform;
                return ei;
            }
            if (waveform == WF_DC)
                return null;
            if (n == 2)
                return new EditInfo("Частота (Гц)", frequency, 4, 500);
            if (n == 3)
                return new EditInfo("Смещение (В)", bias, - 20, 20);
            if (n == 4)
                return new EditInfo("Смещение фазы (градусов)", phaseShift * 180 / pi, - 180, 180).setDimensionless();
            if (n == 5 && waveform == WF_SQUARE)
                return new EditInfo("Скважность", dutyCycle * 100, 0, 100).setDimensionless();
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                maxVoltage = ei.value_Renamed;
            if (n == 3)
                bias = ei.value_Renamed;
            if (n == 2)
            {
                // adjust time zero to maintain continuity ind the waveform
                // even though the frequency has changed.
                double oldfreq = frequency;
                frequency = ei.value_Renamed;
                double maxfreq = 1 / (8 * sim.timeStep);
                if (frequency > maxfreq)
                    frequency = maxfreq;
                double adj = frequency - oldfreq;
                freqTimeZero = sim.t - oldfreq * (sim.t - freqTimeZero) / frequency;
            }
            if (n == 1)
            {
                int ow = waveform;
                //UPGRADE_TODO: Method 'java.awt.Choice.getSelectedIndex' was converted to 'System.Windows.Forms.ComboBox.SelectedIndex' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtChoicegetSelectedIndex'"
                waveform = ei.choice.SelectedIndex;
                if (waveform == WF_DC && ow != WF_DC)
                {
                    ei.newDialog = true;
                    bias = 0;
                }
                else if (waveform != WF_DC && ow == WF_DC)
                {
                    ei.newDialog = true;
                }
                if ((waveform == WF_SQUARE || ow == WF_SQUARE) && waveform != ow)
                    ei.newDialog = true;
                setPoints();
            }
            if (n == 4)
                phaseShift = ei.value_Renamed * pi / 180;
            if (n == 5)
                dutyCycle = ei.value_Renamed * .01;
        }
    }
}