using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace JavaToSharp
{ // CirSim.java (c) 2010 by Paul Falstad

// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage

//Russian translation v1.0 by Spiritus, licrym@gmail.com http://licrym.org Please mail me for updates.
//codepage UTF-8


    public class CirSim
    {
        internal Size winSize;
        private readonly MatrixCalculator _calculator;
        private readonly ISimulationView _view;
        private readonly FindPathInfo _pathFinder;
        private Random random;
        internal Type addingClass;
        internal ToolStripMenuItem scopeVMenuItem;
        internal ToolStripMenuItem scopeIMenuItem;
        internal ToolStripMenuItem scopeMaxMenuItem;
        internal ToolStripMenuItem scopeMinMenuItem;
        internal ToolStripMenuItem scopeFreqMenuItem;
        internal ToolStripMenuItem scopePowerMenuItem;
        internal ToolStripMenuItem scopeIbMenuItem;
        internal ToolStripMenuItem scopeIcMenuItem;
        internal ToolStripMenuItem scopeIeMenuItem;
        internal ToolStripMenuItem scopeVbeMenuItem;
        internal ToolStripMenuItem scopeVbcMenuItem;
        internal ToolStripMenuItem scopeVceMenuItem;
        internal ToolStripMenuItem scopeVIMenuItem;
        internal ToolStripMenuItem scopeXYMenuItem;
        internal ToolStripMenuItem scopeResistMenuItem;
        internal ToolStripMenuItem scopeVceIcMenuItem;
        internal int mouseMode;
        internal int tempMouseMode;
        internal string mouseModeStr = "Select";
        internal const double pi = 3.14159265358979323846;
        internal readonly int MODE_ADD_ELM = 0;
        internal readonly int MODE_DRAG_ALL = 1;
        internal readonly int MODE_DRAG_ROW = 2;
        internal readonly int MODE_DRAG_COLUMN = 3;
        internal readonly int MODE_DRAG_SELECTED = 4;
        internal readonly int MODE_DRAG_POST = 5;
        internal readonly int MODE_SELECT = 6;
        internal const int infoWidth = 120;
        internal int dragX, dragY, initDragX, initDragY;
        internal Rectangle selectedArea;
        internal int gridSize, gridMask, gridRound;
        internal bool analyzeFlag;
        internal bool useBufferedImage;
        internal double t;
        internal int pause = 20;
        internal int scopeSelected = -1;
        internal int menuScope = -1;
        internal int hintType = -1, hintItem1, hintItem2;
        internal string stopMessage;
        internal double timeStep;
        internal const int HINT_LC = 1;
        internal const int HINT_RC = 2;
        internal const int HINT_3DB_C = 3;
        internal const int HINT_TWINT = 4;
        internal const int HINT_3DB_L = 5;
        internal List<CircuitElm> elmList;
        internal CircuitElm dragElm, menuElm, mouseElm, stopElm;
        internal int mousePost = -1;
        internal CircuitElm plotXElm, plotYElm;
        internal int draggingPost;
        internal SwitchElm heldSwitchElm;
        internal double[][] circuitMatrix; 
        internal double[] circuitRightSide; 
        internal double[] origRightSide; 
        internal double[][] origMatrix;
        internal RowInfo[] circuitRowInfo;
        internal int[] circuitPermute;
        internal bool circuitNonLinear;
        internal int circuitMatrixSize, circuitMatrixFullSize;
        internal bool circuitNeedsMap;
        internal int scopeCount;
        internal Scope[] scopes;
        internal int[] scopeColCount;
        internal static EditDialog editDialog;
       // internal static ImportDialog impDialog;
        internal Type[] dumpTypes;
        internal static string muString = "мк";
        internal static string ohmString = "Ом";
        internal string clipboard;
        internal Rectangle circuitArea;
        internal int circuitBottom;
        internal ArrayList undoStack, redoStack;

        internal virtual int getrand(int x)
        {
            int q = random.Next();
            if (q < 0)
                q = -q;
            return q % x;
        }
        internal CircuitCanvas cv;

        internal CirSim(ISimulationView view) 
        {
            _calculator = new MatrixCalculator();
            _pathFinder = new FindPathInfo(this);
            _view = view;
        }

        internal string startCircuitText;
        internal string baseURL = "http://www.falstad.com/circuit/";

        public virtual void init()
        {
            mouseMode = MODE_SELECT;
            tempMouseMode = MODE_SELECT;
            CircuitElm.initClass(this);
            baseURL = Application.StartupPath;
            dumpTypes = new Type[300];
            // these characters are reserved
            dumpTypes['o'] = typeof(Scope);
            dumpTypes['h'] = typeof(Scope);
            dumpTypes['$'] = typeof(Scope);
            dumpTypes['%'] = typeof(Scope);
            dumpTypes['?'] = typeof(Scope);
            dumpTypes['B'] = typeof(Scope);
            //main.Layout = new CircuitLayout();
            cv = new CircuitCanvas(this);
            //cv.addComponentListener(this);
            //cv.addMouseMotionListener(this);
            //cv.addMouseListener(this);
            //cv.addKeyListener(this);
            //main.Controls.Add(cv);
            setGrid();
            elmList = new List<CircuitElm>();
            new ArrayList();
            undoStack = new ArrayList();
            redoStack = new ArrayList();
            scopes = new Scope[20];
            scopeColCount = new int[20];
            scopeCount = 0;

            random = new Random();
        }
        
        internal virtual void handleResize()
        {
            winSize = cv.Size;
            if (winSize.Width == 0)
                return;
            int h = winSize.Height / 5;
//	if (h < 128 && winSize.height > 300)
//	  h = 128;
            circuitArea = new Rectangle(0, 0, winSize.Width, winSize.Height-h);
            int i;
            int minx = 1000, maxx = 0, miny = 1000, maxy = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                // centered text causes problems when trying to center the circuit,
                // so we special-case it here
                if (!ce.isCenteredText)
                {
                    minx = min(ce.x, min(ce.x2, minx));
                    maxx = max(ce.x, max(ce.x2, maxx));
                }
                miny = min(ce.y, min(ce.y2, miny));
                maxy = max(ce.y, max(ce.y2, maxy));
            }
            // center circuit; we don't use snapGrid() because that rounds
            int dx = gridMask & ((circuitArea.Width -(maxx-minx))/2-minx);
            int dy = gridMask & ((circuitArea.Height-(maxy-miny))/2-miny);
            if (dx+minx < 0)
                dx = gridMask & (-minx);
            if (dy+miny < 0)
                dy = gridMask & (-miny);
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.move(dx, dy);
            }
            // after moving elements, need this to avoid singular matrix probs
            needAnalyze();
            circuitBottom = 0;
        }
        
        internal const int resct = 6;
        internal long lastTime = 0, lastFrameTime, lastIterTime, secTime = 0;
        internal int frames = 0;
        internal int steps = 0;
        internal int framerate = 0, steprate = 0;

        public virtual void updateCircuit(Graphics realg)
        {
            if (analyzeFlag)
            {
                analyzeCircuit();
                analyzeFlag = false;
            }
            if (editDialog != null && editDialog.elm is CircuitElm)
                mouseElm = (CircuitElm)(editDialog.elm);
            CircuitElm realMouseElm = mouseElm;
            if (mouseElm == null)
                mouseElm = stopElm;
            setupScopes();
            //var realg = Graphics.FromImage(dbimage);
            CircuitElm.selectColor = Color.Cyan;
            var backBrush = new SolidBrush(Color.Black);
            realg.FillRectangle(backBrush, 0, 0, winSize.Width, winSize.Height);
            if (!View.Parameters.IsStopped)
            {
                try
                {
                    runCircuit();
                }
                catch (Exception e)
                {
                    UserMessageView.Instance.ShowError(e.StackTrace);
                    analyzeFlag = true;
                    _view.UpdateCanvas();
                    return;
                }
            }
            if (!View.Parameters.IsStopped)
            {
                long sysTime = DateTime.Now.Millisecond;
                if (lastTime != 0)
                {
                    int inc = (int)(sysTime-lastTime);
                    double c = View.Parameters.CurrentSpeed;
                    c = Math.Exp(c/3.5-14.2);
                    CircuitElm.currentMult = 1.7 * inc * c;
                }
                if (sysTime-secTime >= 1000)
                {
                    framerate = frames;
                    steprate = steps;
                    frames = 0;
                    steps = 0;
                    secTime = sysTime;
                }
                lastTime = sysTime;
            }
            else
                lastTime = 0;
            CircuitElm.powerMult = Math.Exp(View.Parameters.PowerLight/4.762-7);

            int i;
            Font oldfont = SystemFonts.DefaultFont;
            for (i = 0; i != elmList.Count; i++)
            {
                getElm(i).draw(realg);
            }
            if (tempMouseMode == MODE_DRAG_ROW || tempMouseMode == MODE_DRAG_COLUMN || tempMouseMode == MODE_DRAG_POST || tempMouseMode == MODE_DRAG_SELECTED)
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.drawPost(realg, ce.x, ce.y);
                    ce.drawPost(realg, ce.x2, ce.y2);
                }
            int badnodes = 0;
            // find bad connections, nodes not connected to other elements which
            // intersect other elements' bounding boxes
            for (i = 0; i != nodeList.Count; i++)
            {
                CircuitNode cn = getCircuitNode(i);
                if (!cn.Internal && cn.links.Count == 1)
                {
                    int bb = 0;
                    var cnl = (CircuitNodeLink) cn.links[0];
                    for (int j = 0; j != elmList.Count; j++)
                        if (cnl.elm != getElm(j) && getElm(j).boundingBox.Contains(cn.x, cn.y))
                            bb++;
                    if (bb > 0)
                    {
                        realg.FillEllipse(Brushes.Red, cn.x-3, cn.y-3, 7, 7);
                        badnodes++;
                    }
                }
            }
