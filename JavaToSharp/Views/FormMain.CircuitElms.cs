using System;
using System.Reflection;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain
    {
        private ContextMenuStrip _tsmiAddCircuitElms;

        private void InitializeCircuitElms()
        {
            _tsmiAddCircuitElms = new ContextMenuStrip();
            _tsmiAddCircuitElms.Items.Add(getClassCheckItem("Добавить Соединение", "WireElm"));
            _tsmiAddCircuitElms.Items.Add(getClassCheckItem("Добавить Резистор", "ResistorElm"));
            _tsmiAddCircuitElms.Items.Add(CreatePassMenu());
            _tsmiAddCircuitElms.Items.Add(CreateInputMenu());
            _tsmiAddCircuitElms.Items.Add(CreateActiveMenu());
            _tsmiAddCircuitElms.Items.Add(CreateGateMenu());
            _tsmiAddCircuitElms.Items.Add(CreateChipMenu());
            _tsmiAddCircuitElms.Items.Add(CreateOtherMenu());
            _tsmiAddCircuitElms.Items.Add(getCheckItem("Выбор/перетаскивание выбранного (пробел или Shift+щелчок)", "Select"));
        }

        private ToolStripMenuItem CreatePassMenu()
        {
            var passMenu = new ToolStripMenuItem("Пассивные компоненты");
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Конденсатор", "CapacitorElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Индуктивность", "InductorElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Выключатель", "SwitchElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Кнопочный выключатель", "PushSwitchElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Переключатель", "Switch2Elm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Переменное сопротивление", "PotElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Трансформатор", "TransformerElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Трансформатор с отводом", "TappedTransformerElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Линию передачи", "TransLineElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Реле", "RelayElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Мемристор", "MemristorElm"));
            passMenu.DropDownItems.Add(getClassCheckItem("Добавить Искровой промежуток", "SparkGapElm"));
            return passMenu;
        }

        private ToolStripMenuItem CreateInputMenu()
        {
            var inputMenu = new ToolStripMenuItem("Входы/Выходы");
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Заземление", "GroundElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Ист. постоянного тока (2-вывода)", "DCVoltageElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Ист. переменного тока (2-вывода)", "ACVoltageElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Ист. напряжения (1-вывод)", "RailElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Ист. переменного тока (1-вывод)", "ACRailElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Ист. Прямоуг. напряжения (1-вывод)", "SquareRailElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Аналоговый выход", "OutputElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Логический вход", "LogicInputElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Логический выход", "LogicOutputElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Тактовые импульсы", "ClockElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Свип", "SweepElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Регулируемое напряжение", "VarRailElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Антенну", "AntennaElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Источник тока", "CurrentElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Светодиод", "LEDElm"));
            inputMenu.DropDownItems.Add(getClassCheckItem("Добавить Лампу (beta)", "LampElm"));
            return inputMenu;
        }

        private ToolStripMenuItem CreateActiveMenu()
        {
            var activeMenu = new ToolStripMenuItem("Активные компоненты");
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Диод", "DiodeElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Стабилитрон", "ZenerElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Транзистор (биполярный, NPN)", "NTransistorElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Транзистор (биполярный, PNP)", "PTransistorElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Операционный усилитель (- вверху)", "OpAmpElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Операционный усилитель (+ вверху)", "OpAmpSwapElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить MOSFET (n-канальный)", "NMosfetElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить MOSFET (p-канальный)", "PMosfetElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Полевой транзистор (n-канальный)", "NJfetElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Полевой транзистор (p-канальный)", "PJfetElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Аналоговый выключатель", "AnalogSwitchElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Аналоговый переключатель", "AnalogSwitch2Elm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Тиристор", "SCRElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Туннельный диод", "TunnelDiodeElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить Триод", "TriodeElm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить CCII+", "CC2Elm"));
            activeMenu.DropDownItems.Add(getClassCheckItem("Добавить CCII-", "CC2NegElm"));
            return activeMenu;
        }

        private ToolStripMenuItem CreateGateMenu()
        {
            var gateMenu = new ToolStripMenuItem("Логические элементы");
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить Инвертор", "InverterElm"));
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить элемент И-НЕ", "NandGateElm"));
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить элемент ИЛИ-НЕ", "NorGateElm"));
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить элемент И", "AndGateElm"));
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить элемент ИЛИ", "OrGateElm"));
            gateMenu.DropDownItems.Add(getClassCheckItem("Добавить элемент исключающее ИЛИ", "XorGateElm"));
            return gateMenu;
        }

        private ToolStripMenuItem CreateChipMenu()
        {
            var chipMenu = new ToolStripMenuItem("Микросхемы");
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить D триггер", "DFlipFlopElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить JK триггер", "JKFlipFlopElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить 7ми сегментный светодиод", "SevenSegElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить VCO", "VCOElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить Фазовый компаратор", "PhaseCompElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить Счетчик", "CounterElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить Декадный счетчик", "DecadeElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить 555 Таймер", "TimerElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить ЦАП", "DACElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить АЦП", "ADCElm"));
            chipMenu.DropDownItems.Add(getClassCheckItem("Добавить Защелку", "LatchElm"));
            return chipMenu;
        }

        private ToolStripMenuItem CreateOtherMenu()
        {
            var otherMenu = new ToolStripMenuItem("Прочее");
            otherMenu.DropDownItems.Add(getClassCheckItem("Добавить Текст", "TextElm"));
            otherMenu.DropDownItems.Add(getClassCheckItem("Добавить пробу осциллографа", "ProbeElm"));
            otherMenu.DropDownItems.Add(getCheckItem("Drag All", "DragAll"));
            otherMenu.DropDownItems.Add(getCheckItem("Drag Row", "DragRow"));
            otherMenu.DropDownItems.Add(getCheckItem("Drag Column", "DragColumn"));
            otherMenu.DropDownItems.Add(getCheckItem("Drag Selected", "DragSelected"));
            otherMenu.DropDownItems.Add(getCheckItem("Drag Post", "DragPost"));
            return otherMenu;
        }

        private ToolStripMenuItem getClassCheckItem(string s, string t)
        {
            try
            {
                Type c = Type.GetType("JavaToSharp." + t);
                CircuitElm elm = constructElement(c, 0, 0);
                register(elm);
                if (elm.needsShortcut() && elm.DumpClass == c)
                {
                    int dt = elm.DumpType;
                    s += " (" + (char) dt + ")";
                }
                elm.delete();
            }
            catch (Exception ee)
            {
                UserMessageView.Instance.ShowError(ee.StackTrace);
            }
            return getCheckItem(s, t);
        }

        private CircuitElm constructElement(Type c, int x0, int y0)
        {
            // find element class
            var carr = new Type[2];
            //carr[0] = getClass();
            carr[0] = carr[1] = typeof(int);
            ConstructorInfo cstr;
            try
            {
                cstr = c.GetConstructor(carr);
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
                return null;
            }
            if (cstr == null)
            {
                return null;
            }
            // invoke constructor with starting coordinates
            var oarr = new object[2];
            oarr[0] = x0;
            oarr[1] = y0;
            try
            {
                return (CircuitElm)cstr.Invoke(oarr);
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
            }
            return null;
        }

        private void register(CircuitElm elm)
        {
            int t = elm.DumpType;
            if (t == 0)
            {
                return;
            }
            Type dclass = elm.DumpClass;
            if (_simController.dumpTypes[t] == dclass)
                return;
            if (_simController.dumpTypes[t] != null)
                return;
            _simController.dumpTypes[t] = dclass;
        }

        private ToolStripMenuItem getCheckItem(string s, string t)
        {
            var mi = new ToolStripMenuItem(s) {Tag = t};
            mi.CheckedChanged += tsmiCircuitElm_CheckedChanged;
            return mi;
        }

        private void tsmiCircuitElm_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void FormMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > ' ' && e.KeyChar < 127)
            {
                Type c = _simController.dumpTypes[e.KeyChar];
                if (c == null || c == typeof(Scope))
                    return;
                CircuitElm elm = constructElement(c, 0, 0);
                if (elm == null || !(elm.needsShortcut() && elm.DumpClass == c))
                    return;
                _simController.mouseMode = _simController.MODE_ADD_ELM;
                _simController.mouseModeStr = c.Name;
                _simController.addingClass = c;
            }
            if (e.KeyChar == ' ')
            {
                _simController.mouseMode = _simController.MODE_SELECT;
                _simController.mouseModeStr = "Select";
            }
            _simController.tempMouseMode = _simController.mouseMode;
        }
    }
}