using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using JavaToSharp.Elements;

namespace JavaToSharp
{ // CirSim.java (c) 2010 by Paul Falstad

// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage

//Russian translation v1.0 by Spiritus, licrym@gmail.com http://licrym.org Please mail me for updates.
//codepage UTF-8


    public class CirSim
    {
        internal Thread engine = null;

        internal Size winSize;
        internal Image dbimage;

        internal Random random;

        internal static Control main;
        internal Label titleLabel;
        internal Button resetButton;
        internal Button dumpMatrixButton;
        internal MenuItem exportItem, exportLinkItem, importItem, exitItem, undoItem, redoItem, cutItem, copyItem, pasteItem, selectAllItem, optionsItem;
        internal Menu optionsMenu;
        internal CheckBox stoppedCheck;
        internal ToolStripMenuItem dotsCheckItem;
        internal ToolStripMenuItem voltsCheckItem;
        internal ToolStripMenuItem powerCheckItem;
        internal ToolStripMenuItem smallGridCheckItem;
        internal ToolStripMenuItem showValuesCheckItem;
        internal ToolStripMenuItem euroResistorCheckItem;
        internal ToolStripMenuItem printableCheckItem;
        internal ToolStripMenuItem conventionCheckItem;
        internal HScrollBar speedBar;
        internal HScrollBar currentBar;
        internal Label powerLabel;
        internal HScrollBar powerBar;
        internal ContextMenuStrip elmMenu;
        internal MenuItem elmEditMenuItem;
        internal MenuItem elmCutMenuItem;
        internal MenuItem elmCopyMenuItem;
        internal MenuItem elmDeleteMenuItem;
        internal MenuItem elmScopeMenuItem;
        internal MenuStrip scopeMenu;
        internal MenuStrip transScopeMenu;
        internal MenuStrip mainMenu;
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
        internal MenuItem scopeSelectYMenuItem;
        internal Type addingClass;
        internal int mouseMode = MODE_SELECT;
        internal int tempMouseMode = MODE_SELECT;
        internal string mouseModeStr = "Select";
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
        internal Rectangle selectedArea;
        internal int gridSize, gridMask, gridRound;
        internal bool analyzeFlag;
        internal bool dumpMatrix;
        internal bool useBufferedImage;
        internal bool isMac;
        internal string ctrlMetaKey;
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
        internal ArrayList elmList;
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
        public bool useFrame;
        internal int scopeCount;
        internal Scope[] scopes;
        internal int[] scopeColCount;
        internal static EditDialog editDialog;
        internal static ImportDialog impDialog;
        internal Type[] dumpTypes;
        internal static string muString = "��";
        internal static string ohmString = "��";
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

        internal CirSim() 
        {
            useFrame = false;
        }

        internal string startCircuitText;
        internal string baseURL = "http://www.falstad.com/circuit/";