//	if (mouseElm != null) {
//	    g.setFont(oldfont);
//	    g.drawString("+", mouseElm.x+10, mouseElm.y);
//	    }
            if (dragElm != null && (dragElm.x != dragElm.x2 || dragElm.y != dragElm.y2))
                dragElm.draw(realg);
            int ct = scopeCount;
            if (stopMessage != null)
                ct = 0;
            for (i = 0; i != ct; i++)
                scopes[i].draw(realg);
            var brush = new SolidBrush(CircuitElm.whiteColor);
            if (stopMessage != null)
            {
                realg.DrawString(stopMessage, oldfont, brush, 10, circuitArea.Height);
            }
            else
            {
                if (circuitBottom == 0)
                    calcCircuitBottom();
                var info = new string[10];
                if (mouseElm != null)
                {
                    if (mousePost == -1)
                        mouseElm.getInfo(info);
                    else
                        info[0] = "V = " + CircuitElm.getUnitText2(mouseElm.getPostVoltage(mousePost), "V");
//		 //shownodes
//		for (i = 0; i != mouseElm.getPostCount(); i++)
//		    info[0] += " " + mouseElm.nodes[i];
//		if (mouseElm.getVoltageSourceCount() > 0)
//		    info[0] += ";" + (mouseElm.getVoltageSource()+nodeList.size());
//		

                }
                else
                {
                    info[0] = "t = " + CircuitElm.getUnitText2(t, "Ом");
                }
                if (hintType != -1)
                {
                    for (i = 0; info[i] != null; i++)
                    {
                    }
                    string s = Hint;
                    if (s == null)
                        hintType = -1;
                    else
                        info[i] = s;
                }
                int x = 0;
                if (ct != 0)
                    x = scopes[ct-1].rightEdge() + 20;
                x = max(x, winSize.Width*2/3);

                // count lines of data
                for (i = 0; info[i] != null; i++)
                {
                }
                if (badnodes > 0)
                    info[i++] = badnodes + ((badnodes == 1) ? " плохое соединение" : " плохие соединения");

                // find where to show data; below circuit, not too high unless we need it
                int ybase = winSize.Height-15*i-5;
                ybase = min(ybase, circuitArea.Height);
                ybase = max(ybase, circuitBottom);
                
                for (i = 0; info[i] != null; i++)
                    realg.DrawString(info[i], oldfont, brush, x, ybase+15*(i+1));
            }
            if (selectedArea != Rectangle.Empty)
            {
                var pen = new Pen(CircuitElm.selectColor);
                realg.DrawRectangle(pen, selectedArea.X, selectedArea.Y, selectedArea.Width, selectedArea.Height);
            }
            mouseElm = realMouseElm;
            frames++;
