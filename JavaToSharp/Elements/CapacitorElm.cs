using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class CapacitorElm : CircuitElm
    {
        internal double capacitance;
        internal double compResistance, voltdiff;
        internal Point[] plate1; internal Point[] plate2;
        public const int FLAG_BACK_EULER = 2;
        public CapacitorElm(int xx, int yy) : base(xx, yy)
        {
            capacitance = 1e-5;
        }
        public CapacitorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sCapacitance = st.nextToken();
            bool isParsedCapacitance = double.TryParse(sCapacitance, out capacitance);
            if (!isParsedCapacitance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sVoltdiff = st.nextToken();
            bool isParsedVoltdiff = double.TryParse(sVoltdiff ,out voltdiff);
            if (!isParsedVoltdiff)
            {
                throw new Exception("Не удалось привести к типу double");
            }
        }
        internal virtual bool isTrapezoidal
        {
            get
            {
                return (flags & FLAG_BACK_EULER) == 0;
            }
        }
        internal override void setNodeVoltage(int n, double c)
        {
            base.setNodeVoltage(n, c);
            voltdiff = volts[0]-volts[1];
        }
        internal override void reset()
        {
            current = curcount = 0;
            // put small charge on caps when reset to start oscillators
            voltdiff = 1e-3;
        }
        internal override int DumpType
        {
            get
            {
                return 'c';
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + capacitance + " " + voltdiff;
        }
        internal override void setPoints()
        {
            base.setPoints();
            double f = (dn/2-4)/dn;
            // calc leads
            lead1 = interpPoint(point1, point2, f);
            lead2 = interpPoint(point1, point2, 1-f);
            // calc plates
            plate1 = newPointArray(2);
            plate2 = newPointArray(2);
            interpPoint2(point1, point2, plate1[0], plate1[1], f, 12);
            interpPoint2(point1, point2, plate2[0], plate2[1], 1-f, 12);
        }

        internal override void draw(Graphics g)
        {
            int hs = 12;
            setBbox(point1, point2, hs);

            // draw first lead and plate
            voltageColor = setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen ,point1, lead1);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen,plate1[0], plate1[1]);
           // if (sim.powerCheckItem.State)
             //   g.Color = Color.Gray;

            // draw second lead and plate
            voltageColor = setVoltageColor(g, volts[1]);
            myPen = new Pen(voltageColor);
            drawThickLine(g,myPen, point2, lead2);
           myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,plate2[0], plate2[1]);

            updateDotCount();
            if (sim.dragElm != this)
            {
                drawDots(g, point1, lead1, curcount);
                drawDots(g, point2, lead2, -curcount);
            }
            drawPosts(g);
           
        }
        internal override void stamp()
        {
            // capacitor companion model using trapezoidal approximation
            // (Norton equivalent) consists of a current source in
            // parallel with a resistor.  Trapezoidal is more accurate
            // than backward euler but can cause oscillatory behavior
            // if RC is small relative to the timestep.
            if (isTrapezoidal)
                compResistance = sim.timeStep/(2*capacitance);
            else
                compResistance = sim.timeStep/capacitance;
            sim.stampResistor(nodes[0], nodes[1], compResistance);
            sim.stampRightSide(nodes[0]);
            sim.stampRightSide(nodes[1]);
        }
        internal override void startIteration()
        {
            if (isTrapezoidal)
                curSourceValue = -voltdiff/compResistance-current;
            else
                curSourceValue = -voltdiff/compResistance;
            //System.out.println("cap " + compResistance + " " + curSourceValue + " " + current + " " + voltdiff);
        }

        internal override void calculateCurrent()
        {
            double voltdiff = volts[0] - volts[1];
            // we check compResistance because this might get called
            // before stamp(), which sets compResistance, causing
            // infinite current
            if (compResistance > 0)
                current = voltdiff/compResistance + curSourceValue;
        }
        internal double curSourceValue;
        internal override void doStep()
        {
            sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "конденсатор";
            getBasicInfo(arr);
            arr[3] = "C = " + getUnitText(capacitance, "Ф");
            arr[4] = "P = " + getUnitText(Power, "Вт");
            //double v = getVoltageDiff();
            //arr[4] = "U = " + getUnitText(.5*capacitance*v*v, "J");
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Емкость (Ф)", capacitance, 0, 0);
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                ei.checkbox.Text = "Трапец. апроксимация";
                if(isTrapezoidal)
                {
                    return ei;
                }

                
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0 && ei.value > 0)
                capacitance = ei.value;
            if (n == 1)
            {
                if (ei.checkbox.Checked)
                    flags &= ~FLAG_BACK_EULER;
                else
                    flags |= FLAG_BACK_EULER;
            }
        }
        internal override bool needsShortcut()
        {
            return true;
        }
    }
}