        public virtual void init()
        {
            CircuitElm.initClass(this);
            baseURL = Application.StartupPath;
           
            ctrlMetaKey = "Ctrl";
            
            dumpTypes = new Class[300];
            // these characters are reserved
            dumpTypes['o'] = typeof(Scope);
            dumpTypes[(int)'h'] = typeof(Scope);
            dumpTypes[(int)'$'] = typeof(Scope);
            dumpTypes[(int)'%'] = typeof(Scope);
            dumpTypes[(int)'?'] = typeof(Scope);
            dumpTypes[(int)'B'] = typeof(Scope);

            main.Layout = new CircuitLayout();
            cv = new CircuitCanvas(this);
            cv.addComponentListener(this);
            cv.addMouseMotionListener(this);
            cv.addMouseListener(this);
            cv.addKeyListener(this);
            main.add(cv);

            mainMenu = new PopupMenu();
            Menu m = new Menu("Файл");
            mainMenu.add(m);
            m.add(importItem = getMenuItem("Импорт"));
            m.add(exportItem = getMenuItem("Экспорт"));
            m.add(exportLinkItem = getMenuItem("Экспорт. ссылку"));
            m.addSeparator();
            m.add(exitItem = getMenuItem("Выход"));

            m = new Menu("Осциллограф");
            mainMenu.add(m);
            m.add(getMenuItem("Объединить всё", "stackAll"));
            m.add(getMenuItem("Разъединить всё", "unstackAll"));

            mainMenu.add(getClassCheckItem("Добавить Соединение", "WireElm"));
            mainMenu.add(getClassCheckItem("Добавить Резистор", "ResistorElm"));

            Menu passMenu = new Menu("Пассивные компоненты");
            mainMenu.add(passMenu);
            passMenu.add(getClassCheckItem("Добавить Конденсатор", "CapacitorElm"));
            passMenu.add(getClassCheckItem("Добавить Индуктивность", "InductorElm"));
            passMenu.add(getClassCheckItem("Добавить Выключатель", "SwitchElm"));
            passMenu.add(getClassCheckItem("Добавить Кнопочный выключатель", "PushSwitchElm"));
            passMenu.add(getClassCheckItem("Добавить Переключатель", "Switch2Elm"));
            passMenu.add(getClassCheckItem("Добавить Переменное сопротивление", "PotElm"));
            passMenu.add(getClassCheckItem("Добавить Трансформатор", "TransformerElm"));
            passMenu.add(getClassCheckItem("Добавить Трансформатор с отводом", "TappedTransformerElm"));
            passMenu.add(getClassCheckItem("Добавить Линию передачи", "TransLineElm"));
            passMenu.add(getClassCheckItem("Добавить Реле", "RelayElm"));
            passMenu.add(getClassCheckItem("Добавить Мемристор", "MemristorElm"));
            passMenu.add(getClassCheckItem("Добавить Искровой промежуток", "SparkGapElm"));

            Menu inputMenu = new Menu("Входы/Выходы");
            mainMenu.add(inputMenu);
            inputMenu.add(getClassCheckItem("Добавить Заземление", "GroundElm"));
            inputMenu.add(getClassCheckItem("Добавить Ист. постоянного тока (2-вывода)", "DCVoltageElm"));
            inputMenu.add(getClassCheckItem("Добавить Ист. переменного тока (2-вывода)", "ACVoltageElm"));
            inputMenu.add(getClassCheckItem("Добавить Ист. напряжения (1-вывод)", "RailElm"));
            inputMenu.add(getClassCheckItem("Добавить Ист. переменного тока (1-вывод)", "ACRailElm"));
            inputMenu.add(getClassCheckItem("Добавить Ист. Прямоуг. напряжения (1-вывод)", "SquareRailElm"));
            inputMenu.add(getClassCheckItem("Добавить Аналоговый выход", "OutputElm"));
            inputMenu.add(getClassCheckItem("Добавить Логический вход", "LogicInputElm"));
            inputMenu.add(getClassCheckItem("Добавить Логический выход", "LogicOutputElm"));
            inputMenu.add(getClassCheckItem("Добавить Тактовые импульсы", "ClockElm"));
            inputMenu.add(getClassCheckItem("Добавить Свип", "SweepElm"));
            inputMenu.add(getClassCheckItem("Добавить Регулируемое напряжение", "VarRailElm"));
            inputMenu.add(getClassCheckItem("Добавить Антенну", "AntennaElm"));
            inputMenu.add(getClassCheckItem("Добавить Источник тока", "CurrentElm"));
            inputMenu.add(getClassCheckItem("Добавить Светодиод", "LEDElm"));
            inputMenu.add(getClassCheckItem("Добавить Лампу (beta)", "LampElm"));

            Menu activeMenu = new Menu("Активные компоненты");
            mainMenu.add(activeMenu);
            activeMenu.add(getClassCheckItem("Добавить Диод", "DiodeElm"));
            activeMenu.add(getClassCheckItem("Добавить Стабилитрон", "ZenerElm"));
            activeMenu.add(getClassCheckItem("Добавить Транзистор (биполярный, NPN)", "NTransistorElm"));
            activeMenu.add(getClassCheckItem("Добавить Транзистор (биполярный, PNP)", "PTransistorElm"));
            activeMenu.add(getClassCheckItem("Добавить Операционный усилитель (- вверху)", "OpAmpElm"));
            activeMenu.add(getClassCheckItem("Добавить Операционный усилитель (+ вверху)", "OpAmpSwapElm"));
            activeMenu.add(getClassCheckItem("Добавить MOSFET (n-канальный)", "NMosfetElm"));
            activeMenu.add(getClassCheckItem("Добавить MOSFET (p-канальный)", "PMosfetElm"));
            activeMenu.add(getClassCheckItem("Добавить Полевой транзистор (n-канальный)", "NJfetElm"));
            activeMenu.add(getClassCheckItem("Добавить Полевой транзистор (p-канальный)", "PJfetElm"));
            activeMenu.add(getClassCheckItem("Добавить Аналоговый выключатель", "AnalogSwitchElm"));
            activeMenu.add(getClassCheckItem("Добавить Аналоговый переключатель", "AnalogSwitch2Elm"));
            activeMenu.add(getClassCheckItem("Добавить Тиристор", "SCRElm"));
            //activeMenu.add(getClassCheckItem("Добавить Варикап", "VaractorElm"));
            activeMenu.add(getClassCheckItem("Добавить Туннельный диод", "TunnelDiodeElm"));
            activeMenu.add(getClassCheckItem("Добавить Триод", "TriodeElm"));
            //activeMenu.add(getClassCheckItem("Добавить Динистор", "DiacElm"));
            //activeMenu.add(getClassCheckItem("Добавить Симистор", "TriacElm"));
            //activeMenu.add(getClassCheckItem("Добавить Фоторезистор", "PhotoResistorElm"));
            //activeMenu.add(getClassCheckItem("Добавить Термистор", "ThermistorElm"));
            activeMenu.add(getClassCheckItem("Добавить CCII+", "CC2Elm"));
            activeMenu.add(getClassCheckItem("Добавить CCII-", "CC2NegElm"));

            Menu gateMenu = new Menu("Логические элементы");
            mainMenu.add(gateMenu);
            gateMenu.add(getClassCheckItem("Добавить Инвертор", "InverterElm"));
            gateMenu.add(getClassCheckItem("Добавить элемент И-НЕ", "NandGateElm"));
            gateMenu.add(getClassCheckItem("Добавить элемент ИЛИ-НЕ", "NorGateElm"));
            gateMenu.add(getClassCheckItem("Добавить элемент И", "AndGateElm"));
            gateMenu.add(getClassCheckItem("Добавить элемент ИЛИ", "OrGateElm"));
            gateMenu.add(getClassCheckItem("Добавить элемент исключающее ИЛИ", "XorGateElm"));

            Menu chipMenu = new Menu("Микросхемы");
            mainMenu.add(chipMenu);
            chipMenu.add(getClassCheckItem("Добавить D триггер", "DFlipFlopElm"));
            chipMenu.add(getClassCheckItem("Добавить JK триггер", "JKFlipFlopElm"));
            chipMenu.add(getClassCheckItem("Добавить 7ми сегментный светодиод", "SevenSegElm"));
            chipMenu.add(getClassCheckItem("Добавить VCO", "VCOElm"));
            chipMenu.add(getClassCheckItem("Добавить Фазовый компаратор", "PhaseCompElm"));
            chipMenu.add(getClassCheckItem("Добавить Счетчик", "CounterElm"));
            chipMenu.add(getClassCheckItem("Добавить Декадный счетчик", "DecadeElm"));
            chipMenu.add(getClassCheckItem("Добавить 555 Таймер", "TimerElm"));
            chipMenu.add(getClassCheckItem("Добавить ЦАП", "DACElm"));
            chipMenu.add(getClassCheckItem("Добавить АЦП", "ADCElm"));
            chipMenu.add(getClassCheckItem("Добавить Защелку", "LatchElm"));

            Menu otherMenu = new Menu("Прочее");
            mainMenu.add(otherMenu);
            otherMenu.add(getClassCheckItem("Добавить Текст", "TextElm"));
            otherMenu.add(getClassCheckItem("Добавить пробу осциллографа", "ProbeElm"));
            otherMenu.add(getCheckItem("Drag All (Alt-drag)", "DragAll"));
            otherMenu.add(getCheckItem(isMac ? "Drag Row (Alt-S-drag, S-right)" : "Drag Row (S-right)", "DragRow"));
            otherMenu.add(getCheckItem(isMac ? "Drag Column (Alt-\u2318-drag, \u2318-right)" : "Drag Column (C-right)", "DragColumn"));
            otherMenu.add(getCheckItem("Drag Selected", "DragSelected"));
            otherMenu.add(getCheckItem("Drag Post (" + ctrlMetaKey + "-drag)", "DragPost"));

            mainMenu.add(getCheckItem("Выбор/перетаскивание выбранного (пробел или Shift+щелчок)", "Select"));
            main.add(mainMenu);

            main.add(resetButton = new Button("Сброс"));
            resetButton.addActionListener(this);
            dumpMatrixButton = new Button("Dump Matrix");
            //main.add(dumpMatrixButton);
            dumpMatrixButton.addActionListener(this);
            stoppedCheck = new Checkbox("Остановлено");
            stoppedCheck.addItemListener(this);
            main.add(stoppedCheck);

            main.add(new Label("Скорость симуляции", Label.CENTER));

            // was max of 140
            main.add(speedBar = new Scrollbar(Scrollbar.HORIZONTAL, 3, 1, 0, 260));
            speedBar.addAdjustmentListener(this);

            main.add(new Label("Скорость тока", Label.CENTER));
            currentBar = new Scrollbar(Scrollbar.HORIZONTAL, 50, 1, 1, 100);
            currentBar.addAdjustmentListener(this);
            main.add(currentBar);

            main.add(powerLabel = new Label("Яркость мощности", Label.CENTER));
            main.add(powerBar = new Scrollbar(Scrollbar.HORIZONTAL, 50, 1, 1, 100));
            powerBar.addAdjustmentListener(this);
            powerBar.disable();
            powerLabel.disable();

            main.add(new Label("www.falstad.com"));
            main.add(new Label("Перевод licrym.org")); //"translated into russian by licrym.org"

            if (useFrame)
                main.add(new Label(""));
            Font f = new Font("SansSerif", 0, 10);
            Label l;
            l = new Label("Текущая схема:");
            l.Font = f;
            titleLabel = new Label("Метка");
            titleLabel.Font = f;
            if (useFrame)
            {
                main.add(l);
                main.add(titleLabel);
            }

            setGrid();
            elmList = new ArrayList();
            new ArrayList();
            undoStack = new ArrayList();
            redoStack = new ArrayList();

            scopes = new Scope[20];
            scopeColCount = new int[20];
            scopeCount = 0;

            random = new Random();
            cv.Background = Color.black;
            cv.Foreground = Color.lightGray;

            elmMenu = new PopupMenu();
            elmMenu.add(elmEditMenuItem = getMenuItem("Параметры"));
            elmMenu.add(elmScopeMenuItem = getMenuItem("Смотреть в осциллографе"));
            elmMenu.add(elmCutMenuItem = getMenuItem("Вырезать"));
            elmMenu.add(elmCopyMenuItem = getMenuItem("Копировать"));
            elmMenu.add(elmDeleteMenuItem = getMenuItem("Удалить"));
            main.add(elmMenu);

            scopeMenu = buildScopeMenu(false);
            transScopeMenu = buildScopeMenu(true);

            Menu circuitsMenu = new Menu("Схемы");
            getSetupList(circuitsMenu);
            if (startCircuitText != null)
                readSetup(startCircuitText);
            
           
            if (!powerCheckItem.State)
            {
                main.remove(powerBar);
                main.remove(powerLabel);
                main.validate();
            }
        }

