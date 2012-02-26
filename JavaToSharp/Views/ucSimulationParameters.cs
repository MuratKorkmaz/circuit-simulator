using System;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class ucSimulationParameters : UserControl
    {
        public int SimulationSpeed
        {
            get { return sbSimulationSpeed.Value; }
            set
            {
                sbSimulationSpeed.Scroll -= sbSimulationSpeed_Scroll;
                sbSimulationSpeed.Value = value;
                sbSimulationSpeed.Scroll += sbSimulationSpeed_Scroll;
            }
        }

        public int CurrentSpeed
        {
            get { return sbCurrentSpeed.Value; }
            set
            {
                sbCurrentSpeed.Scroll -= sbCurrentSpeed_Scroll;
                sbCurrentSpeed.Value = value;
                sbCurrentSpeed.Scroll += sbCurrentSpeed_Scroll;
            }
        }

        public int PowerLight
        {
            get { return sbPowerLight.Value; }
            set
            {
                sbPowerLight.Scroll -= sbPowerLight_Scroll;
                sbPowerLight.Value = value;
                sbPowerLight.Scroll += sbPowerLight_Scroll;
            }
        }

        public bool IsStopped
        {
            get { return chbStop.Checked; }
            set 
            { 
                chbStop.CheckedChanged -= chbStop_CheckedChanged;
                chbStop.Checked = value;
                chbStop.CheckedChanged += chbStop_CheckedChanged;
            }
        }

        private ISimulationView _simulationView;

        public ucSimulationParameters()
        {
            InitializeComponent();
        }

        internal void Initialize(ISimulationView view)
        {
            _simulationView = view;
        }

        public void SetSchemeName(string title)
        {
            lbCurrentScheme.Text = string.Format("Текущая схема:\r\n\r\n{0}", title);
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            try
            {
                _simulationView.ResetSimulation();
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
            }
        }

        private void chbStop_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void sbSimulationSpeed_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void sbCurrentSpeed_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void sbPowerLight_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