//	
//	g.setColor(Color.white);
//	g.drawString("Framerate: " + framerate, 10, 10);
//	g.drawString("Steprate: " + steprate,  10, 30);
//	g.drawString("Steprate/iter: " + (steprate/getIterCount()),  10, 50);
//	g.drawString("iterc: " + (getIterCount()),  10, 70);
//	

            //realg.DrawImage(dbimage, 0, 0);
            if (!View.Parameters.IsStopped && circuitMatrix != null)
            {
                // Limit to 50 fps (thanks to JСЊrgen KlС†tzer for this)
                int delay = (int)(1000/50 - (DateTime.Now.Millisecond - lastFrameTime));
                //realg.drawString("delay: " + delay,  10, 90);
                if (delay > 0)
                {
                    try
                    {
                        Thread.Sleep(delay);
                    }
                    catch (ThreadInterruptedException e)
                    {
                    }
                }
                _view.UpdateCanvas();
            }
            lastFrameTime = lastTime;
        }

        internal virtual void setupScopes()
        {
            int i;

            // check scopes to make sure the elements still exist, and remove
            // unused scopes/columns
            int pos = -1;
            for (i = 0; i < scopeCount; i++)
            {
                if (locateElm(scopes[i].elm) < 0)
                    scopes[i].Elm = null;
                if (scopes[i].elm == null)
                {
                    int j;
                    for (j = i; j != scopeCount; j++)
                        scopes[j] = scopes[j+1];
                    scopeCount--;
                    i--;
                    continue;
                }
                if (scopes[i].position > pos+1)
                    scopes[i].position = pos+1;
                pos = scopes[i].position;
            }
            while (scopeCount > 0 && scopes[scopeCount-1].elm == null)
                scopeCount--;
            int h = winSize.Height - circuitArea.Height;
            pos = 0;
            for (i = 0; i != scopeCount; i++)
                scopeColCount[i] = 0;
            for (i = 0; i != scopeCount; i++)
            {
                pos = max(scopes[i].position, pos);
                scopeColCount[scopes[i].position]++;
            }
            int colct = pos+1;
            int iw = infoWidth;
            if (colct <= 2)
                iw = iw*3/2;
            int w = (winSize.Width-iw) / colct;
            int marg = 10;
            if (w < marg*2)
                w = marg*2;
            pos = -1;
            int colh = 0;
            int row = 0;
            int speed = 0;
            for (i = 0; i != scopeCount; i++)
            {
                Scope s = scopes[i];
                if (s.position > pos)
                {
                    pos = s.position;
                    colh = h / scopeColCount[pos];
                    row = 0;
                    speed = s.speed;
                }
                if (s.speed != speed)
                {
                    s.speed = speed;
                    s.resetGraph();
                }
                Rectangle r = new Rectangle(pos*w, winSize.Height-h+colh*row, w-marg, colh);
                row++;
                if (!r.Equals(s.rect))
                    s.Rect = r;
            }
        }

        internal virtual string Hint
        {
            get
            {
                CircuitElm c1 = getElm(hintItem1);
                CircuitElm c2 = getElm(hintItem2);
                if (c1 == null || c2 == null)
                    return null;
                if (hintType == HINT_LC)
                {
                    if (!(c1 is InductorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    InductorElm ie = (InductorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "res.f = " + CircuitElm.getUnitText2(1/(2*pi*Math.Sqrt(ie.inductance* ce.capacitance)), "Р“С†");
                }
                if (hintType == HINT_RC)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "RC = " + CircuitElm.getUnitText2(re.resistance*ce.capacitance, "СЃ");
                }
                if (hintType == HINT_3DB_C)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText2(1/(2*pi*re.resistance*ce.capacitance), "Р“С†");
                }
                if (hintType == HINT_3DB_L)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is InductorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    InductorElm ie = (InductorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText2(re.resistance/(2*pi*ie.inductance), "Р“С†");
                }
                if (hintType == HINT_TWINT)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "fc = " + CircuitElm.getUnitText2(1/(2*pi*re.resistance*ce.capacitance), "Р“С†");
                }
                return null;
            }
        }

        public virtual void toggleSwitch(int n)
        {
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce is SwitchElm)
                {
                    n--;
                    if (n == 0)
                    {
                        ((SwitchElm) ce).toggle();
                        analyzeFlag = true;
                        _view.UpdateCanvas();
                        return;
                    }
                }
            }
        }

        internal virtual void needAnalyze()
        {
            analyzeFlag = true;
            
        }

        internal ArrayList nodeList;
        private CircuitElm[] voltageSources;

        public virtual CircuitNode getCircuitNode(int n)
        {
            if (n >= nodeList.Count)
                return null;
            return (CircuitNode) nodeList[n];
        }

        public virtual CircuitElm getElm(int n)
        {
            if (n >= elmList.Count)
                return null;
            return (CircuitElm) elmList[n];
        }
        
        internal virtual void analyzeCircuit()
        {
            calcCircuitBottom();
            if (elmList.Count == 0)
                return;
            stopMessage = null;
            stopElm = null;
            int i, j;
            int vscount = 0;
            nodeList = new ArrayList();
            bool gotGround = false;
            bool gotRail = false;
            CircuitElm volt = null;

            //System.out.println("ac1");
            // look for voltage or ground element
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce is GroundElm)
                {
                    gotGround = true;
                    break;
                }
                if (ce is RailElm)
                    gotRail = true;
                if (volt == null && ce is VoltageElm)
                    volt = ce;
            }

            // if no ground, and no rails, then the voltage elm's first terminal
            // is ground
            if (!gotGround && volt != null && !gotRail)
            {
                CircuitNode cn = new CircuitNode();
                Point pt = volt.getPost(0);
                cn.x = pt.X;
                cn.y = pt.Y;
                nodeList.Add(cn);
            }
            else
            {
                // otherwise allocate extra node for ground
                CircuitNode cn = new CircuitNode();
                cn.x = cn.y = -1;
                nodeList.Add(cn);
            }
            //System.out.println("ac2");

            // allocate nodes and voltage sources
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                int inodes = ce.InternalNodeCount;
                int ivs = ce.VoltageSourceCount;
                int posts = ce.PostCount;

                // allocate a node for each post and match posts to nodes
                for (j = 0; j != posts; j++)
                {
                    Point pt = ce.getPost(j);
                    int k;
                    for (k = 0; k != nodeList.Count; k++)
                    {
                        CircuitNode cn = getCircuitNode(k);
                        if (pt.X == cn.x && pt.Y == cn.y)
                            break;
                    }
                    if (k == nodeList.Count)
                    {
                        CircuitNode cn = new CircuitNode();
                        cn.x = pt.X;
                        cn.y = pt.Y;
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        cn.links.Add(cnl);
                        ce.setNode(j, nodeList.Count);
                        nodeList.Add(cn);
                    }
                    else
                    {
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        getCircuitNode(k).links.Add(cnl);
                        ce.setNode(j, k);
                        // if it's the ground node, make sure the node voltage is 0,
                        // cause it may not get set later
                        if (k == 0)
                            ce.setNodeVoltage(j, 0);
                    }
                }
                for (j = 0; j != inodes; j++)
                {
                    CircuitNode cn = new CircuitNode();
                    cn.x = cn.y = -1;
                    cn.Internal = true;
                    CircuitNodeLink cnl = new CircuitNodeLink();
                    cnl.num = j+posts;
                    cnl.elm = ce;
                    cn.links.Add(cnl);
                    ce.setNode(cnl.num, nodeList.Count);
                    nodeList.Add(cn);
                }
                vscount += ivs;
            }
            voltageSources = new CircuitElm[vscount];
            vscount = 0;
            circuitNonLinear = false;
            //System.out.println("ac3");

            // determine if circuit is nonlinear
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.nonLinear())
                    circuitNonLinear = true;
                int ivs = ce.VoltageSourceCount;
                for (j = 0; j != ivs; j++)
                {
                    voltageSources[vscount] = ce;
                    ce.setVoltageSource(j, vscount++);
                }
            }

            int matrixSize = nodeList.Count-1 + vscount;
            circuitMatrix = RectangularArrays.ReturnRectangularDoubleArray(matrixSize, matrixSize);
            circuitRightSide = new double[matrixSize];
            origMatrix = RectangularArrays.ReturnRectangularDoubleArray(matrixSize, matrixSize);
            origRightSide = new double[matrixSize];
            circuitMatrixSize = circuitMatrixFullSize = matrixSize;
            circuitRowInfo = new RowInfo[matrixSize];
            circuitPermute = new int[matrixSize];
            int vs = 0;
            for (i = 0; i != matrixSize; i++)
                circuitRowInfo[i] = new RowInfo();
            circuitNeedsMap = false;

            // stamp linear circuit elements
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.stamp();
            }
            //System.out.println("ac4");

            // determine nodes that are unconnected
            bool[] closure = new bool[nodeList.Count];
            bool[] tempclosure = new bool[nodeList.Count];
            bool changed = true;
            closure[0] = true;
            while (changed)
            {
                changed = false;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    // loop through all ce's nodes to see if they are connected
                    // to other nodes not in closure
                    for (j = 0; j < ce.PostCount; j++)
                    {
                        if (!closure[ce.getNode(j)])
                        {
                            if (ce.hasGroundConnection(j))
                                closure[ce.getNode(j)] = changed = true;
                            continue;
                        }
                        int k;
                        for (k = 0; k != ce.PostCount; k++)
                        {
                            if (j == k)
                                continue;
                            int kn = ce.getNode(k);
                            if (ce.getConnection(j, k) && !closure[kn])
                            {
                                closure[kn] = true;
                                changed = true;
                            }
                        }
                    }
                }
                if (changed)
                    continue;

                // connect unconnected nodes
                for (i = 0; i != nodeList.Count; i++)
                    if (!closure[i] && !getCircuitNode(i).Internal)
                    {
                        stampResistor(0, i, 1e8);
                        closure[i] = true;
                        changed = true;
                        break;
                    }
            }

            //_pathFinder.TryFindPath(elmList.ToArray());

            // simplify the matrix; this speeds things up quite a bit
            for (i = 0; i != matrixSize; i++)
            {
                int qm = -1, qp = -1;
                double qv = 0;
                RowInfo re = circuitRowInfo[i];
                if (re.lsChanges || re.dropRow || re.rsChanges)
                    continue;
                double rsadd = 0;

                // look for rows that can be removed
                for (j = 0; j != matrixSize; j++)
                {
                    double q = circuitMatrix[i][j];
                    if (circuitRowInfo[j].type == RowInfo.ROW_CONST)
                    {
                        // keep a running total of const values that have been
                        // removed already
                        rsadd -= circuitRowInfo[j].value*q;
                        continue;
                    }
                    if (Math.Abs(q - 0.0) < double.Epsilon)
                        continue;
                    if (qp == -1)
                    {
                        qp = j;
                        qv = q;
                        continue;
                    }
                    if (qm == -1 && Math.Abs(q - -qv) < double.Epsilon)
                    {
                        qm = j;
                        continue;
                    }
                    break;
                }
                
                if (j == matrixSize)
                {
                    if (qp == -1)
                    {
                        stop("Matrix error", null);
                        return;
                    }
                    RowInfo elt = circuitRowInfo[qp];
                    if (qm == -1)
                    {
                        // we found a row with only one nonzero entry; that value
                        // is a constant
                        int k;
                        for (k = 0; elt.type == RowInfo.ROW_EQUAL && k < 100; k++)
                        {
                            // follow the chain
                            qp = elt.nodeEq;
                            elt = circuitRowInfo[qp];
                        }
                        if (elt.type == RowInfo.ROW_EQUAL)
                        {
                            // break equal chains
                            //System.out.println("Break equal chain");
                            elt.type = RowInfo.ROW_NORMAL;
                            continue;
                        }
                        if (elt.type != RowInfo.ROW_NORMAL)
                        {
                            Console.WriteLine("type already " + elt.type + " for " + qp + "!");
                            continue;
                        }
                        elt.type = RowInfo.ROW_CONST;
                        elt.value = (circuitRightSide[i]+rsadd)/qv;
                        circuitRowInfo[i].dropRow = true;
                        i = -1; // start over from scratch
                    }
                    else if (Math.Abs(circuitRightSide[i]+rsadd - 0.0) < double.Epsilon)
                    {
                        // we found a row with only two nonzero entries, and one
                        // is the negative of the other; the values are equal
                        if (elt.type != RowInfo.ROW_NORMAL)
                        {
                            //System.out.println("swapping");
                            int qq = qm;
                            qm = qp;
                            qp = qq;
                            elt = circuitRowInfo[qp];
                            if (elt.type != RowInfo.ROW_NORMAL)
                            {
                                // we should follow the chain here, but this
                                // hardly ever happens so it's not worth worrying
                                // about
                                Console.WriteLine("swap failed");
                                continue;
                            }
                        }
                        elt.type = RowInfo.ROW_EQUAL;
                        elt.nodeEq = qm;
                        circuitRowInfo[i].dropRow = true;
                    }
                }
            }

            // find size of new matrix
            int nn = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo elt = circuitRowInfo[i];
                if (elt.type == RowInfo.ROW_NORMAL)
                {
                    elt.mapCol = nn++;
                    continue;
                }
                if (elt.type == RowInfo.ROW_EQUAL)
                {
                    RowInfo e2 = null;
                    // resolve chains of equality; 100 max steps to avoid loops
                    for (j = 0; j != 100; j++)
                    {
                        e2 = circuitRowInfo[elt.nodeEq];
                        if (e2.type != RowInfo.ROW_EQUAL)
                            break;
                        if (i == e2.nodeEq)
                            break;
                        elt.nodeEq = e2.nodeEq;
                    }
                }
                if (elt.type == RowInfo.ROW_CONST)
                    elt.mapCol = -1;
            }
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo elt = circuitRowInfo[i];
                if (elt.type == RowInfo.ROW_EQUAL)
                {
                    RowInfo e2 = circuitRowInfo[elt.nodeEq];
                    if (e2.type == RowInfo.ROW_CONST)
                    {
                        // if something is equal to a const, it's a const
                        elt.type = e2.type;
                        elt.value = e2.value;
                        elt.mapCol = -1;
                    }
                    else
                    {
                        elt.mapCol = e2.mapCol;
                    }
                }
            }
            // make the new, simplified matrix
            int newsize = nn;
            double[][] newmatx = RectangularArrays.ReturnRectangularDoubleArray(newsize, newsize);
            double[] newrs = new double[newsize];
            int ii = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo rri = circuitRowInfo[i];
                if (rri.dropRow)
                {
                    rri.mapRow = -1;
                    continue;
                }
                newrs[ii] = circuitRightSide[i];
                rri.mapRow = ii;
                for (j = 0; j != matrixSize; j++)
                {
                    RowInfo ri = circuitRowInfo[j];
                    if (ri.type == RowInfo.ROW_CONST)
                        newrs[ii] -= ri.value*circuitMatrix[i][j];
                    else
                        newmatx[ii][ri.mapCol] += circuitMatrix[i][j];
                }
                ii++;
            }

            circuitMatrix = newmatx;
            circuitRightSide = newrs;
            matrixSize = circuitMatrixSize = newsize;
            for (i = 0; i != matrixSize; i++)
                origRightSide[i] = circuitRightSide[i];
            for (i = 0; i != matrixSize; i++)
                for (j = 0; j != matrixSize; j++)
                    origMatrix[i][j] = circuitMatrix[i][j];
            circuitNeedsMap = true;
            
            // if a matrix is linear, we can do the lu_factor here instead of
            // needing to do it every frame
            if (!circuitNonLinear)
            {
                if (!_calculator.lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                {
                    stop("Singular matrix!", null);
                }
            }
        }


        internal virtual void calcCircuitBottom()
        {
            int i;
            circuitBottom = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                Rectangle rect = getElm(i).boundingBox;
                int bottom = rect.Height + rect.Y;
                if (bottom > circuitBottom)
                    circuitBottom = bottom;
            }
        }

        internal virtual void stop(string s, CircuitElm ce)
        {
            stopMessage = s;
            circuitMatrix = null;
            stopElm = ce;
            View.Parameters.IsStopped = true;
            analyzeFlag = false;
            _view.UpdateCanvas();
        }

        // control voltage source vs with voltage from n1 to n2 (must
        // also call stampVoltageSource())
        internal virtual void stampVCVS(int n1, int n2, double coef, int vs)
        {
            int vn = nodeList.Count+vs;
            stampMatrix(vn, n1, coef);
            stampMatrix(vn, n2, -coef);
        }

        // stamp independent voltage source #vs, from n1 to n2, amount v
        internal virtual void stampVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count+vs;
            stampMatrix(vn, n1, -1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn, v);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, -1);
        }

        // use this if the amount of voltage is going to be updated in doStep()
        internal virtual void stampVoltageSource(int n1, int n2, int vs)
        {
            int vn = nodeList.Count+vs;
            stampMatrix(vn, n1, -1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, -1);
        }

        internal virtual void updateVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count+vs;
            stampRightSide(vn, v);
        }

        internal virtual void stampResistor(int n1, int n2, double r)
        {
            double r0 = 1/r;
            if (double.IsNaN(r0) || double.IsInfinity(r0))
            {
                Console.Write("Nan or Infinity defected: " + r + " " + r0 + "\n");
            }
            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, -r0);
            stampMatrix(n2, n1, -r0);
        }

        internal virtual void stampConductance(int n1, int n2, double r0)
        {
            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, -r0);
            stampMatrix(n2, n1, -r0);
        }

        // current from cn1 to cn2 is equal to voltage from vn1 to 2, divided by g
        internal virtual void stampVCCurrentSource(int cn1, int cn2, int vn1, int vn2, double g)
        {
            stampMatrix(cn1, vn1, g);
            stampMatrix(cn2, vn2, g);
            stampMatrix(cn1, vn2, -g);
            stampMatrix(cn2, vn1, -g);
        }

        internal virtual void stampCurrentSource(int n1, int n2, double i)
        {
            stampRightSide(n1, -i);
            stampRightSide(n2, i);
        }

        // stamp a current source from n1 to n2 depending on current through vs
        internal virtual void stampCCCS(int n1, int n2, int vs, double gain)
        {
            int vn = nodeList.Count+vs;
            stampMatrix(n1, vn, gain);
            stampMatrix(n2, vn, -gain);
        }

        // stamp value x in row i, column j, meaning that a voltage change
        // of dv in node j will increase the current into node i by x dv.
        // (Unless i or j is a voltage source node.)
        internal virtual void stampMatrix(int i, int j, double x)
        {
            if (i > 0 && j > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i-1].mapRow;
                    RowInfo ri = circuitRowInfo[j-1];
                    if (ri.type == RowInfo.ROW_CONST)
                    {
                        //System.out.println("Stamping constant " + i + " " + j + " " + x);
                        circuitRightSide[i] -= x*ri.value;
                        return;
                    }
                    j = ri.mapCol;
                    //System.out.println("stamping " + i + " " + j + " " + x);
                }
                else
                {
                    i--;
                    j--;
                }
                circuitMatrix[i][j] += x;
            }
        }

        // stamp value x on the right side of row i, representing an
        // independent current source flowing into node i
        internal virtual void stampRightSide(int i, double x)
        {
            if (i > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i-1].mapRow;
                    //System.out.println("stamping " + i + " " + x);
                }
                else
                    i--;
                circuitRightSide[i] += x;
            }
        }

        // indicate that the value on the right side of row i changes in doStep()
        internal virtual void stampRightSide(int i)
        {
            //System.out.println("rschanges true " + (i-1));
            if (i > 0)
                circuitRowInfo[i-1].rsChanges = true;
        }

        // indicate that the values on the left side of row i change in doStep()
        internal virtual void stampNonLinear(int i)
        {
            if (i > 0)
                circuitRowInfo[i-1].lsChanges = true;
        }

        internal virtual double IterCount
        {
            get
            {
                if (View.Parameters.CurrentSpeed == 0)
                    return 0;
                //return (Math.exp((speedBar.getValue()-1)/24.) + .5);
                return.1*Math.Exp((View.Parameters.CurrentSpeed-61)/24.0);
            }
        }

        internal bool converged;
        internal int subIterations;
        internal virtual void runCircuit()
        {
            if (circuitMatrix == null || elmList.Count == 0)
            {
                circuitMatrix = null;
                return;
            }
            int iter;
            //int maxIter = getIterCount();
            long steprate = (long)(160*IterCount);
            long tm = DateTime.Now.Millisecond;
            long lit = lastIterTime;
            if (1000 >= steprate*(tm-lastIterTime))
                return;
            for (iter = 1; ; iter++)
            {
                int i, j, k, subiter;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.startIteration();
                }
                steps++;
                const int subiterCount = 5000;
                for (subiter = 0; subiter != subiterCount; subiter++)
                {
                    converged = true;
                    subIterations = subiter;
                    for (i = 0; i != circuitMatrixSize; i++)
                        circuitRightSide[i] = origRightSide[i];
                    if (circuitNonLinear)
                    {
                        for (i = 0; i != circuitMatrixSize; i++)
                            for (j = 0; j != circuitMatrixSize; j++)
                                circuitMatrix[i][j] = origMatrix[i][j];
                    }
                    for (i = 0; i != elmList.Count; i++)
                    {
                        CircuitElm ce = getElm(i);
                        ce.doStep();
                    }
                    if (stopMessage != null)
                        return;
                    for (j = 0; j != circuitMatrixSize; j++)
                    {
                        for (i = 0; i != circuitMatrixSize; i++)
                        {
                            double x = circuitMatrix[i][j];
                            if (double.IsNaN(x) || double.IsInfinity(x))
                            {
                                stop("nan/infinite matrix!", null);
                                return;
                            }
                        }
                    }
                    if (circuitNonLinear)
                    {
                        if (converged && subiter > 0)
                            break;
                        if (!_calculator.lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                        {
                            stop("Singular matrix!", null);
                            return;
                        }
                    }
                    _calculator.lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);

                    for (j = 0; j != circuitMatrixFullSize; j++)
                    {
                        RowInfo ri = circuitRowInfo[j];
                        double res = 0;
                        if (ri.type == RowInfo.ROW_CONST)
                            res = ri.value;
                        else
                            res = circuitRightSide[ri.mapCol];
//		    System.out.println(j + " " + res + " " +
//		      ri.type + " " + ri.mapCol);
                        if (double.IsNaN(res))
                        {
                            converged = false;
                            //debugprint = true;
                            break;
                        }
                        if (j < nodeList.Count-1)
                        {
                            CircuitNode cn = getCircuitNode(j+1);
                            for (k = 0; k != cn.links.Count; k++)
                            {
                                var cnl = (CircuitNodeLink) cn.links[k];
                                cnl.elm.setNodeVoltage(cnl.num, res);
                            }
                        }
                        else
                        {
                            int ji = j-(nodeList.Count-1);
                            //System.out.println("setting vsrc " + ji + " to " + res);
                            voltageSources[ji].setCurrent(ji, res);
                        }
                    }
                    if (!circuitNonLinear)
                        break;
                }
                if (subiter > 5)
                    Console.Write("converged after " + subiter + " iterations\n");
                if (subiter == subiterCount)
                {
                    stop("Convergence failed!", null);
                    break;
                }
                t += timeStep;
                for (i = 0; i != scopeCount; i++)
                    scopes[i].timeStep();
                tm = DateTime.Now.Millisecond;
                lit = tm;
                if (iter*1000 >= steprate*(tm-lastIterTime) || (tm-lastFrameTime > 500))
                    break;
            }
            lastIterTime = lit;
            //System.out.println((System.currentTimeMillis()-lastFrameTime)/(double) iter);
        }

        internal virtual int min(int a, int b)
        {
            return (a < b) ? a : b;
        }
        internal virtual int max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        //todo: replace into UI controls
        /*public virtual void actionPerformed(ActionEvent e)
        {
            string ac = e.ActionCommand;
            if (e.Source == exportItem)
                doImport(false, false);
            if (e.Source == optionsItem)
                doEdit(new EditOptions(this));
            if (e.Source == importItem)
                doImport(true, false);
            if (e.Source == exportLinkItem)
                doImport(false, true);
            if (e.Source == selectAllItem)
                doSelectAll();
            if (e.Source == exitItem)
            {
                destroyFrame();
                return;
            }
            if (String.CompareOrdinal(ac, "stackAll") == 0)
                stackAll();
            if (String.CompareOrdinal(ac, "unstackAll") == 0)
                unstackAll();
            if (e.Source == elmEditMenuItem)
                doEdit(menuElm);
            if (String.CompareOrdinal(ac, "РЈРґР°Р»РёС‚СЊ") == 0)
            {
                if (e.Source != elmDeleteMenuItem)
                    menuElm = null;
                doDelete();
            }
            if (e.Source == elmScopeMenuItem && menuElm != null)
            {
                int i;
                for (i = 0; i != scopeCount; i++)
                    if (scopes[i].elm == null)
                        break;
                if (i == scopeCount)
                {
                    if (scopeCount == scopes.Length)
                        return;
                    scopeCount++;
                    scopes[i] = new Scope(this);
                    scopes[i].position = i;
                    handleResize();
                }
                scopes[i].Elm = menuElm;
            }
            if (menuScope != -1)
            {
                if (String.CompareOrdinal(ac, "remove") == 0)
                    scopes[menuScope].Elm = null;
                if (String.CompareOrdinal(ac, "speed2") == 0)
                    scopes[menuScope].speedUp();
                if (String.CompareOrdinal(ac, "speed1/2") == 0)
                    scopes[menuScope].slowDown();
                if (String.CompareOrdinal(ac, "scale") == 0)
                    scopes[menuScope].adjustScale(.5);
                if (String.CompareOrdinal(ac, "maxscale") == 0)
                    scopes[menuScope].adjustScale(1e-50);
                if (String.CompareOrdinal(ac, "stack") == 0)
                    stackScope(menuScope);
                if (String.CompareOrdinal(ac, "unstack") == 0)
                    unstackScope(menuScope);
                if (String.CompareOrdinal(ac, "selecty") == 0)
                    scopes[menuScope].selectY();
                if (String.CompareOrdinal(ac, "reset") == 0)
                    scopes[menuScope].resetGraph();
                cv.repaint();
            }
            if (ac.IndexOf("setup ", StringComparison.Ordinal) == 0)
            {
                pushUndo();
                readSetupFile(ac.Substring(6), ((MenuItem) e.Source).Label);
            }
        }
        */
        
        protected virtual void stackScope(int s)
        {
            if (s == 0)
            {
                if (scopeCount < 2)
                    return;
                s = 1;
            }
            if (scopes[s].position == scopes[s-1].position)
                return;
            scopes[s].position = scopes[s-1].position;
            for (s++; s < scopeCount; s++)
                scopes[s].position--;
        }

        protected virtual void unstackScope(int s)
        {
            if (s == 0)
            {
                if (scopeCount < 2)
                    return;
                s = 1;
            }
            if (scopes[s].position != scopes[s-1].position)
                return;
            for (; s < scopeCount; s++)
                scopes[s].position++;
        }
        
        //todo: переделать вызов формы редактирования и саму форму
        /*internal virtual void doEdit(Editable eable)
        {
            clearSelection();
            pushUndo();
            if (editDialog != null)
            {
                requestFocus();
                editDialog.Visible = false;
                editDialog = null;
            }
            editDialog = new EditDialog(eable, this);
            editDialog.show();
        }*/

        //todo: переделать импорт
        /*protected virtual void doImport(bool imp, bool url)
        {
            if (impDialog != null)
            {
                requestFocus();
                impDialog.Visible = false;
                impDialog = null;
            }
            string dump = (imp) ? "" : dumpCircuit();
            if (url)
                dump = baseURL + "#" + URLEncoder.encode(dump);
            impDialog = new ImportDialog(this, dump, url);
            impDialog.show();
            pushUndo();
        }*/

        protected virtual string dumpCircuit()
        {
            int i;
            // 32 = linear scale in afilter
            string dump = "$ " + " " + timeStep + " " + IterCount + " " + View.Parameters.CurrentSpeed + " " +
                          CircuitElm.voltageRange + " " + View.Parameters.PowerLight + "\n";
            for (i = 0; i != elmList.Count; i++)
                dump += getElm(i).dump() + "\n";
            for (i = 0; i != scopeCount; i++)
            {
                string d = scopes[i].dump();
                if (d != null)
                    dump += d + "\n";
            }
            if (hintType != -1)
                dump += "h " + hintType + " " + hintItem1 + " " + hintItem2 + "\n";
            return dump;
        }
        
        internal virtual int snapGrid(int x)
        {
            return (x+gridRound) & gridMask;
        }

        protected virtual bool doSwitch(int x, int y)
        {
            if (mouseElm == null || !(mouseElm is SwitchElm))
                return false;
            var se = (SwitchElm) mouseElm;
            se.toggle();
            if (se.momentary)
                heldSwitchElm = se;
            needAnalyze();
            return true;
        }

        internal virtual int locateElm(CircuitElm elm)
        {
            int i;
            for (i = 0; i != elmList.Count; i++)
                if (elm == elmList[i])
                    return i;
            return -1;
        }

        //todo: перенести обработчики событий мыши в форму
        /*
        public virtual void mouseDragged(MouseEventArgs e)
        {
            // ignore right mouse button with no modifiers (needed on PC)
            if ((e.Modifiers & MouseEvent.BUTTON3_MASK) != 0)
            {
                int ex = e.ModifiersEx;
                if ((ex & (MouseEvent.META_DOWN_MASK| MouseEvent.SHIFT_DOWN_MASK| MouseEvent.CTRL_DOWN_MASK| MouseEvent.ALT_DOWN_MASK)) == 0)
                    return;
            }
            if (!circuitArea.Contains(e.X, e.Y))
                return;
            if (dragElm != null)
                dragElm.drag(e.X, e.Y);
            bool success = true;
            switch (tempMouseMode)
            {
                case MODE_DRAG_ALL:
                    dragAll(snapGrid(e.X), snapGrid(e.Y));
                    break;
                case MODE_DRAG_ROW:
                    dragRow(snapGrid(e.X), snapGrid(e.Y));
                    break;
                case MODE_DRAG_COLUMN:
                    dragColumn(snapGrid(e.X), snapGrid(e.Y));
                    break;
                case MODE_DRAG_POST:
                    if (mouseElm != null)
                        dragPost(snapGrid(e.X), snapGrid(e.Y));
                    break;
                case MODE_SELECT:
                    if (mouseElm == null)
                        selectArea(e.X, e.Y);
                    else
                    {
                        tempMouseMode = MODE_DRAG_SELECTED;
                        success = dragSelected(e.X, e.Y);
                    }
                    break;
                case MODE_DRAG_SELECTED:
                    success = dragSelected(e.X, e.Y);
                    break;
            }
            if (success)
            {
                if (tempMouseMode == MODE_DRAG_SELECTED && mouseElm is TextElm)
                {
                    dragX = e.X;
                    dragY = e.Y;
                }
                else
                {
                    dragX = snapGrid(e.X);
                    dragY = snapGrid(e.Y);
                }
            }
            cv.repaint(pause);
        }*/

        protected virtual void dragAll(int x, int y)
        {
            int dx = x-dragX;
            int dy = y-dragY;
            if (dx == 0 && dy == 0)
                return;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                var ce = getElm(i);
                ce.move(dx, dy);
            }
            removeZeroLengthElements();
        }

        protected virtual void dragRow(int x, int y)
        {
            int dy = y-dragY;
            if (dy == 0)
                return;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                var ce = getElm(i);
                if (ce.y == dragY)
                    ce.movePoint(0, 0, dy);
                if (ce.y2 == dragY)
                    ce.movePoint(1, 0, dy);
            }
            removeZeroLengthElements();
        }

        protected virtual void dragColumn(int x, int y)
        {
            int dx = x-dragX;
            if (dx == 0)
                return;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                var ce = getElm(i);
                if (ce.x == dragX)
                    ce.movePoint(0, dx, 0);
                if (ce.x2 == dragX)
                    ce.movePoint(1, dx, 0);
            }
            removeZeroLengthElements();
        }

        protected virtual bool dragSelected(int x, int y)
        {
            bool me = false;
            if (mouseElm != null && !mouseElm.isSelected)
                mouseElm.isSelected = me = true;

            // snap grid, unless we're only dragging text elements
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.isSelected && !(ce is TextElm))
                    break;
            }
            if (i != elmList.Count)
            {
                x = snapGrid(x);
                y = snapGrid(y);
            }

            int dx = x-dragX;
            int dy = y-dragY;
            if (dx == 0 && dy == 0)
            {
                // don't leave mouseElm selected if we selected it above
                if (me)
                    mouseElm.isSelected = false;
                return false;
            }
            bool allowed = true;

            // check if moves are allowed
            for (i = 0; allowed && i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.isSelected && !ce.allowMove(dx, dy))
                    allowed = false;
            }

            if (allowed)
            {
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    if (ce.isSelected)
                        ce.move(dx, dy);
                }
                needAnalyze();
            }

            // don't leave mouseElm selected if we selected it above
            if (me)
                mouseElm.isSelected = false;

            return allowed;
        }

        protected virtual void dragPost(int x, int y)
        {
            if (draggingPost == -1)
            {
                draggingPost = (distanceSq(mouseElm.x, mouseElm.y, x, y) > distanceSq(mouseElm.x2, mouseElm.y2, x, y)) ? 1 : 0;
            }
            int dx = x-dragX;
            int dy = y-dragY;
            if (dx == 0 && dy == 0)
                return;
            mouseElm.movePoint(draggingPost, dx, dy);
            needAnalyze();
        }

        protected virtual void selectArea(int x, int y)
        {
            int x1 = min(x, initDragX);
            int x2 = max(x, initDragX);
            int y1 = min(y, initDragY);
            int y2 = max(y, initDragY);
            selectedArea = new Rectangle(x1, y1, x2-x1, y2-y1);
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.selectRect(selectedArea);
            }
        }

        internal virtual CircuitElm SelectedElm
        {
            set
            {
                int i;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.isSelected = (ce == value);
                }
                mouseElm = value;
            }
        }

        public ISimulationView View
        {
            get { return _view; }
        }

        protected virtual void removeZeroLengthElements()
        {
            for (int i = elmList.Count-1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.x == ce.x2 && ce.y == ce.y2)
                {
                    elmList.RemoveAt(i);
                    ce.delete();
                }
            }
            needAnalyze();
        }

        protected virtual int distanceSq(int x1, int y1, int x2, int y2)
        {
            x2 -= x1;
            y2 -= y1;
            return x2*x2+y2*y2;
        }
        //todo: перенести обработку событий мыши в соответствующие контролы
        /*
        public virtual void mouseMoved(MouseEventArgs e)
        {
            if ((e.Modifiers & MouseEvent.BUTTON1_MASK) != 0)
                return;
            int x = e.X;
            int y = e.Y;
            dragX = snapGrid(x);
            dragY = snapGrid(y);
            draggingPost = -1;
            int i;
            CircuitElm origMouse = mouseElm;
            mouseElm = null;
            mousePost = -1;
            plotXElm = plotYElm = null;
            int bestDist = 100000;
            int bestArea = 100000;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.boundingBox.Contains(x, y))
                {
                    int j;
                    int area = ce.boundingBox.Width * ce.boundingBox.Height;
                    int jn = ce.PostCount;
                    if (jn > 2)
                        jn = 2;
                    for (j = 0; j != jn; j++)
                    {
                        Point pt = ce.getPost(j);
                        int dist = distanceSq(x, y, pt.X, pt.Y);

                        // if multiple elements have overlapping bounding boxes,
                        // we prefer selecting elements that have posts close
                        // to the mouse pointer and that have a small bounding
                        // box area.
                        if (dist <= bestDist && area <= bestArea)
                        {
                            bestDist = dist;
                            bestArea = area;
                            mouseElm = ce;
                        }
                    }
                    if (ce.PostCount == 0)
                        mouseElm = ce;
                }
            }
            scopeSelected = -1;
            if (mouseElm == null)
            {
                for (i = 0; i != scopeCount; i++)
                {
                    Scope s = scopes[i];
                    if (s.rect.Contains(x, y))
                    {
                        s.select();
                        scopeSelected = i;
                    }
                }
                // the mouse pointer was not in any of the bounding boxes, but we
                // might still be close to a post
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    int j;
                    int jn = ce.PostCount;
                    for (j = 0; j != jn; j++)
                    {
                        Point pt = ce.getPost(j);
                        int dist = distanceSq(x, y, pt.X, pt.Y);
                        if (distanceSq(pt.X, pt.Y, x, y) < 26)
                        {
                            mouseElm = ce;
                            mousePost = j;
                            break;
                        }
                    }
                }
            }
            else
            {
                mousePost = -1;
                // look for post close to the mouse pointer
                for (i = 0; i != mouseElm.PostCount; i++)
                {
                    Point pt = mouseElm.getPost(i);
                    if (distanceSq(pt.X, pt.Y, x, y) < 26)
                        mousePost = i;
                }
            }
            if (mouseElm != origMouse)
                cv.Refresh();
        }
        
        public virtual void mouseClicked(MouseEventArgs e)
        {
            if ((e.Modifiers & MouseEvent.BUTTON1_MASK) != 0)
            {
                if (mouseMode == MODE_SELECT || mouseMode == MODE_DRAG_SELECTED)
                    clearSelection();
            }
        }

        public virtual void mouseExited(MouseEventArgs e)
        {
            scopeSelected = -1;
            mouseElm = plotXElm = plotYElm = null;
            cv.Refresh();
        }

        public virtual void mousePressed(MouseEventArgs e)
        {
            Console.WriteLine(e.Modifiers);
            int ex = e.ModifiersEx;
            if ((ex & (MouseEvent.META_DOWN_MASK| MouseEvent.SHIFT_DOWN_MASK)) == 0 && e.PopupTrigger)
            {
                doPopupMenu(e);
                return;
            }
            if ((e.Modifiers & MouseEvent.BUTTON1_MASK) != 0)
            {
                // left mouse
                tempMouseMode = mouseMode;
                if ((ex & MouseEvent.ALT_DOWN_MASK) != 0 && (ex & MouseEvent.META_DOWN_MASK) != 0)
                    tempMouseMode = MODE_DRAG_COLUMN;
                else if ((ex & MouseEvent.ALT_DOWN_MASK) != 0 && (ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                    tempMouseMode = MODE_DRAG_ROW;
                else if ((ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                    tempMouseMode = MODE_SELECT;
                else if ((ex & MouseEvent.ALT_DOWN_MASK) != 0)
                    tempMouseMode = MODE_DRAG_ALL;
                else if ((ex & (MouseEvent.CTRL_DOWN_MASK| MouseEvent.META_DOWN_MASK)) != 0)
                    tempMouseMode = MODE_DRAG_POST;
            }
            else if ((e.Modifiers & MouseEvent.BUTTON3_MASK) != 0)
            {
                // right mouse
                if ((ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                    tempMouseMode = MODE_DRAG_ROW;
                else if ((ex & (MouseEvent.CTRL_DOWN_MASK| MouseEvent.META_DOWN_MASK)) != 0)
                    tempMouseMode = MODE_DRAG_COLUMN;
                else
                    return;
            }

            if (tempMouseMode != MODE_SELECT && tempMouseMode != MODE_DRAG_SELECTED)
                clearSelection();
            if (doSwitch(e.X, e.Y))
                return;
            pushUndo();
            initDragX = e.X;
            initDragY = e.Y;
            if (tempMouseMode != MODE_ADD_ELM || addingClass == null)
                return;

            int x0 = snapGrid(e.X);
            int y0 = snapGrid(e.Y);
            if (!circuitArea.Contains(x0, y0))
                return;

            dragElm = constructElement(addingClass, x0, y0);
        }
        */
        
        //todo: обработать события выбора меню в соответствующих контролах
        /*protected virtual void doPopupMenu(MouseEventArgs e)
        {
            menuElm = mouseElm;
            menuScope = -1;
            if (scopeSelected != -1)
            {
                PopupMenu m = scopes[scopeSelected].Menu;
                menuScope = scopeSelected;
                if (m != null)
                    m.show(e.Component, e.X, e.Y);
            }
            else if (mouseElm != null)
            {
                elmEditMenuItem.Enabled = mouseElm.getEditInfo(0) != null;
                elmScopeMenuItem.Enabled = mouseElm.canViewInScope();
                elmMenu.show(e.Component, e.X, e.Y);
            }
            else
            {
                doMainMenuChecks(mainMenu);
                mainMenu.Show(e.Component, e.X, e.Y);
            }
        }

        protected virtual void doMainMenuChecks(Menu m)
        {
            int i;
            if (m == optionsMenu)
                return;
            for (i = 0; i != m.ItemCount; i++)
            {
                MenuItem mc = m.getItem(i);
                if (mc is Menu)
                    doMainMenuChecks((Menu) mc);
                if (mc is CheckboxMenuItem)
                {
                    CheckboxMenuItem cmi = (CheckboxMenuItem) mc;
                    cmi.State = mouseModeStr.CompareTo(cmi.ActionCommand) == 0;
                }
            }
        }

        public virtual void mouseReleased(MouseEventArgs e)
        {
            int ex = e.ModifiersEx;
            if ((ex & (MouseEvent.SHIFT_DOWN_MASK|MouseEvent.CTRL_DOWN_MASK| MouseEvent.META_DOWN_MASK)) == 0 && e.PopupTrigger)
            {
                //doPopupMenu(e);
                return;
            }
            tempMouseMode = mouseMode;
            selectedArea = Rectangle.Empty;
            bool circuitChanged = false;
            if (heldSwitchElm != null)
            {
                heldSwitchElm.mouseUp();
                heldSwitchElm = null;
                circuitChanged = true;
            }
            if (dragElm != null)
            {
                // if the element is zero size then don't create it
                if (dragElm.x == dragElm.x2 && dragElm.y == dragElm.y2)
                    dragElm.delete();
                else
                {
                    elmList.Add(dragElm);
                    circuitChanged = true;
                }
                dragElm = null;
            }
            if (circuitChanged)
                needAnalyze();
            if (dragElm != null)
                dragElm.delete();
            dragElm = null;
            cv.Refresh();
        }*/

        protected virtual void enableItems()
        {
            //powerBar.Enabled = true;
            //powerLabel.Enabled = true;
        }

        public virtual void itemStateChanged(ToolStripItemEventArgs e)
        {
            //cv.repaint(pause);
            object mi = e.Item;
            
            if (menuScope != -1)
            {
                Scope sc = scopes[menuScope];
                //sc.handleMenu(e, mi);
            }
            if (mi is ToolStripMenuItem)
            {
                var mmi = mi as ToolStripMenuItem;
                mouseMode = MODE_ADD_ELM;
                string s = mmi.Text;
                if (s.Length > 0)
                    mouseModeStr = s;
                if (String.CompareOrdinal(s, "DragAll") == 0)
                    mouseMode = MODE_DRAG_ALL;
                else if (String.CompareOrdinal(s, "DragRow") == 0)
                    mouseMode = MODE_DRAG_ROW;
                else if (String.CompareOrdinal(s, "DragColumn") == 0)
                    mouseMode = MODE_DRAG_COLUMN;
                else if (String.CompareOrdinal(s, "DragSelected") == 0)
                    mouseMode = MODE_DRAG_SELECTED;
                else if (String.CompareOrdinal(s, "DragPost") == 0)
                    mouseMode = MODE_DRAG_POST;
                else if (String.CompareOrdinal(s, "Select") == 0)
                    mouseMode = MODE_SELECT;
                else if (s.Length > 0)
                {
                    try
                    {
                        addingClass = Type.GetType(s);
                    }
                    catch (Exception ex)
                    {
                        UserMessageView.Instance.ShowError(ex.StackTrace);
                    }
                }
                tempMouseMode = mouseMode;
            }
        }

        public void setGrid()
        {
            gridMask = ~(gridSize-1);
            gridRound = gridSize/2-1;
        }

        protected virtual void pushUndo()
        {
            redoStack.Clear();
            string s = dumpCircuit();
            if (undoStack.Count > 0 && String.CompareOrdinal(s, (string)(undoStack[undoStack.Count - 1])) == 0)
                return;
            undoStack.Add(s);
           
        }
        
        protected virtual void setMenuSelection()
        {
            if (menuElm != null)
            {
                if (menuElm.selected)
                    return;
                clearSelection();
                menuElm.selected = true;
            }
        }

        protected virtual void doCut()
        {
            pushUndo();
            setMenuSelection();
            clipboard = "";
            for (int i = elmList.Count-1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.selected)
                {
                    clipboard += ce.dump() + "\n";
                    ce.delete();
                    elmList.RemoveAt(i);
                }
            }
            needAnalyze();
        }

        protected virtual void doDelete()
        {
            pushUndo();
            setMenuSelection();
            for (int i = elmList.Count-1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.selected)
                {
                    ce.delete();
                    elmList.RemoveAt(i);
                }
            }
            needAnalyze();
        }

        protected virtual void doCopy()
        {
            clipboard = "";
            setMenuSelection();
            for (int i = elmList.Count-1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.selected)
                    clipboard += ce.dump() + "\n";
            }
        }
        
        protected virtual void doPaste()
        {
            pushUndo();
            clearSelection();
            
            Rectangle oldbb = Rectangle.Empty;
            for (int i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                Rectangle bb = ce.BoundingBox;
                if (oldbb != Rectangle.Empty)
                    oldbb = Rectangle.Union(oldbb, bb);
                else
                    oldbb = bb;
            }
            int oldsz = elmList.Count;
            //readSetup(clipboard, true);

            // select new items
            Rectangle newbb = Rectangle.Empty;
            for (int i = oldsz; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.selected = true;
                Rectangle bb = ce.BoundingBox;
                if (newbb != Rectangle.Empty)
                    newbb = Rectangle.Union(oldbb, bb);
                else
                    newbb = bb;
            }
            if (oldbb != Rectangle.Empty && 
                newbb != Rectangle.Empty && 
                oldbb.IntersectsWith(newbb))
            {
                // find a place for new items
                int dx = 0, dy = 0;
                int spacew = circuitArea.Width - oldbb.Width - newbb.Width;
                int spaceh = circuitArea.Height - oldbb.Height - newbb.Height;
                if (spacew > spaceh)
                    dx = snapGrid(oldbb.X + oldbb.Width - newbb.X + gridSize);
                else
                    dy = snapGrid(oldbb.Y + oldbb.Height - newbb.Y + gridSize);
                for (int i = oldsz; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.move(dx, dy);
                }
                // center circuit
                handleResize();
            }
            needAnalyze();
        }

        protected virtual void clearSelection()
        {
            for (int i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.selected = false;
            }
        }

        protected virtual void doSelectAll()
        {
            for (int i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.selected = true;
            }
        }
    }
}