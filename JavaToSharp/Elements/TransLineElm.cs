using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class TransLineElm : CircuitElm
    {
        private double delay;
        private double imped;
        private double[] voltageL;
        private double[] voltageR;
        private int lenSteps;
        private int ptr;
        private int width;

        public TransLineElm(int xx, int yy) : base(xx, yy)
        {
            delay = 1000*sim.timeStep;
            imped = 75;
            noDiagonal = true;
            reset();
        }
        public TransLineElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sDelay = st.nextToken();
            bool isParsedDelay = double.TryParse(sDelay, out delay);
            if(!isParsedDelay)
            {
                throw new Exception("Не удалось привести к типу double");
            }

            string sImpred = st.nextToken();
            bool isParsedImpred = double.TryParse(sImpred, out imped);
            if (!isParsedImpred)
            {
                 throw new Exception("Не удалось привести к типу double");
            }
            string sWidth = st.nextToken();
            bool isParsedWidth = int.TryParse(sWidth,out width);
            if(!isParsedWidth)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            // next slot is for resistance (losses), which is not implemented
            st.nextToken();
            noDiagonal = true;
            reset();
        }
        internal override int DumpType
        {
            get
            {
                return 171;
            }
        }
        internal override int PostCount
        {
            get
            {
                return 4;
            }
        }
        internal override int InternalNodeCount
        {
            get
            {
                return 2;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + delay + " " + imped + " " + width + " " + 0.0;
        }
        internal override void drag(int xx, int yy)
        {
            xx = sim.snapGrid(xx);
            yy = sim.snapGrid(yy);
            int w1 = max(sim.gridSize, abs(yy-y));
            int w2 = max(sim.gridSize, abs(xx-x));
            if (w1 > w2)
            {
                xx = x;
                width = w2;
            }
            else
            {
                yy = y;
                width = w1;
            }
            x2 = xx;
            y2 = yy;
            setPoints();
        }

        private Point[] posts;
        private Point[] inner;

        internal override void reset()
        {
            if (Math.Abs(sim.timeStep - 0) < double.Epsilon)
                return;
            lenSteps = (int)(delay/sim.timeStep);
            Console.WriteLine(lenSteps + " steps");
            if (lenSteps > 100000)
                voltageL = voltageR = null;
            else
            {
                voltageL = new double[lenSteps];
                voltageR = new double[lenSteps];
            }
            ptr = 0;
            base.reset();
        }
        internal override void setPoints()
        {
            base.setPoints();
            int ds = (dy == 0) ? sign(dx) : -sign(dy);
            Point p3 = interpPoint(point1, point2, 0, -width*ds);
            Point p4 = interpPoint(point1, point2, 1, -width*ds);
            int sep = sim.gridSize/2;
            Point p5 = interpPoint(point1, point2, 0, -(width/2-sep)*ds);
            Point p6 = interpPoint(point1, point2, 1, -(width/2-sep)*ds);
            Point p7 = interpPoint(point1, point2, 0, -(width/2+sep)*ds);
            Point p8 = interpPoint(point1, point2, 1, -(width/2+sep)*ds);

            // we number the posts like this because we want the lower-numbered
            // points to be on the bottom, so that if some of them are unconnected
            // (which is often true) then the bottom ones will get automatically
            // attached to ground.
            posts = new[] { p3, p4, point1, point2 };
            inner = new[] { p7, p8, p5, p6 };
        }
        internal override void draw(Graphics g)
        {
            setBbox(posts[0], posts[3], 0);
            int segments = (int)(dn/2);
            int ix0 = ptr-1+lenSteps;
            double segf = 1.0/segments;
            int i;
            g.GetNearestColor(Color.DarkGray);
            myBrush = new SolidBrush(Color.DarkGray);
            g.FillRectangle(myBrush,inner[2].X, inner[2].Y, inner[1].X-inner[2].X+2, inner[1].Y-inner[2].Y+2);
            for (i = 0; i != 4; i++)
            {
               voltageColor = setVoltageColor(g, volts[i]);
                myPen = new Pen(voltageColor);
                drawThickLine(g, myPen,posts[i], inner[i]);
            }
            if (voltageL != null)
            {
                for (i = 0; i != segments; i++)
                {
                    int ix1 = (ix0-lenSteps*i/segments) % lenSteps;
                    int ix2 = (ix0-lenSteps*(segments-1-i)/segments) % lenSteps;
                    double v = (voltageL[ix1]+voltageR[ix2])/2;
                    voltageColor =   setVoltageColor(g, v);
                    myPen = new Pen(voltageColor);
                    interpPoint(inner[0], inner[1], ps1, i*segf);
                    interpPoint(inner[2], inner[3], ps2, i*segf);
                    g.DrawLine(myPen,ps1.Y, ps1.Y, ps2.X, ps2.Y);
                    interpPoint(inner[2], inner[3], ps1, (i+1)*segf);
                    drawThickLine(g, myPen,ps1, ps2);
                }
            }
            voltageColor =   setVoltageColor(g, volts[0]);
            drawThickLine(g, myPen ,inner[0], inner[1]);
            drawPosts(g);

            curCount1 = updateDotCount(-current1, curCount1);
            curCount2 = updateDotCount(current2, curCount2);
            if (sim.dragElm != this)
            {
                drawDots(g, posts[0], inner[0], curCount1);
                drawDots(g, posts[2], inner[2], -curCount1);
                drawDots(g, posts[1], inner[1], -curCount2);
                drawDots(g, posts[3], inner[3], curCount2);
            }
        }

        private int voltSource1;
        private int voltSource2;
        private double current1;
        private double current2;
        private double curCount1;
        private double curCount2;
        internal override void setVoltageSource(int n, int v)
        {
            if (n == 0)
                voltSource1 = v;
            else
                voltSource2 = v;
        }
        internal override void setCurrent(int v, double c)
        {
            if (v == voltSource1)
                current1 = c;
            else
                current2 = c;
        }

        internal override void stamp()
        {
            sim.stampVoltageSource(nodes[4], nodes[0], voltSource1);
            sim.stampVoltageSource(nodes[5], nodes[1], voltSource2);
            sim.stampResistor(nodes[2], nodes[4], imped);
            sim.stampResistor(nodes[3], nodes[5], imped);
        }

        internal override void startIteration()
        {
            // calculate voltages, currents sent over wire
            if (voltageL == null)
            {
                sim.stop("Задержка в линии передачи слишком большая!", this);
                return;
            }
            voltageL[ptr] = volts[2]-volts[0] + volts[2]-volts[4];
            voltageR[ptr] = volts[3]-volts[1] + volts[3]-volts[5];
            //System.out.println(volts[2] + " " + volts[0] + " " + (volts[2]-volts[0]) + " " + (imped*current1) + " " + voltageL[ptr]);
//	System.out.println("sending fwd  " + currentL[ptr] + " " + current1);
//	  System.out.println("sending back " + currentR[ptr] + " " + current2);
            //System.out.println("sending back " + voltageR[ptr]);
            ptr = (ptr+1) % lenSteps;
        }
        internal override void doStep()
        {
            if (voltageL == null)
            {
                sim.stop("Задержка в линии передачи слишком большая!", this);
                return;
            }
            sim.updateVoltageSource(nodes[4], nodes[0], voltSource1, -voltageR[ptr]);
            sim.updateVoltageSource(nodes[5], nodes[1], voltSource2, -voltageL[ptr]);
            if (Math.Abs(volts[0]) > 1e-5 || Math.Abs(volts[1]) > 1e-5)
            {
                sim.stop("Необходимо заземлить линию передачи!", this);
            }
        }

        internal override Point getPost(int n)
        {
            return posts[n];
        }

        //double getVoltageDiff() { return volts[0]; }
        internal override int VoltageSourceCount
        {
            get
            {
                return 2;
            }
        }
        internal override bool hasGroundConnection(int n1)
        {
            return false;
        }
        internal override bool getConnection(int n1, int n2)
        {
            return false;
//	if (comparePair(n1, n2, 0, 1))
//	  return true;
//	  if (comparePair(n1, n2, 2, 3))
//	  return true;
//	  return false;
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "линия передачи";
            arr[1] = getUnitText(imped, "Ом");
            arr[2] = "длинна = " + getUnitText(2.9979e8*delay, "м");
            arr[3] = "задержка = " + getUnitText(delay, "с");
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Задержка (с)", delay, 0, 0);
            if (n == 1)
                return new EditInfo("Импенданс (Ом)", imped, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                delay = ei.value;
                reset();
            }
            if (n == 1)
            {
                imped = ei.value;
                reset();
            }
        }
    }
}

