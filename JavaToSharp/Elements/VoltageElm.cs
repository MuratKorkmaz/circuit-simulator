using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class VoltageElm : CircuitElm
    {
         
        private const int FLAG_COS = 2;
        internal int waveform;
        internal const int WF_DC = 0;
        internal const int WF_AC = 1;
        internal const int WF_SQUARE = 2;
        private const int WF_TRIANGLE = 3;
        private const int WF_SAWTOOTH = 4;
        private const int WF_PULSE = 5;
        internal const int WF_VAR = 6;
        internal double frequency;
        internal double maxVoltage;
        private double freqTimeZero;
        internal double bias;
        private double phaseShift;
        private double dutyCycle;
        internal VoltageElm(int xx, int yy, int wf) : base(xx, yy)
        {
            
            
            
            waveform = wf;
            maxVoltage = 5;
            frequency = 40;
            dutyCycle =.5;
            reset();
        }

        protected VoltageElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            maxVoltage = 5;
            frequency = 40;
            waveform = WF_DC;
            dutyCycle =.5;
            try
            {
         
                string sWaweform = st.nextToken();
                bool isParsedWaweform = int.TryParse(sWaweform, out waveform);
                if (!isParsedWaweform)
                {
                    throw new Exception("Не удалось привести к int");
                }

                string sFrequency = st.nextToken();
                bool isParsedFrequency = double.TryParse(sFrequency, out  frequency);
                if (!isParsedFrequency)
                {
                    throw new Exception("Не удалось привести к double");
                }
                string sMaxVoltage = st.nextToken();
                bool isParsedMaxVoltage = double.TryParse(sMaxVoltage, out  maxVoltage);
                 if (!isParsedMaxVoltage)
                {
                    throw new Exception("Не удалось привести к double");
                }
                string sBias =st.nextToken();
                bool isParsedBias = double.TryParse(sBias, out bias);
                if (!isParsedBias)
                {
                    throw new Exception("Не удалось привести к double");
                }
                string sPhaseShift = st.nextToken();
                bool isPhaseShift = double.TryParse(sPhaseShift, out phaseShift);
                if(!isPhaseShift)
                {
                    throw new Exception("Не удалось привести к double");
                }
                string sDutyCycle = st.nextToken();
                bool isDutyCycle = double.TryParse(sDutyCycle, out dutyCycle);
                if(!isDutyCycle)
                {
                    throw new Exception("Не удалось привести к double");
                }
            }
            catch (Exception e)
            {
            }
            if ((flags & FLAG_COS) != 0)
            {
                flags &= ~FLAG_COS;
                phaseShift = pi/2;
            }
            reset();
        }
        internal override int DumpType
        {
            get
            {
                return 'v';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + waveform + " " + frequency + " " + maxVoltage + " " + bias + " " + phaseShift + " " + dutyCycle;
        }
//    void setCurrent(double c) {
//      current = c;
//      System.out.print("v current set to " + c + "\n");
//      }

        internal override void reset()
        {
            freqTimeZero = 0;
            curcount = 0;
        }

        private double triangleFunc(double x)
        {
            if (x < pi)
                return x*(2/pi)-1;
            return 1-(x-pi)*(2/pi);
        }
        internal override void stamp()
        {
            if (waveform == WF_DC)
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource, Voltage);
            else
                sim.stampVoltageSource(nodes[0], nodes[1], voltSource);
        }
        internal override void doStep()
        {
            if (waveform != WF_DC)
                sim.updateVoltageSource(nodes[0], nodes[1], voltSource, Voltage);
        }
        internal double Voltage
        {
            get
            {
                double w = 2*pi*(sim.t-freqTimeZero)*frequency + phaseShift;
                switch (waveform)
                {
                    case WF_DC:
                        return maxVoltage+bias;
                    case WF_AC:
                        return Math.Sin(w)*maxVoltage+bias;
                    case WF_SQUARE:
                        return bias+((w % (2*pi) > (2*pi*dutyCycle)) ? -maxVoltage : maxVoltage);
                    case WF_TRIANGLE:
                        return bias+triangleFunc(w % (2*pi))*maxVoltage;
                    case WF_SAWTOOTH:
                        return bias+(w % (2*pi))*(maxVoltage/pi)-maxVoltage;
                    case WF_PULSE:
                        return ((w % (2*pi)) < 1) ? maxVoltage+bias : bias;
                    default:
                        return 0;
                }
            }
        }

        internal const int circleSize = 17;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads((waveform == WF_DC || waveform == WF_VAR) ? 8 : circleSize*2);
        }
        internal override void draw(Graphics g)
        {
            setBbox(x, y, x2, y2);
            draw2Leads(g);
            if (waveform == WF_DC)
            {
                voltageColor = setVoltageColor(g, volts[0]);
                myPen = new Pen(voltageColor);
                interpPoint2(lead1, lead2, ps1, ps2, 0, 10);
                drawThickLine(g, myPen,ps1, ps2);
                voltageColor = setVoltageColor(g, volts[1]);
                myPen = new Pen(voltageColor);
                int hs = 16;
                setBbox(point1, point2, hs);
                interpPoint2(lead1, lead2, ps1, ps2, 1, hs);
                drawThickLine(g, myPen,ps1, ps2);
            }
            else
            {
                setBbox(point1, point2, circleSize);
                interpPoint(lead1, lead2, ps1,.5);
                drawWaveform(g, ps1);
            }
            updateDotCount();
            if (sim.dragElm != this)
            {
                if (waveform == WF_DC)
                    drawDots(g, point1, point2, curcount);
                else
                {
                    drawDots(g, point1, lead1, curcount);
                    drawDots(g, point2, lead2, -curcount);
                }
            }
            drawPosts(g);
        }

        internal void drawWaveform(Graphics g, Point center)
        {
            g.GetNearestColor(needsHighlight() ? selectColor : Color.Gray);
            int xc = center.X;
            int yc = center.Y;
            drawThickCircle(g, xc, yc, circleSize);
            int wl = 8;
            adjustBbox(xc-circleSize, yc-circleSize, xc+circleSize, yc+circleSize);
            int xc2;
            switch (waveform)
            {
                case WF_DC:
                    {
                        break;
                    }
                case WF_SQUARE:
                    xc2 = (int)(wl*2*dutyCycle-wl+xc);
                    xc2 = max(xc-wl+3, min(xc+wl-3, xc2));
                    drawThickLine(g, xc-wl, yc-wl, xc-wl, yc);
                    drawThickLine(g, xc-wl, yc-wl, xc2, yc-wl);
                    drawThickLine(g, xc2, yc-wl, xc2, yc+wl);
                    drawThickLine(g, xc+wl, yc+wl, xc2, yc+wl);
                    drawThickLine(g, xc+wl, yc, xc+wl, yc+wl);
                    break;
                case WF_PULSE:
                    yc += wl/2;
                    drawThickLine(g, xc-wl, yc-wl, xc-wl, yc);
                    drawThickLine(g, xc-wl, yc-wl, xc-wl/2, yc-wl);
                    drawThickLine(g, xc-wl/2, yc-wl, xc-wl/2, yc);
                    drawThickLine(g, xc-wl/2, yc, xc+wl, yc);
                    break;
                case WF_SAWTOOTH:
                    drawThickLine(g, xc, yc-wl, xc-wl, yc);
                    drawThickLine(g, xc, yc-wl, xc, yc+wl);
                    drawThickLine(g, xc, yc+wl, xc+wl, yc);
                    break;
                case WF_TRIANGLE:
                    {
                        int xl = 5;
                        drawThickLine(g, xc-xl*2, yc, xc-xl, yc-wl);
                        drawThickLine(g, xc-xl, yc-wl, xc, yc);
                        drawThickLine(g, xc, yc, xc+xl, yc+wl);
                        drawThickLine(g, xc+xl, yc+wl, xc+xl*2, yc);
                        break;
                    }
                case WF_AC:
                    {
                        int i;
                        int xl = 10;
                        int ox = -1, oy = -1;
                        for (i = -xl; i <= xl; i++)
                        {
                            int yy = yc+(int)(.95*Math.Sin(i*pi/xl)*wl);
                            if (ox != -1)
                                drawThickLine(g, ox, oy, xc+i, yy);
                            ox = xc+i;
                            oy = yy;
                        }
                        break;
                    }
            }
           
        }

        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal override double Power
        {
            get
            {
                return -VoltageDiff*current;
            }
        }
        internal override double VoltageDiff
        {
            get
            {
                return volts[1] - volts[0];
            }
        }
        internal override void getInfo(string[] arr)
        {
            switch (waveform)
            {
                case WF_DC:
                case WF_VAR:
                    arr[0] = "источник напряжения";
                    break;
                case WF_AC:
                    arr[0] = "ген. перем. тока";
                    break;
                case WF_SQUARE:
                    arr[0] = "ген. прямоуг.";
                    break;
                case WF_PULSE:
                    arr[0] = "ген. импульсов";
                    break;
                case WF_SAWTOOTH:
                    arr[0] = "ген. пилы";
                    break;
                case WF_TRIANGLE:
                    arr[0] = "ген. треугольн.";
                    break;
            }
            arr[1] = "I = " + getCurrentText(Current);
            arr[2] = ((this is RailElm) ? "V = " : "Vd = ") + getVoltageText(VoltageDiff);
            if (waveform != WF_DC && waveform != WF_VAR)
            {
                arr[3] = "f = " + getUnitText(frequency, "Гц");
                arr[4] = "Vmax = " + getVoltageText(maxVoltage);
                int i = 5;
                if (bias != 0)
                    arr[i++] = "Voff = " + getVoltageText(bias);
                else if (frequency > 500)
                    arr[i++] = "длинна волны = " + getUnitText(2.9979e8/frequency, "м");
                arr[i++] = "P = " + getUnitText(Power, "Вт");
            }
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo(waveform == WF_DC ? "Напряжение" : "максимальное напряжение", maxVoltage, -20, 20);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("Форма напряжения", waveform, -1, -1);
                ei.choice = new Choice();
                ei.choice.add("Постоянный ток");  
                ei.choice.add("Переменный ток");
                ei.choice.add("Прямоугольн.");
                ei.choice.add("Треугольн.");
                ei.choice.add("Пилообразн.");
                ei.choice.add("Импульсн.");
                ei.choice.select(waveform);
                return ei;
            }
            if (waveform == WF_DC)
                return null;
            if (n == 2)
                return new EditInfo("Частота (Гц)", frequency, 4, 500);
            if (n == 3)
                return new EditInfo("Смещение (В)", bias, -20, 20);
            if (n == 4)
                return new EditInfo("Смещение фазы (градусов)", phaseShift*180/pi, -180, 180).setDimensionless();
            if (n == 5 && waveform == WF_SQUARE)
                return new EditInfo("Скважность", dutyCycle*100, 0, 100).setDimensionless();
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                maxVoltage = ei.value;
            if (n == 3)
                bias = ei.value;
            if (n == 2)
            {
                // adjust time zero to maintain continuity ind the waveform
                // even though the frequency has changed.
                double oldfreq = frequency;
                frequency = ei.value;
                double maxfreq = 1/(8*sim.timeStep);
                if (frequency > maxfreq)
                    frequency = maxfreq;
                double adj = frequency-oldfreq;
                freqTimeZero = sim.t-oldfreq*(sim.t-freqTimeZero)/frequency;
            }
            if (n == 1)
            {
                int ow = waveform;
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
                phaseShift = ei.value*pi/180;
            if (n == 5)
                dutyCycle = ei.value*.01;
        }
    }
}
