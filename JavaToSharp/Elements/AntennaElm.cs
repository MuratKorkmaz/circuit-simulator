using System;

namespace JavaToSharp
{
    internal class AntennaElm : RailElm
    {
        public AntennaElm(int xx, int yy) : base(xx, yy, WF_DC)
        {
        }
        public AntennaElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            waveform = WF_DC;
        }
        internal double fmphase;
        internal override void stamp()
        {
            sim.stampVoltageSource(0, nodes[0], voltSource);
        }
        internal override void doStep()
        {
            sim.updateVoltageSource(0, nodes[0], voltSource, Voltage);
        }
        internal virtual double Voltage
        {
            get
            {
                fmphase += 2*pi*(2200+Math.Sin(2*pi*sim.t*13)*100)*sim.timeStep;
                double fm = 3*Math.Sin(fmphase);
                return Math.Sin(2*pi*sim.t*3000)*(1.3+Math.Sin(2*pi*sim.t*12))*3 + Math.Sin(2*pi*sim.t*2710)*(1.3+Math.Sin(2*pi*sim.t*13))*3 + Math.Sin(2*pi*sim.t*2433)*(1.3+Math.Sin(2*pi*sim.t*14))*3 + fm;
            }
        }
        internal override int DumpType
        {
            get
            {
                return 'A';
            }
        }
    }
}
