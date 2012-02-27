using System;
using System.Drawing;
using JavaToSharp;

namespace JavaToSharp
{
    internal class CC2Elm : ChipElm
    {
        internal double gain;
        public CC2Elm(int xx, int yy) : base(xx, yy)
        {
            gain = 1;
        }
        public CC2Elm(int xx, int yy, int g) : base(xx, yy)
        {
            gain = g;
        }
        public CC2Elm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            string sGain = st.nextToken();
            bool isParsedGain = double.TryParse(sGain,out gain);
            if (!isParsedGain)
            {
                throw new Exception("Не удалось привести к типу double");
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + gain;
        }
        internal override string ChipName
        {
            get
            {
                return "CC2";
            }
        }
        internal override void setupPins()
        {
            sizeX = 2;
            sizeY = 3;
            pins = new Pin[3];
            pins[0] = new Pin(0, SIDE_W, "X");
            pins[0].output = true;
            pins[1] = new Pin(2, SIDE_W, "Y");
            pins[2] = new Pin(1, SIDE_E, "Z");
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = (gain == 1) ? "CCII+" : "CCII-";
            arr[1] = "X,Y = " + getVoltageText(volts[0]);
            arr[2] = "Z = " + getVoltageText(volts[2]);
            arr[3] = "I = " + getCurrentText(pins[0].current);
        }
        //boolean nonLinear() { return true; }
        internal override void stamp()
        {
            // X voltage = Y voltage
            sim.stampVoltageSource(0, nodes[0], pins[0].voltSource);
            sim.stampVCVS(0, nodes[1], 1, pins[0].voltSource);
            // Z current = gain * X current
            sim.stampCCCS(0, nodes[2], pins[0].voltSource, gain);
        }
        internal override void draw(Graphics g)
        {
            pins[2].current = pins[0].current * gain;
            drawChip(g);
        }
        internal int PostCount
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
        internal virtual int DumpType
        {
            get
            {
                return 179;
            }
        }
    }

    internal class CC2NegElm : CC2Elm
    {
        public CC2NegElm(int xx, int yy) : base(xx, yy, -1)
        {
        }
        internal virtual Type DumpClass
        {
            get
            {
                return typeof(CC2Elm);
            }
        }
    }
}