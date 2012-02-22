using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class TransistorElm : CircuitElm
    {
        private int pnp;
        private double beta;
        private double fgain;
        private double gmin;
        private const int FLAG_FLIP = 1;

        internal TransistorElm(int xx, int yy, bool pnpflag) : base(xx, yy)
        {
            pnp = (pnpflag) ? -1 : 1;
            beta = 100;
            setup();
        }
        public TransistorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sPnp = st.nextToken();
            bool isParsedPnp = int.TryParse(sPnp,out pnp);
            if(!isParsedPnp)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            beta = 100;
            try
            {
                string sLastvbe = st.nextToken();
                bool isParsedLastvbe = double.TryParse(sLastvbe, out lastvbe);
                if (!isParsedLastvbe)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                string sLastvbc = st.nextToken();
                bool isParsedLastvbc = double.TryParse(sLastvbc, out lastvbc);
                if (!isParsedLastvbc)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
                volts[0] = 0;
                volts[1] = -lastvbe;
                volts[2] = -lastvbc;
                string sBeta = st.nextToken();
                bool isParsedBeta = double.TryParse(sBeta, out beta);
                if (!isParsedBeta)
                {
                    throw new Exception("Не удалось привести к типу double");
                }
            }
            catch (Exception e)
            {
            }
            setup();
        }

        private void setup()
        {
            vcrit = vt * Math.Log(vt/(Math.Sqrt(2)*leakage));
            fgain = beta/(beta+1);
            noDiagonal = true;
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void reset()
        {
            volts[0] = volts[1] = volts[2] = 0;
            lastvbc = lastvbe = curcount_c = curcount_e = curcount_b = 0;
        }
        internal override int DumpType
        {
            get
            {
                return 't';
            }
        }
        internal override string dump()
        {
            return @base.dump() + " " + pnp + " " + (volts[0]-volts[1]) + " " + (volts[0]-volts[2]) + " " + beta;
        }

        private double ic;
        private double ie;
        private double ib;
        private double curcount_c;
        private double curcount_e;
        private double curcount_b;
        private Polygon rectPoly;
        private Polygon arrowPoly;

        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, 16);
            setPowerColor(g, true);
            // draw collector
            setVoltageColor(g, volts[1]);
            drawThickLine(g, coll[0], coll[1]);
            // draw emitter
            setVoltageColor(g, volts[2]);
            drawThickLine(g, emit[0], emit[1]);
            // draw arrow
            g.Color = lightGrayColor;
            g.fillPolygon(arrowPoly);
            // draw base
            setVoltageColor(g, volts[0]);
            if (sim.powerCheckItem.State)
                g.Color = Color.Gray;
            drawThickLine(g, point1, @base);
            // draw dots
            curcount_b = updateDotCount(-ib, curcount_b);
            drawDots(g, @base, point1, curcount_b);
            curcount_c = updateDotCount(-ic, curcount_c);
            drawDots(g, coll[1], coll[0], curcount_c);
            curcount_e = updateDotCount(-ie, curcount_e);
            drawDots(g, emit[1], emit[0], curcount_e);
            // draw base rectangle
            setVoltageColor(g, volts[0]);
            setPowerColor(g, true);
            g.fillPolygon(rectPoly);

            if ((needsHighlight() || sim.dragElm == this) && dy == 0)
            {
                g.Color = Color.White;
                g.Font = unitsFont;
                int ds = sign(dx);
                g.drawString("Б", @base.X-10*ds, @base.Y-5);
                g.drawString("К", coll[0].X-3+9*ds, coll[0].Y+4); // x+6 if ds=1, -12 if -1
                g.drawString("Э", emit[0].X-3+9*ds, emit[0].Y+4);
            }
            drawPosts(g);
        }
        internal override Point getPost(int n)
        {
            return (n == 0) ? point1 : (n == 1) ? coll[0] : emit[0];
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
                return (volts[0]-volts[2])*ib + (volts[1]-volts[2])*ic;
            }
        }

        private Point[] rect;
        private Point[] coll;
        private Point[] emit;
        private Point @base;
        internal override void setPoints()
        {
            @base.setPoints();
            int hs = 16;
            if ((flags & FLAG_FLIP) != 0)
                dsign = -dsign;
            int hs2 = hs*dsign*pnp;
            // calc collector, emitter posts
            coll = newPointArray(2);
            emit = newPointArray(2);
            interpPoint2(point1, point2, coll[0], emit[0], 1, hs2);
            // calc rectangle edges
            rect = newPointArray(4);
            interpPoint2(point1, point2, rect[0], rect[1], 1-16/dn, hs);
            interpPoint2(point1, point2, rect[2], rect[3], 1-13/dn, hs);
            // calc points where collector/emitter leads contact rectangle
            interpPoint2(point1, point2, coll[1], emit[1], 1-13/dn, 6*dsign*pnp);
            // calc point where base lead contacts rectangle
            @base = new Point();
            interpPoint(point1, point2, @base, 1-16/dn);

            // rectangle
            rectPoly = createPolygon(rect[0], rect[2], rect[3], rect[1]);

            // arrow
            if (pnp == 1)
                arrowPoly = calcArrow(emit[1], emit[0], 8, 4);
            else
            {
                Point pt = interpPoint(point1, point2, 1-11/dn, -5*dsign*pnp);
                arrowPoly = calcArrow(emit[0], pt, 8, 4);
            }
        }

        private const double leakage = 1e-13; // 1e-6;
        private const double vt =.025;
        private const double vdcoef = 1/vt;
        private const double rgain =.5;
        private double vcrit;
        private double lastvbc;
        private double lastvbe;

        private double limitStep(double vnew, double vold)
        {
            if (vnew > vcrit && Math.Abs(vnew - vold) > (vt + vt))
            {
                if(vold > 0)
                {
                    double arg = 1 + (vnew - vold) / vt;
                    if(arg > 0)
                    {
                        vnew = vold + vt * Math.Log(arg);
                    }
                    else
                    {
                        vnew = vcrit;
                    }
                }
                else
                {
                    vnew = vt *Math.Log(vnew/vt);
                }
                sim.converged = false;
                //System.out.println(vnew + " " + oo + " " + vold);
            }
            return(vnew);
        }
        internal override void stamp()
        {
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
            sim.stampNonLinear(nodes[2]);
        }
        internal override void doStep()
        {
            double vbc = volts[0]-volts[1]; // typically negative
            double vbe = volts[0]-volts[2]; // typically positive
            if (Math.Abs(vbc-lastvbc) >.01 || Math.Abs(vbe-lastvbe) >.01) //.01
                sim.converged = false;
            gmin = 0;
            if (sim.subIterations > 100)
            {
                // if we have trouble converging, put a conductance in parallel with all P-N junctions.
                // Gradually increase the conductance value for each iteration.
                gmin = Math.Exp(-9*Math.Log(10)*(1-sim.subIterations/3000.0));
                if (gmin >.1)
                    gmin =.1;
            }
            //System.out.print("T " + vbc + " " + vbe + "\n");
            vbc = pnp*limitStep(pnp*vbc, pnp*lastvbc);
            vbe = pnp*limitStep(pnp*vbe, pnp*lastvbe);
            lastvbc = vbc;
            lastvbe = vbe;
            double pcoef = vdcoef*pnp;
            double expbc = Math.Exp(vbc*pcoef);
//	    if (expbc > 1e13 || Double.isInfinite(expbc))
//	      expbc = 1e13;
            double expbe = Math.Exp(vbe*pcoef);
            if (expbe < 1)
                expbe = 1;
//	    if (expbe > 1e13 || Double.isInfinite(expbe))
//	      expbe = 1e13;
            ie = pnp*leakage*(-(expbe-1)+rgain*(expbc-1));
            ic = pnp*leakage*(fgain*(expbe-1)-(expbc-1));
            ib = -(ie+ic);
            //System.out.println("gain " + ic/ib);
            //System.out.print("T " + vbc + " " + vbe + " " + ie + " " + ic + "\n");
            double gee = -leakage*vdcoef*expbe;
            double gec = rgain*leakage*vdcoef*expbc;
            double gce = -gee*fgain;
            double gcc = -gec*(1/rgain);

//	    System.out.print("gee = " + gee + "\n");
//	    System.out.print("gec = " + gec + "\n");
//	    System.out.print("gce = " + gce + "\n");
//	    System.out.print("gcc = " + gcc + "\n");
//	    System.out.print("gce+gcc = " + (gce+gcc) + "\n");
//	    System.out.print("gee+gec = " + (gee+gec) + "\n");

            // stamps from page 302 of Pillage.  Node 0 is the base,
            // node 1 the collector, node 2 the emitter.  Also stamp
            // minimum conductance (gmin) between b,e and b,c
            sim.stampMatrix(nodes[0], nodes[0], -gee-gec-gce-gcc + gmin*2);
            sim.stampMatrix(nodes[0], nodes[1], gec+gcc - gmin);
            sim.stampMatrix(nodes[0], nodes[2], gee+gce - gmin);
            sim.stampMatrix(nodes[1], nodes[0], gce+gcc - gmin);
            sim.stampMatrix(nodes[1], nodes[1], -gcc + gmin);
            sim.stampMatrix(nodes[1], nodes[2], -gce);
            sim.stampMatrix(nodes[2], nodes[0], gee+gec - gmin);
            sim.stampMatrix(nodes[2], nodes[1], -gec);
            sim.stampMatrix(nodes[2], nodes[2], -gee + gmin);

            // we are solving for v(k+1), not delta v, so we use formula
            // 10.5.13, multiplying J by v(k)
            sim.stampRightSide(nodes[0], -ib - (gec+gcc)*vbc - (gee+gce)*vbe);
            sim.stampRightSide(nodes[1], -ic + gce*vbe + gcc*vbc);
            sim.stampRightSide(nodes[2], -ie + gee*vbe + gec*vbc);
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = "транзистор (" + ((pnp == -1) ? "PNP)" : "NPN)") + " beta=" + showFormat.format(beta);
            double vbc = volts[0]-volts[1];
            double vbe = volts[0]-volts[2];
            double vce = volts[1]-volts[2];
            if (vbc*pnp >.2)
                arr[1] = vbe*pnp >.2 ? "saturation" : "reverse active";
            else
                arr[1] = vbe*pnp >.2 ? "fwd active" : "cutoff";
            arr[2] = "Iк = " + getCurrentText(ic);
            arr[3] = "Iб = " + getCurrentText(ib);
            arr[4] = "Vбэ = " + getVoltageText(vbe);
            arr[5] = "Vбк = " + getVoltageText(vbc);
            arr[6] = "Vкэ = " + getVoltageText(vce);
        }
        internal override double getScopeValue(int x)
        {
            switch (x)
            {
                case Scope.VAL_IB:
                    return ib;
                case Scope.VAL_IC:
                    return ic;
                case Scope.VAL_IE:
                    return ie;
                case Scope.VAL_VBE:
                    return volts[0]-volts[2];
                case Scope.VAL_VBC:
                    return volts[0]-volts[1];
                case Scope.VAL_VCE:
                    return volts[1]-volts[2];
            }
            return 0;
        }
        internal override string getScopeUnits(int x)
        {
            switch (x)
            {
                case Scope.VAL_IB:
                case Scope.VAL_IC:
                case Scope.VAL_IE:
                    return "A";
                default:
                    return "V";
            }
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Beta/hFE", beta, 10, 1000).setDimensionless();
            if (n == 1)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new Checkbox("Поменять местами Э/К", (flags & FLAG_FLIP) != 0);
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
            {
                beta = ei.value;
                setup();
            }
            if (n == 1)
            {
                if (ei.checkbox.State)
                    flags |= FLAG_FLIP;
                else
                    flags &= ~FLAG_FLIP;
                setPoints();
            }
        }
        internal override bool canViewInScope()
        {
            return true;
        }
    }
}
