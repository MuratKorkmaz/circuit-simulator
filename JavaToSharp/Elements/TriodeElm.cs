using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class TriodeElm : CircuitElm
    {
        internal double mu, kg1;
        private double curcountp;
        private double curcountc;
        private double curcountg;
        private double currentp;
        private double currentg;
        private double currentc;
        private const double gridCurrentR = 6000;

        public TriodeElm(int xx, int yy) : base(xx, yy)
        {
            mu = 93;
            kg1 = 680;
            setup();
        }
        public TriodeElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sMu = st.nextToken();
            bool isParsedMu = double.TryParse(sMu, out mu);
            if (!isParsedMu)
            {
                throw new Exception("Не удалось привести к типу double");
            }
            string sKg1 = st.nextToken();
            bool isParsedKg1 = double.TryParse(sKg1,out kg1);
            if (!isParsedKg1)
            {
                 throw new Exception("Не удалось привести к типу double");
            }
            setup();
        }

        protected virtual void setup()
        {
            noDiagonal = true;
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void reset()
        {
            volts[0] = volts[1] = volts[2] = 0;
            curcount = 0;
        }
        internal override string dump()
        {
            return base.dump() + " " + mu + " " + kg1;
        }
        internal override int DumpType
        {
            get
            {
                return 173;
            }
        }

        private Point[] plate;
        private Point[] grid;
        private Point[] cath;
        private Point midgrid;
        private Point midcath;
        private int circler;
        internal override void setPoints()
        {
            base.setPoints();
            plate = newPointArray(4);
            grid = newPointArray(8);
            cath = newPointArray(4);
            grid[0] = point1;
            const int nearw = 8;
            interpPoint(point1, point2, plate[1], 1, nearw);
            const int farw = 32;
            interpPoint(point1, point2, plate[0], 1, farw);
            const int platew = 18;
            interpPoint2(point2, plate[1], plate[2], plate[3], 1, platew);

            circler = 24;
            interpPoint(point1, point2, grid[1], (dn-circler)/dn, 0);
            int i;
            for (i = 0; i != 3; i++)
            {
                interpPoint(grid[1], point2, grid[2+i*2], (i*3+1)/4.5, 0);
                interpPoint(grid[1], point2, grid[3+i*2], (i*3+2)/4.5, 0);
            }
            midgrid = point2;

            const int cathw = 16;
            midcath = interpPoint(point1, point2, 1, -nearw);
            interpPoint2(point2, plate[1], cath[1], cath[2], -1, cathw);
            interpPoint(point2, plate[1], cath[3], -1.2, -cathw);
            interpPoint(point2, plate[1], cath[0], -farw/(double) nearw, cathw);
        }

        internal override void draw(Graphics g)
        {
            g.Color = Color.Gray;
            drawThickCircle(g, point2.X, point2.Y, circler);
            setBbox(point1, plate[0], 16);
            adjustBbox(cath[0].X, cath[1].Y, point2.X+circler, point2.Y+circler);
            // draw plate
            voltageColor = setVoltageColor(g, volts[0]);
            myPen = new Pen(voltageColor);
            drawThickLine(g, myPen,plate[0], plate[1]);
            drawThickLine(g, myPen,plate[2], plate[3]);
            // draw grid
            voltageColor = setVoltageColor(g, volts[1]);
            myPen = new Pen(voltageColor);
            int i;
            for (i = 0; i != 8; i += 2)
                drawThickLine(g, myPen,grid[i], grid[i+1]);
            // draw cathode
            setVoltageColor(g, volts[2]);
            for (i = 0; i != 3; i++)
                drawThickLine(g, myPen,cath[i], cath[i+1]);
            // draw dots
            curcountp = updateDotCount(currentp, curcountp);
            curcountc = updateDotCount(currentc, curcountc);
            curcountg = updateDotCount(currentg, curcountg);
            if (sim.dragElm != this)
            {
                drawDots(g, plate[0], midgrid, curcountp);
                drawDots(g, midgrid, midcath, curcountc);
                drawDots(g, midcath, cath[1], curcountc+8);
                drawDots(g, cath[1], cath[0], curcountc+8);
                drawDots(g, point1, midgrid, curcountg);
            }
            drawPosts(g);
        }
        internal override Point getPost(int n)
        {
            return (n == 0) ? plate[0] : (n == 1) ? grid[0] : cath[0];
        }
        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override double Power
        {
            get
            {
                return (volts[0]-volts[2])*current;
            }
        }

        private double lastv0;
        private double lastv1;
        private double lastv2;

        internal override void doStep()
        {
            var vs = new double[3];
            vs[0] = volts[0];
            vs[1] = volts[1];
            vs[2] = volts[2];
            if (vs[1] > lastv1 +.5)
                vs[1] = lastv1 +.5;
            if (vs[1] < lastv1 -.5)
                vs[1] = lastv1 -.5;
            if (vs[2] > lastv2 +.5)
                vs[2] = lastv2 +.5;
            if (vs[2] < lastv2 -.5)
                vs[2] = lastv2 -.5;
            int grid = 1;
            int cath = 2;
            int plate = 0;
            double vgk = vs[grid] -vs[cath];
            double vpk = vs[plate]-vs[cath];
            if (Math.Abs(lastv0-vs[0]) >.01 || Math.Abs(lastv1-vs[1]) >.01 || Math.Abs(lastv2-vs[2]) >.01)
                sim.converged = false;
            lastv0 = vs[0];
            lastv1 = vs[1];
            lastv2 = vs[2];
            double ids = 0;
            double gm = 0;
            double Gds = 0;
            double ival = vgk+vpk/mu;
            currentg = 0;
            if (vgk >.01)
            {
                sim.stampResistor(nodes[grid], nodes[cath], gridCurrentR);
                currentg = vgk/gridCurrentR;
            }
            if (ival < 0)
            {
                // should be all zero, but that causes a singular matrix,
                // so instead we treat it as a large resistor
                Gds = 1e-8;
                ids = vpk*Gds;
            }
            else
            {
                ids = Math.Pow(ival, 1.5)/kg1;
                double q = 1.5*Math.Sqrt(ival)/kg1;
                // gm = dids/dgk;
                // Gds = dids/dpk;
                Gds = q;
                gm = q/mu;
            }
            currentp = ids;
            currentc = ids+currentg;
            double rs = -ids + Gds*vpk + gm*vgk;
            sim.stampMatrix(nodes[plate], nodes[plate], Gds);
            sim.stampMatrix(nodes[plate], nodes[cath], -Gds-gm);
            sim.stampMatrix(nodes[plate], nodes[grid], gm);

            sim.stampMatrix(nodes[cath], nodes[plate], -Gds);
            sim.stampMatrix(nodes[cath], nodes[cath], Gds+gm);
            sim.stampMatrix(nodes[cath], nodes[grid], -gm);

            sim.stampRightSide(nodes[plate], rs);
            sim.stampRightSide(nodes[cath], -rs);
        }

        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
            sim.stampNonLinear(nodes[2]);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "триод";
            double vbc = volts[0]-volts[1];
            double vbe = volts[0]-volts[2];
            double vce = volts[1]-volts[2];
            arr[1] = "Vсэ = " + getVoltageText(vbe);
            arr[2] = "Vск = " + getVoltageText(vbc);
            arr[3] = "Vак = " + getVoltageText(vce);
        }
        // grid not connected to other terminals
        internal override bool getConnection(int n1, int n2)
        {
            return !(n1 == 1 || n2 == 1);
        }
    }
}

