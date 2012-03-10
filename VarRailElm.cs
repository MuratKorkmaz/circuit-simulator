using System;
using System.Drawing;
using System.Windows.Forms;

namespace circuit_emulator
{
    internal class VarRailElm : RailElm
    {
        internal Label label;
        internal ScrollBar slider;
        internal String sliderText;

        public VarRailElm(int xx, int yy) : base(xx, yy, WF_VAR)
        {
            sliderText = "Voltage";
            frequency = maxVoltage;
            createSlider();
        }

        public VarRailElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st)
            : base(xa, ya, xb, yb, f, st)
        {
            sliderText = st.NextToken();
            while (st.HasMoreTokens())
                sliderText += (' ' + st.NextToken());
            createSlider();
        }

        internal override int DumpType
        {
            get { return 172; }
        }

        internal override double Voltage
        {
            get
            {
                frequency = slider.Value*(maxVoltage - bias)/100.0 + bias;
                return frequency;
            }
        }

        internal override String dump()
        {
            return base.dump() + " " + sliderText;
        }

        internal virtual void createSlider()
        {
            waveform = WF_VAR;
            label = new Label();
            label.Text = sliderText;
            label.TextAlign = ContentAlignment.MiddleCenter;
            CirSim.main.flowLayoutPanel1.Controls.Add(label);
            var value_Renamed = (int) ((frequency - bias)*100/(maxVoltage - bias));
            slider = new HScrollBar();
            slider.Value = value_Renamed;
            CirSim.main.flowLayoutPanel1.Controls.Add(slider);
            CirSim.main.flowLayoutPanel1.Invalidate();
        }

        internal override void delete()
        {
            CirSim.main.flowLayoutPanel1.Controls.Remove(label);
            CirSim.main.flowLayoutPanel1.Controls.Remove(slider);
        }

        public override EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Мин. напряжение", bias, - 20, 20);
            if (n == 1)
                return new EditInfo("Макс. напряжение", maxVoltage, - 20, 20);
            if (n == 2)
            {
                var ei = new EditInfo("Slider Text", 0, - 1, - 1);
                ei.text = sliderText;
                return ei;
            }
            return null;
        }

        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                bias = ei.value_Renamed;
            if (n == 1)
                maxVoltage = ei.value_Renamed;
            if (n == 2)
            {
                sliderText = ei.textf.Text;
                label.Text = sliderText;
            }
        }
    }
}