        internal bool shown = false;

        public virtual void triggerShow()
        {
            if (!shown)
                show();
            shown = true;
        }

        #region UI Form

        internal virtual ContextMenuStrip buildScopeMenu(bool t)
        {
            ContextMenuStrip m = new ContextMenuStrip();
            m.Items.Add(getMenuItem("Убрать", "remove"));
            m.add(getMenuItem("Скорость 2x", "speed2"));
            m.add(getMenuItem("Скорость 1/2x", "speed1/2"));
            m.add(getMenuItem("Масштаб 2x", "scale"));
            m.add(getMenuItem("Максимальный масштаб", "maxscale"));
            m.add(getMenuItem("Объединить", "stack"));
            m.add(getMenuItem("Разъединить", "unstack"));
            m.add(getMenuItem("Сброс", "reset"));
            if (t)
            {
                m.add(scopeIbMenuItem = getCheckItem("Показать Iб"));
                m.add(scopeIcMenuItem = getCheckItem("Показать Iк"));
                m.add(scopeIeMenuItem = getCheckItem("Показать Iэ"));
                m.add(scopeVbeMenuItem = getCheckItem("Показать Vбэ"));
                m.add(scopeVbcMenuItem = getCheckItem("Показать Vбк"));
                m.add(scopeVceMenuItem = getCheckItem("Показать Vкэ"));
                m.add(scopeVceIcMenuItem = getCheckItem("Показать Vкэ и Iк"));
            }
            else
            {
                m.add(scopeVMenuItem = getCheckItem("Показать Напряжение"));
                m.add(scopeIMenuItem = getCheckItem("Показать Ток"));
                m.add(scopePowerMenuItem = getCheckItem("Показать Потребленную мощность"));
                m.add(scopeMaxMenuItem = getCheckItem("Показать Пиковое значение"));
                m.add(scopeMinMenuItem = getCheckItem("Показать Отрицательное пиковое значение"));
                m.add(scopeFreqMenuItem = getCheckItem("Показать Частоту"));
                m.add(scopeVIMenuItem = getCheckItem("Показать V и I"));
                m.add(scopeXYMenuItem = getCheckItem("График X/Y"));
                m.add(scopeSelectYMenuItem = getMenuItem("Показать Y", "selecty"));
                m.add(scopeResistMenuItem = getCheckItem("Показать Сопротивление"));
            }
            main.add(m);
            return m;
        }

        internal virtual MenuItem getMenuItem(string s)
        {
            MenuItem mi = new MenuItem(s);
            mi.addActionListener(this);
            return mi;
        }

        internal virtual MenuItem getMenuItem(string s, string ac)
        {
            MenuItem mi = new MenuItem(s);
            mi.ActionCommand = ac;
            mi.addActionListener(this);
            return mi;
        }

        internal virtual ToolStripMenuItem getCheckItem(string s)
        {
            CheckboxMenuItem mi = new CheckboxMenuItem(s);
            mi.addItemListener(this);
            mi.ActionCommand = "";
            return mi;
        }

        internal virtual CheckboxMenuItem getClassCheckItem(string s, string t)
        {
            try
            {
                Type c = Type.GetType(t);
                CircuitElm elm = constructElement(c, 0, 0);
                register(c, elm);
                int dt = 0;
                if (elm.needsShortcut() && elm.DumpClass == c)
                {
                    dt = elm.DumpType;
                    s += " (" + (char)dt + ")";
                }
                elm.delete();
            }
            catch (Exception ee)
            {
                ee.printStackTrace();
            }
            return getCheckItem(s, t);
        }

        internal virtual CheckboxMenuItem getCheckItem(string s, string t)
        {
            CheckboxMenuItem mi = new CheckboxMenuItem(s);
            mi.addItemListener(this);
            mi.ActionCommand = t;
            return mi;
        }

        #endregion

        internal virtual void register(Type c, CircuitElm elm)
        {
            int t = elm.DumpType;
            if (t == 0)
            {
                Console.WriteLine("no dump type: " + c);
                return;
            }
            Type dclass = elm.DumpClass;
            if (dumpTypes[t] == dclass)
                return;
            if (dumpTypes[t] != null)
            {
                Console.WriteLine("dump type conflict: " + c + " " + dumpTypes[t]);
                return;
            }
            dumpTypes[t] = dclass;
        }

