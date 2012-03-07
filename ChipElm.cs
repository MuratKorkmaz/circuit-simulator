namespace circuit_emulator
{
    abstract class ChipElm:CircuitElm
    {
        virtual internal int Size
        {
            set
            {
                csize = value;
                cspc = 8 * value;
                cspc2 = cspc * 2;
                flags &= ~ FLAG_SMALL;
                flags |= ((value == 1)?FLAG_SMALL:0);
            }
		
        }
        override internal abstract int VoltageSourceCount{get;}
        virtual internal System.String ChipName
        {
            get
            {
                return "chip";
            }
		
        }
        internal int csize, cspc, cspc2;
        internal int bits;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_SMALL '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_SMALL = 1;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_FLIP_X '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_FLIP_X = 1024;
        //UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_FLIP_Y '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal int FLAG_FLIP_Y = 2048;
        public ChipElm(int xx, int yy):base(xx, yy)
        {
            if (needsBits())
                bits = (this is DecadeElm)?10:4;
            noDiagonal = true;
            setupPins();
            Size = sim.smallGridCheckItem.Checked?1:2;
        }
        public ChipElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f)
        {
            if (needsBits())
                bits = System.Int32.Parse(st.NextToken());
            noDiagonal = true;
            setupPins();
            Size = (f & FLAG_SMALL) != 0?1:2;
            int i;
            for (i = 0; i != PostCount; i++)
            {
                if (pins[i].state)
                {
                    //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                    volts[i] = System.Double.Parse(st.NextToken());
                    pins[i].value_Renamed = volts[i] > 2.5;
                }
            }
        }
        internal virtual bool needsBits()
        {
            return false;
        }
        internal abstract void  setupPins();
        internal override void  draw(System.Drawing.Graphics g)
        {
            drawChip(g);
        }
        internal virtual void  drawChip(System.Drawing.Graphics g)
        {
            int i;
            //UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
            System.Drawing.Font f = new System.Drawing.Font("SansSerif", 10 * csize, System.Drawing.FontStyle.Regular);
            SupportClass.GraphicsManager.manager.SetFont(g, f);
            System.Drawing.Font fm = SupportClass.GraphicsManager.manager.GetFont(g);
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                setVoltageColor(g, volts[i]);
                System.Drawing.Point a = p.post;
                System.Drawing.Point b = p.stub;
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawThickLine(g, ref a, ref b);
                p.curcount = updateDotCount(p.current, p.curcount);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                drawDots(g, ref b, ref a, p.curcount);
                if (p.bubble)
                {
                    SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.White);
                    drawThickCircle(g, p.bubbleX, p.bubbleY, 1);
                    SupportClass.GraphicsManager.manager.SetColor(g, lightGrayColor);
                    drawThickCircle(g, p.bubbleX, p.bubbleY, 3);
                }
                SupportClass.GraphicsManager.manager.SetColor(g, whiteColor);
                //UPGRADE_ISSUE: Method 'java.awt.FontMetrics.stringWidth' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtFontMetricsstringWidth_javalangString'"
                int sw = (int)g.MeasureString(p.text, f).Width;
                //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                g.DrawString(p.text, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), p.textloc.X - sw / 2, p.textloc.Y + SupportClass.GetAscent(fm) / 2 - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
                if (p.lineOver)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.FontMetrics.getAscent' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    int ya = p.textloc.Y - SupportClass.GetAscent(fm) / 2;
                    g.DrawLine(SupportClass.GraphicsManager.manager.GetPen(g), p.textloc.X - sw / 2, ya, p.textloc.X + sw / 2, ya);
                }
            }
            SupportClass.GraphicsManager.manager.SetColor(g, needsHighlight()?selectColor:lightGrayColor);
            drawThickPolygon(g, rectPointsX, rectPointsY, 4);
            if (clockPointsX != null)
                g.DrawLines(SupportClass.GraphicsManager.manager.GetPen(g), SupportClass.GetPoints(clockPointsX, clockPointsY, 3));
            for (i = 0; i != PostCount; i++)
                drawPost(g, pins[i].post.X, pins[i].post.Y, nodes[i]);
        }
        internal int[] rectPointsX, rectPointsY;
        internal int[] clockPointsX, clockPointsY;
        internal Pin[] pins;
        internal int sizeX, sizeY;
        internal bool lastClock;
        internal override void  drag(int xx, int yy)
        {
            yy = sim.snapGrid(yy);
            if (xx < x)
            {
                xx = x; yy = y;
            }
            else
            {
                y = y2 = yy;
                x2 = sim.snapGrid(xx);
            }
            setPoints();
        }
        internal override void  setPoints()
        {
            if (x2 - x > sizeX * cspc2 && this == sim.dragElm)
                Size = 2;
            int hs = cspc;
            int x0 = x + cspc2; int y0 = y;
            int xr = x0 - cspc;
            int yr = y0 - cspc;
            int xs = sizeX * cspc2;
            int ys = sizeY * cspc2;
            rectPointsX = new int[]{xr, xr + xs, xr + xs, xr};
            rectPointsY = new int[]{yr, yr, yr + ys, yr + ys};
            setBbox(xr, yr, rectPointsX[2], rectPointsY[2]);
            int i;
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                switch (p.side)
                {
				
                    case SIDE_N:  p.setPoint(x0, y0, 1, 0, 0, - 1, 0, 0); break;
				
                    case SIDE_S:  p.setPoint(x0, y0, 1, 0, 0, 1, 0, ys - cspc2); break;
				
                    case SIDE_W:  p.setPoint(x0, y0, 0, 1, - 1, 0, 0, 0); break;
				
                    case SIDE_E:  p.setPoint(x0, y0, 0, 1, 1, 0, xs - cspc2, 0); break;
                }
            }
        }
        internal override System.Drawing.Point getPost(int n)
        {
            return pins[n].post;
        }
        internal override void  setVoltageSource(int j, int vs)
        {
            int i;
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                if (p.output && j-- == 0)
                {
                    p.voltSource = vs;
                    return ;
                }
            }
            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Console.Out.WriteLine("setVoltageSource failed for " + this);
        }
        internal override void  stamp()
        {
            int i;
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                if (p.output)
                    sim.stampVoltageSource(0, nodes[i], p.voltSource);
            }
        }
        internal virtual void  execute()
        {
        }
        internal override void  doStep()
        {
            int i;
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                if (!p.output)
                    p.value_Renamed = volts[i] > 2.5;
            }
            execute();
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                if (p.output)
                    sim.updateVoltageSource(0, nodes[i], p.voltSource, p.value_Renamed?5:0);
            }
        }
        internal override void  reset()
        {
            int i;
            for (i = 0; i != PostCount; i++)
            {
                pins[i].value_Renamed = false;
                pins[i].curcount = 0;
                volts[i] = 0;
            }
            lastClock = false;
        }
	
        internal override System.String dump()
        {
            int t = DumpType;
            System.String s = base.dump();
            if (needsBits())
                s += (" " + bits);
            int i;
            for (i = 0; i != PostCount; i++)
            {
                if (pins[i].state)
                    s += (" " + volts[i]);
            }
            return s;
        }
	
        internal override void  getInfo(System.String[] arr)
        {
            arr[0] = ChipName;
            int i, a = 1;
            for (i = 0; i != PostCount; i++)
            {
                Pin p = pins[i];
                if (arr[a] != null)
                    arr[a] += "; ";
                else
                    arr[a] = "";
                System.String t = p.text;
                if (p.lineOver)
                    t += '\'';
                if (p.clock)
                    t = "Clk";
                arr[a] += (t + " = " + getVoltageText(volts[i]));
                if (i % 2 == 1)
                    a++;
            }
        }
        internal override void  setCurrent(int x, double c)
        {
            int i;
            for (i = 0; i != PostCount; i++)
                if (pins[i].output && pins[i].voltSource == x)
                    pins[i].current = c;
        }
        internal override bool getConnection(int n1, int n2)
        {
            return false;
        }
        internal override bool hasGroundConnection(int n1)
        {
            return pins[n1].output;
        }
	
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Flip X", (flags & FLAG_FLIP_X) != 0);
                return ei;
            }
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, - 1, - 1);
                ei.checkbox = SupportClass.CheckBoxSupport.CreateCheckBox("Flip Y", (flags & FLAG_FLIP_Y) != 0);
                return ei;
            }
            return null;
        }
        public override void  setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_FLIP_X;
                else
                    flags &= ~ FLAG_FLIP_X;
                setPoints();
            }
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_FLIP_Y;
                else
                    flags &= ~ FLAG_FLIP_Y;
                setPoints();
            }
        }
	
        //UPGRADE_NOTE: Final was removed from the declaration of 'SIDE_N '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal const int SIDE_N = 0;
        //UPGRADE_NOTE: Final was removed from the declaration of 'SIDE_S '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal const int SIDE_S = 1;
        //UPGRADE_NOTE: Final was removed from the declaration of 'SIDE_W '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal const int SIDE_W = 2;
        //UPGRADE_NOTE: Final was removed from the declaration of 'SIDE_E '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        internal const int SIDE_E = 3;
        //UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Pin' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
        internal class Pin
        {
            private void  InitBlock(ChipElm enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private ChipElm enclosingInstance;
            public ChipElm Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
			
            }
            internal Pin(ChipElm enclosingInstance, int p, int s, System.String t)
            {
                InitBlock(enclosingInstance);
                pos = p; side = s; text = t;
            }
            internal System.Drawing.Point post, stub;
            internal System.Drawing.Point textloc;
            internal int pos, side, voltSource, bubbleX, bubbleY;
            internal System.String text;
            internal bool lineOver, bubble, clock, output, value_Renamed, state;
            internal double curcount, current;
            internal virtual void  setPoint(int px, int py, int dx, int dy, int dax, int day, int sx, int sy)
            {
                if ((Enclosing_Instance.flags & Enclosing_Instance.FLAG_FLIP_X) != 0)
                {
                    dx = - dx;
                    dax = - dax;
                    px += Enclosing_Instance.cspc2 * (Enclosing_Instance.sizeX - 1);
                    sx = - sx;
                }
                if ((Enclosing_Instance.flags & Enclosing_Instance.FLAG_FLIP_Y) != 0)
                {
                    dy = - dy;
                    day = - day;
                    py += Enclosing_Instance.cspc2 * (Enclosing_Instance.sizeY - 1);
                    sy = - sy;
                }
                int xa = px + Enclosing_Instance.cspc2 * dx * pos + sx;
                int ya = py + Enclosing_Instance.cspc2 * dy * pos + sy;
                post = new System.Drawing.Point(xa + dax * Enclosing_Instance.cspc2, ya + day * Enclosing_Instance.cspc2);
                stub = new System.Drawing.Point(xa + dax * Enclosing_Instance.cspc, ya + day * Enclosing_Instance.cspc);
                textloc = new System.Drawing.Point(xa, ya);
                if (bubble)
                {
                    bubbleX = xa + dax * 10 * Enclosing_Instance.csize;
                    bubbleY = ya + day * 10 * Enclosing_Instance.csize;
                }
                if (clock)
                {
                    Enclosing_Instance.clockPointsX = new int[3];
                    Enclosing_Instance.clockPointsY = new int[3];
                    Enclosing_Instance.clockPointsX[0] = xa + dax * Enclosing_Instance.cspc - dx * Enclosing_Instance.cspc / 2;
                    Enclosing_Instance.clockPointsY[0] = ya + day * Enclosing_Instance.cspc - dy * Enclosing_Instance.cspc / 2;
                    Enclosing_Instance.clockPointsX[1] = xa;
                    Enclosing_Instance.clockPointsY[1] = ya;
                    Enclosing_Instance.clockPointsX[2] = xa + dax * Enclosing_Instance.cspc + dx * Enclosing_Instance.cspc / 2;
                    Enclosing_Instance.clockPointsY[2] = ya + day * Enclosing_Instance.cspc + dy * Enclosing_Instance.cspc / 2;
                }
            }
        }
    }
}