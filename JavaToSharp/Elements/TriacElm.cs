using System;
using System.Drawing;

// stub implementation of TriacElm, based on SCRElm
// FIXME need to add TriacElm to srclist
// FIXME need to uncomment TriacElm line from CirSim.java


// Silicon-Controlled Rectifier
// 3 nodes, 1 internal node
// 0 = anode, 1 = cathode, 2 = gate
// 0, 3 = variable resistor
// 3, 2 = diode
// 2, 1 = 50 ohm resistor

namespace JavaToSharp
{
    internal class TriacElm : CircuitElm
    {
        private const int anode = 0;
        private const int cnode = 1;
        private const int gnode = 2;
        private const int inode = 3;
        private Diode diode;
        public TriacElm(int xx, int yy) : base(xx, yy)
        {
            setDefaults();
            setup();
        }
        public TriacElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            setDefaults();
            try
            {
                string sLastVac = st.nextToken();
                bool isParsedLastVac = double.TryParse(sLastVac,out lastvac);
                if(!isParsedLastVac)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                string sLastvag = st.nextToken();
                bool isParsedLastvag = double.TryParse(sLastvag, out lastvag);
                if(!isParsedLastvag)
                {
                     throw new Exception("Не удалось привести к типу double");
                }
                volts[anode] = 0;
                volts[cnode] = -lastvac;
                volts[gnode] = -lastvag;
                string sTriggerI = (st.nextToken());
                bool isParsedTriggerI = double.TryParse(sTriggerI,out triggerI);
                if (!isParsedTriggerI)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                string sHoldingI = (st.nextToken());
               bool isParsedHoldingI = double.TryParse(sHoldingI, out holdingI);
                if (!isParsedHoldingI)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                string sCresistance = (st.nextToken());
               bool isParsedCresistance = double.TryParse(sCresistance,out cresistance);
                if (!isParsedCresistance)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
            }
            catch (Exception e)
            {
            }
            setup();
        }

        protected virtual void setDefaults()
        {
            cresistance = 50;
            holdingI =.0082;
            triggerI =.01;
        }