        internal virtual void handleResize()
        {
            winSize = cv.Size;
            if (winSize.width == 0)
                return;
            dbimage = main.createImage(winSize.width, winSize.height);
            int h = winSize.height / 5;
//	if (h < 128 && winSize.height > 300)
//	  h = 128;
            circuitArea = new Rectangle(0, 0, winSize.width, winSize.height-h);
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
            int dx = gridMask & ((circuitArea.width -(maxx-minx))/2-minx);
            int dy = gridMask & ((circuitArea.height-(maxy-miny))/2-miny);
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
            if (winSize == null || winSize.width == 0)
                return;
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
            var g = Graphics.FromImage(dbimage);
            CircuitElm.selectColor = Color.Cyan;
            Brush backBrush;
            if (printableCheckItem.State)
            {
                CircuitElm.whiteColor = Color.Black;
                CircuitElm.lightGrayColor = Color.Black;
                backBrush = Brushes.White;
            }
            else
            {
                CircuitElm.whiteColor = Color.White;
                CircuitElm.lightGrayColor = Color.LightGray;
                backBrush = Brushes.Black;
            }
            g.FillRectangle(backBrush, 0, 0, winSize.width, winSize.height);
            if (!stoppedCheck.State)
            {
                try
                {
                    runCircuit();
                }
                catch (Exception e)
                {
                    UserMessageView.Instance.ShowError(e.StackTrace);
                    analyzeFlag = true;
                    cv.repaint();
                    return;
                }
            }
            if (!stoppedCheck.State)
            {
                long sysTime = DateTime.Now.Millisecond;
                if (lastTime != 0)
                {
                    int inc = (int)(sysTime-lastTime);
                    double c = currentBar.Value;
                    c = Math.Exp(c/3.5-14.2);
                    CircuitElm.currentMult = 1.7 * inc * c;
                    if (!conventionCheckItem.State)
                        CircuitElm.currentMult = -CircuitElm.currentMult;
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
            CircuitElm.powerMult = Math.Exp(powerBar.Value/4.762-7);

            int i;
            Font oldfont = g.Font;
            for (i = 0; i != elmList.Count; i++)
            {
                if (powerCheckItem.State)
                    g.Color = Color.gray;
//	    else if (conductanceCheckItem.getState())
//	      g.setColor(Color.white);
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
                if (!cn.internal && cn.links.Count == 1)
                {
                    int bb = 0;
                    var cnl = (CircuitNodeLink) cn.links[0];
                    for (int j = 0; j != elmList.Count; j++)
                        if (cnl.elm != getElm(j) && getElm(j).boundingBox.Contains(cn.x, cn.y))
                            bb++;
                    if (bb > 0)
                    {
                        g.FillEllipse(Brushes.Red, cn.x-3, cn.y-3, 7, 7);
                        badnodes++;
                    }
                }
            }
//	if (mouseElm != null) {
//	    g.setFont(oldfont);
//	    g.drawString("+", mouseElm.x+10, mouseElm.y);
//	    }
            if (dragElm != null && (dragElm.x != dragElm.x2 || dragElm.y != dragElm.y2))
                dragElm.draw(g);
            int ct = scopeCount;
            if (stopMessage != null)
                ct = 0;
            for (i = 0; i != ct; i++)
                scopes[i].draw(g);
            var brush = new SolidBrush(CircuitElm.whiteColor);
            if (stopMessage != null)
            {
                g.DrawString(stopMessage, oldfont, brush, 10, circuitArea.Height);
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
                        info[0] = "V = " + CircuitElm.getUnitText(mouseElm.getPostVoltage(mousePost), "V");
//		 //shownodes
//		for (i = 0; i != mouseElm.getPostCount(); i++)
//		    info[0] += " " + mouseElm.nodes[i];
//		if (mouseElm.getVoltageSourceCount() > 0)
//		    info[0] += ";" + (mouseElm.getVoltageSource()+nodeList.size());
//		

                }
                else
                {
                    CircuitElm.showFormat.MinimumFractionDigits = 2;
                    info[0] = "t = " + CircuitElm.getUnitText(t, "с");
                    CircuitElm.showFormat.MinimumFractionDigits = 0;
                }
                if (hintType != -1)
                {
                    for (i = 0; info[i] != null; i++)
                        ;
                    string s = Hint;
                    if (s == null)
                        hintType = -1;
                    else
                        info[i] = s;
                }
                int x = 0;
                if (ct != 0)
                    x = scopes[ct-1].rightEdge() + 20;
                x = max(x, winSize.width*2/3);

                // count lines of data
                for (i = 0; info[i] != null; i++)
                    ;
                if (badnodes > 0)
                    info[i++] = badnodes + ((badnodes == 1) ? " плохое соединение" : " плохие соединения");

                // find where to show data; below circuit, not too high unless we need it
                int ybase = winSize.height-15*i-5;
                ybase = min(ybase, circuitArea.Height);
                ybase = max(ybase, circuitBottom);
                for (i = 0; info[i] != null; i++)
                    g.DrawString(info[i], x, ybase+15*(i+1));
            }
            if (selectedArea != null)
            {
                var pen = new Pen(CircuitElm.selectColor);
                g.DrawRectangle(pen, selectedArea.X, selectedArea.Y, selectedArea.Width, selectedArea.Height);
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

            realg.drawImage(dbimage, 0, 0, this);
            if (!stoppedCheck.State && circuitMatrix != null)
            {
                // Limit to 50 fps (thanks to Jьrgen Klцtzer for this)
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
                cv.repaint(0);
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
            int h = winSize.height - circuitArea.height;
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
            int w = (winSize.width-iw) / colct;
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
                Rectangle r = new Rectangle(pos*w, winSize.height-h+colh*row, w-marg, colh);
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
                    return "res.f = " + CircuitElm.getUnitText(1/(2*pi*Math.Sqrt(ie.inductance* ce.capacitance)), "Гц");
                }
                if (hintType == HINT_RC)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "RC = " + CircuitElm.getUnitText(re.resistance*ce.capacitance, "с");
                }
                if (hintType == HINT_3DB_C)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText(1/(2*pi*re.resistance*ce.capacitance), "Гц");
                }
                if (hintType == HINT_3DB_L)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is InductorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    InductorElm ie = (InductorElm) c2;
                    return "f.3db = " + CircuitElm.getUnitText(re.resistance/(2*pi*ie.inductance), "Гц");
                }
                if (hintType == HINT_TWINT)
                {
                    if (!(c1 is ResistorElm))
                        return null;
                    if (!(c2 is CapacitorElm))
                        return null;
                    ResistorElm re = (ResistorElm) c1;
                    CapacitorElm ce = (CapacitorElm) c2;
                    return "fc = " + CircuitElm.getUnitText(1/(2*pi*re.resistance*ce.capacitance), "Гц");
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
                        cv.repaint();
                        return;
                    }
                }
            }
        }

        internal virtual void needAnalyze()
        {
            analyzeFlag = true;
            cv.repaint();
        }

        internal ArrayList nodeList;
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
                cn.x = pt.x;
                cn.y = pt.y;
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
                        if (pt.x == cn.x && pt.y == cn.y)
                            break;
                    }
                    if (k == nodeList.Count)
                    {
                        CircuitNode cn = new CircuitNode();
                        cn.x = pt.x;
                        cn.y = pt.y;
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        cn.links.addElement(cnl);
                        ce.setNode(j, nodeList.Count);
                        nodeList.Add(cn);
                    }
                    else
                    {
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.num = j;
                        cnl.elm = ce;
                        getCircuitNode(k).links.addElement(cnl);
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
                    cn.internal = true;
                    CircuitNodeLink cnl = new CircuitNodeLink();
                    cnl.num = j+posts;
                    cnl.elm = ce;
                    cn.links.addElement(cnl);
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
//ORIGINAL LINE: circuitMatrix = new double[matrixSize][matrixSize];
//JAVA TO VB & C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            circuitMatrix = RectangularArrays.ReturnRectangularDoubleArray(matrixSize, matrixSize);
            circuitRightSide = new double[matrixSize];
//ORIGINAL LINE: origMatrix = new double[matrixSize][matrixSize];
//JAVA TO VB & C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
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
                    if (!closure[i] && !getCircuitNode(i).internal)
                    {
                        Console.WriteLine("узел " + i + " не подключен");
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
                    FindPathInfo fpi = new FindPathInfo(FindPathInfo.INDUCT, ce, ce.getNode(1));
                    // first try findPath with maximum depth of 5, to avoid slowdowns
                    if (!fpi.findPath(ce.getNode(0), 5) && !fpi.findPath(ce.getNode(0)))
                    {
                        Console.WriteLine(ce + " нет пути");
                        ce.reset();
                    }
                }
                // look for current sources with no current path
                if (ce is CurrentElm)
                {
                    FindPathInfo fpi = new FindPathInfo(FindPathInfo.INDUCT, ce, ce.getNode(1));
                    if (!fpi.findPath(ce.getNode(0)))
                    {
                        stop("Нет пути для источника тока!", ce);
                        return;
                    }
                }
                // look for voltage source loops
                if ((ce is VoltageElm && ce.PostCount == 2) || ce is WireElm)
                {
                    FindPathInfo fpi = new FindPathInfo(FindPathInfo.VOLTAGE, ce, ce.getNode(1));
                    if (fpi.findPath(ce.getNode(0)))
                    {
                        stop("Короткое замыкание источника напряжения!", ce);
                        return;
                    }
                }
                // look for shorted caps, or caps w/ voltage but no R
                if (ce is CapacitorElm)
                {
                    FindPathInfo fpi = new FindPathInfo(FindPathInfo.SHORT, ce, ce.getNode(1));
                    if (fpi.findPath(ce.getNode(0)))
                    {
                        Console.WriteLine(ce + " shorted");
                        ce.reset();
                    }
                    else
                    {
                        fpi = new FindPathInfo(FindPathInfo.CAP_V, ce, ce.getNode(1));
                        if (fpi.findPath(ce.getNode(0)))
                        {
                            stop("Короткое замыкание конденсатора!", ce);
                            return;
                        }
                    }
                }
            }
            //System.out.println("ac6");

            // simplify the matrix; this speeds things up quite a bit
            for (i = 0; i != matrixSize; i++)
            {
                int qm = -1, qp = -1;
                double qv = 0;
                RowInfo re = circuitRowInfo[i];
//	    System.out.println("row " + i + " " + re.lsChanges + " " + re.rsChanges + " " +
//			       re.dropRow);
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
                    if (q == 0)
                        continue;
                    if (qp == -1)
                    {
                        qp = j;
                        qv = q;
                        continue;
                    }
                    if (qm == -1 && q == -qv)
                    {
                        qm = j;
                        continue;
                    }
                    break;
                }
                //System.out.println("line " + i + " " + qp + " " + qm + " " + j);
