using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class SweepElm : CircuitElm
    {
        private double maxV;
        private double maxF;
        private double minF;
        private double sweepTime;
        private double frequency;
        private const int FLAG_LOG = 1;
        private const int FLAG_BIDIR = 2;

        public SweepElm(int xx, int yy) : base(xx, yy)
        {
            minF = 20;
            maxF = 4000;
            maxV = 5;
            sweepTime =.1;
            flags = FLAG_BIDIR;
            reset();
        }
        public SweepElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sMinF = st.nextToken();
            bool isParsedMinF = double.TryParse(sMinF, out minF);
            if (!isParsedMinF)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sMaxF = st.nextToken();
            bool isParsedMaxF = double.TryParse(sMaxF, out maxF);
            if (!isParsedMaxF)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sMaxV = st.nextToken();
            bool isParsedMaxV = double.TryParse(sMaxV, out maxV);
            if (!isParsedMaxV)
            {
               throw new Exception("Не удалось привести к типу double"); 
            }
            string sSweepTime = st.nextToken();
            bool isParsedSweepTime = double.TryParse(sSweepTime, out sweepTime);
            if (!isParsedSweepTime)
            {
                 throw new Exception("Не удалось привести к типу double"); 
            }
            reset();
        }
        internal override int DumpType
        {
            get
            {
                return 170;
            }
        }
        internal override int PostCount
        {
            get
            {
                return 1;
            }
        }

        private const int circleSize = 17;

        internal override string dump()
        {
            return base.dump() + " " + minF + " " + maxF + " " + maxV + " " + sweepTime;
        }
        internal override void setPoints()
        {
            base.setPoints();
            lead1 = interpPoint(point1, point2, 1-circleSize/dn);
        }
        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, circleSize);
            voltageColor = setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,point1, lead1);
            int xc = point2.X;
            int yc = point2.Y;
            drawThickCircle(g, xc, yc, circleSize);
            int wl = 8;
            adjustBbox(xc-circleSize, yc-circleSize, xc+circleSize, yc+circleSize);
            int i;
            int xl = 10;
            int ox = -1, oy = -1;
            long tm =  (DateTime.Now.Ticks - 621355968000000000) / 10000;
            //double w = (this == mouseElm ? 3 : 2);
            tm %= 2000;
            if (tm > 1000)
                tm = 2000-tm;
            double w = 1+tm*.002;
            if (!sim.View.Parameters.IsStopped)
                w = 1+2*(frequency-minF)/(maxF-minF);
            for (i = -xl; i <= xl; i++)
            {
                int yy = yc+(int)(.95*Math.Sin(i*pi*w/xl)*wl);
                if (ox != -1)
                    drawThickLine(g, ox, oy, xc+i, yy);
                ox = xc+i;
                oy = yy;
            }
          

            drawPosts(g);
            curcount = updateDotCount(-current, curcount);
            if (sim.dragElm != this)
                drawDots(g, point1, lead1, curcount);
        }

        internal override void stamp()
        {
            sim.stampVoltageSource(0, nodes[0], voltSource);
        }

        private double fadd;
        private double fmul;
        private double freqTime;
        private double savedTimeStep;
        private int dir = 1;

        protected virtual void setParams()
        {
            if (frequency < minF || frequency > maxF)
            {
                frequency = minF;
                freqTime = 0;
                dir = 1;
            }
            if ((flags & FLAG_LOG) == 0)
            {
                fadd = dir*sim.timeStep*(maxF-minF)/sweepTime;
                fmul = 1;
            }
            else
            {
                fadd = 0;
                fmul = Math.Pow(maxF/minF, dir*sim.timeStep/sweepTime);
            }
            savedTimeStep = sim.timeStep;
        }
        internal override void reset()
        {
            frequency = minF;
            freqTime = 0;
            dir = 1;
            setParams();
        }

        private double v;
        internal override void startIteration()
        {
            // has timestep been changed?
            if (sim.timeStep != savedTimeStep)
                setParams();
            v = Math.Sin(freqTime)*maxV;
            freqTime += frequency*2*pi*sim.timeStep;
            frequency = frequency*fmul+fadd;
            if (frequency >= maxF && dir == 1)
            {
                if ((flags & FLAG_BIDIR) != 0)
                {
                    fadd = -fadd;
                    fmul = 1/fmul;
                    dir = -1;
                }
                else
                    frequency = minF;
            }
            if (frequency <= minF && dir == -1)
            {
                fadd = -fadd;
                fmul = 1/fmul;
                dir = 1;
            }
        }
        internal override void doStep()
        {
            sim.updateVoltageSource(0, nodes[0], voltSource, v);
        }

        internal override double VoltageDiff
        {
            get
            {
                return volts[0];
            }
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "свип " + (((flags & FLAG_LOG) == 0) ? "(линейн.)" : "(логарифм.)");
            arr[1] = "I = " + getCurrentDText(Current);
            arr[2] = "V = " + getVoltageText(volts[0]);
            arr[3] = "f = " + getUnitText(frequency, "Гц");
            arr[4] = "диап. = " + getUnitText(minF, "Гц") + " .. " + getUnitText(maxF, "Гц");
            arr[5] = "время = " + getUnitText(sweepTime, "с");
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Мин. частота (Гц)", minF, 0, 0);
            if (n == 1)
                return new EditInfo("макс. частота (Гц)", maxF, 0, 0);
            if (n == 2)
                return new EditInfo("Период свипа (с)", sweepTime, 0, 0);
            if (n == 3)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if ((flags & FLAG_LOG) != 0)
                {
                    ei.checkbox.Text = "Логарифмич.";
                }
                return ei;
            }
            if (n == 4)
                return new EditInfo("Макс. напряжение", maxV, 0, 0);
            if (n == 5)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if ((flags & FLAG_BIDIR) != 0)
                {
                    ei.checkbox.Text = "Двунаправленн.";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            double maxfreq = 1/(8*sim.timeStep);
            if (n == 0)
            {
                minF = ei.value;
                if (minF > maxfreq)
                    minF = maxfreq;
            }
            if (n == 1)
            {
                maxF = ei.value;
                if (maxF > maxfreq)
                    maxF = maxfreq;
            }
            if (n == 2)
                sweepTime = ei.value;
            if (n == 3)
            {
                flags &= ~FLAG_LOG;
                if (ei.checkbox.Checked)
                    flags |= FLAG_LOG;
            }
            if (n == 4)
                maxV = ei.value;
            if (n == 5)
            {
                flags &= ~FLAG_BIDIR;
                if (ei.checkbox.Checked)
                    flags |= FLAG_BIDIR;
            }
            setParams();
        }
    }
}

