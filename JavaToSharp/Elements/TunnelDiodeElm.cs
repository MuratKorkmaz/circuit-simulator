using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class TunnelDiodeElm : CircuitElm
    {
        public TunnelDiodeElm(int xx, int yy) : base(xx, yy)
        {
            setup();
        }
        public TunnelDiodeElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            setup();
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal virtual void setup()
        {
        }
        internal override int DumpType
        {
            get
            {
                return 175;
            }
        }

        private const int hs = 8;
        private Polygon poly;
        private Point[] cathode;

        internal override void setPoints()
        {
            base.setPoints();
            calcLeads(16);
            cathode = newPointArray(4);
            Point[] pa = newPointArray(2);
            interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
            interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
            interpPoint2(lead1, lead2, cathode[2], cathode[3],.8, hs);
            poly = createPolygon(pa[0], pa[1], lead2);
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, hs);

            double v1 = volts[0];
            double v2 = volts[1];

            draw2Leads(g);

            // draw arrow thingy
            setVoltageColor(g, v1);
            g.fillPolygon(poly);

            // draw thing arrow is pointing to
            voltageColor= setVoltageColor(g, v2);
            myPen = new Pen(voltageColor); 
            drawThickLine(g, myPen,cathode[0], cathode[1]);
            drawThickLine(g, myPen,cathode[2], cathode[0]);
            drawThickLine(g, myPen,cathode[3], cathode[1]);

            doDots(g);
            drawPosts(g);
        }

        internal override void reset()
        {
            lastvoltdiff = volts[0] = volts[1] = curcount = 0;
        }

        private double lastvoltdiff;

        protected virtual double limitStep(double vnew, double vold)
        {
            // Prevent voltage changes of more than 1V when iterating.  Wow, I thought it would be
            // much harder than this to prevent convergence problems.
            if (vnew > vold+1)
                return vold+1;
            if (vnew < vold-1)
                return vold-1;
            return vnew;
        }
        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }

        private const double pvp =.1;
        private const double pip = 4.7e-3;
        private const double pvv =.37;
        private const double pvt =.026;
        private const double pvpp =.525;
        private const double piv = 370e-6;
        internal override void doStep()
        {
            double voltdiff = volts[0] - volts[1];
            if (Math.Abs(voltdiff-lastvoltdiff) >.01)
                sim.converged = false;
            //System.out.println(voltdiff + " " + lastvoltdiff + " " + Math.abs(voltdiff-lastvoltdiff));
            voltdiff = limitStep(voltdiff, lastvoltdiff);
            lastvoltdiff = voltdiff;

            double i = pip*Math.Exp(-pvpp/pvt)*(Math.Exp(voltdiff/pvt)-1) + pip*(voltdiff/pvp)*Math.Exp(1-voltdiff/pvp) + piv*Math.Exp(voltdiff-pvv);

            double geq = pip*Math.Exp(-pvpp/pvt)*Math.Exp(voltdiff/pvt)/pvt + pip*Math.Exp(1-voltdiff/pvp)/pvp - Math.Exp(1-voltdiff/pvp)*pip*voltdiff/(pvp*pvp) + Math.Exp(voltdiff-pvv)*piv;
            double nc = i - geq*voltdiff;
            sim.stampConductance(nodes[0], nodes[1], geq);
            sim.stampCurrentSource(nodes[0], nodes[1], nc);
        }
        internal override void calculateCurrent()
        {
            double voltdiff = volts[0] - volts[1];
            current = pip*Math.Exp(-pvpp/pvt)*(Math.Exp(voltdiff/pvt)-1) + pip*(voltdiff/pvp)*Math.Exp(1-voltdiff/pvp) + piv*Math.Exp(voltdiff-pvv);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "туннельный диод";
            arr[1] = "I = " + getCurrentText(Current);
            arr[2] = "Vd = " + getVoltageText(VoltageDiff);
            arr[3] = "P = " + getUnitText(Power, "Вт");
        }
    }
}
