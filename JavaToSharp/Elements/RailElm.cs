using System;
using System.Drawing;

namespace JavaToSharp
{
    internal class RailElm : VoltageElm
    {
        public RailElm(int xx, int yy) : base(xx, yy, WF_DC)
        {
        }
        internal RailElm(int xx, int yy, int wf) : base(xx, yy, wf)
        {
        }
        public RailElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal readonly int FLAG_CLOCK = 1;
        internal override int DumpType
        {
            get
            {
                return 'R';
            }
        }
        internal override int PostCount
        {
            get
            {
                return 1;
            }
        }

        internal override void setPoints()
        {
            base.setPoints();
            lead1 = interpPoint(point1, point2, 1-circleSize/dn);
        }
        internal override void draw(Graphics g)
        {
            setBbox(point1, point2, circleSize);
            setVoltageColor(g, volts[0]);
            drawThickLine(g, point1, lead1);
            bool clock = waveform == WF_SQUARE && (flags & FLAG_CLOCK) != 0;
            if (waveform == WF_DC || waveform == WF_VAR || clock)
            {
                Font f = new Font("SansSerif", 0, 12);
                g.Font = f;
                g.Color = needsHighlight() ? selectColor : whiteColor;
                setPowerColor(g, false);
                double v = Voltage;
                string s = getShortUnitText(v, "В");
                if (Math.Abs(v) < 1)
                    s = showFormat.format(v) + "В";
                if (Voltage > 0)
                    s = "+" + s;
                if (this is AntennaElm)
                    s = "Ant";
                if (clock)
                    s = "CLK";
                drawCenteredText(g, s, x2, y2, true);
            }
            else
            {
                drawWaveform(g, point2);
            }
            drawPosts(g);
            curcount = updateDotCount(-current, curcount);
            if (sim.dragElm != this)
                drawDots(g, point1, lead1, curcount);
        }
        internal override double VoltageDiff
        {
            get
            {
                return volts[0];
            }
        }
        internal override void stamp()
        {
            if (waveform == WF_DC)
                sim.stampVoltageSource(0, nodes[0], voltSource, Voltage);
            else
                sim.stampVoltageSource(0, nodes[0], voltSource);
        }
        internal override void doStep()
        {
            if (waveform != WF_DC)
                sim.updateVoltageSource(0, nodes[0], voltSource, Voltage);
        }
        internal override bool hasGroundConnection(int n1)
        {
            return true;
        }
    }
}
