using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class TransformerElm : CircuitElm
    {
        private double inductance;
        private double ratio;
        private double couplingCoef;
        private Point[] ptEnds;
        private Point[] ptCoil;
        private Point[] ptCore;
        private new readonly double[] current;
        private new readonly double[] curcount;
        private int width;
        private const int FLAG_BACK_EULER = 2;

        public TransformerElm(int xx, int yy) : base(xx, yy)
        {
            inductance = 4;
            ratio = 1;
            width = 32;
            noDiagonal = true;
            couplingCoef =.999;
            current = new double[2];
            curcount = new double[2];
        }

        public TransformerElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            width = max(32, abs(yb-ya));
            string sInductance = st.nextToken();
            bool isParsedInductance = double.TryParse(sInductance, out inductance);
            if(!isParsedInductance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sRatio = st.nextToken();
            bool isParsedRatio = double.TryParse(sRatio, out ratio);
            if(!isParsedRatio)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            current = new double[2];
            curcount = new double[2];
            string sCurrent = st.nextToken();
            bool isParsedCurrent = double.TryParse(sCurrent, out current[0]);
            if (!isParsedCurrent )
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sCurrent1 = st.nextToken();
            bool isParsedCurrent1 = double.TryParse(sCurrent1,out current[1]);
            if(!isParsedCurrent1)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            couplingCoef =.999;
            try
            {
                string sCouplingCoef = st.nextToken();
                bool isParsedCouplingCoef = double.TryParse(sCouplingCoef , out couplingCoef );
                if (!isParsedCouplingCoef)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
            }
            catch (Exception e)
            {
            }
            noDiagonal = true;
        }
        internal override void drag(int xx, int yy)
        {
            xx = sim.snapGrid(xx);
            yy = sim.snapGrid(yy);
            width = max(32, abs(yy-y));
            if (xx == x)
                yy = y;
            x2 = xx;
            y2 = yy;
            setPoints();
        }
        internal override int DumpType
        {
            get
            {
                return 'T';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + inductance + " " + ratio + " " + current[0] + " " + current[1] + " " + couplingCoef;
        }
        internal virtual bool isTrapezoidal
        {
            get
            {
                return (flags & FLAG_BACK_EULER) == 0;
            }
        }
        internal override void draw(Graphics g)
        {
            int i;
            for (i = 0; i != 4; i++)
            {
                voltageColor =  setVoltageColor(g, volts[i]);
                myPen = new Pen(voltageColor);
                drawThickLine(g, myPen,ptEnds[i], ptCoil[i]);
            }
            for (i = 0; i != 2; i++)
            {
                drawCoil(g, dsign*(i == 1 ? -6 : 6), ptCoil[i], ptCoil[i+2], volts[i], volts[i+2]);
            }
            g.Color = needsHighlight() ? selectColor : lightGrayColor;
            for (i = 0; i != 2; i++)
            {
                drawThickLine(g, myPen,ptCore[i], ptCore[i+2]);
                curcount[i] = updateDotCount(current[i], curcount[i]);
            }
            for (i = 0; i != 2; i++)
            {
                drawDots(g, ptEnds[i], ptCoil[i], curcount[i]);
                drawDots(g, ptCoil[i], ptCoil[i+2], curcount[i]);
                drawDots(g, ptEnds[i+2], ptCoil[i+2], -curcount[i]);
            }

            drawPosts(g);
            setBbox(ptEnds[0], ptEnds[3], 0);
        }

        internal override void setPoints()
        {
            base.setPoints();
            point2.Y = point1.Y;
            ptEnds = newPointArray(4);
            ptCoil = newPointArray(4);
            ptCore = newPointArray(4);
            ptEnds[0] = point1;
            ptEnds[1] = point2;
            interpPoint(point1, point2, ptEnds[2], 0, -dsign*width);
            interpPoint(point1, point2, ptEnds[3], 1, -dsign*width);
            double ce =.5-12/dn;
            double cd =.5-2/dn;
            int i;
            for (i = 0; i != 4; i += 2)
            {
                interpPoint(ptEnds[i], ptEnds[i+1], ptCoil[i], ce);
                interpPoint(ptEnds[i], ptEnds[i+1], ptCoil[i+1], 1-ce);
                interpPoint(ptEnds[i], ptEnds[i+1], ptCore[i], cd);
                interpPoint(ptEnds[i], ptEnds[i+1], ptCore[i+1], 1-cd);
            }
        }
        internal override Point getPost(int n)
        {
            return ptEnds[n];
        }
        internal override int PostCount
        {
            get
            {
                return 4;
            }
        }
        internal override void reset()
        {
            current[0] = current[1] = volts[0] = volts[1] = volts[2] = volts[3] = curcount[0] = curcount[1] = 0;
        }

        private double a1;
        private double a2;
        private double a3;
        private double a4;

        internal override void stamp()
        {
            // equations for transformer:
            //   v1 = L1 di1/dt + M  di2/dt
            //   v2 = M  di1/dt + L2 di2/dt
            // we invert that to get:
            //   di1/dt = a1 v1 + a2 v2
            //   di2/dt = a3 v1 + a4 v2
            // integrate di1/dt using trapezoidal approx and we get:
            //   i1(t2) = i1(t1) + dt/2 (i1(t1) + i1(t2))
            //          = i1(t1) + a1 dt/2 v1(t1) + a2 dt/2 v2(t1) +
            //                     a1 dt/2 v1(t2) + a2 dt/2 v2(t2)
            // the norton equivalent of this for i1 is:
            //  a. current source, I = i1(t1) + a1 dt/2 v1(t1) + a2 dt/2 v2(t1)
            //  b. resistor, G = a1 dt/2
            //  c. current source controlled by voltage v2, G = a2 dt/2
            // and for i2:
            //  a. current source, I = i2(t1) + a3 dt/2 v1(t1) + a4 dt/2 v2(t1)
            //  b. resistor, G = a3 dt/2
            //  c. current source controlled by voltage v2, G = a4 dt/2
            //
            // For backward euler,
            //
            //   i1(t2) = i1(t1) + a1 dt v1(t2) + a2 dt v2(t2)
            //
            // So the current source value is just i1(t1) and we use
            // dt instead of dt/2 for the resistor and VCCS.
            //
            // first winding goes from node 0 to 2, second is from 1 to 3
            double l1 = inductance;
            double l2 = inductance*ratio*ratio;
            double m = couplingCoef*Math.Sqrt(l1*l2);
            // build inverted matrix
            double deti = 1/(l1*l2-m*m);
            double ts = Trapezoidal ? sim.timeStep/2 : sim.timeStep;
            a1 = l2*deti*ts; // we multiply dt/2 into a1..a4 here
            a2 = -m*deti*ts;
            a3 = -m*deti*ts;
            a4 = l1*deti*ts;
            sim.stampConductance(nodes[0], nodes[2], a1);
            sim.stampVCCurrentSource(nodes[0], nodes[2], nodes[1], nodes[3], a2);
            sim.stampVCCurrentSource(nodes[1], nodes[3], nodes[0], nodes[2], a3);
            sim.stampConductance(nodes[1], nodes[3], a4);
            sim.stampRightSide(nodes[0]);
            sim.stampRightSide(nodes[1]);
            sim.stampRightSide(nodes[2]);
            sim.stampRightSide(nodes[3]);
        }
        internal override void startIteration()
        {
            double voltdiff1 = volts[0]-volts[2];
            double voltdiff2 = volts[1]-volts[3];
            if (Trapezoidal)
            {
                curSourceValue1 = voltdiff1*a1+voltdiff2*a2+current[0];
                curSourceValue2 = voltdiff1*a3+voltdiff2*a4+current[1];
            }
            else
            {
                curSourceValue1 = current[0];
                curSourceValue2 = current[1];
            }
        }

        private double curSourceValue1;
        private double curSourceValue2;

        internal override void doStep()
        {
            sim.stampCurrentSource(nodes[0], nodes[2], curSourceValue1);
            sim.stampCurrentSource(nodes[1], nodes[3], curSourceValue2);
        }
        internal override void calculateCurrent()
        {
            double voltdiff1 = volts[0]-volts[2];
            double voltdiff2 = volts[1]-volts[3];
            current[0] = voltdiff1*a1 + voltdiff2*a2 + curSourceValue1;
            current[1] = voltdiff1*a3 + voltdiff2*a4 + curSourceValue2;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "трансформатор";
            arr[1] = "L = " + getUnitText(inductance, "Гн");
            arr[2] = "трансформац. = 1:" + ratio;
            arr[3] = "Vd1 = " + getVoltageText(volts[0]-volts[2]);
            arr[4] = "Vd2 = " + getVoltageText(volts[1]-volts[3]);
            arr[5] = "I1 = " + getCurrentText(current[0]);
            arr[6] = "I2 = " + getCurrentText(current[1]);
        }
        internal override bool getConnection(int n1, int n2)
        {
            if (comparePair(n1, n2, 0, 2))
                return true;
            if (comparePair(n1, n2, 1, 3))
                return true;
            return false;
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Индуктивность первичной обмотки (Гн)", inductance,.01, 5);
            if (n == 1)
                return new EditInfo("Коэффициент трансформации", ratio, 1, 10).setDimensionless();
            if (n == 2)
                return new EditInfo("Коэффициент связи", couplingCoef, 0, 1).setDimensionless();
            if (n == 3)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if (Trapezoidal)
                {
                    ei.checkbox.Text = "трапецив. апроксимация";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                inductance = ei.value;
            if (n == 1)
                ratio = ei.value;
            if (n == 2 && ei.value > 0 && ei.value < 1)
                couplingCoef = ei.value;
            if (n == 3)
            {
                if (ei.checkbox.Checked)
                    flags &= ~Inductor.FLAG_BACK_EULER;
                else
                    flags |= Inductor.FLAG_BACK_EULER;
            }
        }

        public bool Trapezoidal { get; set; }
    }
}
