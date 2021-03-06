namespace circuit_emulator
{
    class Diode
    {
        internal int[] nodes;
        internal CirSim sim;
	
	
        internal Diode(CirSim s)
        {
            sim = s;
            nodes = new int[2];
        }
        internal virtual void  setup(double fw, double zv)
        {
            fwdrop = fw;
            zvoltage = zv;
            vdcoef = System.Math.Log(1 / leakage + 1) / fwdrop;
            vt = 1 / vdcoef;
            // critical voltage for limiting; current is vt/sqrt(2) at
            // this voltage
            vcrit = vt * System.Math.Log(vt / (System.Math.Sqrt(2) * leakage));
            if (zvoltage == 0)
                zoffset = 0;
            else
            {
                // calculate offset which will give us 5mA at zvoltage
                double i = - .005;
                zoffset = zvoltage - System.Math.Log(- (1 + i / leakage)) / vdcoef;
            }
        }
	
        internal virtual void  reset()
        {
            lastvoltdiff = 0;
        }
	
        public double leakage = 1e-14; // was 1e-9;
        internal double vt, vdcoef, fwdrop, zvoltage, zoffset;
        internal double lastvoltdiff;
        internal double vcrit;
	
        internal virtual double limitStep(double vnew, double vold)
        {
            double arg;
            double oo = vnew;
		
            // check new voltage; has current changed by factor of e^2?
            if (vnew > vcrit && System.Math.Abs(vnew - vold) > (vt + vt))
            {
                if (vold > 0)
                {
                    arg = 1 + (vnew - vold) / vt;
                    if (arg > 0)
                    {
                        // adjust vnew so that the current is the same
                        // as in linearized model from previous iteration.
                        // current at vnew = old current * arg
                        vnew = vold + vt * System.Math.Log(arg);
                        // current at v0 = 1uA
                        double v0 = System.Math.Log(1e-6 / leakage) * vt;
                        vnew = System.Math.Max(v0, vnew);
                    }
                    else
                    {
                        vnew = vcrit;
                    }
                }
                else
                {
                    // adjust vnew so that the current is the same
                    // as in linearized model from previous iteration.
                    // (1/vt = slope of load line)
                    vnew = vt * System.Math.Log(vnew / vt);
                }
                sim.converged = false;
                //System.out.println(vnew + " " + oo + " " + vold);
            }
            else if (vnew < 0 && zoffset != 0)
            {
                // for Zener breakdown, use the same logic but translate the values
                vnew = - vnew - zoffset;
                vold = - vold - zoffset;
			
                if (vnew > vcrit && System.Math.Abs(vnew - vold) > (vt + vt))
                {
                    if (vold > 0)
                    {
                        arg = 1 + (vnew - vold) / vt;
                        if (arg > 0)
                        {
                            vnew = vold + vt * System.Math.Log(arg);
                            double v0 = System.Math.Log(1e-6 / leakage) * vt;
                            vnew = System.Math.Max(v0, vnew);
                            //System.out.println(oo + " " + vnew);
                        }
                        else
                        {
                            vnew = vcrit;
                        }
                    }
                    else
                    {
                        vnew = vt * System.Math.Log(vnew / vt);
                    }
                    sim.converged = false;
                }
                vnew = - (vnew + zoffset);
            }
            return vnew;
        }
	
        internal virtual void  stamp(int n0, int n1)
        {
            nodes[0] = n0;
            nodes[1] = n1;
            sim.stampNonLinear(nodes[0]);
            sim.stampNonLinear(nodes[1]);
        }
	
        internal virtual void  doStep(double voltdiff)
        {
            // used to have .1 here, but needed .01 for peak detector
            if (System.Math.Abs(voltdiff - lastvoltdiff) > .01)
                sim.converged = false;
            voltdiff = limitStep(voltdiff, lastvoltdiff);
            lastvoltdiff = voltdiff;
		
            if (voltdiff >= 0 || zvoltage == 0)
            {
                // regular diode or forward-biased zener
                double eval = System.Math.Exp(voltdiff * vdcoef);
                // make diode linear with negative voltages; aids convergence
                if (voltdiff < 0)
                    eval = 1;
                double geq = vdcoef * leakage * eval;
                double nc = (eval - 1) * leakage - geq * voltdiff;
                sim.stampConductance(nodes[0], nodes[1], geq);
                sim.stampCurrentSource(nodes[0], nodes[1], nc);
            }
            else
            {
                // Zener diode
			
                /* 
			* I(Vd) = Is * (exp[Vd*C] - exp[(-Vd-Vz)*C] - 1 )
			*
			* geq is I'(Vd)
			* nc is I(Vd) + I'(Vd)*(-Vd)
			*/
			
                double geq = leakage * vdcoef * (System.Math.Exp(voltdiff * vdcoef) + System.Math.Exp((- voltdiff - zoffset) * vdcoef));
			
                double nc = leakage * (System.Math.Exp(voltdiff * vdcoef) - System.Math.Exp((- voltdiff - zoffset) * vdcoef) - 1) + geq * (- voltdiff);
			
                sim.stampConductance(nodes[0], nodes[1], geq);
                sim.stampCurrentSource(nodes[0], nodes[1], nc);
            }
        }
	
        internal virtual double calculateCurrent(double voltdiff)
        {
            if (voltdiff >= 0 || zvoltage == 0)
                return leakage * (System.Math.Exp(voltdiff * vdcoef) - 1);
            return leakage * (System.Math.Exp(voltdiff * vdcoef) - System.Math.Exp((- voltdiff - zoffset) * vdcoef) - 1);
        }
    }
}