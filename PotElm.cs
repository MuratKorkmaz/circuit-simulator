namespace circuit_emulator
{
    class PotElm:CircuitElm
    {
        override internal int PostCount
        {
            get
            {
                return 3;
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 174;
            }
		
        }
        internal double position, maxResistance, resistance1, resistance2;
        internal double current1, current2, current3;
        internal double curcount1, curcount2, curcount3;
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar slider;
        internal System.Windows.Forms.Label label;
        internal System.String sliderText;
        public PotElm(int xx, int yy):base(xx, yy)
        {
            setup();
            maxResistance = 1000;
            position = .5;
            sliderText = "Сопротивление";
            createSlider();
        }
        public PotElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            maxResistance = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            position = System.Double.Parse(st.NextToken());
            sliderText = st.NextToken();
            while (st.HasMoreTokens())
                sliderText += (' ' + st.NextToken());
            createSlider();
        }
        internal virtual void  setup()
        {
        }
        internal override System.Drawing.Point getPost(int n)
        {
            return (n == 0)?point1:((n == 1)?point2:post3);
        }
        internal override System.String dump()
        {
            return base.dump() + " " + maxResistance + " " + position + " " + sliderText;
        }
        internal virtual void  createSlider()
        {
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
            int value_Renamed = (int) (position * 100);
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control2;
            temp_Control2 = null;
            CirSim.main.Controls.Add(temp_Control2);
            CirSim.main.Invalidate();
            //UPGRADE_TODO: Method 'java.awt.Scrollbar.addAdjustmentListener' was converted to 'System.Windows.Forms.ScrollEventArgs' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtScrollbaraddAdjustmentListener_javaawteventAdjustmentListener'"
            slider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.adjustmentValueChanged);
        }
        public virtual void  adjustmentValueChanged(System.Object event_sender, System.Windows.Forms.ScrollEventArgs e)
        {
            sim.analyzeFlag = true;
            setPoints();
        }
        internal override void  delete()
        {
            CirSim.main.Controls.Remove(label);
            CirSim.main.Controls.Remove(slider);
        }
        internal System.Drawing.Point post3, corner2, arrowPoint, midpoint, arrow1, arrow2;
        internal System.Drawing.Point ps3, ps4;
        internal int bodyLen;
        internal override void  setPoints()
        {
            base.setPoints();
            int offset = 0;
            if (abs(dx) > abs(dy))
            {
                dx = sim.snapGrid(dx / 2) * 2;
                point2.X = x2 = point1.X + dx;
                offset = (dx < 0)?dy:- dy;
                point2.Y = point1.Y;
            }
            else
            {
                dy = sim.snapGrid(dy / 2) * 2;
                point2.Y = y2 = point1.Y + dy;
                offset = (dy > 0)?dx:- dx;
                point2.X = point1.X;
            }
            if (offset == 0)
                offset = sim.gridSize;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            dn = distance(ref point1, ref point2);
            int bodyLen = 32;
            calcLeads(bodyLen);
            position = slider.Value * .0099 + .005;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int soff = (int) ((position - .5) * bodyLen);
            //int offset2 = offset - sign(offset)*4;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            post3 = interpPoint(ref point1, ref point2, .5, offset);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            corner2 = interpPoint(ref point1, ref point2, soff / dn + .5, offset);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            arrowPoint = interpPoint(ref point1, ref point2, soff / dn + .5, 8 * sign(offset));
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            midpoint = interpPoint(ref point1, ref point2, soff / dn + .5);
            arrow1 = new System.Drawing.Point(0, 0);
            arrow2 = new System.Drawing.Point(0, 0);
            double clen = abs(offset) - 8;
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            interpPoint2(ref corner2, ref arrowPoint, ref arrow1, ref arrow2, (clen - 8) / clen, 8);
            ps3 = new System.Drawing.Point(0, 0);
            ps4 = new System.Drawing.Point(0, 0);
        }
	
        internal override void  draw(System.Drawing.Graphics g)
        {
            int segments = 16;
            int i;
            int ox = 0;
            int hs = sim.euroResistorCheckItem.Checked?6:8;
            double v1 = volts[0];
            double v2 = volts[1];
            double v3 = volts[2];
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            setBbox(ref point1, ref point2, hs);
            draw2Leads(g);
            setPowerColor(g, true);
            double segf = 1.0 / segments;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int divide = (int) (segments * position);
            if (!sim.euroResistorCheckItem.Checked)
            {
                // draw zigzag
                for (i = 0; i != segments; i++)
                {
                    int nx = 0;
                    switch (i & 3)
                    {
					
                        case 0:  nx = 1; break;
					
                        case 2:  nx = - 1; break;
					
                        default:  nx = 0; break;
					
                    }
                    double v = v1 + (v3 - v1) * i / divide;
                    if (i >= divide)
                        v = v3 + (v2 - v3) * (i - divide) / (segments - divide);
                    setVoltageColor(g, v);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref lead1, ref lead2, ref ps1, i * segf, hs * ox);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint(ref lead1, ref lead2, ref ps2, (i + 1) * segf, hs * nx);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps1, ref ps2);
                    ox = nx;
                }
            }
            else
            {
                // draw rectangle
                setVoltageColor(g, v1);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 0, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
                for (i = 0; i != segments; i++)
                {
                    double v = v1 + (v3 - v1) * i / divide;
                    if (i >= divide)
                        v = v3 + (v2 - v3) * (i - divide) / (segments - divide);
                    setVoltageColor(g, v);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, i * segf, hs);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    interpPoint2(ref lead1, ref lead2, ref ps3, ref ps4, (i + 1) * segf, hs);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps1, ref ps3);
                    //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                    drawThickLine(g, ref ps2, ref ps4);
                }
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                interpPoint2(ref lead1, ref lead2, ref ps1, ref ps2, 1, hs);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref ps1, ref ps2);
            }
            setVoltageColor(g, v3);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref post3, ref corner2);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref corner2, ref arrowPoint);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref arrow1, ref arrowPoint);
            //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
            drawThickLine(g, ref arrow2, ref arrowPoint);
            curcount1 = updateDotCount(current1, curcount1);
            curcount2 = updateDotCount(current2, curcount2);
            curcount3 = updateDotCount(current3, curcount3);
            if (sim.dragElm != this)
            {
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref point1, ref midpoint, curcount1);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref point2, ref midpoint, curcount2);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref post3, ref corner2, curcount3);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref corner2, ref midpoint, curcount3 + distance(ref post3, ref corner2));
            }
            drawPosts(g);
        }
        internal override void  calculateCurrent()
        {
            current1 = (volts[0] - volts[2]) / resistance1;
            current2 = (volts[1] - volts[2]) / resistance2;
            current3 = - current1 - current2;
        }
        internal override void  stamp()
        {
            resistance1 = maxResistance * position;
            resistance2 = maxResistance * (1 - position);
            sim.stampResistor(nodes[0], nodes[2], resistance1);
            sim.stampResistor(nodes[2], nodes[1], resistance2);
        }
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = "перемен. сопротивление";
            arr[1] = "Vd = " + getVoltageDText(VoltageDiff);
            arr[2] = "R1 = " + getUnitText(resistance1, CirSim.ohmString);
            arr[3] = "R2 = " + getUnitText(resistance2, CirSim.ohmString);
            arr[4] = "I1 = " + getCurrentDText(current1);
            arr[5] = "I2 = " + getCurrentDText(current2);
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Сопротивление (Ом)", maxResistance, 0, 0);
            if (n == 1)
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
                maxResistance = ei.value_Renamed;
            if (n == 1)
            {
                sliderText = ei.textf.Text;
                label.Text = sliderText;
            }
        }
    }
}