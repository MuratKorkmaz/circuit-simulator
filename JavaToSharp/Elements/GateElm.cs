using System;
using System.Drawing;

namespace JavaToSharp.Elements
{
    internal abstract class GateElm : CircuitElm
    {
        internal readonly int FLAG_SMALL = 1;
        internal int inputCount = 2;
        internal bool lastOutput;

        public GateElm(int xx, int yy) : base(xx, yy)
        {
            noDiagonal = true;
            inputCount = 2;
            Size = sim.smallGridCheckItem.State ? 1 : 2;
        }
        public GateElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sInputCount = st.nextToken();
            bool isParsedInputCount = int.TryParse(sInputCount,out inputCount);
            if (!isParsedInputCount)
            {
                throw new Exception("Не удалось привести к типу int");
            }
            lastOutput = new (double)double? (st.nextToken()) > 2.5;
            noDiagonal = true;
            Size = (f & FLAG_SMALL) != 0 ? 1 : 2;
        }
        internal virtual bool isInverting
        {
            get
            {
                return false;
            }
        }
        internal int gsize, gwidth, gwidth2, gheight, hs2;
        internal virtual int Size
        {
            set
            {
                gsize = value;
                gwidth = 7*value;
                gwidth2 = 14*value;
                gheight = 8*value;
                flags = (value == 1) ? FLAG_SMALL : 0;
            }
        }
        internal override string dump()
        {
            return base.dump() + " " + inputCount + " " + volts[inputCount];
        }
        internal Point[] inPosts; internal Point[] inGates;
        internal int ww;
        internal override void setPoints()
        {
            base.setPoints();
            if (dn > 150 && this == sim.dragElm)
                Size = 2;
            int hs = gheight;
            int i;
            ww = gwidth2; // was 24
            if (ww > dn/2)
                ww = (int)(dn/2);
            if (Inverting && ww+8 > dn/2)
                ww = (int)(dn/2-8);
            calcLeads(ww*2);
            inPosts = new Point[inputCount];
            inGates = new Point[inputCount];
            allocNodes();
            int i0 = -inputCount/2;
            for (i = 0; i != inputCount; i++, i0++)
            {
                if (i0 == 0 && (inputCount & 1) == 0)
                    i0++;
                inPosts[i] = interpPoint(point1, point2, 0, hs*i0);
                inGates[i] = interpPoint(lead1, lead2, 0, hs*i0);
                volts[i] = (lastOutput ^ Inverting) ? 5 : 0;
            }
            hs2 = gwidth*(inputCount/2+1);
            setBbox(point1, point2, hs2);
        }
        internal override void draw(Graphics g)
        {
            int i;
            for (i = 0; i != inputCount; i++)
            {
                setVoltageColor(g, volts[i]);
                drawThickLine(g, inPosts[i], inGates[i]);
            }
            setVoltageColor(g, volts[inputCount]);
            drawThickLine(g, lead2, point2);
            g.Color = needsHighlight() ? selectColor : lightGrayColor;
            drawThickPolygon(g, gatePoly);
            if (linePoints != null)
                for (i = 0; i != linePoints.Length-1; i++)
                    drawThickLine(g, linePoints[i], linePoints[i+1]);
            if (Inverting)
                drawThickCircle(g, pcircle.X, pcircle.Y, 3);
            curcount = updateDotCount(current, curcount);
            drawDots(g, lead2, point2, curcount);
            drawPosts(g);
        }
        internal Polygon gatePoly;
        internal Point pcircle; internal Point[] linePoints;
        internal override int PostCount
        {
            get
            {
                return inputCount+1;
            }
        }
        internal override Point getPost(int n)
        {
            if (n == inputCount)
                return point2;
            return inPosts[n];
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 1;
            }
        }
        internal abstract string GateName {get;}
        internal override void getInfo(string[] arr)
        {
            arr[0] = GateName;
            arr[1] = "Vout = " + getVoltageText(volts[inputCount]);
            arr[2] = "Iout = " + getCurrentText(Current);
        }
        internal override void stamp()
        {
            sim.stampVoltageSource(0, nodes[inputCount], voltSource);
        }
        internal virtual bool getInput(int x)
        {
            return volts[x] > 2.5;
        }
        internal abstract bool calcFunction();
        internal override void doStep()
        {
            int i;
            bool f = calcFunction();
            if (Inverting)
                f = !f;
            lastOutput = f;
            double res = f ? 5 : 0;
            sim.updateVoltageSource(0, nodes[inputCount], voltSource, res);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("количество входов", inputCount, 1, 8).setDimensionless();
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            inputCount = (int) ei.value;
            setPoints();
        }
        // there is no current path through the gate inputs, but there
        // is an indirect path through the output to ground.
        internal override bool getConnection(int n1, int n2)
        {
            return false;
        }
        internal override bool hasGroundConnection(int n1)
        {
            return (n1 == inputCount);
        }
    }
}

