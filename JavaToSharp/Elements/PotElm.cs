using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class PotElm : CircuitElm //AdjustmentListener
    {
        private double position;
        private double maxResistance;
        private double resistance1;
        private double resistance2;
        private double current1;
        private double current2;
        private double current3;
        private double curcount1;
        private double curcount2;
        private double curcount3;
        private ScrollBar slider;
        private Label label;
        private string sliderText;
        public PotElm(int xx, int yy) : base(xx, yy)
        {
            setup();
            maxResistance = 1000;
            position =.5;
            sliderText = "Сопротивление";
            createSlider();
        }
        public PotElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sMaxResistance = st.nextToken();
            bool isParsedMaxResistance = double.TryParse(sMaxResistance,out maxResistance);
            if (!isParsedMaxResistance)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sPosition = st.nextToken();
            bool isParsedPosition = double.TryParse(sPosition, out position);
            if(!isParsedPosition)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            sliderText = st.nextToken();
            while (st.hasMoreTokens())
                sliderText += ' ' + st.nextToken();
            createSlider();
        }

        protected virtual void setup()
        {
        }
        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override int DumpType
        {
            get
            {
                return 174;
            }
        }
        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : (n == 1) ? point2 : post3;
        }
        internal override string dump()
        {
            return base.dump() + " " + maxResistance + " " + position + " " + sliderText;
        }

        protected virtual void createSlider()
        {
            //sim.main.Add(label = new Label(sliderText, Label.CENTER));
            int value = (int)(position*100);
            //sim.main.Add(slider = new ScrollBar(ScrollBar.HORIZONTAL, value, 1, 0, 101));
            //sim.main.validate();
            //slider.addAdjustmentListener(this);
        }
       // public virtual void adjustmentValueChanged(AdjustmentEvent e)
      //  {
         //   sim.analyzeFlag = true;
         //   setPoints();
      //  }
        internal override void delete()
        {
        //    sim.main.Remove(label);
        //    sim.main.Remove(slider);
        }

        private Point post3;
        private Point corner2;
        private Point arrowPoint;
        private Point midpoint;
        private Point arrow1;
        private Point arrow2;
        private Point ps3;
        private Point ps4;

        internal override void setPoints()
        {
            base.setPoints();
            int offset = 0;
            if (abs(dx) > abs(dy))
            {
                dx = sim.snapGrid(dx/2)*2;
                point2.X = x2 = point1.X + dx;
                offset = (dx < 0) ? dy : -dy;
                point2.Y = point1.Y;
            }
            else
            {
                dy = sim.snapGrid(dy/2)*2;
                point2.Y = y2 = point1.Y + dy;
                offset = (dy > 0) ? dx : -dx;
                point2.X = point1.X;
            }
            if (offset == 0)
                offset = sim.gridSize;
            dn = distance(point1, point2);
            int bodyLen = 32;
            calcLeads(bodyLen);
            position = slider.Value*.0099+.005;
            int soff = (int)((position-.5)*bodyLen);
            //int offset2 = offset - sign(offset)*4;
            post3 = interpPoint(point1, point2,.5, offset);
            corner2 = interpPoint(point1, point2, soff/dn+.5, offset);
            arrowPoint = interpPoint(point1, point2, soff/dn+.5, 8*sign(offset));
            midpoint = interpPoint(point1, point2, soff/dn+.5);
            arrow1 = new Point();
            arrow2 = new Point();
            double clen = abs(offset)-8;
            interpPoint2(corner2, arrowPoint, arrow1, arrow2, (clen-8)/clen, 8);
            ps3 = new Point();
            ps4 = new Point();
        }

        internal override void draw(Graphics g)
        {
            int segments = 16;
            int i;
            int ox = 0;
            double v1 = volts[0];
            double v2 = volts[1];
            double v3 = volts[2];
            draw2Leads(g);
            double segf = 1.0/segments;
            int divide = (int)(segments*position);
           voltageColor= setVoltageColor(g, v3);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen ,post3, corner2);
            drawThickLine(g, myPen, corner2, arrowPoint);
            drawThickLine(g, myPen, arrow1, arrowPoint);
            drawThickLine(g, myPen, arrow2, arrowPoint);
            curcount1 = updateDotCount(current1, curcount1);
            curcount2 = updateDotCount(current2, curcount2);
            curcount3 = updateDotCount(current3, curcount3);
            if (sim.dragElm != this)
            {
                drawDots(g, point1, midpoint, curcount1);
                drawDots(g, point2, midpoint, curcount2);
                drawDots(g, post3, corner2, curcount3);
                drawDots(g, corner2, midpoint, curcount3+distance(post3, corner2));
            }
            drawPosts(g);
        }

        internal override void calculateCurrent()
        {
            current1 = (volts[0]-volts[2])/resistance1;
            current2 = (volts[1]-volts[2])/resistance2;
            current3 = -current1-current2;
        }
        internal override void stamp()
        {
            resistance1 = maxResistance*position;
            resistance2 = maxResistance*(1-position);
            sim.stampResistor(nodes[0], nodes[2], resistance1);
            sim.stampResistor(nodes[2], nodes[1], resistance2);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "перемен. сопротивление";
            arr[1] = "Vd = " + getVoltageDText(VoltageDiff);
            arr[2] = "R1 = " + getUnitText(resistance1, "Ом");
            arr[3] = "R2 = " + getUnitText(resistance2, "Ом");
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
                EditInfo ei = new EditInfo("Slider Text", 0, -1, -1);
                ei.text = sliderText;
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                maxResistance = ei.value;
            if (n == 1)
            {
                sliderText = ei.textf.Text;
                label.Text = sliderText;
            }
        }
    }
}

