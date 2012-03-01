// CirSim.java (c) 2010 by Paul Falstad

// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage

//Russian translation v1.0 by Spiritus, licrym@gmail.com http://licrym.org Please mail me for updates.
//codepage UTF-8
using System;
using System.IO;
using System.Windows.Forms;

//UPGRADE_TODO: Class 'java.awt.Frame' was converted to 'System.Windows.Forms.Form' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtFrame'"
namespace circuit_emulator
{
    [Serializable]
    public class CirSim:System.Windows.Forms.Form
    {
        static private System.Int32 state4;
        private static void  keyDown(System.Object event_sender, System.Windows.Forms.KeyEventArgs e)
        {
            state4 = ((int) System.Windows.Forms.Control.MouseButtons  | (int) System.Windows.Forms.Control.ModifierKeys);
        }
        private static void  mouseDown(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            state4 = ((int) e.Button | (int) System.Windows.Forms.Control.ModifierKeys);
        }
        virtual public System.String AppletInfo
        {
            get
            {
                return "Circuit by Paul Falstad";
            }
		
        }
        virtual internal System.String Hint
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
                    return "res.f = " + CircuitElm.getUnitText(1 / (2 * pi * System.Math.Sqrt(ie.inductance * ce.capacitance)), "Р“С†");
                }
                if (hintType == HINT_RC)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "RC = " + CircuitElm.getUnitText(re.resistance * ce.capacitance, "СЃ");
                }
                if (hintType == HINT_3DB_C)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText(1 / (2 * pi * re.resistance * ce.capacitance), "Р“С†");
                }
                if (hintType == HINT_3DB_L)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is InductorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    InductorElm ie = (InductorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText(re.resistance / (2 * pi * ie.inductance), "Р“С†");
                }
                if (hintType == HINT_TWINT)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "fc = " + CircuitElm.getUnitText(1 / (2 * pi * re.resistance * ce.capacitance), "Р“С†");
                }
                return null;
            }
		
        }
        virtual internal double IterCount
        {
            get
            {
                if (speedBar.Value == 0)
                    return 0;
                //return (Math.exp((speedBar.getValue()-1)/24.) + .5);
                return .1 * System.Math.Exp((speedBar.Value - 61) / 24.0);
            }
		
        }
        virtual internal System.Uri CodeBase
        {
            get
            {
                try
                {
                    if (applet != null)
                    {
                        //UPGRADE_TODO: The equivalent in .NET for method 'java.applet.Applet.getCodeBase' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                        return new System.Uri(System.IO.Directory.GetCurrentDirectory());
                    }
                    System.IO.FileInfo f = new System.IO.FileInfo(".");
                    //UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
                    return new System.Uri("file:" + f.FullName + "/");
                }
                catch (System.Exception e)
                {
                    SupportClass.WriteStackTrace(e, Console.Error);
                    return null;
                }
            }
		
        }
        virtual internal CircuitElm SelectedElm
        {
            set
            {
                int i;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.Selected = ce == value;
                }
                mouseElm = value;
            }
		
        }
	
        internal SupportClass.ThreadClass engine = null;
	
        internal System.Drawing.Size winSize
        {
            get
            {
                return winSize_Renamed;
            }
		
            set
            {
                winSize_Renamed = value;
            }
		
        }
        internal System.Drawing.Size winSize_Renamed;
        internal System.Drawing.Image dbimage;
	
        internal System.Random random;
        public const int sourceRadius = 7;
        public const double freqMult = 3.14159265 * 2 * 4;
	
        internal static System.Windows.Forms.Control main;
        internal System.Windows.Forms.Label titleLabel;
        internal System.Windows.Forms.Button resetButton;
        internal System.Windows.Forms.Button dumpMatrixButton;
        internal System.Windows.Forms.MenuItem exportItem, exportLinkItem, importItem, exitItem, undoItem, redoItem, cutItem, copyItem, pasteItem, selectAllItem, optionsItem;
        internal System.Windows.Forms.MenuItem optionsMenu;
        internal System.Windows.Forms.CheckBox stoppedCheck;
        internal System.Windows.Forms.MenuItem dotsCheckItem;
        internal System.Windows.Forms.MenuItem voltsCheckItem;
        internal System.Windows.Forms.MenuItem powerCheckItem;
        internal System.Windows.Forms.MenuItem smallGridCheckItem;
        internal System.Windows.Forms.MenuItem showValuesCheckItem;
        internal System.Windows.Forms.MenuItem conductanceCheckItem;
        internal System.Windows.Forms.MenuItem euroResistorCheckItem;
        internal System.Windows.Forms.MenuItem printableCheckItem;
        internal System.Windows.Forms.MenuItem conventionCheckItem;
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar speedBar;
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar currentBar;
        internal System.Windows.Forms.Label powerLabel;
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar powerBar;
        internal System.Windows.Forms.ContextMenu elmMenu;
        internal System.Windows.Forms.MenuItem elmEditMenuItem;
        internal System.Windows.Forms.MenuItem elmCutMenuItem;
        internal System.Windows.Forms.MenuItem elmCopyMenuItem;
        internal System.Windows.Forms.MenuItem elmDeleteMenuItem;
        internal System.Windows.Forms.MenuItem elmScopeMenuItem;
        internal System.Windows.Forms.ContextMenu scopeMenu;
        internal System.Windows.Forms.ContextMenu transScopeMenu;
        internal System.Windows.Forms.ContextMenu mainMenu;
        internal System.Windows.Forms.MenuItem scopeVMenuItem;
        internal System.Windows.Forms.MenuItem scopeIMenuItem;
        internal System.Windows.Forms.MenuItem scopeMaxMenuItem;
        internal System.Windows.Forms.MenuItem scopeMinMenuItem;
        internal System.Windows.Forms.MenuItem scopeFreqMenuItem;
        internal System.Windows.Forms.MenuItem scopePowerMenuItem;
        internal System.Windows.Forms.MenuItem scopeIbMenuItem;
        internal System.Windows.Forms.MenuItem scopeIcMenuItem;
        internal System.Windows.Forms.MenuItem scopeIeMenuItem;
        internal System.Windows.Forms.MenuItem scopeVbeMenuItem;
        internal System.Windows.Forms.MenuItem scopeVbcMenuItem;
        internal System.Windows.Forms.MenuItem scopeVceMenuItem;
        internal System.Windows.Forms.MenuItem scopeVIMenuItem;
        internal System.Windows.Forms.MenuItem scopeXYMenuItem;
        internal System.Windows.Forms.MenuItem scopeResistMenuItem;
        internal System.Windows.Forms.MenuItem scopeVceIcMenuItem;
        internal System.Windows.Forms.MenuItem scopeSelectYMenuItem;
        internal System.Type addingClass;
        internal int mouseMode = MODE_SELECT;
        internal int tempMouseMode = MODE_SELECT;
        internal System.String mouseModeStr = "Select";
        internal const double pi = 3.14159265358979323846;
        internal const int MODE_ADD_ELM = 0;
        internal const int MODE_DRAG_ALL = 1;
        internal const int MODE_DRAG_ROW = 2;
        internal const int MODE_DRAG_COLUMN = 3;
        internal const int MODE_DRAG_SELECTED = 4;
        internal const int MODE_DRAG_POST = 5;
        internal const int MODE_SELECT = 6;
        internal const int infoWidth = 120;
        internal int dragX, dragY, initDragX, initDragY;
        internal int selectedSource;
        internal System.Drawing.Rectangle selectedArea
        {
            get
            {
                return selectedArea_Renamed;
            }
		
            set
            {
                selectedArea_Renamed = value;
            }
		
        }
        internal System.Drawing.Rectangle selectedArea_Renamed;
        internal int gridSize, gridMask, gridRound;
        internal bool dragging;
        internal bool analyzeFlag;
        internal bool dumpMatrix;
        internal bool useBufferedImage;
        internal bool isMac;
        internal System.String ctrlMetaKey;
        internal double t;
        internal int pause = 10;
        internal int scopeSelected = - 1;
        internal int menuScope = - 1;
        internal int hintType = - 1, hintItem1, hintItem2;
        internal System.String stopMessage;
        internal double timeStep;
        internal const int HINT_LC = 1;
        internal const int HINT_RC = 2;
        internal const int HINT_3DB_C = 3;
        internal const int HINT_TWINT = 4;
        internal const int HINT_3DB_L = 5;
        internal System.Collections.ArrayList elmList;
        internal System.Collections.ArrayList setupList;
        internal CircuitElm dragElm, menuElm, mouseElm, stopElm;
        internal int mousePost = - 1;
        internal CircuitElm plotXElm, plotYElm;
        internal int draggingPost;
        internal SwitchElm heldSwitchElm;
        internal double[][] circuitMatrix;
        internal double[] circuitRightSide, origRightSide;
        internal double[][] origMatrix;
        internal RowInfo[] circuitRowInfo;
        internal int[] circuitPermute;
        internal bool circuitNonLinear;
        internal int voltageSourceCount;
        internal int circuitMatrixSize, circuitMatrixFullSize;
        internal bool circuitNeedsMap;
        public bool useFrame;
        internal int scopeCount;
        internal Scope[] scopes;
        internal int[] scopeColCount;
        internal static EditDialog editDialog;
        internal static ImportDialog impDialog;
        internal System.Type[] dumpTypes;
        internal static System.String muString = "мк";
        internal static System.String ohmString = "Ом";
        internal System.String clipboard;
        internal System.Drawing.Rectangle circuitArea
        {
            get
            {
                return circuitArea_Renamed;
            }
		
            set
            {
                circuitArea_Renamed = value;
            }
		
        }
        internal System.Drawing.Rectangle circuitArea_Renamed;
        internal int circuitBottom;
        internal System.Collections.ArrayList undoStack, redoStack;
	
        internal virtual int getrand(int x)
        {
            //UPGRADE_TODO: Method 'java.util.Random.nextInt' was converted to 'System.Random.Next' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
            int q = random.Next();
            if (q < 0)
                q = - q;
            return q % x;
        }
        internal CircuitCanvas cv;
        internal circuit_emulator.Circuit applet;
	
        //UPGRADE_TODO: Class 'java.awt.Frame' was converted to 'System.Windows.Forms.Form' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtFrame'"
        internal CirSim(circuit_emulator.Circuit a):base()
        {
            this.Text = "Circuit Simulator v1.5n";
            applet = a;
            useFrame = false;
            speedBar = new HScrollBar();
            powerBar = new HScrollBar();
            
        }
	
        internal System.String startCircuit = null;
        internal System.String startLabel = null;
        internal System.String startCircuitText = null;
        internal System.String baseURL = "http://www.falstad.com/circuit/";
	
        public virtual void  init()
        {
            System.String euroResistor = null;
            System.String useFrameStr = null;
            bool printable = false;
            bool convention = true;
		
            CircuitElm.initClass(this);
		
            try
            {
                //UPGRADE_TODO: Method 'java.applet.Applet.getDocumentBase' was converted to 'DocumentBase' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaappletAppletgetDocumentBase'"
                baseURL = applet.DocumentBase.PathAndQuery;
                // look for circuit embedded in URL
                //UPGRADE_TODO: Method 'java.applet.Applet.getDocumentBase' was converted to 'DocumentBase' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaappletAppletgetDocumentBase'"
                System.String doc = applet.DocumentBase.ToString();
                int in_Renamed = doc.IndexOf('#');
                if (in_Renamed > 0)
                {
                    System.String x = null;
                    try
                    {
                        x = doc.Substring(in_Renamed + 1);
                        x = System.Web.HttpUtility.UrlDecode(x);
                        startCircuitText = x;
                    }
                    catch (System.Exception e)
                    {
                        System.Console.Out.WriteLine("can't decode " + x);
                        SupportClass.WriteStackTrace(e, Console.Error);
                    }
                }
                in_Renamed = doc.LastIndexOf('/');
                if (in_Renamed > 0)
                    baseURL = doc.Substring(0, (in_Renamed + 1) - (0));
			
                System.String param = applet.getParameter("PAUSE");
                if (param != null)
                    pause = System.Int32.Parse(param);
                startCircuit = applet.getParameter("startCircuit");
                startLabel = applet.getParameter("startLabel");
                euroResistor = applet.getParameter("euroResistors");
                useFrameStr = applet.getParameter("useFrame");
                System.String x2 = applet.getParameter("whiteBackground");
                if (x2 != null && x2.ToUpper().Equals("true".ToUpper()))
                    printable = true;
                x2 = applet.getParameter("conventionalCurrent");
                if (x2 != null && x2.ToUpper().Equals("true".ToUpper()))
                    convention = false;
            }
            catch (System.Exception e)
            {
            }
		
            bool euro = (euroResistor != null && euroResistor.ToUpper().Equals("true".ToUpper()));
            useFrame = (useFrameStr == null || !useFrameStr.ToUpper().Equals("false".ToUpper()));
            if (useFrame)
                main = this;
            else
                main = applet;
		
            //UPGRADE_TODO: Method 'java.lang.System.getProperty' was converted to 'System.Environment.GetEnvironmentVariable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangSystemgetProperty_javalangString'"
            System.String os = System.Environment.GetEnvironmentVariable("OS");
            isMac = (os.IndexOf("Mac ") == 0);
            ctrlMetaKey = (isMac)?"\u2318":"Ctrl";
            //UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangSystem'"
            //System.String jv = System_Renamed.getProperty("java.class.version");
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            //double jvf = System.Double.Parse(jv);
            //if (jvf >= 48)
            //{
            //    muString = "РјРє";
                muString = "мк";
            //    ohmString = "\u03a9";
            //    useBufferedImage = true;
            //}
		
            dumpTypes = new System.Type[300];
            // these characters are reserved
            dumpTypes[(int) 'o'] = typeof(Scope);
            dumpTypes[(int) 'h'] = typeof(Scope);
            dumpTypes[(int) '$'] = typeof(Scope);
            dumpTypes[(int) '%'] = typeof(Scope);
            dumpTypes[(int) '?'] = typeof(Scope);
            dumpTypes[(int) 'B'] = typeof(Scope);
		
            //UPGRADE_ISSUE: Method 'java.awt.Container.setLayout' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtContainersetLayout_javaawtLayoutManager'"
            //todo revert comment: main.setLayout(new CircuitLayout());
            cv = new CircuitCanvas(this);
            //UPGRADE_WARNING: Extra logic should be included into componentHidden to know if the Component is hidden. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1144'"
            cv.VisibleChanged += new System.EventHandler(this.componentHidden);
            cv.Move += new System.EventHandler(this.componentMoved);
            cv.Resize += new System.EventHandler(this.componentResized);
            //UPGRADE_WARNING: Extra logic should be included into componentShown to know if the Component is visible. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1145'"
            cv.VisibleChanged += new System.EventHandler(this.componentShown);
            cv.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMoved);
            cv.MouseDown += new System.Windows.Forms.MouseEventHandler(CirSim.mouseDown);
            cv.Click += new System.EventHandler(this.mouseClicked);
            cv.MouseEnter += new System.EventHandler(this.mouseEntered);
            cv.MouseLeave += new System.EventHandler(this.mouseExited);
            cv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mousePressed);
            cv.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mouseReleased);
            cv.KeyDown += new System.Windows.Forms.KeyEventHandler(CirSim.keyDown);
            cv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyPressed);
            cv.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyReleased);
            cv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.keyTyped);
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            main.Controls.Add(cv);
		
            mainMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MainMenu mb = null;
            if (useFrame)
                mb = new System.Windows.Forms.MainMenu();
            System.Windows.Forms.MenuItem m = new System.Windows.Forms.MenuItem("Файл");
            if (useFrame)
                mb.MenuItems.Add(m);
            else
                mainMenu.MenuItems.Add(m);
            m.MenuItems.Add(importItem = getMenuItem("Импорт"));
            m.MenuItems.Add(exportItem = getMenuItem("Экспорт"));
            m.MenuItems.Add(exportLinkItem = getMenuItem("Экспорт. ссылку"));
            m.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            m.MenuItems.Add(exitItem = getMenuItem("Выход"));
		
            m = new System.Windows.Forms.MenuItem("Правка");
            m.MenuItems.Add(undoItem = getMenuItem("Отменить"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut;
            temp_Shortcut = new System.Windows.Forms.Shortcut();
            temp_Shortcut = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.Z + 131072);
            undoItem.Shortcut = temp_Shortcut;
            m.MenuItems.Add(redoItem = getMenuItem("Повторить"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut2;
            temp_Shortcut2 = new System.Windows.Forms.Shortcut();
            temp_Shortcut2 = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.Z + 196608);
            redoItem.Shortcut = temp_Shortcut2;
            m.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            m.MenuItems.Add(cutItem = getMenuItem("Вырезать"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut3;
            temp_Shortcut3 = new System.Windows.Forms.Shortcut();
            temp_Shortcut3 = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.X + 131072);
            cutItem.Shortcut = temp_Shortcut3;
            m.MenuItems.Add(copyItem = getMenuItem("Копировать"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut4;
            temp_Shortcut4 = new System.Windows.Forms.Shortcut();
            temp_Shortcut4 = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.C + 131072);
            copyItem.Shortcut = temp_Shortcut4;
            m.MenuItems.Add(pasteItem = getMenuItem("Вставить"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut5;
            temp_Shortcut5 = new System.Windows.Forms.Shortcut();
            temp_Shortcut5 = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.V + 131072);
            pasteItem.Shortcut = temp_Shortcut5;
            pasteItem.Enabled = false;
            m.MenuItems.Add(selectAllItem = getMenuItem("Выбрать всё"));
            //UPGRADE_TODO: The equivalent in .NET for constructor 'java.awt.MenuShortcut.MenuShortcut' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Windows.Forms.Shortcut temp_Shortcut6;
            temp_Shortcut6 = new System.Windows.Forms.Shortcut();
            temp_Shortcut6 = (System.Windows.Forms.Shortcut) ((int) System.Windows.Forms.Keys.A + 131072);
            selectAllItem.Shortcut = temp_Shortcut6;
            if (useFrame)
                mb.MenuItems.Add(m);
            else
                mainMenu.MenuItems.Add(m);
		
            m = new System.Windows.Forms.MenuItem("Осциллограф");
            if (useFrame)
                mb.MenuItems.Add(m);
            else
                mainMenu.MenuItems.Add(m);
            m.MenuItems.Add(getMenuItem("Объединить всё", "stackAll"));
            m.MenuItems.Add(getMenuItem("Разъединить всё", "unstackAll"));
		
            optionsMenu = m = new System.Windows.Forms.MenuItem("Настройки");
            if (useFrame)
                mb.MenuItems.Add(m);
            else
                mainMenu.MenuItems.Add(m);
            m.MenuItems.Add(dotsCheckItem = getCheckItem("Показывать ток"));
            dotsCheckItem.Checked = true;
            m.MenuItems.Add(voltsCheckItem = getCheckItem("Показывать напряжение"));
            voltsCheckItem.Checked = true;
            m.MenuItems.Add(powerCheckItem = getCheckItem("Показывать мощность"));
            m.MenuItems.Add(showValuesCheckItem = getCheckItem("Показывать значения"));
            showValuesCheckItem.Checked = true;
            //m.add(conductanceCheckItem = getCheckItem("Show Conductance"));
            m.MenuItems.Add(smallGridCheckItem = getCheckItem("Мелкая сетка"));
            m.MenuItems.Add(euroResistorCheckItem = getCheckItem("Резисторы по ГОСТ")); //must be default in russia
            euroResistorCheckItem.Checked = euro;
            m.MenuItems.Add(printableCheckItem = getCheckItem("Белый фон"));
            printableCheckItem.Checked = printable;
            m.MenuItems.Add(conventionCheckItem = getCheckItem("Общепринятое направление тока"));
            conventionCheckItem.Checked = convention;
            m.MenuItems.Add(optionsItem = getMenuItem("Другие настройки..."));
		
            System.Windows.Forms.MenuItem circuitsMenu = new System.Windows.Forms.MenuItem("Схемы");
            if (useFrame)
                mb.MenuItems.Add(circuitsMenu);
            else
                mainMenu.MenuItems.Add(circuitsMenu);
		
            mainMenu.MenuItems.Add(getClassCheckItem("Добавить Соединение", "WireElm"));
            mainMenu.MenuItems.Add(getClassCheckItem("Добавить Резистор", "ResistorElm"));
		
            System.Windows.Forms.MenuItem passMenu = new System.Windows.Forms.MenuItem("Пассивные компоненты");
            mainMenu.MenuItems.Add(passMenu);
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Конденсатор", "CapacitorElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Индуктивность", "InductorElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Выключатель", "SwitchElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Кнопочный выключатель", "PushSwitchElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Переключатель", "Switch2Elm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Переменное сопротивление", "PotElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Трансформатор", "TransformerElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Трансформатор с отводом", "TappedTransformerElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Линию передачи", "TransLineElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Реле", "RelayElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Мемристор", "MemristorElm"));
            passMenu.MenuItems.Add(getClassCheckItem("Добавить Искровой промежуток", "SparkGapElm"));
		
            System.Windows.Forms.MenuItem inputMenu = new System.Windows.Forms.MenuItem("Входы/Выходы");
            mainMenu.MenuItems.Add(inputMenu);
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Заземление", "GroundElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Ист. постоянного тока (2-вывода)", "DCVoltageElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Ист. переменного тока (2-вывода)", "ACVoltageElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Ист. напряжения (1-вывод)", "RailElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Ист. переменного тока (1-вывод)", "ACRailElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Ист. Прямоуг. напряжения (1-вывод)", "SquareRailElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Аналоговый выход", "OutputElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Логический вход", "LogicInputElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Логический выход", "LogicOutputElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Тактовые импульсы", "ClockElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Свип", "SweepElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Регулируемое напряжение", "VarRailElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Антенну", "AntennaElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Источник тока", "CurrentElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Светодиод", "LEDElm"));
            inputMenu.MenuItems.Add(getClassCheckItem("Добавить Лампу (beta)", "LampElm"));
		
            System.Windows.Forms.MenuItem activeMenu = new System.Windows.Forms.MenuItem("Активные компоненты");
            mainMenu.MenuItems.Add(activeMenu);
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Диод", "DiodeElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Стабилитрон", "ZenerElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Транзистор (биполярный, NPN)", "NTransistorElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Транзистор (биполярный, PNP)", "PTransistorElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Операционный усилитель (- вверху)", "OpAmpElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Операционный усилитель (+ вверху)", "OpAmpSwapElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить MOSFET (n-канальный)", "NMosfetElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить MOSFET (p-канальный)", "PMosfetElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Полевой транзистор (n-канальный)", "NJfetElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Полевой транзистор (p-канальный)", "PJfetElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Аналоговый выключатель", "AnalogSwitchElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Аналоговый переключатель", "AnalogSwitch2Elm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Тиристор", "SCRElm"));
            //activeMenu.add(getClassCheckItem("Добавить Варикап", "VaractorElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Туннельный диод", "TunnelDiodeElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить Триод", "TriodeElm"));
            //activeMenu.add(getClassCheckItem("Добавить Динистор", "DiacElm"));
            //activeMenu.add(getClassCheckItem("Добавить Симистор", "TriacElm"));
            //activeMenu.add(getClassCheckItem("Добавить Фоторезистор", "PhotoResistorElm"));
            //activeMenu.add(getClassCheckItem("Добавить Термистор", "ThermistorElm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить CCII+", "CC2Elm"));
            activeMenu.MenuItems.Add(getClassCheckItem("Добавить CCII-", "CC2NegElm"));
		
            System.Windows.Forms.MenuItem gateMenu = new System.Windows.Forms.MenuItem("Логические элементы");
            mainMenu.MenuItems.Add(gateMenu);
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить Инвертор", "InverterElm"));
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить элемент И-НЕ", "NandGateElm"));
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить элемент ИЛИ-НЕ", "NorGateElm"));
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить элемент И", "AndGateElm"));
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить элемент ИЛИ", "OrGateElm"));
            gateMenu.MenuItems.Add(getClassCheckItem("Добавить элемент исключающее ИЛИ", "XorGateElm"));
		
            System.Windows.Forms.MenuItem chipMenu = new System.Windows.Forms.MenuItem("Микросхемы");
            mainMenu.MenuItems.Add(chipMenu);
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить D триггер", "DFlipFlopElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить JK триггер", "JKFlipFlopElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить 7ми сегментный светодиод", "SevenSegElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить VCO", "VCOElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить Фазовый компаратор", "PhaseCompElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить Счетчик", "CounterElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить Декадный счетчик", "DecadeElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить 555 Таймер", "TimerElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить ЦАП", "DACElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить АЦП", "ADCElm"));
            chipMenu.MenuItems.Add(getClassCheckItem("Добавить Защелку", "LatchElm"));
		
            System.Windows.Forms.MenuItem otherMenu = new System.Windows.Forms.MenuItem("Прочее");
            mainMenu.MenuItems.Add(otherMenu);
            otherMenu.MenuItems.Add(getClassCheckItem("Добавить Текст", "TextElm"));
            otherMenu.MenuItems.Add(getClassCheckItem("Добавить пробу осциллографа", "ProbeElm"));
            otherMenu.MenuItems.Add(getCheckItem("Drag All (Alt-drag)", "DragAll"));
            otherMenu.MenuItems.Add(getCheckItem(isMac?"Drag Row (Alt-S-drag, S-right)":"Drag Row (S-right)", "DragRow"));
            otherMenu.MenuItems.Add(getCheckItem(isMac?"Drag Column (Alt-\u2318-drag, \u2318-right)":"Drag Column (C-right)", "DragColumn"));
            otherMenu.MenuItems.Add(getCheckItem("Drag Selected", "DragSelected"));
            otherMenu.MenuItems.Add(getCheckItem("Drag Post (" + ctrlMetaKey + "-drag)", "DragPost"));
		
            mainMenu.MenuItems.Add(getCheckItem("Выбор/перетаскивание выбранного (пробел или Shift+щелчок)", "Select"));
            main.ContextMenu = mainMenu;
		
            System.Windows.Forms.Button temp_Button2;
            temp_Button2 = new System.Windows.Forms.Button();
            temp_Button2.Text = "Сброс";
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control;
            temp_Control = resetButton = temp_Button2;
            main.Controls.Add(temp_Control);
            resetButton.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(resetButton);
            System.Windows.Forms.Button temp_Button3;
            temp_Button3 = new System.Windows.Forms.Button();
            temp_Button3.Text = "Dump Matrix";
            dumpMatrixButton = temp_Button3;
            //main.add(dumpMatrixButton);
            dumpMatrixButton.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(dumpMatrixButton);
            stoppedCheck = SupportClass.CheckBoxSupport.CreateCheckBox("Остановлено");
            stoppedCheck.Click += new System.EventHandler(this.itemStateChanged);
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            main.Controls.Add(stoppedCheck);
		
            System.Windows.Forms.Label temp_Label2;
            //UPGRADE_TODO: The equivalent in .NET for field 'java.awt.Label.CENTER' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            temp_Label2 = new System.Windows.Forms.Label();
            temp_Label2.Text = "";
            temp_Label2.TextAlign = (System.Drawing.ContentAlignment) System.Drawing.ContentAlignment.MiddleCenter;
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control2;
            temp_Control2 = temp_Label2;
            main.Controls.Add(temp_Control2);
		
            // was max of 140
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control3;
            temp_Control3 = null;
            temp_Control3 = "Скорость симуляции";
            main.Controls.Add(temp_Control3);
            //UPGRADE_TODO: Method 'java.awt.Scrollbar.addAdjustmentListener' was converted to 'System.Windows.Forms.ScrollEventArgs' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtScrollbaraddAdjustmentListener_javaawteventAdjustmentListener'"
            speedBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.adjustmentValueChanged);
		
            System.Windows.Forms.Label temp_Label4;
            //UPGRADE_TODO: The equivalent in .NET for field 'java.awt.Label.CENTER' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            temp_Label4 = new System.Windows.Forms.Label();
            temp_Label4.Text = "Скорость тока";
            temp_Label4.TextAlign = (System.Drawing.ContentAlignment) System.Drawing.ContentAlignment.MiddleCenter;
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control4;
            temp_Control4 = temp_Label4;
            main.Controls.Add(temp_Control4);
            currentBar = new System.Windows.Forms.HScrollBar();
            currentBar.Value = 50;
            currentBar.Minimum = 1;
            currentBar.Maximum = 100;
            //UPGRADE_TODO: Method 'java.awt.Scrollbar.addAdjustmentListener' was converted to 'System.Windows.Forms.ScrollEventArgs' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtScrollbaraddAdjustmentListener_javaawteventAdjustmentListener'"
            currentBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.adjustmentValueChanged);
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            main.Controls.Add(currentBar);
		
            System.Windows.Forms.Label temp_Label6;
            //UPGRADE_TODO: The equivalent in .NET for field 'java.awt.Label.CENTER' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            temp_Label6 = new System.Windows.Forms.Label();
            temp_Label6.Text = "Яркость мощности";
            temp_Label6.TextAlign = (System.Drawing.ContentAlignment) System.Drawing.ContentAlignment.MiddleCenter;
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control5;
            temp_Control5 = powerLabel = temp_Label6;
            main.Controls.Add(temp_Control5);
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control6;
            temp_Control6 = null;
            main.Controls.Add(temp_Control6);
            //UPGRADE_TODO: Method 'java.awt.Scrollbar.addAdjustmentListener' was converted to 'System.Windows.Forms.ScrollEventArgs' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtScrollbaraddAdjustmentListener_javaawteventAdjustmentListener'"
            powerBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.adjustmentValueChanged);
            powerBar.Enabled = false;
            powerLabel.Enabled = false;
		
            System.Windows.Forms.Label temp_Label8;
            temp_Label8 = new System.Windows.Forms.Label();
            temp_Label8.Text = "www.falstad.com";
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control7;
            temp_Control7 = temp_Label8;
            main.Controls.Add(temp_Control7);
            System.Windows.Forms.Label temp_Label10;
            temp_Label10 = new System.Windows.Forms.Label();
            temp_Label10.Text = "Перевод licrym.org";
            //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
            System.Windows.Forms.Control temp_Control8;
            temp_Control8 = temp_Label10;
            main.Controls.Add(temp_Control8); //"translated into russian by licrym.org"
		
            if (useFrame)
            {
                System.Windows.Forms.Label temp_Label12;
                temp_Label12 = new System.Windows.Forms.Label();
                temp_Label12.Text = "";
                //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
                System.Windows.Forms.Control temp_Control9;
                temp_Control9 = temp_Label12;
                main.Controls.Add(temp_Control9);
            }
            //UPGRADE_NOTE: If the given Font Name does not exist, a default Font instance is created. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1075'"
            System.Drawing.Font f = new System.Drawing.Font("SansSerif", 10, System.Drawing.FontStyle.Regular);
            System.Windows.Forms.Label l;
            System.Windows.Forms.Label temp_Label13;
            temp_Label13 = new System.Windows.Forms.Label();
            temp_Label13.Text = "Текущая схема:";
            l = temp_Label13;
            l.Font = f;
            System.Windows.Forms.Label temp_Label14;
            temp_Label14 = new System.Windows.Forms.Label();
            temp_Label14.Text = "Метка";
            titleLabel = temp_Label14;
            titleLabel.Font = f;
            if (useFrame)
            {
                //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
                main.Controls.Add(l);
                //UPGRADE_TODO: Method 'java.awt.Container.add' was converted to 'System.Windows.Forms.ContainerControl.Controls.Add' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtContaineradd_javaawtComponent'"
                main.Controls.Add(titleLabel);
            }
		
            setGrid();
            elmList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            setupList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            undoStack = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
            redoStack = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
		
            scopes = new Scope[20];
            scopeColCount = new int[20];
            scopeCount = 0;
		
            random = new System.Random();
            cv.BackColor = System.Drawing.Color.Black;
            cv.ForeColor = System.Drawing.Color.LightGray;
		
            elmMenu = new System.Windows.Forms.ContextMenu();
            elmMenu.MenuItems.Add(elmEditMenuItem = getMenuItem("Параметры"));
            elmMenu.MenuItems.Add(elmScopeMenuItem = getMenuItem("Смотреть в осциллографе"));
            elmMenu.MenuItems.Add(elmCutMenuItem = getMenuItem("Вырезать"));
            elmMenu.MenuItems.Add(elmCopyMenuItem = getMenuItem("Копировать"));
            elmMenu.MenuItems.Add(elmDeleteMenuItem = getMenuItem("Удалить"));
            main.ContextMenu = elmMenu;
		
            scopeMenu = buildScopeMenu(false);
            transScopeMenu = buildScopeMenu(true);
		
            getSetupList(circuitsMenu, false);
            if (useFrame)
                Menu = mb;
            if (startCircuitText != null)
                readSetup(startCircuitText);
            else if (stopMessage == null && startCircuit != null)
                readSetupFile(startCircuit, startLabel);
		
            if (useFrame)
            {
                //UPGRADE_ISSUE: Method 'java.awt.Window.getToolkit' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtWindowgetToolkit'"
                //todo remove comment: getToolkit();
                System.Drawing.Size screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
                Size = new System.Drawing.Size(860, 640);
                handleResize();
                System.Drawing.Size x = Size;
                //UPGRADE_TODO: Method 'java.awt.Component.setLocation' was converted to 'System.Windows.Forms.Control.Location' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetLocation_int_int'"
                Location = new System.Drawing.Point((screen.Width - x.Width) / 2, (screen.Height - x.Height) / 2);
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                Show();
            }
            else
            {
                if (!powerCheckItem.Checked)
                {
                    main.Controls.Remove(powerBar);
                    main.Controls.Remove(powerLabel);
                    main.Invalidate();
                }
                Hide();
                handleResize();
                applet.Invalidate();
            }
            main.Focus();
        }
	
        internal bool shown = false;
	
        public virtual void  triggerShow()
        {
            if (!shown)
            {
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                Show();
            }
            shown = true;
        }
	
        internal virtual System.Windows.Forms.ContextMenu buildScopeMenu(bool t)
        {
            System.Windows.Forms.ContextMenu m = new System.Windows.Forms.ContextMenu();
            m.MenuItems.Add(getMenuItem("Убрать", "remove"));
            m.MenuItems.Add(getMenuItem("Скорость 2x", "speed2"));
            m.MenuItems.Add(getMenuItem("Скорость 1/2x", "speed1/2"));
            m.MenuItems.Add(getMenuItem("Масштаб 2x", "scale"));
            m.MenuItems.Add(getMenuItem("Максимальный масштаб", "maxscale"));
            m.MenuItems.Add(getMenuItem("Объединить", "stack"));
            m.MenuItems.Add(getMenuItem("Разъединить", "unstack"));
            m.MenuItems.Add(getMenuItem("Сброс", "reset"));
            if (t)
            {
                m.MenuItems.Add(scopeIbMenuItem = getCheckItem("Показать Iб"));
                m.MenuItems.Add(scopeIcMenuItem = getCheckItem("Показать Iк"));
                m.MenuItems.Add(scopeIeMenuItem = getCheckItem("Показать Iэ"));
                m.MenuItems.Add(scopeVbeMenuItem = getCheckItem("Показать Vбэ"));
                m.MenuItems.Add(scopeVbcMenuItem = getCheckItem("Показать Vбк"));
                m.MenuItems.Add(scopeVceMenuItem = getCheckItem("Показать Vкэ"));
                m.MenuItems.Add(scopeVceIcMenuItem = getCheckItem("Показать Vкэ и Iк"));
            }
            else
            {
                m.MenuItems.Add(scopeVMenuItem = getCheckItem("Показать Напряжение"));
                m.MenuItems.Add(scopeIMenuItem = getCheckItem("Показать Ток"));
                m.MenuItems.Add(scopePowerMenuItem = getCheckItem("Показать Потребленную мощность"));
                m.MenuItems.Add(scopeMaxMenuItem = getCheckItem("Показать Пиковое значение"));
                m.MenuItems.Add(scopeMinMenuItem = getCheckItem("Показать Отрицательное пиковое значение"));
                m.MenuItems.Add(scopeFreqMenuItem = getCheckItem("Показать Частоту"));
                m.MenuItems.Add(scopeVIMenuItem = getCheckItem("Показать V и I"));
                m.MenuItems.Add(scopeXYMenuItem = getCheckItem("График X/Y"));
                m.MenuItems.Add(scopeSelectYMenuItem = getMenuItem("Показать Y", "selecty"));
                m.MenuItems.Add(scopeResistMenuItem = getCheckItem("Показать Сопротивление"));
            }
            main.ContextMenu = m;
            return m;
        }
	
        internal virtual System.Windows.Forms.MenuItem getMenuItem(System.String s)
        {
            System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(s);
            mi.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(mi);
            return mi;
        }
	
        internal virtual System.Windows.Forms.MenuItem getMenuItem(System.String s, System.String ac)
        {
            System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(s);
            SupportClass.CommandManager.SetCommand(mi, ac);
            mi.Click += new System.EventHandler(this.actionPerformed);
            SupportClass.CommandManager.CheckCommand(mi);
            return mi;
        }
	
        internal virtual System.Windows.Forms.MenuItem getCheckItem(System.String s)
        {
            System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(s);
            mi.Click += new System.EventHandler(this.itemStateChanged);
            SupportClass.CommandManager.SetCommand(mi, "");
            return mi;
        }
	
        internal virtual System.Windows.Forms.MenuItem getClassCheckItem(System.String s, System.String t)
        {
            try
            {
                //UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                System.Type c = System.Type.GetType(t);
                CircuitElm elm = constructElement(c, 0, 0);
                register(c, elm);
                int dt = 0;
                if (elm.needsShortcut() && elm.DumpClass == c)
                {
                    dt = elm.DumpType;
                    s += (" (" + (char) dt + ")");
                }
                elm.delete();
            }
            catch (System.Exception ee)
            {
                SupportClass.WriteStackTrace(ee, Console.Error);
            }
            return getCheckItem(s, t);
        }
	
        internal virtual System.Windows.Forms.MenuItem getCheckItem(System.String s, System.String t)
        {
            System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem(s);
            mi.Click += new System.EventHandler(this.itemStateChanged);
            SupportClass.CommandManager.SetCommand(mi, t);
            return mi;
        }
	
        internal virtual void  register(System.Type c, CircuitElm elm)
        {
            int t = elm.DumpType;
            if (t == 0)
            {
                System.Console.Out.WriteLine("no dump type: " + c);
                return ;
            }
            System.Type dclass = elm.DumpClass;
            if (dumpTypes[t] == dclass)
                return ;
            if (dumpTypes[t] != null)
            {
                System.Console.Out.WriteLine("dump type conflict: " + c + " " + dumpTypes[t]);
                return ;
            }
            dumpTypes[t] = dclass;
        }
	
        internal virtual void  handleResize()
        {
            winSize = cv.Size;
            if (winSize.Width == 0)
                return ;
            dbimage = new System.Drawing.Bitmap(winSize.Width, winSize.Height);
            int h = winSize.Height / 5;
            /*if (h < 128 && winSize.height > 300)
		h = 128;*/
            circuitArea = new System.Drawing.Rectangle(0, 0, winSize.Width, winSize.Height - h);
            int i;
            int minx = 1000, maxx = 0, miny = 1000, maxy = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                // centered text causes problems when trying to center the circuit,
                // so we special-case it here
                if (!ce.CenteredText)
                {
                    minx = min(ce.x, min(ce.x2, minx));
                    maxx = max(ce.x, max(ce.x2, maxx));
                }
                miny = min(ce.y, min(ce.y2, miny));
                maxy = max(ce.y, max(ce.y2, maxy));
            }
            // center circuit; we don't use snapGrid() because that rounds
            int dx = gridMask & ((circuitArea.Width - (maxx - minx)) / 2 - minx);
            int dy = gridMask & ((circuitArea.Height - (maxy - miny)) / 2 - miny);
            if (dx + minx < 0)
                dx = gridMask & (- minx);
            if (dy + miny < 0)
                dy = gridMask & (- miny);
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.move(dx, dy);
            }
            // after moving elements, need this to avoid singular matrix probs
            needAnalyze();
            circuitBottom = 0;
        }
	
        internal virtual void  destroyFrame()
        {
            if (applet == null)
                Dispose();
            else
                applet.destroyFrame();
        }
	
        //UPGRADE_NOTE: The equivalent of method 'java.awt.Component.handleEvent' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        //UPGRADE_ISSUE: Class 'java.awt.Event' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
        public bool handleEvent(EventArgs ev)
        {//todo revert window destroy event
            //UPGRADE_ISSUE: Field 'java.awt.Event.id' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
            //UPGRADE_ISSUE: Field 'java.awt.Event.WINDOW_DESTROY' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtEvent'"
            //if (ev.id == Event.WINDOW_DESTROY)
            {
                destroyFrame();
                return true;
            }
            //UPGRADE_ISSUE: Method 'java.awt.Component.handleEvent' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtComponenthandleEvent_javaawtEvent'"
            //return base.handleEvent(ev);
        }
	
        protected override void  OnPaint(System.Windows.Forms.PaintEventArgs g_EventArg)
        {
            System.Drawing.Graphics g = null;
            if (g_EventArg != null)
                g = g_EventArg.Graphics;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        internal const int resct = 6;
        internal long lastTime = 0, lastFrameTime, lastIterTime, secTime = 0;
        internal int frames = 0;
        internal int steps = 0;
        internal int framerate = 0, steprate = 0;
	
        public virtual void  updateCircuit(System.Drawing.Graphics realg)
        {
            CircuitElm realMouseElm;
            if (winSize.IsEmpty || winSize.Width == 0)
                return ;
            if (analyzeFlag)
            {
                analyzeCircuit();
                analyzeFlag = false;
            }
            if (editDialog != null && editDialog.elm is CircuitElm)
                mouseElm = (CircuitElm) (editDialog.elm);
            realMouseElm = mouseElm;
            if (mouseElm == null)
                mouseElm = stopElm;
            setupScopes();
            System.Drawing.Graphics g = null;
            g = System.Drawing.Graphics.FromImage(dbimage);
            CircuitElm.selectColor = System.Drawing.Color.Cyan;
            if (printableCheckItem.Checked)
            {
                CircuitElm.whiteColor = System.Drawing.Color.Black;
                CircuitElm.lightGrayColor = System.Drawing.Color.Black;
                SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.White);
            }
            else
            {
                CircuitElm.whiteColor = System.Drawing.Color.White;
                CircuitElm.lightGrayColor = System.Drawing.Color.LightGray;
                SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Black);
            }
            g.FillRectangle(SupportClass.GraphicsManager.manager.GetPaint(g), 0, 0, winSize.Width, winSize.Height);
            if (!stoppedCheck.Checked)
            {
                try
                {
                    runCircuit();
                }
                catch (System.Exception e)
                {
                    SupportClass.WriteStackTrace(e, Console.Error);
                    analyzeFlag = true;
                    //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                    cv.Refresh();
                    return ;
                }
            }
            if (!stoppedCheck.Checked)
            {
                long sysTime = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
                if (lastTime != 0)
                {
                    int inc = (int) (sysTime - lastTime);
                    double c = currentBar.Value;
                    c = System.Math.Exp(c / 3.5 - 14.2);
                    CircuitElm.currentMult = 1.7 * inc * c;
                    if (!conventionCheckItem.Checked)
                        CircuitElm.currentMult = - CircuitElm.currentMult;
                }
                if (sysTime - secTime >= 1000)
                {
                    framerate = frames; steprate = steps;
                    frames = 0; steps = 0;
                    secTime = sysTime;
                }
                lastTime = sysTime;
            }
            else
                lastTime = 0;
            CircuitElm.powerMult = System.Math.Exp(powerBar.Value / 4.762 - 7);
		
            int i;
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.Graphics.getFont' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Drawing.Font oldfont = SupportClass.GraphicsManager.manager.GetFont(g);
            for (i = 0; i != elmList.Count; i++)
            {
                if (powerCheckItem.Checked)
                    SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Gray);
                /*else if (conductanceCheckItem.getState())
			g.setColor(Color.white);*/
                getElm(i).draw(g);
            }
            if (tempMouseMode == MODE_DRAG_ROW || tempMouseMode == MODE_DRAG_COLUMN || tempMouseMode == MODE_DRAG_POST || tempMouseMode == MODE_DRAG_SELECTED)
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.drawPost(g, ce.x, ce.y);
                    ce.drawPost(g, ce.x2, ce.y2);
                }
            int badnodes = 0;
            // find bad connections, nodes not connected to other elements which
            // intersect other elements' bounding boxes
            for (i = 0; i != nodeList.Count; i++)
            {
                CircuitNode cn = getCircuitNode(i);
                if (!cn.internal_Renamed && cn.links.Count == 1)
                {
                    int bb = 0, j;
                    CircuitNodeLink cnl = (CircuitNodeLink) cn.links[0];
                    for (j = 0; j != elmList.Count; j++)
                        if (cnl.elm != getElm(j) && getElm(j).boundingBox.Contains(cn.x, cn.y))
                            bb++;
                    if (bb > 0)
                    {
                        SupportClass.GraphicsManager.manager.SetColor(g, System.Drawing.Color.Red);
                        g.FillEllipse(SupportClass.GraphicsManager.manager.GetPaint(g), cn.x - 3, cn.y - 3, 7, 7);
                        badnodes++;
                    }
                }
            }
            /*if (mouseElm != null) {
		g.setFont(oldfont);
		g.drawString("+", mouseElm.x+10, mouseElm.y);
		}*/
            if (dragElm != null && (dragElm.x != dragElm.x2 || dragElm.y != dragElm.y2))
                dragElm.draw(g);
            SupportClass.GraphicsManager.manager.SetFont(g, oldfont);
            int ct = scopeCount;
            if (stopMessage != null)
                ct = 0;
            for (i = 0; i != ct; i++)
                scopes[i].draw(g);
            SupportClass.GraphicsManager.manager.SetColor(g, CircuitElm.whiteColor);
            if (stopMessage != null)
            {
                //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                g.DrawString(stopMessage, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), 10, circuitArea.Height - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
            }
            else
            {
                if (circuitBottom == 0)
                    calcCircuitBottom();
                System.String[] info = new System.String[10];
                if (mouseElm != null)
                {
                    if (mousePost == - 1)
                        mouseElm.getInfo(info);
                    else
                        info[0] = "V = " + CircuitElm.getUnitText(mouseElm.getPostVoltage(mousePost), "V");
                    /* //shownodes
				for (i = 0; i != mouseElm.getPostCount(); i++)
				info[0] += " " + mouseElm.nodes[i];
				if (mouseElm.getVoltageSourceCount() > 0)
				info[0] += ";" + (mouseElm.getVoltageSource()+nodeList.size());
				*/
                }
                else
                {
                    CircuitElm.showFormat.setMinimumFractionDigits(2);
                    info[0] = "t = " + CircuitElm.getUnitText(t, "СЃ");
                    CircuitElm.showFormat.setMinimumFractionDigits(0);
                }
                if (hintType != - 1)
                {
                    for (i = 0; info[i] != null; i++)
                        ;
                    System.String s = Hint;
                    if (s == null)
                        hintType = - 1;
                    else
                        info[i] = s;
                }
                int x = 0;
                if (ct != 0)
                    x = scopes[ct - 1].rightEdge() + 20;
                x = max(x, winSize.Width * 2 / 3);
			
                // count lines of data
                for (i = 0; info[i] != null; i++)
                    ;
                if (badnodes > 0)
                    info[i++] = badnodes + ((badnodes == 1) ? " плохое соединение" : " плохие соединения");
			
                // find where to show data; below circuit, not too high unless we need it
                int ybase = winSize.Height - 15 * i - 5;
                ybase = min(ybase, circuitArea.Height);
                ybase = max(ybase, circuitBottom);
                for (i = 0; info[i] != null; i++)
                {
                    //UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
                    g.DrawString(info[i], SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, ybase + 15 * (i + 1) - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
                }
            }
            if (!selectedArea.IsEmpty)
            {
                SupportClass.GraphicsManager.manager.SetColor(g, CircuitElm.selectColor);
                g.DrawRectangle(SupportClass.GraphicsManager.manager.GetPen(g), selectedArea.X, selectedArea.Y, selectedArea.Width, selectedArea.Height);
            }
            mouseElm = realMouseElm;
            frames++;
            /*
		g.setColor(Color.white);
		g.drawString("Framerate: " + framerate, 10, 10);
		g.drawString("Steprate: " + steprate,  10, 30);
		g.drawString("Steprate/iter: " + (steprate/getIterCount()),  10, 50);
		g.drawString("iterc: " + (getIterCount()),  10, 70);
		*/
		
            //UPGRADE_WARNING: Method 'java.awt.Graphics.drawImage' was converted to 'System.Drawing.Graphics.drawImage' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
            realg.DrawImage(dbimage, 0, 0);
            if (!stoppedCheck.Checked && circuitMatrix != null)
            {
                // Limit to 50 fps (thanks to JСЊrgen KlС†tzer for this)
                long delay = 1000 / 50 - ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 - lastFrameTime);
                //realg.drawString("delay: " + delay,  10, 90);
                if (delay > 0)
                {
                    try
                    {
                        //UPGRADE_TODO: Method 'java.lang.Thread.sleep' was converted to 'System.Threading.Thread.Sleep' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javalangThreadsleep_long'"
                        System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * delay));
                    }
                    catch (System.Threading.ThreadInterruptedException e)
                    {
                    }
                }
			
                //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint_long'"
                cv.Refresh();
            }
            lastFrameTime = lastTime;
        }
	
        internal virtual void  setupScopes()
        {
            int i;
		
            // check scopes to make sure the elements still exist, and remove
            // unused scopes/columns
            int pos = - 1;
            for (i = 0; i < scopeCount; i++)
            {
                if (locateElm(scopes[i].elm) < 0)
                    scopes[i].Elm = null;
                if (scopes[i].elm == null)
                {
                    int j;
                    for (j = i; j != scopeCount; j++)
                        scopes[j] = scopes[j + 1];
                    scopeCount--;
                    i--;
                    continue;
                }
                if (scopes[i].position > pos + 1)
                    scopes[i].position = pos + 1;
                pos = scopes[i].position;
            }
            while (scopeCount > 0 && scopes[scopeCount - 1].elm == null)
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
            int colct = pos + 1;
            int iw = infoWidth;
            if (colct <= 2)
                iw = iw * 3 / 2;
            int w = (winSize.Width - iw) / colct;
            int marg = 10;
            if (w < marg * 2)
                w = marg * 2;
            pos = - 1;
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
                System.Drawing.Rectangle r = new System.Drawing.Rectangle(pos * w, winSize.Height - h + colh * row, w - marg, colh);
                row++;
                if (!r.Equals(s.rect))
                    s.Rect = r;
            }
        }
	
        public virtual void  toggleSwitch(int n)
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
                        //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                        cv.Refresh();
                        return ;
                    }
                }
            }
        }
	
        internal virtual void  needAnalyze()
        {
            analyzeFlag = true;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        internal System.Collections.ArrayList nodeList;
        internal CircuitElm[] voltageSources;
	
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
	
        internal virtual void  analyzeCircuit()
        {
            calcCircuitBottom();
            if ((elmList.Count == 0))
                return ;
            stopMessage = null;
            stopElm = null;
            int i, j;
            int vscount = 0;
            nodeList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
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
                System.Drawing.Point pt = volt.getPost(0);
                cn.x = pt.X;
                cn.y = pt.Y;
                nodeList.Add(cn);
            }
            else
            {
                // otherwise allocate extra node for ground
                CircuitNode cn = new CircuitNode();
                cn.x = cn.y = - 1;
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
                    System.Drawing.Point pt = ce.getPost(j);
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
                    cn.x = cn.y = - 1;
                    cn.internal_Renamed = true;
                    CircuitNodeLink cnl = new CircuitNodeLink();
                    cnl.num = j + posts;
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
            voltageSourceCount = vscount;
		
            int matrixSize = nodeList.Count - 1 + vscount;
            circuitMatrix = new double[matrixSize][];
            for (int i2 = 0; i2 < matrixSize; i2++)
            {
                circuitMatrix[i2] = new double[matrixSize];
            }
            circuitRightSide = new double[matrixSize];
            origMatrix = new double[matrixSize][];
            for (int i3 = 0; i3 < matrixSize; i3++)
            {
                origMatrix[i3] = new double[matrixSize];
            }
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
                    if (!closure[i] && !getCircuitNode(i).internal_Renamed)
                    {
                        System.Console.Out.WriteLine("узел " + i + " не подключен");
                        stampResistor(0, i, 1e8);
                        closure[i] = true;
                        changed = true;
                        break;
                    }
            }
            //System.out.println("ac5");
		
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                // look for inductors with no current path
                if (ce is InductorElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.INDUCT, ce, ce.getNode(1));
                    // first try findPath with maximum depth of 5, to avoid slowdowns
                    if (!fpi.findPath(ce.getNode(0), 5) && !fpi.findPath(ce.getNode(0)))
                    {
                        //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                        System.Console.Out.WriteLine(ce + " нет пути");
                        ce.reset();
                    }
                }
                // look for current sources with no current path
                if (ce is CurrentElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.INDUCT, ce, ce.getNode(1));
                    if (!fpi.findPath(ce.getNode(0)))
                    {
                        stop("Короткое замыкание источника напряжения!", ce);
                        return ;
                    }
                }
                // look for voltage source loops
                if ((ce is VoltageElm && ce.PostCount == 2) || ce is WireElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.VOLTAGE, ce, ce.getNode(1));
                    if (fpi.findPath(ce.getNode(0)))
                    {
                        stop("Короткое замыкание источника напряжения!", ce);
                        return ;
                    }
                }
                // look for shorted caps, or caps w/ voltage but no R
                if (ce is CapacitorElm)
                {
                    FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.SHORT, ce, ce.getNode(1));
                    if (fpi.findPath(ce.getNode(0)))
                    {
                        //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                        System.Console.Out.WriteLine(ce + " shorted");
                        ce.reset();
                    }
                    else
                    {
                        fpi = new FindPathInfo(this, FindPathInfo.CAP_V, ce, ce.getNode(1));
                        if (fpi.findPath(ce.getNode(0)))
                        {
                            stop("Короткое замыкание конденсатора!", ce);
                            return ;
                        }
                    }
                }
            }
            //System.out.println("ac6");
		
            // simplify the matrix; this speeds things up quite a bit
            for (i = 0; i != matrixSize; i++)
            {
                int qm = - 1, qp = - 1;
                double qv = 0;
                RowInfo re = circuitRowInfo[i];
                /*System.out.println("row " + i + " " + re.lsChanges + " " + re.rsChanges + " " +
			re.dropRow);*/
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
                        rsadd -= circuitRowInfo[j].value_Renamed * q;
                        continue;
                    }
                    if (q == 0)
                        continue;
                    if (qp == - 1)
                    {
                        qp = j;
                        qv = q;
                        continue;
                    }
                    if (qm == - 1 && q == - qv)
                    {
                        qm = j;
                        continue;
                    }
                    break;
                }
                //System.out.println("line " + i + " " + qp + " " + qm + " " + j);
                /*if (qp != -1 && circuitRowInfo[qp].lsChanges) {
			System.out.println("lschanges");
			continue;
			}
			if (qm != -1 && circuitRowInfo[qm].lsChanges) {
			System.out.println("lschanges");
			continue;
			}*/
                if (j == matrixSize)
                {
                    if (qp == - 1)
                    {
                        stop("Matrix error", null);
                        return ;
                    }
                    RowInfo elt = circuitRowInfo[qp];
                    if (qm == - 1)
                    {
                        // we found a row with only one nonzero entry; that value
                        // is a constant
                        int k;
                        for (k = 0; elt.type == RowInfo.ROW_EQUAL && k < 100; k++)
                        {
                            // follow the chain
                            /*System.out.println("following equal chain from " +
						i + " " + qp + " to " + elt.nodeEq);*/
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
                            System.Console.Out.WriteLine("type already " + elt.type + " for " + qp + "!");
                            continue;
                        }
                        elt.type = RowInfo.ROW_CONST;
                        elt.value_Renamed = (circuitRightSide[i] + rsadd) / qv;
                        circuitRowInfo[i].dropRow = true;
                        //System.out.println(qp + " * " + qv + " = const " + elt.value);
                        i = - 1; // start over from scratch
                    }
                    else if (circuitRightSide[i] + rsadd == 0)
                    {
                        // we found a row with only two nonzero entries, and one
                        // is the negative of the other; the values are equal
                        if (elt.type != RowInfo.ROW_NORMAL)
                        {
                            //System.out.println("swapping");
                            int qq = qm;
                            qm = qp; qp = qq;
                            elt = circuitRowInfo[qp];
                            if (elt.type != RowInfo.ROW_NORMAL)
                            {
                                // we should follow the chain here, but this
                                // hardly ever happens so it's not worth worrying
                                // about
                                System.Console.Out.WriteLine("swap failed");
                                continue;
                            }
                        }
                        elt.type = RowInfo.ROW_EQUAL;
                        elt.nodeEq = qm;
                        circuitRowInfo[i].dropRow = true;
                        //System.out.println(qp + " = " + qm);
                    }
                }
            }
            //System.out.println("ac7");
		
            // find size of new matrix
            int nn = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo elt = circuitRowInfo[i];
                if (elt.type == RowInfo.ROW_NORMAL)
                {
                    elt.mapCol = nn++;
                    //System.out.println("col " + i + " maps to " + elt.mapCol);
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
                    elt.mapCol = - 1;
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
                        elt.value_Renamed = e2.value_Renamed;
                        elt.mapCol = - 1;
                        //System.out.println(i + " = [late]const " + elt.value);
                    }
                    else
                    {
                        elt.mapCol = e2.mapCol;
                        //System.out.println(i + " maps to: " + e2.mapCol);
                    }
                }
            }
            //System.out.println("ac8");
		
            /*System.out.println("matrixSize = " + matrixSize);
		
		for (j = 0; j != circuitMatrixSize; j++) {
		System.out.println(j + ": ");
		for (i = 0; i != circuitMatrixSize; i++)
		System.out.print(circuitMatrix[j][i] + " ");
		System.out.print("  " + circuitRightSide[j] + "\n");
		}
		System.out.print("\n");*/
		
		
            // make the new, simplified matrix
            int newsize = nn;
            double[][] newmatx = new double[newsize][];
            for (int i4 = 0; i4 < newsize; i4++)
            {
                newmatx[i4] = new double[newsize];
            }
            double[] newrs = new double[newsize];
            int ii = 0;
            for (i = 0; i != matrixSize; i++)
            {
                RowInfo rri = circuitRowInfo[i];
                if (rri.dropRow)
                {
                    rri.mapRow = - 1;
                    continue;
                }
                newrs[ii] = circuitRightSide[i];
                rri.mapRow = ii;
                //System.out.println("Row " + i + " maps to " + ii);
                for (j = 0; j != matrixSize; j++)
                {
                    RowInfo ri = circuitRowInfo[j];
                    if (ri.type == RowInfo.ROW_CONST)
                        newrs[ii] -= ri.value_Renamed * circuitMatrix[i][j];
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
		
            /*
		System.out.println("matrixSize = " + matrixSize + " " + circuitNonLinear);
		for (j = 0; j != circuitMatrixSize; j++) {
		for (i = 0; i != circuitMatrixSize; i++)
		System.out.print(circuitMatrix[j][i] + " ");
		System.out.print("  " + circuitRightSide[j] + "\n");
		}
		System.out.print("\n");*/
		
            // if a matrix is linear, we can do the lu_factor here instead of
            // needing to do it every frame
            if (!circuitNonLinear)
            {
                if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                {
                    stop("Singular matrix!", null);
                    return ;
                }
            }
        }
	
        internal virtual void  calcCircuitBottom()
        {
            int i;
            circuitBottom = 0;
            for (i = 0; i != elmList.Count; i++)
            {
                System.Drawing.Rectangle rect = getElm(i).boundingBox;
                int bottom = rect.Height + rect.Y;
                if (bottom > circuitBottom)
                    circuitBottom = bottom;
            }
        }
	
        //UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'FindPathInfo' to access its enclosing instance. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1019'"
        internal class FindPathInfo
        {
            private void  InitBlock(CirSim enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }
            private CirSim enclosingInstance;
            public CirSim Enclosing_Instance
            {
                get
                {
                    return enclosingInstance;
                }
			
            }
            internal const int INDUCT = 1;
            internal const int VOLTAGE = 2;
            internal const int SHORT = 3;
            internal const int CAP_V = 4;
            internal bool[] used;
            internal int dest;
            internal CircuitElm firstElm;
            internal int type;
            internal FindPathInfo(CirSim enclosingInstance, int t, CircuitElm e, int d)
            {
                InitBlock(enclosingInstance);
                dest = d;
                type = t;
                firstElm = e;
                used = new bool[Enclosing_Instance.nodeList.Count];
            }
            internal virtual bool findPath(int n1)
            {
                return findPath(n1, - 1);
            }
            internal virtual bool findPath(int n1, int depth)
            {
                if (n1 == dest)
                    return true;
                if (depth-- == 0)
                    return false;
                if (used[n1])
                {
                    //System.out.println("used " + n1);
                    return false;
                }
                used[n1] = true;
                int i;
                for (i = 0; i != Enclosing_Instance.elmList.Count; i++)
                {
                    CircuitElm ce = Enclosing_Instance.getElm(i);
                    if (ce == firstElm)
                        continue;
                    if (type == INDUCT)
                    {
                        if (ce is CurrentElm)
                            continue;
                    }
                    if (type == VOLTAGE)
                    {
                        if (!(ce.Wire || ce is VoltageElm))
                            continue;
                    }
                    if (type == SHORT && !ce.Wire)
                        continue;
                    if (type == CAP_V)
                    {
                        if (!(ce.Wire || ce is CapacitorElm || ce is VoltageElm))
                            continue;
                    }
                    if (n1 == 0)
                    {
                        // look for posts which have a ground connection;
                        // our path can go through ground
                        int j;
                        for (j = 0; j != ce.PostCount; j++)
                            if (ce.hasGroundConnection(j) && findPath(ce.getNode(j), depth))
                            {
                                used[n1] = false;
                                return true;
                            }
                    }
                    int j2;
                    for (j2 = 0; j2 != ce.PostCount; j2++)
                    {
                        //System.out.println(ce + " " + ce.getNode(j));
                        if (ce.getNode(j2) == n1)
                            break;
                    }
                    if (j2 == ce.PostCount)
                        continue;
                    if (ce.hasGroundConnection(j2) && findPath(0, depth))
                    {
                        //System.out.println(ce + " has ground");
                        used[n1] = false;
                        return true;
                    }
                    if (type == INDUCT && ce is InductorElm)
                    {
                        double c = ce.getCurrent();
                        if (j2 == 0)
                            c = - c;
                        //System.out.println("matching " + c + " to " + firstElm.getCurrent());
                        //System.out.println(ce + " " + firstElm);
                        if (System.Math.Abs(c - firstElm.getCurrent()) > 1e-10)
                            continue;
                    }
                    int k;
                    for (k = 0; k != ce.PostCount; k++)
                    {
                        if (j2 == k)
                            continue;
                        //System.out.println(ce + " " + ce.getNode(j) + "-" + ce.getNode(k));
                        if (ce.getConnection(j2, k) && findPath(ce.getNode(k), depth))
                        {
                            //System.out.println("got findpath " + n1);
                            used[n1] = false;
                            return true;
                        }
                        //System.out.println("back on findpath " + n1);
                    }
                }
                used[n1] = false;
                //System.out.println(n1 + " failed");
                return false;
            }
        }
	
        internal virtual void  stop(System.String s, CircuitElm ce)
        {
            stopMessage = s;
            circuitMatrix = null;
            stopElm = ce;
            stoppedCheck.Checked = true;
            analyzeFlag = false;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        // control voltage source vs with voltage from n1 to n2 (must
        // also call stampVoltageSource())
        internal virtual void  stampVCVS(int n1, int n2, double coef, int vs)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, coef);
            stampMatrix(vn, n2, - coef);
        }
	
        // stamp independent voltage source #vs, from n1 to n2, amount v
        internal virtual void  stampVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, - 1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn, v);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, - 1);
        }
	
        // use this if the amount of voltage is going to be updated in doStep()
        internal virtual void  stampVoltageSource(int n1, int n2, int vs)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(vn, n1, - 1);
            stampMatrix(vn, n2, 1);
            stampRightSide(vn);
            stampMatrix(n1, vn, 1);
            stampMatrix(n2, vn, - 1);
        }
	
        internal virtual void  updateVoltageSource(int n1, int n2, int vs, double v)
        {
            int vn = nodeList.Count + vs;
            stampRightSide(vn, v);
        }
	
        internal virtual void  stampResistor(int n1, int n2, double r)
        {
            double r0 = 1 / r;
            if (System.Double.IsNaN(r0) || System.Double.IsInfinity(r0))
            {
                System.Console.Out.Write("плохое сопротивление " + r + " " + r0 + "\n");
                int a = 0;
                a /= a;
            }
            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, - r0);
            stampMatrix(n2, n1, - r0);
        }
	
        internal virtual void  stampConductance(int n1, int n2, double r0)
        {
            stampMatrix(n1, n1, r0);
            stampMatrix(n2, n2, r0);
            stampMatrix(n1, n2, - r0);
            stampMatrix(n2, n1, - r0);
        }
	
        // current from cn1 to cn2 is equal to voltage from vn1 to 2, divided by g
        internal virtual void  stampVCCurrentSource(int cn1, int cn2, int vn1, int vn2, double g)
        {
            stampMatrix(cn1, vn1, g);
            stampMatrix(cn2, vn2, g);
            stampMatrix(cn1, vn2, - g);
            stampMatrix(cn2, vn1, - g);
        }
	
        internal virtual void  stampCurrentSource(int n1, int n2, double i)
        {
            stampRightSide(n1, - i);
            stampRightSide(n2, i);
        }
	
        // stamp a current source from n1 to n2 depending on current through vs
        internal virtual void  stampCCCS(int n1, int n2, int vs, double gain)
        {
            int vn = nodeList.Count + vs;
            stampMatrix(n1, vn, gain);
            stampMatrix(n2, vn, - gain);
        }
	
        // stamp value x in row i, column j, meaning that a voltage change
        // of dv in node j will increase the current into node i by x dv.
        // (Unless i or j is a voltage source node.)
        internal virtual void  stampMatrix(int i, int j, double x)
        {
            if (i > 0 && j > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i - 1].mapRow;
                    RowInfo ri = circuitRowInfo[j - 1];
                    if (ri.type == RowInfo.ROW_CONST)
                    {
                        //System.out.println("Stamping constant " + i + " " + j + " " + x);
                        circuitRightSide[i] -= x * ri.value_Renamed;
                        return ;
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
        internal virtual void  stampRightSide(int i, double x)
        {
            if (i > 0)
            {
                if (circuitNeedsMap)
                {
                    i = circuitRowInfo[i - 1].mapRow;
                    //System.out.println("stamping " + i + " " + x);
                }
                else
                    i--;
                circuitRightSide[i] += x;
            }
        }
	
        // indicate that the value on the right side of row i changes in doStep()
        internal virtual void  stampRightSide(int i)
        {
            //System.out.println("rschanges true " + (i-1));
            if (i > 0)
                circuitRowInfo[i - 1].rsChanges = true;
        }
	
        // indicate that the values on the left side of row i change in doStep()
        internal virtual void  stampNonLinear(int i)
        {
            if (i > 0)
                circuitRowInfo[i - 1].lsChanges = true;
        }
	
        internal bool converged;
        internal int subIterations;
        internal virtual void  runCircuit()
        {
            if (circuitMatrix == null || elmList.Count == 0)
            {
                circuitMatrix = null;
                return ;
            }
            int iter;
            //int maxIter = getIterCount();
            bool debugprint = dumpMatrix;
            dumpMatrix = false;
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            long steprate = (long) (160 * IterCount);
            long tm = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
            long lit = lastIterTime;
            if (1000 >= steprate * (tm - lastIterTime))
                return ;
            for (iter = 1; ; iter++)
            {
                int i, j, k, subiter;
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.startIteration();
                }
                steps++;
                //UPGRADE_NOTE: Final was removed from the declaration of 'subiterCount '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
                int subiterCount = 5000;
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
                        return ;
                    bool printit = debugprint;
                    debugprint = false;
                    for (j = 0; j != circuitMatrixSize; j++)
                    {
                        for (i = 0; i != circuitMatrixSize; i++)
                        {
                            double x = circuitMatrix[i][j];
                            if (System.Double.IsNaN(x) || System.Double.IsInfinity(x))
                            {
                                stop("nan/infinite matrix!", null);
                                return ;
                            }
                        }
                    }
                    if (printit)
                    {
                        for (j = 0; j != circuitMatrixSize; j++)
                        {
                            for (i = 0; i != circuitMatrixSize; i++)
                                System.Console.Out.Write(circuitMatrix[j][i] + ",");
                            System.Console.Out.Write("  " + circuitRightSide[j] + "\n");
                        }
                        System.Console.Out.Write("\n");
                    }
                    if (circuitNonLinear)
                    {
                        if (converged && subiter > 0)
                            break;
                        if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                        {
                            stop("Singular matrix!", null);
                            return ;
                        }
                    }
                    lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);
				
                    for (j = 0; j != circuitMatrixFullSize; j++)
                    {
                        RowInfo ri = circuitRowInfo[j];
                        double res = 0;
                        if (ri.type == RowInfo.ROW_CONST)
                            res = ri.value_Renamed;
                        else
                            res = circuitRightSide[ri.mapCol];
                        /*System.out.println(j + " " + res + " " +
					ri.type + " " + ri.mapCol);*/
                        if (System.Double.IsNaN(res))
                        {
                            converged = false;
                            //debugprint = true;
                            break;
                        }
                        if (j < nodeList.Count - 1)
                        {
                            CircuitNode cn = getCircuitNode(j + 1);
                            for (k = 0; k != cn.links.Count; k++)
                            {
                                CircuitNodeLink cnl = (CircuitNodeLink) cn.links[k];
                                cnl.elm.setNodeVoltage(cnl.num, res);
                            }
                        }
                        else
                        {
                            int ji = j - (nodeList.Count - 1);
                            //System.out.println("setting vsrc " + ji + " to " + res);
                            voltageSources[ji].setCurrent(ji, res);
                        }
                    }
                    if (!circuitNonLinear)
                        break;
                }
                if (subiter > 5)
                    System.Console.Out.Write("converged after " + subiter + " iterations\n");
                if (subiter == subiterCount)
                {
                    stop("Convergence failed!", null);
                    break;
                }
                t += timeStep;
                for (i = 0; i != scopeCount; i++)
                    scopes[i].timeStep();
                tm = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
                lit = tm;
                if (iter * 1000 >= steprate * (tm - lastIterTime) || (tm - lastFrameTime > 500))
                    break;
            }
            lastIterTime = lit;
            //System.out.println((System.currentTimeMillis()-lastFrameTime)/(double) iter);
        }
	
        internal virtual int min(int a, int b)
        {
            return (a < b)?a:b;
        }
        internal virtual int max(int a, int b)
        {
            return (a > b)?a:b;
        }
	
        internal virtual void  editFuncPoint(int x, int y)
        {
            // XXX
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint_long'"
            cv.Refresh();
        }
	
        public virtual void  componentHidden(System.Object event_sender, System.EventArgs e)
        {
        }
        public virtual void  componentMoved(System.Object event_sender, System.EventArgs e)
        {
        }
        public virtual void  componentShown(System.Object event_sender, System.EventArgs e)
        {
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        public virtual void  componentResized(System.Object event_sender, System.EventArgs e)
        {
            handleResize();
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint_long'"
            cv.Refresh();
        }
        public virtual void  actionPerformed(System.Object event_sender, System.EventArgs e)
        {
            System.String ac = SupportClass.CommandManager.GetCommand(event_sender);
            if (event_sender == resetButton)
            {
                int i;
			
                // on IE, drawImage() stops working inexplicably every once in
                // a while.  Recreating it fixes the problem, so we do that here.
                dbimage = new System.Drawing.Bitmap(winSize.Width, winSize.Height);
			
                for (i = 0; i != elmList.Count; i++)
                    getElm(i).reset();
                for (i = 0; i != scopeCount; i++)
                    scopes[i].resetGraph();
                analyzeFlag = true;
                t = 0;
                stoppedCheck.Checked = false;
                //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                cv.Refresh();
            }
            if (event_sender == dumpMatrixButton)
                dumpMatrix = true;
            if (event_sender == exportItem)
                doImport(false, false);
            if (event_sender == optionsItem)
                doEdit(new EditOptions(this));
            if (event_sender == importItem)
                doImport(true, false);
            if (event_sender == exportLinkItem)
                doImport(false, true);
            if (event_sender == undoItem)
                doUndo();
            if (event_sender == redoItem)
                doRedo();
            if (String.CompareOrdinal(ac, "Вырезать") == 0)
            {
                if (event_sender != elmCutMenuItem)
                    menuElm = null;
                doCut();
            }
            if (String.CompareOrdinal(ac, "Копировать") == 0)
            {
                if (event_sender != elmCopyMenuItem)
                    menuElm = null;
                doCopy();
            }
            if (String.CompareOrdinal(ac, "Вставить") == 0)
                doPaste();
            if (event_sender == selectAllItem)
                doSelectAll();
            if (event_sender == exitItem)
            {
                destroyFrame();
                return ;
            }
            if (String.CompareOrdinal(ac, "stackAll") == 0)
                stackAll();
            if (String.CompareOrdinal(ac, "unstackAll") == 0)
                unstackAll();
            if (event_sender == elmEditMenuItem)
                doEdit(menuElm);
            if (String.CompareOrdinal(ac, "Удалить") == 0)
            {
                if (event_sender != elmDeleteMenuItem)
                    menuElm = null;
                doDelete();
            }
            if (event_sender == elmScopeMenuItem && menuElm != null)
            {
                int i;
                for (i = 0; i != scopeCount; i++)
                    if (scopes[i].elm == null)
                        break;
                if (i == scopeCount)
                {
                    if (scopeCount == scopes.Length)
                        return ;
                    scopeCount++;
                    scopes[i] = new Scope(this);
                    scopes[i].position = i;
                    handleResize();
                }
                scopes[i].Elm = menuElm;
            }
            if (menuScope != - 1)
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
                //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                cv.Refresh();
            }
            if (ac.IndexOf("setup ") == 0)
            {
                pushUndo();
                readSetupFile(ac.Substring(6), ((System.Windows.Forms.MenuItem) event_sender).Text);
            }
        }
	
        internal virtual void  stackScope(int s)
        {
            if (s == 0)
            {
                if (scopeCount < 2)
                    return ;
                s = 1;
            }
            if (scopes[s].position == scopes[s - 1].position)
                return ;
            scopes[s].position = scopes[s - 1].position;
            for (s++; s < scopeCount; s++)
                scopes[s].position--;
        }
	
        internal virtual void  unstackScope(int s)
        {
            if (s == 0)
            {
                if (scopeCount < 2)
                    return ;
                s = 1;
            }
            if (scopes[s].position != scopes[s - 1].position)
                return ;
            for (; s < scopeCount; s++)
                scopes[s].position++;
        }
	
        internal virtual void  stackAll()
        {
            int i;
            for (i = 0; i != scopeCount; i++)
            {
                scopes[i].position = 0;
                scopes[i].showMax_Renamed_Field = scopes[i].showMin_Renamed_Field = false;
            }
        }
	
        internal virtual void  unstackAll()
        {
            int i;
            for (i = 0; i != scopeCount; i++)
            {
                scopes[i].position = i;
                scopes[i].showMax_Renamed_Field = true;
            }
        }
	
        internal virtual void  doEdit(Editable eable)
        {
            clearSelection();
            pushUndo();
            if (editDialog != null)
            {
                Focus();
                //UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                editDialog.Visible = false;
                editDialog = null;
            }
            editDialog = new EditDialog(eable, this);
            //UPGRADE_TODO: Method 'java.awt.Dialog.show' was converted to 'System.Windows.Forms.Form.ShowDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogshow'"
            editDialog.ShowDialog();
        }
	
        internal virtual void  doImport(bool imp, bool url)
        {
            if (impDialog != null)
            {
                Focus();
                //UPGRADE_TODO: Method 'java.awt.Component.setVisible' was converted to 'System.Windows.Forms.Control.Visible' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentsetVisible_boolean'"
                //UPGRADE_TODO: 'System.Windows.Forms.Application.Run' must be called to start a main form. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1135'"
                impDialog.Visible = false;
                impDialog = null;
            }
            System.String dump = (imp)?"":dumpCircuit();
            if (url)
                dump = baseURL + "#" + System.Web.HttpUtility.UrlEncode(dump);
            impDialog = new ImportDialog(this, dump, url);
            //UPGRADE_TODO: Method 'java.awt.Dialog.show' was converted to 'System.Windows.Forms.Form.ShowDialog' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtDialogshow'"
            impDialog.ShowDialog();
            pushUndo();
        }
	
        internal virtual System.String dumpCircuit()
        {
            int i;
            int f = (dotsCheckItem.Checked)?1:0;
            f |= ((smallGridCheckItem.Checked)?2:0);
            f |= ((voltsCheckItem.Checked)?0:4);
            f |= ((powerCheckItem.Checked)?8:0);
            f |= ((showValuesCheckItem.Checked)?0:16);
            // 32 = linear scale in afilter
            System.String dump = "$ " + f + " " + timeStep + " " + IterCount + " " + currentBar.Value + " " + CircuitElm.voltageRange + " " + powerBar.Value + "\n";
            for (i = 0; i != elmList.Count; i++)
                dump += (getElm(i).dump() + "\n");
            for (i = 0; i != scopeCount; i++)
            {
                System.String d = scopes[i].dump();
                if (d != null)
                    dump += (d + "\n");
            }
            if (hintType != - 1)
                dump += ("h " + hintType + " " + hintItem1 + " " + hintItem2 + "\n");
            return dump;
        }
	
        public virtual void  adjustmentValueChanged(System.Object event_sender, System.Windows.Forms.ScrollEventArgs e)
        {
            //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
            System.Console.Out.Write(((System.Windows.Forms.ScrollBar) event_sender).Value + "\n");
        }
	
        internal virtual System.IO.MemoryStream readUrlData(string url)
        {
            //UPGRADE_ISSUE: Method 'java.net.URL.getContent' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javanetURLgetContent'"
            //System.Object o = url.GetContent();
            //System.IO.BinaryReader fis = (System.IO.BinaryReader) o;
            BinaryReader fis = new BinaryReader(new FileStream(url, FileMode.Open));
            long available;
            available = fis.BaseStream.Length - fis.BaseStream.Position;
            System.IO.MemoryStream ba = new System.IO.MemoryStream((int) available);
            int blen = 1024;
            sbyte[] b = new sbyte[blen];
            while (true)
            {
                int len = SupportClass.ReadInput(fis.BaseStream, b, 0, b.Length);
                if (len <= 0)
                    break;
                ba.Write(SupportClass.ToByteArray(b), 0, len);
            }
            return ba;
        }
	
        internal virtual void  getSetupList(System.Windows.Forms.MenuItem menu, bool retry)
        {
            System.Windows.Forms.MenuItem[] stack = new System.Windows.Forms.MenuItem[6];
            int stackptr = 0;
            stack[stackptr++] = menu;
            try
            {
                //UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
                string url = (CodeBase.LocalPath + "setuplist.txt");
                System.IO.MemoryStream ba = readUrlData(url);
                sbyte[] b = SupportClass.ToSByteArray(ba.ToArray());
                long len = ba.Length;
                int p;
                if (len == 0 || b[0] != '#')
                {
                    // got a redirect, try again
                    getSetupList(menu, true);
                    return ;
                }
                for (p = 0; p < len; )
                {
                    int l;
                    for (l = 0; l != len - p; l++)
                        if (b[l + p] == '\n')
                        {
                            l++;
                            break;
                        }
                    System.String tempStr;
                    //UPGRADE_TODO: The differences in the Format  of parameters for constructor 'java.lang.String.String'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                    tempStr = System.Text.Encoding.GetEncoding("UTF-8").GetString(SupportClass.ToByteArray(b));
                    System.String line = new System.String(tempStr.ToCharArray(), p, l - 1);
                    if (line[0] == '#')
                    {
                    }
                    else if (line[0] == '+')
                    {
                        System.Windows.Forms.MenuItem n = new System.Windows.Forms.MenuItem(line.Substring(1));
                        menu.MenuItems.Add(n);
                        menu = stack[stackptr++] = n;
                    }
                    else if (line[0] == '-')
                    {
                        menu = stack[--stackptr - 1];
                    }
                    else
                    {
                        int i = line.IndexOf(' ');
                        if (i > 0)
                        {
                            System.String title = line.Substring(i + 1);
                            bool first = false;
                            if (line[0] == '>')
                                first = true;
                            System.String file = line.Substring(first?1:0, (i) - (first?1:0));
                            menu.MenuItems.Add(getMenuItem(title, "setup " + file));
                            if (first && startCircuit == null)
                            {
                                startCircuit = file;
                                startLabel = title;
                            }
                        }
                    }
                    p += l;
                }
            }
            catch (System.Exception e)
            {
                SupportClass.WriteStackTrace(e, Console.Error);
                stop("Can't read setuplist.txt!", null);
            }
        }
	
        internal virtual void  readSetup(System.String text)
        {
            readSetup(text, false);
        }
	
        internal virtual void  readSetup(System.String text, bool retain)
        {
            readSetup(SupportClass.ToSByteArray(SupportClass.ToByteArray(text)), text.Length, retain);
            titleLabel.Text = "untitled";
        }
	
        internal virtual void  readSetupFile(String str, String title)
        {
            t = 0;
            Console.Out.WriteLine(str);
            try
            {
                //UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
                string url = CodeBase.LocalPath + "circuits/" + str;
                System.IO.MemoryStream ba = readUrlData(url);
                readSetup(SupportClass.ToSByteArray(ba.ToArray()), (int)ba.Length, false);
            }
            catch (System.Exception e)
            {
                SupportClass.WriteStackTrace(e, Console.Error);
                stop("Unable to read " + str + "!", null);
            }
            titleLabel.Text = title;
        }
	
        internal virtual void  readSetup(sbyte[] b, int len, bool retain)
        {
            int i;
            if (!retain)
            {
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.delete();
                }
                elmList.Clear();
                hintType = - 1;
                timeStep = 5e-6;
                dotsCheckItem.Checked = true;
                smallGridCheckItem.Checked = false;
                powerCheckItem.Checked = false;
                voltsCheckItem.Checked = true;
                showValuesCheckItem.Checked = true;
                setGrid();
                speedBar.Value = 117; // 57
                currentBar.Value = 50;
                powerBar.Value = 50;
                CircuitElm.voltageRange = 5;
                scopeCount = 0;
            }
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
            int p;
            for (p = 0; p < len; )
            {
                int l;
                int linelen = 0;
                for (l = 0; l != len - p; l++)
                    if (b[l + p] == '\n' || b[l + p] == '\r')
                    {
                        linelen = l++;
                        if (l + p < b.Length && b[l + p] == '\n')
                            l++;
                        break;
                    }
                System.String line = null; try
                {
                    System.String tempStr;
                    //UPGRADE_TODO: The differences in the Format  of parameters for constructor 'java.lang.String.String'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                    tempStr = System.Text.Encoding.GetEncoding("UTF-8").GetString(SupportClass.ToByteArray(b));
                    line = new System.String(tempStr.ToCharArray(), p, linelen);
                }
                catch (System.Exception e)
                {
                    SupportClass.WriteStackTrace(e, Console.Error);
                }
                SupportClass.Tokenizer st = new SupportClass.Tokenizer(line);
                while (st.HasMoreTokens())
                {
                    System.String type = st.NextToken();
                    int tint = type[0];
                    try
                    {
                        if (tint == 'o')
                        {
                            Scope sc = new Scope(this);
                            sc.position = scopeCount;
                            sc.undump(st);
                            scopes[scopeCount++] = sc;
                            break;
                        }
                        if (tint == 'h')
                        {
                            readHint(st);
                            break;
                        }
                        if (tint == '$')
                        {
                            readOptions(st);
                            break;
                        }
                        if (tint == '%' || tint == '?' || tint == 'B')
                        {
                            // ignore afilter-specific stuff
                            break;
                        }
                        if (tint >= '0' && tint <= '9')
                            tint = System.Int32.Parse(type);
                        int x1 = System.Int32.Parse(st.NextToken());
                        int y1 = System.Int32.Parse(st.NextToken());
                        int x2 = System.Int32.Parse(st.NextToken());
                        int y2 = System.Int32.Parse(st.NextToken());
                        int f = System.Int32.Parse(st.NextToken());
                        CircuitElm ce = null;
                        System.Type cls = dumpTypes[tint];
                        if (cls == null)
                        {
                            System.Console.Out.WriteLine("unrecognized dump type: " + type);
                            break;
                        }
                        // find element class
                        System.Type[] carr = new System.Type[6];
                        //carr[0] = getClass();
                        carr[0] = carr[1] = carr[2] = carr[3] = carr[4] = typeof(int);
                        carr[5] = typeof(SupportClass.Tokenizer);
                        System.Reflection.ConstructorInfo cstr = null;
                        cstr = cls.GetConstructor(carr);
					
                        // invoke constructor with starting coordinates
                        System.Object[] oarr = new System.Object[6];
                        //oarr[0] = this;
                        oarr[0] = (System.Int32) x1;
                        oarr[1] = (System.Int32) y1;
                        oarr[2] = (System.Int32) x2;
                        oarr[3] = (System.Int32) y2;
                        oarr[4] = (System.Int32) f;
                        oarr[5] = st;
                        ce = (CircuitElm) cstr.Invoke(oarr);
                        ce.setPoints();
                        elmList.Add(ce);
                    }
                    catch (System.Reflection.TargetInvocationException ee)
                    {
                        SupportClass.WriteStackTrace(ee.GetBaseException(), Console.Error);
                        break;
                    }
                    catch (System.Exception ee)
                    {
                        SupportClass.WriteStackTrace(ee, Console.Error);
                        break;
                    }
                    break;
                }
                p += l;
            }
            enableItems();
            if (!retain)
                handleResize(); // for scopes
            needAnalyze();
        }
	
        internal virtual void  readHint(SupportClass.Tokenizer st)
        {
            hintType = System.Int32.Parse(st.NextToken());
            hintItem1 = System.Int32.Parse(st.NextToken());
            hintItem2 = System.Int32.Parse(st.NextToken());
        }
	
        internal virtual void  readOptions(SupportClass.Tokenizer st)
        {
            int flags = System.Int32.Parse(st.NextToken());
            dotsCheckItem.Checked = (flags & 1) != 0;
            smallGridCheckItem.Checked = (flags & 2) != 0;
            voltsCheckItem.Checked = (flags & 4) == 0;
            powerCheckItem.Checked = (flags & 8) == 8;
            showValuesCheckItem.Checked = (flags & 16) == 0;
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            timeStep = System.Double.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            double sp = System.Double.Parse(st.NextToken());
            //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
            int sp2 = (int) (System.Math.Log(10 * sp) * 24 + 61.5);
            //int sp2 = (int) (Math.log(sp)*24+1.5);
            speedBar.Value = sp2;
            currentBar.Value = System.Int32.Parse(st.NextToken());
            //UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
            CircuitElm.voltageRange = System.Double.Parse(st.NextToken());
            try
            {
                powerBar.Value = System.Int32.Parse(st.NextToken());
            }
            catch (System.Exception e)
            {
            }
            setGrid();
        }
	
        internal virtual int snapGrid(int x)
        {
            return (x + gridRound) & gridMask;
        }
	
        internal virtual bool doSwitch(int x, int y)
        {
            if (mouseElm == null || !(mouseElm is SwitchElm))
                return false;
            SwitchElm se = (SwitchElm) mouseElm;
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
            return - 1;
        }
	/*
        public virtual void  mouseDragged(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            // ignore right mouse button with no modifiers (needed on PC)
            //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
            if ((state4 & (int) System.Windows.Forms.MouseButtons.Right) != 0)
            {
                int ex = e.getModifiersEx();
                if ((ex & (MouseEvent.META_DOWN_MASK | MouseEvent.SHIFT_DOWN_MASK | MouseEvent.CTRL_DOWN_MASK | MouseEvent.ALT_DOWN_MASK)) == 0)
                    return ;
            }
            if (!circuitArea.Contains(e.X, e.Y))
                return ;
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
            dragging = true;
            if (success)
            {
                if (tempMouseMode == MODE_DRAG_SELECTED && mouseElm is TextElm)
                {
                    dragX = e.X; dragY = e.Y;
                }
                else
                {
                    dragX = snapGrid(e.X); dragY = snapGrid(e.Y);
                }
            }
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint_long'"
            cv.Refresh();
        }
	*/
        internal virtual void  dragAll(int x, int y)
        {
            int dx = x - dragX;
            int dy = y - dragY;
            if (dx == 0 && dy == 0)
                return ;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.move(dx, dy);
            }
            removeZeroLengthElements();
        }
	
        internal virtual void  dragRow(int x, int y)
        {
            int dy = y - dragY;
            if (dy == 0)
                return ;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.y == dragY)
                    ce.movePoint(0, 0, dy);
                if (ce.y2 == dragY)
                    ce.movePoint(1, 0, dy);
            }
            removeZeroLengthElements();
        }
	
        internal virtual void  dragColumn(int x, int y)
        {
            int dx = x - dragX;
            if (dx == 0)
                return ;
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.x == dragX)
                    ce.movePoint(0, dx, 0);
                if (ce.x2 == dragX)
                    ce.movePoint(1, dx, 0);
            }
            removeZeroLengthElements();
        }
	
        internal virtual bool dragSelected(int x, int y)
        {
            bool me = false;
            if (mouseElm != null && !mouseElm.Selected)
                mouseElm.Selected = me = true;
		
            // snap grid, unless we're only dragging text elements
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.Selected && !(ce is TextElm))
                    break;
            }
            if (i != elmList.Count)
            {
                x = snapGrid(x);
                y = snapGrid(y);
            }
		
            int dx = x - dragX;
            int dy = y - dragY;
            if (dx == 0 && dy == 0)
            {
                // don't leave mouseElm selected if we selected it above
                if (me)
                    mouseElm.Selected = false;
                return false;
            }
            bool allowed = true;
		
            // check if moves are allowed
            for (i = 0; allowed && i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                if (ce.Selected && !ce.allowMove(dx, dy))
                    allowed = false;
            }
		
            if (allowed)
            {
                for (i = 0; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    if (ce.Selected)
                        ce.move(dx, dy);
                }
                needAnalyze();
            }
		
            // don't leave mouseElm selected if we selected it above
            if (me)
                mouseElm.Selected = false;
		
            return allowed;
        }
	
        internal virtual void  dragPost(int x, int y)
        {
            if (draggingPost == - 1)
            {
                draggingPost = (distanceSq(mouseElm.x, mouseElm.y, x, y) > distanceSq(mouseElm.x2, mouseElm.y2, x, y))?1:0;
            }
            int dx = x - dragX;
            int dy = y - dragY;
            if (dx == 0 && dy == 0)
                return ;
            mouseElm.movePoint(draggingPost, dx, dy);
            needAnalyze();
        }
	
        internal virtual void  selectArea(int x, int y)
        {
            int x1 = min(x, initDragX);
            int x2 = max(x, initDragX);
            int y1 = min(y, initDragY);
            int y2 = max(y, initDragY);
            selectedArea = new System.Drawing.Rectangle(x1, y1, x2 - x1, y2 - y1);
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                //UPGRADE_NOTE: ref keyword was added to struct-type parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1303'"
                //todo ce.selectRect(ref selectedArea);
            }
        }
	
        internal virtual void  removeZeroLengthElements()
        {
            int i;
            bool changed = false;
            for (i = elmList.Count - 1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.x == ce.x2 && ce.y == ce.y2)
                {
                    elmList.RemoveAt(i);
                    ce.delete();
                    changed = true;
                }
            }
            needAnalyze();
        }
	
        public virtual void  mouseMoved(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
            if ((state4 & (int) System.Windows.Forms.MouseButtons.Left) != 0)
                return ;
            int x = e.X;
            int y = e.Y;
            dragX = snapGrid(x); dragY = snapGrid(y);
            draggingPost = - 1;
            int i;
            CircuitElm origMouse = mouseElm;
            mouseElm = null;
            mousePost = - 1;
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
                        System.Drawing.Point pt = ce.getPost(j);
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
            scopeSelected = - 1;
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
                        System.Drawing.Point pt = ce.getPost(j);
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
                mousePost = - 1;
                // look for post close to the mouse pointer
                for (i = 0; i != mouseElm.PostCount; i++)
                {
                    System.Drawing.Point pt = mouseElm.getPost(i);
                    if (distanceSq(pt.X, pt.Y, x, y) < 26)
                        mousePost = i;
                }
            }
            if (mouseElm != origMouse)
            {
                //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
                cv.Refresh();
            }
        }
	
        internal virtual int distanceSq(int x1, int y1, int x2, int y2)
        {
            x2 -= x1;
            y2 -= y1;
            return x2 * x2 + y2 * y2;
        }
	
        public virtual void  mouseClicked(System.Object event_sender, System.EventArgs e)
        {
            //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
            if ((state4 & (int) System.Windows.Forms.MouseButtons.Left) != 0)
            {
                if (mouseMode == MODE_SELECT || mouseMode == MODE_DRAG_SELECTED)
                    clearSelection();
            }
        }
        public virtual void  mouseEntered(System.Object event_sender, System.EventArgs e)
        {
        }
        public virtual void  mouseExited(System.Object event_sender, System.EventArgs e)
        {
            scopeSelected = - 1;
            mouseElm = plotXElm = plotYElm = null;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        public virtual void  mousePressed(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
            System.Console.Out.WriteLine(state4);
            //todo int ex = e.getModifiersEx();
            //UPGRADE_TODO: Method 'java.awt.event.MouseEvent.isPopupTrigger' was converted to 'System.Windows.Forms.MouseButtons.Right' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawteventMouseEventisPopupTrigger'"
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                doPopupMenu(event_sender, e);
                return ;
            }
            //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
            if ((state4 & (int) System.Windows.Forms.MouseButtons.Left) != 0)
            {
                // left mouse
                tempMouseMode = mouseMode;
                //todo uncomment after refactoring
                //if ((ex & MouseEvent.ALT_DOWN_MASK) != 0 && (ex & MouseEvent.META_DOWN_MASK) != 0)
                //    tempMouseMode = MODE_DRAG_COLUMN;
                //else if ((ex & MouseEvent.ALT_DOWN_MASK) != 0 && (ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                //    tempMouseMode = MODE_DRAG_ROW;
                //else if ((ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                //    tempMouseMode = MODE_SELECT;
                //else if ((ex & MouseEvent.ALT_DOWN_MASK) != 0)
                //    tempMouseMode = MODE_DRAG_ALL;
                //else if ((ex & (MouseEvent.CTRL_DOWN_MASK | MouseEvent.META_DOWN_MASK)) != 0)
                //    tempMouseMode = MODE_DRAG_POST;
            }
            else
            {
                //UPGRADE_NOTE: The 'java.awt.event.InputEvent.getModifiers' method simulation might not work for some controls. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1284'"
                if ((state4 & (int) System.Windows.Forms.MouseButtons.Right) != 0)
                {
                    // right mouse
                    //todo uncomment after refactoring
                    //if ((ex & MouseEvent.SHIFT_DOWN_MASK) != 0)
                    //    tempMouseMode = MODE_DRAG_ROW;
                    //else if ((ex & (MouseEvent.CTRL_DOWN_MASK | MouseEvent.META_DOWN_MASK)) != 0)
                    //    tempMouseMode = MODE_DRAG_COLUMN;
                    //else
                    //    return ;
                }
            }
		
            if (tempMouseMode != MODE_SELECT && tempMouseMode != MODE_DRAG_SELECTED)
                clearSelection();
            if (doSwitch(e.X, e.Y))
                return ;
            pushUndo();
            initDragX = e.X;
            initDragY = e.Y;
            dragging = true;
            if (tempMouseMode != MODE_ADD_ELM || addingClass == null)
                return ;
		
            int x0 = snapGrid(e.X);
            int y0 = snapGrid(e.Y);
            if (!circuitArea.Contains(x0, y0))
                return ;
		
            dragElm = constructElement(addingClass, x0, y0);
        }
	
        internal virtual CircuitElm constructElement(System.Type c, int x0, int y0)
        {
            // find element class
            System.Type[] carr = new System.Type[2];
            //carr[0] = getClass();
            carr[0] = carr[1] = typeof(int);
            System.Reflection.ConstructorInfo cstr = null;
            try
            {
                cstr = c.GetConstructor(carr);
            }
            catch (System.MethodAccessException ee)
            {
                System.Console.Out.WriteLine("caught NoSuchMethodException " + c);
                return null;
            }
            catch (System.Exception ee)
            {
                SupportClass.WriteStackTrace(ee, Console.Error);
                return null;
            }
		
            // invoke constructor with starting coordinates
            System.Object[] oarr = new System.Object[2];
            oarr[0] = (System.Int32) x0;
            oarr[1] = (System.Int32) y0;
            try
            {
                return (CircuitElm) cstr.Invoke(oarr);
            }
            catch (System.Exception ee)
            {
                SupportClass.WriteStackTrace(ee, Console.Error);
            }
            return null;
        }
	
        internal virtual void  doPopupMenu(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            menuElm = mouseElm;
            menuScope = - 1;
            if (scopeSelected != - 1)
            {
                System.Windows.Forms.ContextMenu m = scopes[scopeSelected].Menu;
                menuScope = scopeSelected;
                if (m != null)
                {
                    //UPGRADE_TODO: Method 'java.awt.PopupMenu.show' was converted to 'System.Windows.Forms.ContextMenu.Show' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPopupMenushow_javaawtComponent_int_int'"
                    m.Show(((System.Windows.Forms.Control) event_sender), new System.Drawing.Point(e.X, e.Y));
                }
            }
            else if (mouseElm != null)
            {
                elmEditMenuItem.Enabled = mouseElm.getEditInfo(0) != null;
                elmScopeMenuItem.Enabled = mouseElm.canViewInScope();
                //UPGRADE_TODO: Method 'java.awt.PopupMenu.show' was converted to 'System.Windows.Forms.ContextMenu.Show' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPopupMenushow_javaawtComponent_int_int'"
                elmMenu.Show(((System.Windows.Forms.Control) event_sender), new System.Drawing.Point(e.X, e.Y));
            }
            else
            {
                //todo doMainMenuChecks(mainMenu);
                //UPGRADE_TODO: Method 'java.awt.PopupMenu.show' was converted to 'System.Windows.Forms.ContextMenu.Show' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtPopupMenushow_javaawtComponent_int_int'"
                mainMenu.Show(((System.Windows.Forms.Control) event_sender), new System.Drawing.Point(e.X, e.Y));
            }
        }
	
        internal virtual void  doMainMenuChecks(System.Windows.Forms.MenuItem m)
        {
            int i;
            if (m == optionsMenu)
                return ;
            for (i = 0; i != m.MenuItems.Count; i++)
            {
                System.Windows.Forms.MenuItem mc = m.MenuItems[i];
                if (mc is System.Windows.Forms.MenuItem)
                    doMainMenuChecks((System.Windows.Forms.MenuItem) mc);
                if (mc is System.Windows.Forms.MenuItem)
                {
                    System.Windows.Forms.MenuItem cmi = (System.Windows.Forms.MenuItem) mc;
                    cmi.Checked = String.CompareOrdinal(mouseModeStr, SupportClass.CommandManager.GetCommand(cmi)) == 0;
                }
            }
        }
	
        public virtual void  mouseReleased(System.Object event_sender, System.Windows.Forms.MouseEventArgs e)
        {
            //todo int ex = e.getModifiersEx();
            //UPGRADE_TODO: Method 'java.awt.event.MouseEvent.isPopupTrigger' was converted to 'System.Windows.Forms.MouseButtons.Right' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawteventMouseEventisPopupTrigger'"
            //todo
            //if ((ex & (MouseEvent.SHIFT_DOWN_MASK | MouseEvent.CTRL_DOWN_MASK | MouseEvent.META_DOWN_MASK)) == 0 && e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    doPopupMenu(event_sender, e);
            //    return ;
            //}
            tempMouseMode = mouseMode;
            selectedArea = System.Drawing.Rectangle.Empty;
            dragging = false;
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
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint'"
            cv.Refresh();
        }
	
        internal virtual void  enableItems()
        {
            if (powerCheckItem.Checked)
            {
                powerBar.Enabled = true;
                powerLabel.Enabled = true;
            }
            else
            {
                powerBar.Enabled = false;
                powerLabel.Enabled = false;
            }
            enableUndoRedo();
        }
	
        public virtual void  itemStateChanged(System.Object event_sender, System.EventArgs e)
        {
            if (event_sender is System.Windows.Forms.MenuItem)
                ((System.Windows.Forms.MenuItem) event_sender).Checked = !((System.Windows.Forms.MenuItem) event_sender).Checked;
            //UPGRADE_TODO: Method 'java.awt.Component.repaint' was converted to 'System.Windows.Forms.Control.Refresh' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtComponentrepaint_long'"
            cv.Refresh();
            System.Object mi = event_sender;
            if (mi == stoppedCheck)
                return ;
            if (mi == smallGridCheckItem)
                setGrid();
            if (mi == powerCheckItem)
            {
                if (powerCheckItem.Checked)
                    voltsCheckItem.Checked = false;
                else
                    voltsCheckItem.Checked = true;
            }
            if (mi == voltsCheckItem && voltsCheckItem.Checked)
                powerCheckItem.Checked = false;
            enableItems();
            if (menuScope != - 1)
            {
                Scope sc = scopes[menuScope];
                sc.handleMenu(event_sender, e, mi);
            }
            if (mi is System.Windows.Forms.MenuItem)
            {
                System.Windows.Forms.MenuItem mmi = (System.Windows.Forms.MenuItem) mi;
                mouseMode = MODE_ADD_ELM;
                System.String s = SupportClass.CommandManager.GetCommand(mmi);
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
                        //UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
                        addingClass = System.Type.GetType(s);
                    }
                    catch (System.Exception ee)
                    {
                        SupportClass.WriteStackTrace(ee, Console.Error);
                    }
                }
                tempMouseMode = mouseMode;
            }
        }
	
        internal virtual void  setGrid()
        {
            gridSize = (smallGridCheckItem.Checked)?8:16;
            gridMask = ~ (gridSize - 1);
            gridRound = gridSize / 2 - 1;
        }
	
        internal virtual void  pushUndo()
        {
            redoStack.Clear();
            System.String s = dumpCircuit();
            if (undoStack.Count > 0 && String.CompareOrdinal(s, (System.String) (undoStack[undoStack.Count - 1])) == 0)
                return ;
            undoStack.Add(s);
            enableUndoRedo();
        }
	
        internal virtual void  doUndo()
        {
            if (undoStack.Count == 0)
                return ;
            redoStack.Add(dumpCircuit());
            System.Object tempObject;
            tempObject = undoStack[undoStack.Count - 1];
            undoStack.RemoveAt(undoStack.Count - 1);
            System.String s = (System.String) (tempObject);
            readSetup(s);
            enableUndoRedo();
        }
	
        internal virtual void  doRedo()
        {
            if (redoStack.Count == 0)
                return ;
            undoStack.Add(dumpCircuit());
            System.Object tempObject;
            tempObject = redoStack[redoStack.Count - 1];
            redoStack.RemoveAt(redoStack.Count - 1);
            System.String s = (System.String) (tempObject);
            readSetup(s);
            enableUndoRedo();
        }
	
        internal virtual void  enableUndoRedo()
        {
            redoItem.Enabled = redoStack.Count > 0;
            undoItem.Enabled = undoStack.Count > 0;
        }
	
        internal virtual void  setMenuSelection()
        {
            if (menuElm != null)
            {
                if (menuElm.selected)
                    return ;
                clearSelection();
                menuElm.Selected = true;
            }
        }
	
        internal virtual void  doCut()
        {
            int i;
            pushUndo();
            setMenuSelection();
            clipboard = "";
            for (i = elmList.Count - 1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.Selected)
                {
                    clipboard += (ce.dump() + "\n");
                    ce.delete();
                    elmList.RemoveAt(i);
                }
            }
            enablePaste();
            needAnalyze();
        }
	
        internal virtual void  doDelete()
        {
            int i;
            pushUndo();
            setMenuSelection();
            for (i = elmList.Count - 1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.Selected)
                {
                    ce.delete();
                    elmList.RemoveAt(i);
                }
            }
            needAnalyze();
        }
	
        internal virtual void  doCopy()
        {
            int i;
            clipboard = "";
            setMenuSelection();
            for (i = elmList.Count - 1; i >= 0; i--)
            {
                CircuitElm ce = getElm(i);
                if (ce.Selected)
                    clipboard += (ce.dump() + "\n");
            }
            enablePaste();
        }
	
        internal virtual void  enablePaste()
        {
            pasteItem.Enabled = clipboard.Length > 0;
        }
	
        internal virtual void  doPaste()
        {
            pushUndo();
            clearSelection();
            int i;
            System.Drawing.Rectangle oldbb = System.Drawing.Rectangle.Empty;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                System.Drawing.Rectangle bb = ce.BoundingBox;
                if (!oldbb.IsEmpty)
                    oldbb = System.Drawing.Rectangle.Union(oldbb, bb);
                else
                    oldbb = bb;
            }
            int oldsz = elmList.Count;
            readSetup(clipboard, true);
		
            // select new items
            System.Drawing.Rectangle newbb = System.Drawing.Rectangle.Empty;
            for (i = oldsz; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.Selected = true;
                System.Drawing.Rectangle bb = ce.BoundingBox;
                if (!newbb.IsEmpty)
                    newbb = System.Drawing.Rectangle.Union(newbb, bb);
                else
                    newbb = bb;
            }
            if (!oldbb.IsEmpty && !newbb.IsEmpty && oldbb.IntersectsWith(newbb))
            {
                // find a place for new items
                int dx = 0, dy = 0;
                int spacew = circuitArea.Width - oldbb.Width - newbb.Width;
                int spaceh = circuitArea.Height - oldbb.Height - newbb.Height;
                if (spacew > spaceh)
                    dx = snapGrid(oldbb.X + oldbb.Width - newbb.X + gridSize);
                else
                    dy = snapGrid(oldbb.Y + oldbb.Height - newbb.Y + gridSize);
                for (i = oldsz; i != elmList.Count; i++)
                {
                    CircuitElm ce = getElm(i);
                    ce.move(dx, dy);
                }
                // center circuit
                handleResize();
            }
            needAnalyze();
        }
	
        internal virtual void  clearSelection()
        {
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.Selected = false;
            }
        }
	
        internal virtual void  doSelectAll()
        {
            int i;
            for (i = 0; i != elmList.Count; i++)
            {
                CircuitElm ce = getElm(i);
                ce.Selected = true;
            }
        }
	
        public virtual void  keyPressed(System.Object event_sender, System.Windows.Forms.KeyEventArgs e)
        {
        }
        public virtual void  keyReleased(System.Object event_sender, System.Windows.Forms.KeyEventArgs e)
        {
        }
	
        public virtual void  keyTyped(System.Object event_sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.event.KeyEvent.getKeyChar' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            if (e.KeyChar > ' ' && e.KeyChar < 127)
            {
                //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.event.KeyEvent.getKeyChar' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                System.Type c = dumpTypes[e.KeyChar];
                if (c == null || c == typeof(Scope))
                    return ;
                CircuitElm elm = null;
                elm = constructElement(c, 0, 0);
                if (elm == null || !(elm.needsShortcut() && elm.DumpClass == c))
                    return ;
                mouseMode = MODE_ADD_ELM;
                //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Class.getName' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                mouseModeStr = c.FullName;
                addingClass = c;
            }
            //UPGRADE_TODO: The equivalent in .NET for method 'java.awt.event.KeyEvent.getKeyChar' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            if (e.KeyChar == ' ')
            {
                mouseMode = MODE_SELECT;
                mouseModeStr = "Select";
            }
            tempMouseMode = mouseMode;
        }
	
        // factors a matrix into upper and lower triangular matrices by
        // gaussian elimination.  On entry, a[0..n-1][0..n-1] is the
        // matrix to be factored.  ipvt[] returns an integer vector of pivot
        // indices, used in the lu_solve() routine.
        internal virtual bool lu_factor(double[][] a, int n, int[] ipvt)
        {
            double[] scaleFactors;
            int i, j, k;
		
            scaleFactors = new double[n];
		
            // divide each row by its largest element, keeping track of the
            // scaling factors
            for (i = 0; i != n; i++)
            {
                double largest = 0;
                for (j = 0; j != n; j++)
                {
                    double x = System.Math.Abs(a[i][j]);
                    if (x > largest)
                        largest = x;
                }
                // if all zeros, it's a singular matrix
                if (largest == 0)
                    return false;
                scaleFactors[i] = 1.0 / largest;
            }
		
            // use Crout's method; loop through the columns
            for (j = 0; j != n; j++)
            {
			
                // calculate upper triangular elements for this column
                for (i = 0; i != j; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != i; k++)
                        q -= a[i][k] * a[k][j];
                    a[i][j] = q;
                }
			
                // calculate lower triangular elements for this column
                double largest = 0;
                int largestRow = - 1;
                for (i = j; i != n; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != j; k++)
                        q -= a[i][k] * a[k][j];
                    a[i][j] = q;
                    double x = System.Math.Abs(q);
                    if (x >= largest)
                    {
                        largest = x;
                        largestRow = i;
                    }
                }
			
                // pivoting
                if (j != largestRow)
                {
                    double x;
                    for (k = 0; k != n; k++)
                    {
                        x = a[largestRow][k];
                        a[largestRow][k] = a[j][k];
                        a[j][k] = x;
                    }
                    scaleFactors[largestRow] = scaleFactors[j];
                }
			
                // keep track of row interchanges
                ipvt[j] = largestRow;
			
                // avoid zeros
                if (a[j][j] == 0.0)
                {
                    System.Console.Out.WriteLine("avoided zero");
                    a[j][j] = 1e-18;
                }
			
                if (j != n - 1)
                {
                    double mult = 1.0 / a[j][j];
                    for (i = j + 1; i != n; i++)
                        a[i][j] *= mult;
                }
            }
            return true;
        }
	
        // Solves the set of n linear equations using a LU factorization
        // previously performed by lu_factor.  On input, b[0..n-1] is the right
        // hand side of the equations, and on output, contains the solution.
        internal virtual void  lu_solve(double[][] a, int n, int[] ipvt, double[] b)
        {
            int i;
		
            // find first nonzero b element
            for (i = 0; i != n; i++)
            {
                int row = ipvt[i];
			
                double swap = b[row];
                b[row] = b[i];
                b[i] = swap;
                if (swap != 0)
                    break;
            }
		
            int bi = i++;
            for (; i < n; i++)
            {
                int row = ipvt[i];
                int j;
                double tot = b[row];
			
                b[row] = b[i];
                // forward substitution using the lower triangular matrix
                for (j = bi; j < i; j++)
                    tot -= a[i][j] * b[j];
                b[i] = tot;
            }
            for (i = n - 1; i >= 0; i--)
            {
                double tot = b[i];
			
                // back-substitution using the upper triangular matrix
                int j;
                for (j = i + 1; j != n; j++)
                    tot -= a[i][j] * b[j];
                b[i] = tot / a[i][i];
            }
        }
    }
}