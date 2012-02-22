// stub PhotoResistorElm based on SparkGapElm
// FIXME need to uncomment PhotoResistorElm line from CirSim.java
// FIXME need to add PhotoResistorElm.java to srclist


using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class PhotoResistorElm : CircuitElm
    {
        private double minresistance;
        private double maxresistance;
        private double resistance;
        private Scrollbar slider;
        private Label label;
        public PhotoResistorElm(int xx, int yy) : base(xx, yy)
        {
            maxresistance = 1e9;
            minresistance = 1e3;
            createSlider();
        }
        public PhotoResistorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sMinresistance = st.nextToken();
            bool isParsedMinresistance = double.TryParse(sMinresistance, out minresistance);
            if (!isParsedMinresistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sMaxresistance = st.nextToken();
            bool isParsedMaxresistance = double.TryParse(sMaxresistance, out maxresistance);
            if (!isParsedMaxresistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            createSlider();
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override int DumpType
        {
            get
            {
                return 186;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + minresistance + " " + maxresistance;
        }

        private Point ps3;
        private Point ps4;

        protected virtual void createSlider()
        {
            sim.main.Add(label = new Label("Уровень света", Label.CENTER));
            int value = 50;
            sim.main.Add(slider = new Scrollbar(Scrollbar.HORIZONTAL, value, 1, 0, 101));
            sim.main.validate();
        }
        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(32);
            ps3 = new Point();
            ps4 = new Point();
        }
        internal override void delete()
        {
            sim.main.Remove(label);
            sim.main.Remove(slider);
        }

        internal override void draw(Graphics g)
        {
            int i;
            double v1 = volts[0];
            double v2 = volts[1];
            setBbox(point1, point2, 6);
            draw2Leads(g);
            // FIXME need to draw properly, see ResistorElm.java
            setPowerColor(g, true);
            doDots(g);
            drawPosts(g);
        }

        protected override void calculateCurrent()
        {
            double vd = volts[0] - volts[1];
            current = vd/resistance;
        }
        internal override void startIteration()
        {
            double vd = volts[0] - volts[1];
            // FIXME set resistance as appropriate, using slider.getValue()
            resistance = minresistance;
            //System.out.print(this + " res current set to " + current + "\n");
        }
        internal override void doStep()
        {
            sim.stampResistor(nodes[0], nodes[1], resistance);
        }
        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
        internal override void getInfo(string[] arr)
        {
            // FIXME
            arr[0] = "spark gap";
            getBasicInfo(arr);
            arr[3] = "R = " + getUnitText(resistance, sim.ohmString);
            arr[4] = "Ron = " + getUnitText(minresistance, sim.ohmString);
            arr[5] = "Roff = " + getUnitText(maxresistance, sim.ohmString);
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Минимальное сопртивление (Ом)", minresistance, 0, 0);
            if (n == 1)
                return new EditInfo("Максимальное сопротивление (Ом)", maxresistance, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (ei.value > 0 && n == 0)
                minresistance = ei.value;
            if (ei.value > 0 && n == 1)
                maxresistance = ei.value;
        }
    }
}