        protected virtual void setup()
        {
            diode = new Diode(sim);
            diode.setup(.8, 0);
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void reset()
        {
            volts[anode] = volts[cnode] = volts[gnode] = 0;
            diode.reset();
            lastvag = lastvac = curcount_a = curcount_c = curcount_g = 0;
        }
        internal override int DumpType
        {
            get
            {
                return 183;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + (volts[anode]-volts[cnode]) + " " + (volts[anode]-volts[gnode]) + " " + triggerI + " "+ holdingI + " " + cresistance;
        }

        private double ia;
        private double ic;
        private double ig;
        private double curcount_a;
        private double curcount_c;
        private double curcount_g;
        private double lastvac;
        private double lastvag;
        private double cresistance;
        private double triggerI;
        private double holdingI;

        private const int hs = 8;
        private Polygon poly;
        private Point[] cathode;
        private Point[] gate;

        internal override void setPoints()
        {
            base.setPoints();
            int dir = 0;
            if (abs(dx) > abs(dy))
            {
                dir = -sign(dx)*sign(dy);
                point2.Y = point1.Y;
            }
            else
            {
                dir = sign(dy)*sign(dx);
                point2.X = point1.X;
            }
            if (dir == 0)
                dir = 1;
            calcLeads(16);
            cathode = newPointArray(2);
            Point[] pa = newPointArray(2);
            interpPoint2(lead1, lead2, pa[0], pa[1], 0, hs);
            interpPoint2(lead1, lead2, cathode[0], cathode[1], 1, hs);
            poly = createPolygon(pa[0], pa[1], lead2);

            gate = newPointArray(2);
            double leadlen = (dn-16)/2;
            int gatelen = sim.gridSize;
            gatelen +=(int) leadlen % sim.gridSize;
            if (leadlen < gatelen)
            {
                x2 = x;
                y2 = y;
                return;
            }
            interpPoint(lead2, point2, gate[0], gatelen/leadlen, gatelen*dir);
            interpPoint(lead2, point2, gate[1], gatelen/leadlen, sim.gridSize*2*dir);
        }

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, hs);
            adjustBbox(gate[0], gate[1]);

            double v1 = volts[anode];
            double v2 = volts[cnode];

            draw2Leads(g);

            // draw arrow thingy
            setVoltageColor(g, v1);
            g.fillPolygon(poly);

            // draw thing arrow is pointing to
            voltageColor = setVoltageColor(g, v2);
            myPen = new Pen(voltageColor);
           drawThickLine(g, myPen,cathode[0], cathode[1]);

           drawThickLine(g, myPen, lead2, gate[0]);
           drawThickLine(g, myPen, gate[0], gate[1]);

            curcount_a = updateDotCount(ia, curcount_a);
            curcount_c = updateDotCount(ic, curcount_c);
            curcount_g = updateDotCount(ig, curcount_g);
            if (sim.dragElm != this)
            {
                drawDots(g, point1, lead2, curcount_a);
                drawDots(g, point2, lead2, curcount_c);
                drawDots(g, gate[1], gate[0], curcount_g);
                drawDots(g, gate[0], lead2, curcount_g+distance(gate[1], gate[0]));
            }
            drawPosts(g);
        }


        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : (n == 1) ? point2 : gate[1];
        }

        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override int InternalNodeCount
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
                return (volts[anode]-volts[gnode])*ia + (volts[cnode]-volts[gnode])*ic;
            }
        }

        private double aresistance;
        internal override void stamp()
        {
            sim.stampNonLinear(nodes[anode]);
            sim.stampNonLinear(nodes[cnode]);
            sim.stampNonLinear(nodes[gnode]);
            sim.stampNonLinear(nodes[inode]);
            sim.stampResistor(nodes[gnode], nodes[cnode], cresistance);
            diode.stamp(nodes[inode], nodes[gnode]);
        }

        internal override void doStep()
        {
            double vac = volts[anode]-volts[cnode]; // typically negative
            double vag = volts[anode]-volts[gnode]; // typically positive
            if (Math.Abs(vac-lastvac) >.01 || Math.Abs(vag-lastvag) >.01)
                sim.converged = false;
            lastvac = vac;
            lastvag = vag;
            diode.doStep(volts[inode]-volts[gnode]);
            double icmult = 1/triggerI;
            double iamult = 1/holdingI - icmult;
            //System.out.println(icmult + " " + iamult);
            aresistance = (-icmult*ic + ia*iamult > 1) ?.0105 : 10e5;
            //System.out.println(vac + " " + vag + " " + sim.converged + " " + ic + " " + ia + " " + aresistance + " " + volts[inode] + " " + volts[gnode] + " " + volts[anode]);
            sim.stampResistor(nodes[anode], nodes[inode], aresistance);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "SCR";
            double vac = volts[anode]-volts[cnode];
            double vag = volts[anode]-volts[gnode];
            double vgc = volts[gnode]-volts[cnode];
            arr[1] = "Ia = " + getCurrentText(ia);
            arr[2] = "Ig = " + getCurrentText(ig);
            arr[3] = "Vac = " + getVoltageText(vac);
            arr[4] = "Vag = " + getVoltageText(vag);
            arr[5] = "Vgc = " + getVoltageText(vgc);
        }
        internal override void calculateCurrent()
        {
            ic = (volts[cnode]-volts[gnode])/cresistance;
            ia = (volts[anode]-volts[inode])/aresistance;
            ig = -ic-ia;
        }
        public override EditInfo getEditInfo(int n)
        {
            // ohmString doesn't work here on linux
            if (n == 0)
                return new EditInfo("Trigger Current (A)", triggerI, 0, 0);
            if (n == 1)
                return new EditInfo("Holding Current (A)", holdingI, 0, 0);
            if (n == 2)
                return new EditInfo("Gate-Cathode Resistance (ohms)", cresistance, 0, 0);
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0 && ei.value > 0)
                triggerI = ei.value;
            if (n == 1 && ei.value > 0)
                holdingI = ei.value;
            if (n == 2 && ei.value > 0)
                cresistance = ei.value;
        }
    }
}

