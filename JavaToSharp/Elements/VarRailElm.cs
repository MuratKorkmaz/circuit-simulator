using System.Windows.Forms;

namespace JavaToSharp
{
    internal class VarRailElm : RailElm
    {
        private Scrollbar slider;
        private Label label;
        private string sliderText;
        public VarRailElm(int xx, int yy) : base(xx, yy, WF_VAR)
        {
            sliderText = "Voltage";
            frequency = maxVoltage;
            createSlider();
        }
        public VarRailElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
            sliderText = st.nextToken();
            while (st.hasMoreTokens())
                sliderText += ' ' + st.nextToken();
            createSlider();
        }
        internal override string dump()
        {
            return base.dump() + " " + sliderText;
        }
        internal override int DumpType
        {
            get
            {
                return 172;
            }
        }

        protected virtual void createSlider()
        {
            waveform = WF_VAR;
            sim.main.add(label = new Label(sliderText, Label.CENTER));
            int value = (int)((frequency-bias)*100/(maxVoltage-bias));
            sim.main.add(slider = new Scrollbar(Scrollbar.HORIZONTAL, value, 1, 0, 101));
            sim.main.validate();
        }
        internal new virtual double Voltage
        {
            get
            {
                frequency = slider.Value * (maxVoltage-bias) / 100.+ bias;
                return frequency;
            }
        }
        internal override void delete()
        {
            sim.main.remove(label);
            sim.main.remove(slider);
        }
        public new virtual EditInfo getEditInfo(int n)
        {
            if (n == 0)
                return new EditInfo("Мин. напряжение", bias, -20, 20);
            if (n == 1)
                return new EditInfo("Макс. напряжение", maxVoltage, -20, 20);
            if (n == 2)
            {
                EditInfo ei = new EditInfo("Slider Text", 0, -1, -1);
                ei.text = sliderText;
                return ei;
            }
            return null;
        }
        public override void setEditValue(int n, EditInfo ei)
        {
            if (n == 0)
                bias = ei.value;
            if (n == 1)
                maxVoltage = ei.value;
            if (n == 2)
            {
                sliderText = ei.textf.Text;
                label.Text = sliderText;
            }
        }
    }
}
