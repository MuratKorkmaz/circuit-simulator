namespace circuit_emulator
{
    class LampElm:CircuitElm
    {
        override internal int DumpType
        {
            get
            {
                return 181;
            }
		
        }
        virtual internal System.Drawing.Color TempColor
        {
            get
            {
                if (temp < 1200)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    int x = (int) (255 * (temp - 800) / 400);
                    if (x < 0)
                        x = 0;
                    return System.Drawing.Color.FromArgb(x, 0, 0);
                }
                if (temp < 1700)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    int x = (int) (255 * (temp - 1200) / 500);
                    if (x < 0)
                        x = 0;
                    return System.Drawing.Color.FromArgb(255, x, 0);
                }
                if (temp < 2400)
                {
                    //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                    int x = (int) (255 * (temp - 1700) / 700);
                    if (x < 0)
                        x = 0;
                    return System.Drawing.Color.FromArgb(255, 255, x);
                }
                return System.Drawing.Color.White;
            }
		
        }
        internal double resistance;
        //UPGRADE_NOTE: Final was removed from the declaration of 'roomTemp '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal double roomTemp = 300;
        internal double temp, nom_pow, nom_v, warmTime, coolTime;
        public LampElm(int xx, int yy):base(xx, yy)
        {
            temp = roomTemp;
            nom_pow = 100;
            nom_v = 120;
            warmTime = .4;
            coolTime = .4;
        }
        public LampElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            temp = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            nom_pow = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            nom_v = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            warmTime = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            coolTime = System.Double.Parse(st.NextToken());
        }
        internal override System.String dump()
        {
            return base.dump() + " " + temp + " " + nom_pow + " " + nom_v + " " + warmTime + " " + coolTime;
        }
	
        internal System.Drawing.Point[] bulbLead, filament;
        internal System.Drawing.Point bulb;
        internal int bulbR;
	
        internal override void  reset()
        {
            base.reset();
            temp = roomTemp;
        }
        //UPGRADE_NOTE: Final was removed from the declaration of 'filament_len '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int filament_len = 24;
        internal override void  setPoints()
        {
            base.setPoints();
            int llen = 16;
            calcLeads(llen);
            bulbLead = newPointArray(2);
            filament = newPointArray(2);
            bulbR = 20;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            filament[0] = interpPoint(ref lead1, ref lead2, 0, filament_len);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            filament[1] = interpPoint(ref lead1, ref lead2, 1, filament_len);
            double br = filament_len - System.Math.Sqrt(bulbR * bulbR - llen * llen);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            bulbLead[0] = interpPoint(ref lead1, ref lead2, 0, br);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            bulbLead[1] = interpPoint(ref lead1, ref lead2, 1, br);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            bulb = interpPoint(ref filament[0], ref filament[1], .5);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            double v1 = volts[0];
            double v2 = volts[1];
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, 4);
            adjustBbox(bulb.X - bulbR, bulb.Y - bulbR, bulb.X + bulbR, bulb.Y + bulbR);
            // adjustbbox
            draw2Leads(g);
            setPowerColor(g, true);
            SupportClass.GraphicsManager.manager.SetColor(g, TempColor);
            g.FillEllipse(SupportClass.GraphicsManager.manager.GetPaint(g), bulb.X - bulbR, bulb.Y - bulbR, bulbR * 2, bulbR * 2);
            SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.White);
            drawThickCircle(g, bulb.X, bulb.Y, bulbR);
            setVoltageColor(g, v1);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref lead1, ref filament[0]);
            setVoltageColor(g, v2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref lead2, ref filament[1]);
            setVoltageColor(g, (v1 + v2) * .5);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref filament[0], ref filament[1]);
            updateDotCount();
            if (sim.dragElm != this)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref point1, ref lead1, curcount);
                double cc = curcount + (dn - 16) / 2;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref lead1, ref filament[0], cc);
                cc += filament_len;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref filament[0], ref filament[1], cc);
                cc += 16;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref filament[1], ref lead2, cc);
                cc += filament_len;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref lead2, ref point2, curcount);
            }
            drawPosts(g);
        }
	
        internal override void  calculateCurrent()
        {
            current = (volts[0] - volts[1]) / resistance;
            //System.out.print(this + " res current set to " + current + "\n");
        }
        internal override void  stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void  startIteration()
        {
            // based on http://www.intusoft.com/nlpdf/nl11.pdf
            double nom_r = nom_v * nom_v / nom_pow;
            // this formula doesn't work for values over 5390
            double tp = (temp > 5390)?5390:temp;
            resistance = nom_r * (1.26104 - 4.90662 * System.Math.Sqrt(17.1839 / tp - 0.00318794) - 7.8569 / (tp - 187.56));
            double cap = 1.57e-4 * nom_pow;
            double capw = cap * warmTime / .4;
            double capc = cap * coolTime / .4;
            //System.out.println(nom_r + " " + (resistance/nom_r));
            temp += Power * sim.timeStep / capw;
            double cr = 2600 / nom_pow;
            temp -= sim.timeStep * (temp - roomTemp) / (capc * cr);
            //System.out.println(capw + " " + capc + " " + temp + " " +resistance);
        }
        internal override void  doStep()
        {
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "лампа";
            getBasicInfo(arr);
            arr[3] = "R = " + getUnitText(resistance, CirSim.ohmString);
            arr[4] = "P = " + getUnitText(Power, "Вт");
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            arr[5] = "T = " + ((int) temp) + " K";
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Номинальная мощность", nom_pow, 0, 0);
            if (n == 1)
                return new EditInfo("Номинальное напряжение", nom_v, 0, 0);
            if (n == 2)
                return new EditInfo("Время разогрева (с)", warmTime, 0, 0);
            if (n == 3)
                return new EditInfo("Время остывания (с)", coolTime, 0, 0);
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0 && ei.value_Renamed > 0)
                nom_pow = ei.value_Renamed;
            if (n == 1 && ei.value_Renamed > 0)
                nom_v = ei.value_Renamed;
            if (n == 2 && ei.value_Renamed > 0)
                warmTime = ei.value_Renamed;
            if (n == 3 && ei.value_Renamed > 0)
                coolTime = ei.value_Renamed;
        }
    }
}