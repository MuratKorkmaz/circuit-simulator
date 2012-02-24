using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class Scope
    {
        private const int FLAG_YELM = 32;
        private const int VAL_POWER = 1;
        internal const int VAL_IB = 1;
        internal const int VAL_IC = 2;
        internal const int VAL_IE = 3;
        internal const int VAL_VBE = 4;
        internal const int VAL_VBC = 5;
        internal const int VAL_VCE = 6;
        private const int VAL_R = 2;
        private double[] minV;
        private double[] maxV;
        private double minMaxV;
        private double[] minI;
        private double[] maxI;
        private double minMaxI;
        private int scopePointCount = 128;
        private int ptr;
        internal int ctr, speed, position;
        private int value;
        internal int ivalue;
        private string text;
        internal Rectangle rect;
        private bool showI;
        private bool showV;
        internal bool BshowMax;
        internal bool BshowMin;
        internal bool BshowFreq;
        private bool lockScale;
        private bool plot2d;
        private bool plotXY;
        internal CircuitElm elm, xElm;
        private CircuitElm yElm;
        private MemoryImageSource imageSource;
        private Image image;
        private int[] pixels;
        private int draw_ox;
        private int draw_oy;
        private float[] dpixels;
        private readonly CirSim sim;
        internal Scope(CirSim s)
        {
            rect = new Rectangle();
            reset();
            sim = s;
        }

        protected virtual void showCurrent(bool b)
        {
            showI = b;
            value = ivalue = 0;
        }

        protected virtual void showVoltage(bool b)
        {
            showV = b;
            value = ivalue = 0;
        }
        internal virtual void showMax(bool b)
        {
           BshowMax = b;
        }
        internal virtual void showMin(bool b)
        {
            BshowMin = b;
        }
        internal virtual void showFreq(bool b)
        {
            BshowFreq = b;
        }
        internal virtual bool LockScale
        {
            set
            {
                lockScale = value;
            }
        }
        internal virtual void resetGraph()
        {
            scopePointCount = 1;
            while (scopePointCount <= rect.Width)
                scopePointCount *= 2;
            minV = new double[scopePointCount];
            maxV = new double[scopePointCount];
            minI = new double[scopePointCount];
            maxI = new double[scopePointCount];
            ptr = ctr = 0;
            allocImage();
        }
        internal virtual bool active()
        {
            return elm != null;
        }

        protected virtual void reset()
        {
            resetGraph();
            minMaxV = 5;
            minMaxI =.1;
            speed = 64;
            showI = showV = BshowMax = true;
            BshowFreq = lockScale = BshowMin = false;
            plot2d = false;
            // no showI for Output
            if (elm != null && (elm is OutputElm || elm is LogicOutputElm || elm is ProbeElm))
                showI = false;
            value = ivalue = 0;
            if (elm is TransistorElm)
                value = VAL_VCE;
        }
        internal virtual Rectangle Rect
        {
            set
            {
                rect = value;
                resetGraph();
            }
        }
        internal virtual int Width
        {
            get
            {
                return rect.Width;
            }
        }
        internal virtual int rightEdge()
        {
            return rect.X + rect.Width;
        }

        internal virtual CircuitElm Elm
        {
            set
            {
                elm = value;
                reset();
            }
        }

        internal virtual void timeStep()
        {
            if (elm == null)
                return;
            double v = elm.getScopeValue(value);
            if (v < minV[ptr])
                minV[ptr] = v;
            if (v > maxV[ptr])
                maxV[ptr] = v;
            double i = 0;
            if (value == 0 || ivalue != 0)
            {
                i = (ivalue == 0) ? elm.Current : elm.getScopeValue(ivalue);
                if (i < minI[ptr])
                    minI[ptr] = i;
                if (i > maxI[ptr])
                    maxI[ptr] = i;
            }

            if (plot2d && dpixels != null)
            {
                bool newscale = false;
                while (v > minMaxV || v < -minMaxV)
                {
                    minMaxV *= 2;
                    newscale = true;
                }
                double yval = i;
                if (plotXY)
                    yval = (yElm == null) ? 0 : yElm.VoltageDiff;
                while (yval > minMaxI || yval < -minMaxI)
                {
                    minMaxI *= 2;
                    newscale = true;
                }
                if (newscale)
                    clear2dView();
                double xa = v/minMaxV;
                double ya = yval/minMaxI;
                int x = (int)(rect.Width *(1+xa)*.499);
                int y = (int)(rect.Height*(1-ya)*.499);
                drawTo(x, y);
            }
            else
            {
                ctr++;
                if (ctr >= speed)
                {
                    ptr = (ptr+1) & (scopePointCount-1);
                    minV[ptr] = maxV[ptr] = v;
                    minI[ptr] = maxI[ptr] = i;
                    ctr = 0;
                }
            }
        }

        protected virtual void drawTo(int x2, int y2)
        {
            if (draw_ox == -1)
            {
                draw_ox = x2;
                draw_oy = y2;
            }
            // need to draw a line from x1,y1 to x2,y2
            if (draw_ox == x2 && draw_oy == y2)
            {
                dpixels[x2+rect.Width*y2] = 1;
            }
            else if (CircuitElm.abs(y2-draw_oy) > CircuitElm.abs(x2-draw_ox))
            {
                // y difference is greater, so we step along y's
                // from min to max y and calculate x for each step
                double sgn = CircuitElm.sign(y2-draw_oy);
                int x, y;
                for (y = draw_oy; y != y2+sgn; y +=Convert.ToInt32(sgn))
                {
                    x = draw_ox+(x2-draw_ox)*(y-draw_oy)/(y2-draw_oy);
                    dpixels[x+rect.Width*y] = 1;
                }
            }
            else
            {
                // x difference is greater, so we step along x's
                // from min to max x and calculate y for each step
                double sgn = CircuitElm.sign(x2-draw_ox);
                int x, y;
                for (x = draw_ox; Math.Abs(x - (x2+sgn)) > double.Epsilon; x += Convert.ToInt32(sgn))
                {
                    y = draw_oy+(y2-draw_oy)*(x-draw_ox)/(x2-draw_ox);
                    dpixels[x+rect.Width*y] = 1;
                }
            }
            draw_ox = x2;
            draw_oy = y2;
        }

        protected virtual void clear2dView()
        {
            int i;
            for (i = 0; i != dpixels.Length; i++)
                dpixels[i] = 0;
            draw_ox = draw_oy = -1;
        }

        internal virtual void adjustScale(double x)
        {
            minMaxV *= x;
            minMaxI *= x;
        }

        protected virtual void draw2d(Graphics g)
        {
            int i;
            if (pixels == null || dpixels == null)
                return;
            int col = (sim.printableCheckItem.State) ? 0xFFFFFFFF : 0;
            for (i = 0; i != pixels.Length; i++)
                pixels[i] = col;
            for (i = 0; i != rect.Width; i++)
                pixels[i + rect.Width * (rect.Height / 2)] = Convert.ToInt32(0xFF00FF00);
            int ycol = (plotXY) ? Convert.ToInt32(0xFF00FF00) : Convert.ToInt32(0xFFFFFF00);
            for (i = 0; i != rect.Height; i++)
                pixels[rect.Width/2+rect.Width*i] = ycol;
            for (i = 0; i != pixels.Length; i++)
            {
                int q = (int)(255*dpixels[i]);
                if (q > 0)
                    pixels[i] = (int) (0xFF000000 | (0x10101*q));
                dpixels[i] *=(float).997;
            }
            g.drawImage(image, rect.X, rect.Y, null);
            g.Color = elm.whiteColor;
            g.fillOval(rect.X+draw_ox-2, rect.Y+draw_oy-2, 5, 5);
            int yt = rect.Y+10;
            int x = rect.X;
            if (text != null && rect.Y + rect.Height > yt+5)
            {
                g.drawString(text, x, yt);
                yt += 15;
            }
        }

        internal virtual void draw(Graphics g)
        {
            if (elm == null)
                return;
            if (plot2d)
            {
                draw2d(g);
                return;
            }
            if (pixels == null)
                return;
            int i;
            int col = (sim.printableCheckItem.State) ? 0xFFFFFFFF : 0;
            for (i = 0; i != pixels.Length; i++)
                pixels[i] = col;
            int x = 0;
            int maxy = (rect.Height-1)/2;
            int y = maxy;

            bool gotI = false;
            bool gotV = false;
            const int minRange = 4;
            double realMaxV = -1e8;
            double realMaxI = -1e8;
            double realMinV = 1e8;
            double realMinI = 1e8;
            int curColor = Convert.ToInt32(0xFFFFFF00);
            int voltColor = (int) ((value > 0) ? 0xFFFFFFFF : 0xFF00FF00);
            if (sim.scopeSelected == -1 && elm == sim.mouseElm)
                curColor = voltColor = Convert.ToInt32(0xFF00FFFF);
            int ipa = ptr+scopePointCount-rect.Width;
            for (i = 0; i != rect.Width; i++)
            {
                int ip = (i+ipa) & (scopePointCount-1);
                while (maxV[ip] > minMaxV)
                    minMaxV *= 2;
                while (minV[ip] < -minMaxV)
                    minMaxV *= 2;
                while (maxI[ip] > minMaxI)
                    minMaxI *= 2;
                while (minI[ip] < -minMaxI)
                    minMaxI *= 2;
            }

            double gridStep = 1e-8;
            double gridMax = (showI ? minMaxI : minMaxV);
            while (gridStep*100 < gridMax)
                gridStep *= 10;
            if (maxy*gridStep/gridMax <.3)
                gridStep = 0;

            int ll;
            bool sublines = (maxy*gridStep/gridMax > 3);
            for (ll = -100; ll <= 100; ll++)
            {
                // don't show gridlines if plotting multiple values,
                // or if lines are too close together (except for center line)
                if (ll != 0 && ((showI && showV) || gridStep == 0))
                    continue;
                int yl = maxy-(int)(maxy*ll*gridStep/gridMax);
                if (yl < 0 || yl >= rect.Height-1)
                    continue;
                col = (int) (ll == 0 ? 0xFF909090 : 0xFF404040);
                if (ll % 10 != 0)
                {
                    col = Convert.ToInt32(0xFF101010);
                    if (!sublines)
                        continue;
                }
                for (i = 0; i != rect.Width; i++)
                    pixels[i+yl*rect.Width] = col;
            }

            gridStep = 1e-15;
            double ts = sim.timeStep*speed;
            while (gridStep < ts*5)
                gridStep *= 10;
            double tstart = sim.t-sim.timeStep*speed*rect.Width;
            double tx = sim.t-(sim.t % gridStep);
            for (ll = 0; ; ll++)
            {
                double tl = tx-gridStep*ll;
                int gx = (int)((tl-tstart)/ts);
                if (gx < 0)
                    break;
                if (gx >= rect.Width)
                    continue;
                if (tl < 0)
                    continue;
                col = Convert.ToInt32(0xFF202020);
                if (((tl+gridStep/4) % (gridStep*10)) < gridStep)
                {
                    col = Convert.ToInt32(0xFF909090);
                    if (((tl+gridStep/4) % (gridStep*100)) < gridStep)
                        col = Convert.ToInt32(0xFF4040D0);
                }
                for (i = 0; i < pixels.Length; i += rect.Width)
                    pixels[i+gx] = col;
            }

            // these two loops are pretty much the same, and should be
            // combined!
            if (value == 0 && showI)
            {
                int ox = -1, oy = -1;
                int j;
                for (i = 0; i != rect.Width; i++)
                {
                    int ip = (i+ipa) & (scopePointCount-1);
                    int miniy = (int)((maxy/minMaxI)*minI[ip]);
                    int maxiy = (int)((maxy/minMaxI)*maxI[ip]);
                    if (maxI[ip] > realMaxI)
                        realMaxI = maxI[ip];
                    if (minI[ip] < realMinI)
                        realMinI = minI[ip];
                    if (miniy <= maxy)
                    {
                        if (miniy < -minRange || maxiy > minRange)
                            gotI = true;
                        if (ox != -1)
                        {
                            if (miniy == oy && maxiy == oy)
                                continue;
                            for (j = ox; j != x+i; j++)
                                pixels[j+rect.Width*(y-oy)] = curColor;
                            ox = oy = -1;
                        }
                        if (miniy == maxiy)
                        {
                            ox = x+i;
                            oy = miniy;
                            continue;
                        }
                        for (j = miniy; j <= maxiy; j++)
                            pixels[x+i+rect.Width*(y-j)] = curColor;
                    }
                }
                if (ox != -1)
                    for (j = ox; j != x+i; j++)
                        pixels[j+rect.Width*(y-oy)] = curColor;
            }
            if (value != 0 || showV)
            {
                int ox = -1, oy = -1, j ;
                for (i = 0; i != rect.Width; i++)
                {
                    int ip = (i+ipa) & (scopePointCount-1);
                    int minvy = (int)((maxy/minMaxV)*minV[ip]);
                    int maxvy = (int)((maxy/minMaxV)*maxV[ip]);
                    if (maxV[ip] > realMaxV)
                        realMaxV = maxV[ip];
                    if (minV[ip] < realMinV)
                        realMinV = minV[ip];
                    if ((value != 0 || showV) && minvy <= maxy)
                    {
                        if (minvy < -minRange || maxvy > minRange)
                            gotV = true;
                        if (ox != -1)
                        {
                            if (minvy == oy && maxvy == oy)
                                continue;
                            for (j = ox; j != x+i; j++)
                                pixels[j+rect.Width*(y-oy)] = voltColor;
                            ox = oy = -1;
                        }
                        if (minvy == maxvy)
                        {
                            ox = x+i;
                            oy = minvy;
                            continue;
                        }
                        for (j = minvy; j <= maxvy; j++)
                            pixels[x+i+rect.Width*(y-j)] = voltColor;
                    }
                }
                if (ox != -1)
                    for (j = ox; j != x+i; j++)
                        pixels[j+rect.Width*(y-oy)] = voltColor;
            }
            double freq = 0;
            if (BshowFreq)
            {
                // try to get frequency
                // get average
                double avg = 0;
                for (i = 0; i != rect.Width; i++)
                {
                    int ip = (i+ipa) & (scopePointCount-1);
                    avg += minV[ip]+maxV[ip];
                }
                avg /= i*2;
                int state = 0;
                double thresh = avg*.05;
                int oi = 0;
                double avperiod = 0;
                int periodct = -1;
                double avperiod2 = 0;
                // count period lengths
                for (i = 0; i != rect.Width; i++)
                {
                    int ip = (i+ipa) & (scopePointCount-1);
                    double q = maxV[ip]-avg;
                    int os = state;
                    if (q < thresh)
                        state = 1;
                    else if (q > -thresh)
                        state = 2;
                    if (state == 2 && os == 1)
                    {
                        int pd = i-oi;
                        oi = i;
                        // short periods can't be counted properly
                        if (pd < 12)
                            continue;
                        // skip first period, it might be too short
                        if (periodct >= 0)
                        {
                            avperiod += pd;
                            avperiod2 += pd*pd;
                        }
                        periodct++;
                    }
                }
                avperiod /= periodct;
                avperiod2 /= periodct;
                double periodstd = Math.Sqrt(avperiod2-avperiod*avperiod);
                freq = 1/(avperiod*sim.timeStep*speed);
                // don't show freq if standard deviation is too great
                if (periodct < 1 || periodstd > 2)
                    freq = 0;
                // System.out.println(freq + " " + periodstd + " " + periodct);
            }
            g.drawImage(image, rect.X, rect.Y, null);
            g.Color = elm.whiteColor;
            int yt = rect.Y+10;
            x += rect.X;
            if (BshowMax)
            {
                if (value != 0)
                    g.drawString(elm.getUnitText(realMaxV, elm.getScopeUnits(value)), x, yt);
                else if (showV)
                    g.drawString(elm.getVoltageText(realMaxV), x, yt);
                else if (showI)
                    g.drawString(elm.getCurrentText(realMaxI), x, yt);
                yt += 15;
            }
            if (BshowMin)
            {
                int ym = rect.Y+rect.Height-5;
                if (value != 0)
                    g.drawString(elm.getUnitText(realMinV, elm.getScopeUnits(value)), x, ym);
                else if (showV)
                    g.drawString(elm.getVoltageText(realMinV), x, ym);
                else if (showI)
                    g.drawString(elm.getCurrentText(realMinI), x, ym);
            }
            if (text != null && rect.Y + rect.Height > yt+5)
            {
                g.drawString(text, x, yt);
                yt += 15;
            }
            if (BshowFreq && freq != 0 && rect.Y + rect.Height > yt+5)
                g.drawString(elm.getUnitText(freq, "Гц"), x, yt);
            if (ptr > 5 && !lockScale)
            {
                if (!gotI && minMaxI > 1e-4)
                    minMaxI /= 2;
                if (!gotV && minMaxV > 1e-4)
                    minMaxV /= 2;
            }
        }

        internal virtual void speedUp()
        {
            if (speed > 1)
            {
                speed /= 2;
                resetGraph();
            }
        }
        internal virtual void slowDown()
        {
            speed *= 2;
            resetGraph();
        }

        internal virtual PopupMenu Menu
        {
            get
            {
                if (elm == null)
                    return null;
                if (elm is TransistorElm)
                {
                    sim.scopeIbMenuItem.State = value == VAL_IB;
                    sim.scopeIcMenuItem.State = value == VAL_IC;
                    sim.scopeIeMenuItem.State = value == VAL_IE;
                    sim.scopeVbeMenuItem.State = value == VAL_VBE;
                    sim.scopeVbcMenuItem.State = value == VAL_VBC;
                    sim.scopeVceMenuItem.State = value == VAL_VCE && ivalue != VAL_IC;
                    sim.scopeVceIcMenuItem.State = value == VAL_VCE && ivalue == VAL_IC;
                    return sim.transScopeMenu;
                }
                else
                {
                    sim.scopeVMenuItem.State = showV && value == 0;
                    sim.scopeIMenuItem.State = showI && value == 0;
                    sim.scopeMaxMenuItem.State = showMax;
                    sim.scopeMinMenuItem.State = showMin;
                    sim.scopeFreqMenuItem.State = showFreq;
                    sim.scopePowerMenuItem.State = value == VAL_POWER;
                    sim.scopeVIMenuItem.State = plot2d && !plotXY;
                    sim.scopeXYMenuItem.State = plotXY;
                    sim.scopeSelectYMenuItem.Enabled = plotXY;
                    sim.scopeResistMenuItem.State = value == VAL_R;
                    sim.scopeResistMenuItem.Enabled = elm is MemristorElm;
                    return sim.scopeMenu;
                }
            }
        }

        protected virtual int Value
        {
            set
            {
                reset();
                value = value;
            }
        }
        internal virtual string dump()
        {
            if (elm == null)
                return null;
            int flags = (showI ? 1 : 0) | (showV ? 2 : 0) | (BshowMax ? 0 : 4) | (BshowFreq ? 8 : 0) | (lockScale ? 16 : 0) | (plot2d ? 64 : 0) | (plotXY ? 128 : 0) | (BshowMin ? 256 : 0); // showMax used to be always on
            flags |= FLAG_YELM; // yelm present
            int eno = sim.locateElm(elm);
            if (eno < 0)
                return null;
            int yno = yElm == null ? -1 : sim.locateElm(yElm);
            string x = "o " + eno + " " + speed + " " + value + " " + flags + " " + minMaxV + " " + minMaxI + " " + position + " " + yno;
            if (text != null)
                x += " " + text;
            return x;
        }
        internal virtual void undump(StringTokenizer st)
        {
            reset();
            int e;
            string sE = st.nextToken();
            bool isE = int.TryParse(sE, out e);
            if (!isE)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            if (e == -1)
                return;
            elm = sim.getElm(e);
            string sSpeed = st.nextToken();
            bool isParsedSpeed = int.TryParse(sSpeed, out speed);
            if (!isParsedSpeed)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            string sValue = st.nextToken();
            bool isParsedValue = int.TryParse(sValue, out value);
            if (!isParsedValue)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            int flags;
            string sFlags = st.nextToken();
            bool isParsedFlags = int.TryParse(sFlags, out flags);
            if (!isParsedFlags)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            string sMinMaxV = st.nextToken();
            bool isParsedMinMaxV = double.TryParse(sMinMaxV, out minMaxV);
            if (!isParsedMinMaxV)
            {
                 throw new Exception("Не удалось привести к типу double");
            }
            string sMinMaxI = st.nextToken();
            bool isParsedMinMaxI = double.TryParse(sMinMaxI, out minMaxI);
            if (!isParsedMinMaxI)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            if (Math.Abs(minMaxV - 0) < double.Epsilon)
                minMaxV =.5;
            if (Math.Abs(minMaxI - 0) < double.Epsilon)
                minMaxI = 1;
            text = null;
            yElm = null;
            try
            {
                string sPosition = (st.nextToken());
                bool isParsedPosition = int.TryParse(sPosition,out position);
                if(!isParsedPosition)
                {
                     throw new Exception("Не удалось привести к типу int");
                }
                int ye = -1;
                if ((flags & FLAG_YELM) != 0)
                {
                    string sYe = st.nextToken();
                    bool isYe = int.TryParse(sYe, out ye);
                    if(!isYe)
                    {
                        throw new Exception("Не удалось привести к типу int");
                    }
                    if (ye != -1)
                        yElm = sim.getElm(ye);
                }
                while (st.hasMoreTokens())
                {
                    if (text == null)
                        text = st.nextToken();
                    else
                        text += " " + st.nextToken();
                }
            }
            catch (Exception ee)
            {
            }
            showI = (flags & 1) != 0;
            showV = (flags & 2) != 0;
            BshowMax = (flags & 4) == 0;
            BshowFreq = (flags & 8) != 0;
            lockScale = (flags & 16) != 0;
            plot2d = (flags & 64) != 0;
            plotXY = (flags & 128) != 0;
            BshowMin = (flags & 256) != 0;
        }
        internal virtual void allocImage()
        {
            pixels = null;
            int w = rect.Width;
            int h = rect.Height;
            if (w == 0 || h == 0)
                return;
            if (sim.useBufferedImage)
            {
                try
                {
//		 simulate the following code using reflection:
//		   dbimage = new BufferedImage(d.width, d.height,
//		   BufferedImage.TYPE_INT_RGB);
//		   DataBuffer db = (DataBuffer)(((BufferedImage)dbimage).
//		   getRaster().getDataBuffer());
//		   DataBufferInt dbi = (DataBufferInt) db;
//		   pixels = dbi.getData();
//		
                    Type biclass = Type.GetType("java.awt.image.BufferedImage");
                    Type dbiclass = Type.GetType("java.awt.image.DataBufferInt");
                    Type rasclass = Type.GetType("java.awt.image.Raster");
                    Constructor cstr = biclass.GetConstructor(new Class[] { typeof(int), typeof(int), typeof(int) });
                    image = (Image) cstr.newInstance(new object[] { new int?(w), new int?(h), new int?(BufferedImage.TYPE_INT_RGB)});
                    Method m = biclass.GetMethod("getRaster");
                    object ras = m.Invoke(image);
                    object db = rasclass.GetMethod("getDataBuffer").Invoke(ras);
                    pixels = (int[]) dbiclass.GetMethod("getData").Invoke(db);
                }
                catch (Exception ee)
                {
                    // ee.printStackTrace();
                    Console.WriteLine("BufferedImage failed");
                }
            }
            if (pixels == null)
            {
                pixels = new int[w*h];
                int i;
                for (i = 0; i != w*h; i++)
                    pixels[i] = Convert.ToInt32(0xff000000);
                imageSource = new MemoryImageSource(w, h, pixels, 0, w);
                imageSource.Animated = true;
                imageSource.FullBufferUpdates = true;
                image = sim.cv.createImage(imageSource);
            }
            dpixels = new float[w*h];
            draw_ox = draw_oy = -1;
        }

        internal virtual void handleMenu(ItemEvent e, object mi)
        {
            if (mi == sim.scopeVMenuItem)
                showVoltage(sim.scopeVMenuItem.State);
            if (mi == sim.scopeIMenuItem)
                showCurrent(sim.scopeIMenuItem.State);
            if (mi == sim.scopeMaxMenuItem)
                showMax(sim.scopeMaxMenuItem.State);
            if (mi == sim.scopeMinMenuItem)
                showMin(sim.scopeMinMenuItem.State);
            if (mi == sim.scopeFreqMenuItem)
                showFreq(sim.scopeFreqMenuItem.State);
            if (mi == sim.scopePowerMenuItem)
                Value = VAL_POWER;
            if (mi == sim.scopeIbMenuItem)
                Value = VAL_IB;
            if (mi == sim.scopeIcMenuItem)
                Value = VAL_IC;
            if (mi == sim.scopeIeMenuItem)
                Value = VAL_IE;
            if (mi == sim.scopeVbeMenuItem)
                Value = VAL_VBE;
            if (mi == sim.scopeVbcMenuItem)
                Value = VAL_VBC;
            if (mi == sim.scopeVceMenuItem)
                Value = VAL_VCE;
            if (mi == sim.scopeVceIcMenuItem)
            {
                plot2d = true;
                plotXY = false;
                value = VAL_VCE;
                ivalue = VAL_IC;
                resetGraph();
            }

            if (mi == sim.scopeVIMenuItem)
            {
                plot2d = sim.scopeVIMenuItem.State;
                plotXY = false;
                resetGraph();
            }
            if (mi == sim.scopeXYMenuItem)
            {
                plotXY = plot2d = sim.scopeXYMenuItem.State;
                if (yElm == null)
                    selectY();
                resetGraph();
            }
            if (mi == sim.scopeResistMenuItem)
                Value = VAL_R;
        }

        internal virtual void select()
        {
            sim.mouseElm = elm;
            if (plotXY)
            {
                sim.plotXElm = elm;
                sim.plotYElm = yElm;
            }
        }

        internal virtual void selectY()
        {
            int e = yElm == null ? -1 : sim.locateElm(yElm);
            int firstE = e;
            while (true)
            {
                for (e++; e < sim.elmList.Count; e++)
                {
                    CircuitElm ce = sim.getElm(e);
                    if ((ce is OutputElm || ce is ProbeElm) && ce != elm)
                    {
                        yElm = ce;
                        return;
                    }
                }
                if (firstE == -1)
                    return;
                e = firstE = -1;
            }
        }
    }
}