//	    if (qp != -1 && circuitRowInfo[qp].lsChanges) {
//		System.out.println("lschanges");
//		continue;
//	    }
//	    if (qm != -1 && circuitRowInfo[qm].lsChanges) {
//		System.out.println("lschanges");
//		continue;
//		}
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
//			System.out.println("following equal chain from " +
//					   i + " " + qp + " to " + elt.nodeEq);
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
                        //System.out.println(qp + " * " + qv + " = const " + elt.value);
                        i = -1; // start over from scratch
                    }
                    else if (circuitRightSide[i]+rsadd == 0)
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

//	System.out.println("matrixSize = " + matrixSize);
//	
//	for (j = 0; j != circuitMatrixSize; j++) {
//	    System.out.println(j + ": ");
//	    for (i = 0; i != circuitMatrixSize; i++)
//		System.out.print(circuitMatrix[j][i] + " ");
//	    System.out.print("  " + circuitRightSide[j] + "\n");
//	}
//	System.out.print("\n");


            // make the new, simplified matrix
            int newsize = nn;
//ORIGINAL LINE: double[][] newmatx = new double[newsize][newsize];
//JAVA TO VB & C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
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
                //System.out.println("Row " + i + " maps to " + ii);
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

//	
//	System.out.println("matrixSize = " + matrixSize + " " + circuitNonLinear);
//	for (j = 0; j != circuitMatrixSize; j++) {
//	    for (i = 0; i != circuitMatrixSize; i++)
//		System.out.print(circuitMatrix[j][i] + " ");
//	    System.out.print("  " + circuitRightSide[j] + "\n");
//	}
//	System.out.print("\n");

            // if a matrix is linear, we can do the lu_factor here instead of
            // needing to do it every frame
            if (!circuitNonLinear)
            {
                if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                {
                    stop("Singular matrix!", null);
                    return;
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
                int bottom = rect.height + rect.y;
                if (bottom > circuitBottom)
                    circuitBottom = bottom;
            }
        }

        internal class FindPathInfo
        {
            internal const int INDUCT = 1;
            internal const int VOLTAGE = 2;
            internal const int SHORT = 3;
            internal const int CAP_V = 4;
            internal bool[] used;
            internal int dest;
            internal CircuitElm firstElm;
            internal int type;
//JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
            internal FindPathInfo(int t, CircuitElm e, int d)
            {
                dest = d;
//JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
                type = t;
                firstElm = e;
//JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
                used = new bool[nodeList.size()];
            }
            internal virtual bool findPath(int n1)
            {
                return findPath(n1, -1);
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
//JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
                for (i = 0; i != elmList.size(); i++)
                {
//JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
                    CircuitElm ce = getElm(i);
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
                    int j;
                    for (j = 0; j != ce.PostCount; j++)
                    {
                        //System.out.println(ce + " " + ce.getNode(j));
                        if (ce.getNode(j) == n1)
                            break;
                    }
                    if (j == ce.PostCount)
                        continue;
                    if (ce.hasGroundConnection(j) && findPath(0, depth))
                    {
                        //System.out.println(ce + " has ground");
                        used[n1] = false;
                        return true;
                    }
                    if (type == INDUCT && ce is InductorElm)
                    {
                        double c = ce.Current;
                        if (j == 0)
                            c = -c;
                        //System.out.println("matching " + c + " to " + firstElm.getCurrent());
                        //System.out.println(ce + " " + firstElm);
                        if (Math.Abs(c-firstElm.Current) > 1e-10)
                            continue;
                    }
                    int k;
                    for (k = 0; k != ce.PostCount; k++)
                    {
                        if (j == k)
                            continue;
                        //System.out.println(ce + " " + ce.getNode(j) + "-" + ce.getNode(k));
                        if (ce.getConnection(j, k) && findPath(ce.getNode(k), depth))
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



        internal virtual void stop(string s, CircuitElm ce)
        {
            stopMessage = s;
            circuitMatrix = null;
            stopElm = ce;
            stoppedCheck.State = true;
            analyzeFlag = false;
            cv.repaint();
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
                if (speedBar.Value == 0)
                    return 0;
                //return (Math.exp((speedBar.getValue()-1)/24.) + .5);
                return.1*Math.Exp((speedBar.Value-61)/24.0);
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
            bool debugprint = dumpMatrix;
            dumpMatrix = false;
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
                    bool printit = debugprint;
                    debugprint = false;
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
                    if (printit)
                    {
                        for (j = 0; j != circuitMatrixSize; j++)
                        {
                            for (i = 0; i != circuitMatrixSize; i++)
                                Console.Write(circuitMatrix[j][i] + ",");
                            Console.Write("  " + circuitRightSide[j] + "\n");
                        }
                        Console.Write("\n");
                    }
                    if (circuitNonLinear)
                    {
                        if (converged && subiter > 0)
                            break;
                        if (!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                        {
                            stop("Singular matrix!", null);
                            return;
                        }
                    }
                    lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);

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

        public virtual void componentShown(ComponentEvent e)
        {
            cv.repaint();
        }

        public virtual void componentResized(ComponentEvent e)
        {
            handleResize();
            cv.repaint(100);
        }
        public virtual void actionPerformed(ActionEvent e)
        {
            string ac = e.ActionCommand;
            if (e.Source == resetButton)
            {
                int i;

                // on IE, drawImage() stops working inexplicably every once in
                // a while.  Recreating it fixes the problem, so we do that here.
                dbimage = main.createImage(winSize.width, winSize.height);

                for (i = 0; i < elmList.Count; i++)
                    getElm(i).reset();
                for (i = 0; i < scopeCount; i++)
                    scopes[i].resetGraph();
                analyzeFlag = true;
                t = 0;
                stoppedCheck.State = false;
                cv.repaint();
            }
            if (e.Source == dumpMatrixButton)
                dumpMatrix = true;
            if (e.Source == exportItem)
                doImport(false, false);
            if (e.Source == optionsItem)
                doEdit(new EditOptions(this));
            if (e.Source == importItem)
                doImport(true, false);
            if (e.Source == exportLinkItem)
                doImport(false, true);
            if (e.Source == undoItem)
                doUndo();
            if (e.Source == redoItem)
                doRedo();
            if (String.CompareOrdinal(ac, "Вырезать") == 0)
            {
                if (e.Source != elmCutMenuItem)
                    menuElm = null;
                doCut();
            }
            if (String.CompareOrdinal(ac, "Копировать") == 0)
            {
                if (e.Source != elmCopyMenuItem)
                    menuElm = null;
                doCopy();
            }
            if (String.CompareOrdinal(ac, "Вставить") == 0)
                doPaste();
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
            if (String.CompareOrdinal(ac, "Удалить") == 0)
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

        protected virtual void stackAll()
        {
            int i;
            for (i = 0; i != scopeCount; i++)
            {
                scopes[i].position = 0;
                scopes[i].showMax(false);
                scopes[i].showMin(false);
            }
        }

        protected virtual void unstackAll()
        {
            int i;
            for (i = 0; i != scopeCount; i++)
            {
                scopes[i].position = i;
                scopes[i].showMax(true);
            }
        }

        internal virtual void doEdit(Editable eable)
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
        }

        protected virtual void doImport(bool imp, bool url)
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
        }

        protected virtual string dumpCircuit()
        {
            int i;
            int f = (dotsCheckItem.State) ? 1 : 0;
            f |= (smallGridCheckItem.State) ? 2 : 0;
            f |= (voltsCheckItem.State) ? 0 : 4;
            f |= (powerCheckItem.State) ? 8 : 0;
            f |= (showValuesCheckItem.State) ? 0 : 16;
            // 32 = linear scale in afilter
            string dump = "$ " + f + " " + timeStep + " " + IterCount + " " + currentBar.Value + " " + CircuitElm.voltageRange + " " + powerBar.Value + "\n";
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

        public virtual void adjustmentValueChanged(AdjustmentEvent e)
        {
            Console.Write(((Scrollbar) e.Source).Value + "\n");
        }

//JAVA TO VB & C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ByteArrayOutputStream readUrlData(URL url) throws java.io.IOException
        protected virtual ByteArrayOutputStream readUrlData(URL url)
        {
            object o = url.Content;
            FilterInputStream fis = (FilterInputStream) o;
            ByteArrayOutputStream ba = new ByteArrayOutputStream(fis.available());
            int blen = 1024;
            sbyte[] b = new sbyte[blen];
            while (true)
            {
                int len = fis.read(b);
                if (len <= 0)
                    break;
                ba.write(b, 0, len);
            }
            return ba;
        }
        
        protected virtual void getSetupList(Menu menu)
        {
            Menu[] stack = new Menu[6];
            int stackptr = 0;
            stack[stackptr++] = menu;
            try
            {
                byte[] b = File.ReadAllBytes("setuplist.txt");
                int len = b.Length;
                int p;
                if (len == 0 || b[0] != '#')
                {
                    // got a redirect, try again
                    getSetupList(menu);
                    return;
                }
                for (p = 0; p < len;)
                {
                    int l;
                    for (l = 0; l != len-p; l++)
                        if (b[l+p] == '\n')
                        {
                            l++;
                            break;
                        }
                    string line = new string(b, p, l-1, "UTF-8");
                    if (line[0] == '#')
                        ;
                    else if (line[0] == '+')
                    {
                        Menu n = new Menu(line.Substring(1));
                        menu.add(n);
                        menu = stack[stackptr++] = n;
                    }
                    else if (line[0] == '-')
                    {
                        menu = stack[--stackptr-1];
                    }
                    else
                    {
                        int i = line.IndexOf(' ');
                        if (i > 0)
                        {
                            string title = line.Substring(i+1);
                            bool first = line[0] == '>';
                            int start = first ? 1 : 0;
                            string file = line.Substring(start, i - start);
                            menu.add(getMenuItem(title, "setup " + file));
                            if (first)
                            {
                            }
                        }
                    }
                    p += l;
                }
            }
            catch (Exception e)
            {
                e.printStackTrace();
                stop("Can't read setuplist.txt!", null);
            }
        }

        internal virtual void readSetup(string text)
        {
            readSetup(text, false);
        }

        protected virtual void readSetup(string text, bool retain)
        {
            readSetup(text.Bytes, text.Length, retain);
            titleLabel.Text = "untitled";
        }

        protected virtual void readSetupFile(string str, string title)
        {
            t = 0;
            Console.WriteLine(str);
            try
            {
                URL url = new URL(CodeBase + "circuits/" + str);
                ByteArrayOutputStream ba = readUrlData(url);
                readSetup(ba.toByteArray(), ba.size(), false);
            }
            catch (Exception e)
            {
                e.printStackTrace();
                stop("Unable to read " + str + "!", null);
            }
            titleLabel.Text = title;
        }

        protected virtual void readSetup(sbyte[] b, int len, bool retain)
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
                hintType = -1;
                timeStep = 5e-6;
                dotsCheckItem.State = true;
                smallGridCheckItem.State = false;
                powerCheckItem.State = false;
                voltsCheckItem.State = true;
                showValuesCheckItem.State = true;
                setGrid();
                speedBar.Value = 117; // 57
                currentBar.Value = 50;
                powerBar.Value = 50;
                CircuitElm.voltageRange = 5;
                scopeCount = 0;
            }
            cv.repaint();
            int p;
            for (p = 0; p < len;)
            {
                int l;
                int linelen = 0;
                for (l = 0; l != len-p; l++)
                    if (b[l+p] == '\n' || b[l+p] == '\r')
                    {
                        linelen = l++;
                        if (l+p < b.Length && b[l+p] == '\n')
                            l++;
                        break;
                    }
                string line = null;
                try
                {
                    line = new string(b, p, linelen, "UTF-8");
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
                StringTokenizer st = new StringTokenizer(line);
                while (st.hasMoreTokens())
                {
                    string type = st.nextToken();
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
                            tint = new (int)int?(type);
                        int x1 = new (int)int?(st.nextToken());
                        int y1 = new (int)int?(st.nextToken());
                        int x2 = new (int)int?(st.nextToken());
                        int y2 = new (int)int?(st.nextToken());
                        int f = new (int)int?(st.nextToken());
                        CircuitElm ce = null;
                        Type cls = dumpTypes[tint];
                        if (cls == null)
                        {
                            Console.WriteLine("unrecognized dump type: " + type);
                            break;
                        }
                        // find element class
                        Type[] carr = new Class[6];
                        //carr[0] = getClass();
                        carr[0] = carr[1] = carr[2] = carr[3] = carr[4] = typeof(int);
                        carr[5] = typeof(StringTokenizer);
                        Constructor cstr = null;
                        cstr = cls.GetConstructor(carr);

                        // invoke constructor with starting coordinates
                        object[] oarr = new object[6];
                        //oarr[0] = this;
                        oarr[0] = x1;
                        oarr[1] = y1;
                        oarr[2] = x2;
                        oarr[3] = y2;
                        oarr[4] = f;
                        oarr[5] = st;
                        ce = (CircuitElm) cstr.newInstance(oarr);
                        ce.setPoints();
                        elmList.Add(ce);
                    }
                    catch (java.lang.reflect.InvocationTargetException ee)
                    {
                        ee.TargetException.printStackTrace();
                        break;
                    }
                    catch (Exception ee)
                    {
                        ee.printStackTrace();
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

        internal virtual void readHint(StringTokenizer st)
        {
            hintType = new (int)int?(st.nextToken());
            hintItem1 = new (int)int?(st.nextToken());
            hintItem2 = new (int)int?(st.nextToken());
        }

        internal virtual void readOptions(StringTokenizer st)
        {
            int flags = new (int)int?(st.nextToken());
            dotsCheckItem.State = (flags & 1) != 0;
            smallGridCheckItem.State = (flags & 2) != 0;
            voltsCheckItem.State = (flags & 4) == 0;
            powerCheckItem.State = (flags & 8) == 8;
            showValuesCheckItem.State = (flags & 16) == 0;
            timeStep = new (double)double? (st.nextToken());
            double sp = new (double)double?(st.nextToken());
            int sp2 = (int)(Math.Log(10*sp)*24+61.5);
            //int sp2 = (int) (Math.log(sp)*24+1.5);
            speedBar.Value = sp2;
            currentBar.Value = new (int)int?(st.nextToken());
            CircuitElm.voltageRange = new (double)double? (st.nextToken());
            try
            {
                powerBar.Value = new (int)int?(st.nextToken());
            }
            catch (Exception e)
            {
            }
            setGrid();
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

        public virtual void mouseDragged(MouseEvent e)
        {
            // ignore right mouse button with no modifiers (needed on PC)
            if ((e.Modifiers & MouseEvent.BUTTON3_MASK) != 0)
            {
                int ex = e.ModifiersEx;
                if ((ex & (MouseEvent.META_DOWN_MASK| MouseEvent.SHIFT_DOWN_MASK| MouseEvent.CTRL_DOWN_MASK| MouseEvent.ALT_DOWN_MASK)) == 0)
                    return;
            }
            if (!circuitArea.contains(e.X, e.Y))
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
        }

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

            int dx = x-dragX;
            int dy = y-dragY;
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
                    ce.Selected = ce == value;
                }
                mouseElm = value;
            }
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

        public virtual void mouseMoved(MouseEvent e)
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
                if (ce.boundingBox.contains(x, y))
                {
                    int j;
                    int area = ce.boundingBox.width * ce.boundingBox.height;
                    int jn = ce.PostCount;
                    if (jn > 2)
                        jn = 2;
                    for (j = 0; j != jn; j++)
                    {
                        Point pt = ce.getPost(j);
                        int dist = distanceSq(x, y, pt.x, pt.y);

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
                    if (s.rect.contains(x, y))
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
                        int dist = distanceSq(x, y, pt.x, pt.y);
                        if (distanceSq(pt.x, pt.y, x, y) < 26)
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
                    if (distanceSq(pt.x, pt.y, x, y) < 26)
                        mousePost = i;
                }
            }
            if (mouseElm != origMouse)
                cv.repaint();
        }

        protected virtual int distanceSq(int x1, int y1, int x2, int y2)
        {
            x2 -= x1;
            y2 -= y1;
            return x2*x2+y2*y2;
        }

        public virtual void mouseClicked(MouseEvent e)
        {
            if ((e.Modifiers & MouseEvent.BUTTON1_MASK) != 0)
            {
                if (mouseMode == MODE_SELECT || mouseMode == MODE_DRAG_SELECTED)
                    clearSelection();
            }
        }
        public virtual void mouseEntered(MouseEvent e)
        {
        }

        public virtual void mouseExited(MouseEvent e)
        {
            scopeSelected = -1;
            mouseElm = plotXElm = plotYElm = null;
            cv.repaint();
        }

        public virtual void mousePressed(MouseEvent e)
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
            if (!circuitArea.contains(x0, y0))
                return;

            dragElm = constructElement(addingClass, x0, y0);
        }

        protected virtual CircuitElm constructElement(Type c, int x0, int y0)
        {
            // find element class
            Type[] carr = new Class[2];
            //carr[0] = getClass();
            carr[0] = carr[1] = typeof(int);
            Constructor cstr = null;
            try
            {
                cstr = c.GetConstructor(carr);
            }
            catch (NoSuchMethodException ee)
            {
                Console.WriteLine("caught NoSuchMethodException " + c);
                return null;
            }
            catch (Exception ee)
            {
                ee.printStackTrace();
                return null;
            }

            // invoke constructor with starting coordinates
            object[] oarr = new object[2];
            oarr[0] = new int?(x0);
            oarr[1] = new int?(y0);
            try
            {
                return (CircuitElm) cstr.newInstance(oarr);
            }
            catch (Exception ee)
            {
                ee.printStackTrace();
            }
            return null;
        }

        protected virtual void doPopupMenu(MouseEvent e)
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
                mainMenu.show(e.Component, e.X, e.Y);
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

        public virtual void mouseReleased(MouseEvent e)
        {
            int ex = e.ModifiersEx;
            if ((ex & (MouseEvent.SHIFT_DOWN_MASK|MouseEvent.CTRL_DOWN_MASK| MouseEvent.META_DOWN_MASK)) == 0 && e.PopupTrigger)
            {
                doPopupMenu(e);
                return;
            }
            tempMouseMode = mouseMode;
            selectedArea = null;
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
            cv.repaint();
        }

        protected virtual void enableItems()
        {
            if (powerCheckItem.State)
            {
                powerBar.enable();
                powerLabel.enable();
            }
            else
            {
                powerBar.disable();
                powerLabel.disable();
            }
            enableUndoRedo();
        }

        public virtual void itemStateChanged(ItemEvent e)
        {
            cv.repaint(pause);
            object mi = e.ItemSelectable;
            if (mi == stoppedCheck)
                return;
            if (mi == smallGridCheckItem)
                setGrid();
            if (mi == powerCheckItem)
            {
                if (powerCheckItem.State)
                    voltsCheckItem.State = false;
                else
                    voltsCheckItem.State = true;
            }
            if (mi == voltsCheckItem && voltsCheckItem.State)
                powerCheckItem.State = false;
            enableItems();
            if (menuScope != -1)
            {
                Scope sc = scopes[menuScope];
                sc.handleMenu(e, mi);
            }
            if (mi is CheckboxMenuItem)
            {
                MenuItem mmi = (MenuItem) mi;
                mouseMode = MODE_ADD_ELM;
                string s = mmi.ActionCommand;
                if (s.Length > 0)
                    mouseModeStr = s;
                if (s.CompareTo("DragAll") == 0)
                    mouseMode = MODE_DRAG_ALL;
                else if (s.CompareTo("DragRow") == 0)
                    mouseMode = MODE_DRAG_ROW;
                else if (s.CompareTo("DragColumn") == 0)
                    mouseMode = MODE_DRAG_COLUMN;
                else if (s.CompareTo("DragSelected") == 0)
                    mouseMode = MODE_DRAG_SELECTED;
                else if (s.CompareTo("DragPost") == 0)
                    mouseMode = MODE_DRAG_POST;
                else if (s.CompareTo("Select") == 0)
                    mouseMode = MODE_SELECT;
                else if (s.Length > 0)
                {
                    try
                    {
                        addingClass = Type.GetType(s);
                    }
                    catch (Exception ee)
                    {
                        ee.printStackTrace();
                    }
                }
                tempMouseMode = mouseMode;
            }
        }

        protected virtual void setGrid()
        {
            gridSize = (smallGridCheckItem.State) ? 8 : 16;
            gridMask = ~(gridSize-1);
            gridRound = gridSize/2-1;
        }

        protected virtual void pushUndo()
        {
            redoStack.Clear();
            string s = dumpCircuit();
            if (undoStack.Count > 0 && System.String.CompareOrdinal(s, (string)(undoStack[undoStack.Count - 1])) == 0)
                return;
            undoStack.Add(s);
            enableUndoRedo();
        }

        protected virtual void enableUndoRedo()
        {
            redoItem.Enabled = redoStack.Count > 0;
            undoItem.Enabled = undoStack.Count > 0;
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
            enablePaste();
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
            enablePaste();
        }

        protected virtual void enablePaste()
        {
            pasteItem.Enabled = clipboard.Length > 0;
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
            readSetup(clipboard, true);

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

        public virtual void keyPressed(KeyEvent e)
        {
        }
        public virtual void keyReleased(KeyEvent e)
        {
        }

        public virtual void keyTyped(KeyEvent e)
        {
            if (e.KeyChar > ' ' && e.KeyChar < 127)
            {
                Type c = dumpTypes[e.KeyChar];
                if (c == null || c == typeof(Scope))
                    return;
                CircuitElm elm = null;
                elm = constructElement(c, 0, 0);
                if (elm == null || !(elm.needsShortcut() && elm.DumpClass == c))
                    return;
                mouseMode = MODE_ADD_ELM;
                mouseModeStr = c.Name;
                addingClass = c;
            }
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
        protected virtual bool lu_factor(double[][] a, int n, int[] ipvt)
        {
            var scaleFactors = new double[n];

            // divide each row by its largest element, keeping track of the
            // scaling factors
            for (int i = 0; i != n; i++)
            {
                double largest = 0;
                for (int j = 0; j != n; j++)
                {
                    double x = Math.Abs(a[i][j]);
                    if (x > largest)
                        largest = x;
                }
                // if all zeros, it's a singular matrix
                if (Math.Abs(largest - 0) < double.Epsilon)
                    return false;
                scaleFactors[i] = 1.0/largest;
            }

            // use Crout's method; loop through the columns
            for (int j = 0; j != n; j++)
            {
                // calculate upper triangular elements for this column
                int k;
                for (int i = 0; i != j; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != i; k++)
                        q -= a[i][k]*a[k][j];
                    a[i][j] = q;
                }

                // calculate lower triangular elements for this column
                double largest = 0;
                int largestRow = -1;
                for (int i = j; i != n; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != j; k++)
                        q -= a[i][k]*a[k][j];
                    a[i][j] = q;
                    double x = Math.Abs(q);
                    if (x >= largest)
                    {
                        largest = x;
                        largestRow = i;
                    }
                }

                // pivoting
                if (j != largestRow)
                {
                    for (k = 0; k != n; k++)
                    {
                        double x = a[largestRow][k];
                        a[largestRow][k] = a[j][k];
                        a[j][k] = x;
                    }
                    scaleFactors[largestRow] = scaleFactors[j];
                }

                // keep track of row interchanges
                ipvt[j] = largestRow;

                // avoid zeros
                if (Math.Abs(a[j][j] - 0.0) < double.Epsilon)
                {
                    Console.WriteLine("avoided zero");
                    a[j][j]=1e-18;
                }

                if (j != n-1)
                {
                    double mult = 1.0/a[j][j];
                    for (int i = j+1; i != n; i++)
                        a[i][j] *= mult;
                }
            }
            return true;
        }

        // Solves the set of n linear equations using a LU factorization
        // previously performed by lu_factor.  On input, b[0..n-1] is the right
        // hand side of the equations, and on output, contains the solution.
        protected virtual void lu_solve(double[][] a, int n, int[] ipvt, double[] b)
        {
            int bi = 0;
            // find first nonzero b element
            for (int i = 0; i != n; i++)
            {
                int row = ipvt[i];
                double swap = b[row];
                b[row] = b[i];
                b[i] = swap;
                if (Math.Abs(swap - 0) > double.Epsilon)
                {
                    bi = i + 1;
                    break;
                }
            }
            
            for (int i = bi; i < n; i++)
            {
                int row = ipvt[i];
                double tot = b[row];
                b[row] = b[i];
                // forward substitution using the lower triangular matrix
                for (int j = bi; j < i; j++)
                    tot -= a[i][j]*b[j];
                b[i] = tot;
            }
            for (int i = n-1; i >= 0; i--)
            {
                double tot = b[i];
                // back-substitution using the upper triangular matrix
                for (int j = i+1; j != n; j++)
                    tot -= a[i][j]*b[j];
                b[i] = tot/a[i][i];
            }
        }

    }


//----------------------------------------------------------------------------------------
//	Copyright � 2008 - 2010 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length.
//----------------------------------------------------------------------------------------
    internal static class RectangularArrays
    {
        internal static double[][] ReturnRectangularDoubleArray(int Size1, int Size2)
        {
            double[][] Array = new double[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new double[Size2];
            }
            return Array;
        }
    }
}