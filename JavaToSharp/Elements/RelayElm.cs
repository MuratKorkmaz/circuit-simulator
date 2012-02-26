using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp.Elements
{ // 0 = switch
// 1 = switch end 1
// 2 = switch end 2
// ...
// 3n   = coil
// 3n+1 = coil
// 3n+2 = end of coil resistor

    internal class RelayElm : CircuitElm
    {
        internal double inductance;
        internal Inductor ind;
        internal double r_on, r_off, onCurrent;
        internal Point[] coilPosts; internal Point[] coilLeads; internal Point[][] swposts; internal Point[][] swpoles; internal Point[] ptSwitch;
        internal Point[] lines;
        internal double coilCurrent; internal double[] switchCurrent; internal double coilCurCount; internal double[] switchCurCount;
        internal double d_position, coilR;
        internal int i_position;
        internal int poleCount;
        internal int openhs;
        internal readonly int nSwitch0 = 0;
        internal readonly int nSwitch1 = 1;
        internal readonly int nSwitch2 = 2;
        internal int nCoil1, nCoil2, nCoil3;
        internal readonly int FLAG_SWAP_COIL = 1;

        public RelayElm(int xx, int yy) : base(xx, yy)
        {
            ind = new Inductor(sim);
            inductance =.2;
            ind.setup(inductance, 0, Inductor.FLAG_BACK_EULER);
            noDiagonal = true;
            onCurrent =.02;
            r_on =.05;
            r_off = 1e6;
            coilR = 20;
            coilCurrent = coilCurCount = 0;
            poleCount = 1;
            setupPoles();
        }
        public RelayElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
        {
            string sPoleCount = st.nextToken();
            bool isParsedPoleCount = int.TryParse(sPoleCount,out poleCount);
            if (!isParsedPoleCount)
            {
                throw new Exception("�� ������� �������� � ���� int");
            }
            string sInductance = st.nextToken();
            bool isParsedInductance = double.TryParse(sInductance, out inductance);
            if (!isParsedInductance)
            {
               throw new Exception("�� ������� �������� � ���� double");
            }

            string sCoilCurrent = st.nextToken();
            bool isParsedCoilCurrent = double.TryParse(sCoilCurrent,out coilCurrent);
            if (!isParsedCoilCurrent)
            {
                throw new Exception("�� ������� �������� � ���� double");
            }
            string sR_on = st.nextToken();
            bool isParsedR_on = double.TryParse(sR_on, out r_on);
            if (!isParsedR_on)
            {
                throw new Exception("�� ������� �������� � ���� double");
            }
            string sR_off = st.nextToken();
            bool isParsedR_off = double.TryParse(sR_off, out r_off);
            if (!isParsedR_off)
            {
                throw new Exception("�� ������� �������� � ���� double");
            }
            string sOnCurrent = st.nextToken();
            bool isParsedOnCurrent = double.TryParse(sOnCurrent ,out onCurrent);
            if (!isParsedOnCurrent)
            {
                throw new Exception("�� ������� �������� � ���� double");
            }
            string sCoilR = st.nextToken();
            bool isParsedCoilR = double.TryParse(sCoilR, out coilR);
            if (!isParsedCoilR)
            {
                throw new Exception("�� ������� �������� � ���� double");
            }
            noDiagonal = true;
            ind = new Inductor(sim);
            ind.setup(inductance, coilCurrent, Inductor.FLAG_BACK_EULER);
            setupPoles();
        }

        internal virtual void setupPoles()
        {
            nCoil1 = 3*poleCount;
            nCoil2 = nCoil1+1;
            nCoil3 = nCoil1+2;
            if (switchCurrent == null || switchCurrent.Length != poleCount)
            {
                switchCurrent = new double[poleCount];
                switchCurCount = new double[poleCount];
            }
        }

        internal override int DumpType
        {
            get
            {
                return 178;
            }
        }

        internal override string dump()
        {
            return base.dump() + " " + poleCount + " " + inductance + " " + coilCurrent + " " + r_on + " " + r_off + " " + onCurrent + " " + coilR;
        }

        internal override void draw(Graphics g)
        {
            int i, p;
            for (i = 0; i != 2; i++)
            {
                voltageColor= setVoltageColor(g, volts[nCoil1+i]);
                myPen = new Pen(voltageColor);
                drawThickLine(g, myPen,coilLeads[i], coilPosts[i]);
            }
            int x = ((flags & FLAG_SWAP_COIL) != 0) ? 1 : 0;
            drawCoil(g, dsign*6, coilLeads[x], coilLeads[1-x], volts[nCoil1+x], volts[nCoil2-x]);

            // draw lines
            g.GetNearestColor(Color.DarkGray);
            for (i = 0; i != poleCount; i++)
            {
                if (i == 0)
                    interpPoint(point1, point2, lines[i*2],.5, openhs*2+5*dsign-i*openhs*3);
                else
                    interpPoint(point1, point2, lines[i*2],.5, (int)(openhs*(-i*3+3-.5+d_position))+5*dsign);
                interpPoint(point1, point2, lines[i*2+1],.5, (int)(openhs*(-i*3-.5+d_position))-5*dsign);
                g.DrawLine(myPen,lines[i*2].X, lines[i*2].Y, lines[i*2+1].X, lines[i*2+1].Y);
            }

            for (p = 0; p != poleCount; p++)
            {
                int po = p*3;
                for (i = 0; i != 3; i++)
                {
                    // draw lead
                  voltageColor=  setVoltageColor(g, volts[nSwitch0+po+i]);
                    myPen = new Pen(voltageColor );
                    drawThickLine(g, myPen ,swposts[p][i], swpoles[p][i]);
                }

                interpPoint(swpoles[p][1], swpoles[p][2], ptSwitch[p], d_position);
                //setVoltageColor(g, volts[nSwitch0]);
                g.GetNearestColor(Color.LightGray);
                drawThickLine(g, myPen,swpoles[p][0], ptSwitch[p]);
                switchCurCount[p] = updateDotCount(switchCurrent[p], switchCurCount[p]);
                drawDots(g, swposts[p][0], swpoles[p][0], switchCurCount[p]);

                if (i_position != 2)
                    drawDots(g, swpoles[p][i_position+1], swposts[p][i_position+1], switchCurCount[p]);
            }

            coilCurCount = updateDotCount(coilCurrent, coilCurCount);

            drawDots(g, coilPosts[0], coilLeads[0], coilCurCount);
            drawDots(g, coilLeads[0], coilLeads[1], coilCurCount);
            drawDots(g, coilLeads[1], coilPosts[1], coilCurCount);

            drawPosts(g);
            setBbox(coilPosts[0], coilLeads[1], 0);
            adjustBbox(swpoles[poleCount-1][0], swposts[poleCount-1][1]); // XXX
        }

        internal override void setPoints()
        {
            base.setPoints();
            setupPoles();
            allocNodes();
            openhs = -dsign*16;

            // switch
            calcLeads(32);
//ORIGINAL LINE: swposts = new Point[poleCount][3];
            swposts = RectangularArrays.ReturnRectangularPointArray(poleCount, 3);
//ORIGINAL LINE: swpoles = new Point[poleCount][3];
            swpoles = RectangularArrays.ReturnRectangularPointArray(poleCount, 3);
            int i, j;
            for (i = 0; i != poleCount; i++)
            {
                for (j = 0; j != 3; j++)
                {
                    swposts[i][j] = new Point();
                    swpoles[i][j] = new Point();
                }
                interpPoint(lead1, lead2, swpoles[i][0], 0, -openhs*3*i);
                interpPoint(lead1, lead2, swpoles[i][1], 1, -openhs*3*i-openhs);
                interpPoint(lead1, lead2, swpoles[i][2], 1, -openhs*3*i+openhs);
                interpPoint(point1, point2, swposts[i][0], 0, -openhs*3*i);
                interpPoint(point1, point2, swposts[i][1], 1, -openhs*3*i-openhs);
                interpPoint(point1, point2, swposts[i][2], 1, -openhs*3*i+openhs);
            }

            // coil
            coilPosts = newPointArray(2);
            coilLeads = newPointArray(2);
            ptSwitch = newPointArray(poleCount);

            int x = ((flags & FLAG_SWAP_COIL) != 0) ? 1 : 0;
            interpPoint(point1, point2, coilPosts[0], x, openhs*2);
            interpPoint(point1, point2, coilPosts[1], x, openhs*3);
            interpPoint(point1, point2, coilLeads[0],.5, openhs*2);
            interpPoint(point1, point2, coilLeads[1],.5, openhs*3);

            // lines
            lines = newPointArray(poleCount*2);
        }
        internal override Point getPost(int n)
        {
            if (n < 3*poleCount)
                return swposts[n / 3][n % 3];
            return coilPosts[n-3*poleCount];
        }
        internal override int PostCount
        {
            get
            {
                return 2+poleCount*3;
            }
        }
        internal override int InternalNodeCount
        {
            get
            {
                return 1;
            }
        }
        internal override void reset()
        {
            base.reset();
            ind.reset();
            coilCurrent = coilCurCount = 0;
            int i;
            for (i = 0; i != poleCount; i++)
                switchCurrent[i] = switchCurCount[i] = 0;
        }
        internal double a1, a2, a3, a4;
        internal override void stamp()
        {
            // inductor from coil post 1 to internal node
            ind.stamp(nodes[nCoil1], nodes[nCoil3]);
            // resistor from internal node to coil post 2
            sim.stampResistor(nodes[nCoil3], nodes[nCoil2], coilR);

            int i;
            for (i = 0; i != poleCount*3; i++)
                sim.stampNonLinear(nodes[nSwitch0+i]);
        }
        internal override void startIteration()
        {
            ind.startIteration(volts[nCoil1]-volts[nCoil3]);

            // magic value to balance operate speed with reset speed semi-realistically
            double magic = 1.3;
            double pmult = Math.Sqrt(magic+1);
            double p = coilCurrent*pmult/onCurrent;
            d_position = Math.Abs(p*p) - 1.3;
            if (d_position < 0)
                d_position = 0;
            if (d_position > 1)
                d_position = 1;
            if (d_position <.1)
                i_position = 0;
            else if (d_position >.9)
                i_position = 1;
            else
                i_position = 2;
            //System.out.println("ind " + this + " " + current + " " + voltdiff);
        }

        // we need this to be able to change the matrix for each step
        internal override bool nonLinear()
        {
            return true;
        }

        internal override void doStep()
        {
            double voltdiff = volts[nCoil1]-volts[nCoil3];
            ind.doStep(voltdiff);
            int p;
            for (p = 0; p != poleCount*3; p += 3)
            {
                sim.stampResistor(nodes[nSwitch0+p], nodes[nSwitch1+p], i_position == 0 ? r_on : r_off);
                sim.stampResistor(nodes[nSwitch0+p], nodes[nSwitch2+p], i_position == 1 ? r_on : r_off);
            }
        }
        internal override void calculateCurrent()
        {
            double voltdiff = volts[nCoil1]-volts[nCoil3];
            coilCurrent = ind.calculateCurrent(voltdiff);

            // actually this isn't correct, since there is a small amount
            // of current through the switch when off
            int p;
            for (p = 0; p != poleCount; p++)
            {
                if (i_position == 2)
                    switchCurrent[p] = 0;
                else
                    switchCurrent[p] = (volts[nSwitch0+p*3]-volts[nSwitch1+p*3+i_position])/r_on;
            }
        }
        internal override void getInfo(string[] arr)
        {
            arr[0] = i_position == 0 ? "���� (����)" : i_position == 1 ? "���� (���)" : "����";
            int i;
            int ln = 1;
            for (i = 0; i != poleCount; i++)
                arr[ln++] = "I" + (i+1) + " = " + getCurrentDText(switchCurrent[i]);
            arr[ln++] = "������� I = " + getCurrentDText(coilCurrent);
            arr[ln++] = "������� Vd = " + getVoltageDText(volts[nCoil1] - volts[nCoil2]);
        }
        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("������������� (��)", inductance, 0, 0);
            if (n == 1)
                return new EditInfo("������������� ��� (��)", r_on, 0, 0);
            if (n == 2)
                return new EditInfo("������������� ���� (��)", r_off, 0, 0);
            if (n == 3)
                return new EditInfo("��� ��� (A)", onCurrent, 0, 0);
            if (n == 4)
                return new EditInfo("���������� ���������� �����", poleCount, 1, 4).setDimensionless();
            if (n == 5)
                return new EditInfo("������������� ������� (��)", coilR, 0, 0);
            if (n == 6)
            {
                EditInfo ei = new EditInfo("", 0, -1, -1);
                ei.checkbox = new CheckBox();
                if ((flags & FLAG_SWAP_COIL) != 0)
                {
                    ei.checkbox.Text = "�������� ����������� �������";
                }
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0 && ei.value > 0)
            {
                inductance = ei.value;
                ind.setup(inductance, coilCurrent, Inductor.FLAG_BACK_EULER);
            }
            if (n == 1 && ei.value > 0)
                r_on = ei.value;
            if (n == 2 && ei.value > 0)
                r_off = ei.value;
            if (n == 3 && ei.value > 0)
                onCurrent = ei.value;
            if (n == 4 && ei.value >= 1)
            {
                poleCount = (int) ei.value;
                setPoints();
            }
            if (n == 5 && ei.value > 0)
                coilR = ei.value;
            if (n == 6)
            {
                if (ei.checkbox.Checked)
                    flags |= FLAG_SWAP_COIL;
                else
                    flags &= ~FLAG_SWAP_COIL;
                setPoints();
            }
        }
        internal override bool getConnection(int n1, int n2)
        {
            return (n1 / 3 == n2 / 3);
        }
    }



//----------------------------------------------------------------------------------------
//	Copyright � 2008 - 2010 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length.
//----------------------------------------------------------------------------------------
    internal static partial class RectangularArrays
    {
        internal static Point[][] ReturnRectangularPointArray(int Size1, int Size2)
        {
            Point[][] Array = new Point[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new Point[Size2];
            }
            return Array;
        }
    }
}