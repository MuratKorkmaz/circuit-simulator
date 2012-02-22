namespace JavaToSharp
{
    internal class PhaseCompElm : ChipElm
    {
        public PhaseCompElm(int xx, int yy) : base(xx, yy)
        {
        }
        public PhaseCompElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal override string ChipName
        {
            get
            {
                return "фазовый компаратор";
            }
        }
        internal override void setupPins()
        {
            sizeX = 2;
            sizeY = 2;
            pins = new Pin[3];
            pins[0] = new Pin(0, SIDE_W, "I1");
            pins[1] = new Pin(1, SIDE_W, "I2");
            pins[2] = new Pin(0, SIDE_E, "O");
            pins[2].output = true;
        }
        internal override bool nonLinear()
        {
            return true;
        }
        internal override void stamp()
        {
            int vn = sim.nodeList.Count+pins[2].voltSource;
            sim.stampNonLinear(vn);
            sim.stampNonLinear(0);
            sim.stampNonLinear(nodes[2]);
        }

        private bool ff1;
        private bool ff2;

        internal override void doStep()
        {
            bool v1 = volts[0] > 2.5;
            bool v2 = volts[1] > 2.5;
            if (v1 && !pins[0].value)
                ff1 = true;
            if (v2 && !pins[1].value)
                ff2 = true;
            if (ff1 && ff2)
                ff1 = ff2 = false;
            double @out = (ff1) ? 5 : (ff2) ? 0 : -1;
            //System.out.println(out + " " + v1 + " " + v2);
            if (@out != -1)
                sim.stampVoltageSource(0, nodes[2], pins[2].voltSource, @out);
            else
            {
                // tie current through output pin to 0
                int vn = sim.nodeList.Count+pins[2].voltSource;
                sim.stampMatrix(vn, vn, 1);
            }
            pins[0].value = v1;
            pins[1].value = v2;
        }
        internal override int PostCount
        {
            get
            {
                return 3;
            }
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal override int DumpType
        {
            get
            {
                return 161;
            }
        }
    }
}

