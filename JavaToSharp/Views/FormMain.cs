using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain : Form, ISimulationView
    {
        private readonly CirSim _simController;
        private readonly ucSimulationParameters _ucSimulationParameters;

        public FormMain()
        {
            InitializeComponent();
            _ucSimulationParameters = new ucSimulationParameters();
            _simController = new CirSim();
            _simController.init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Image Image
        {
            get { return pbCircuit.Image; }
        }

        public void ResetSimulation()
        {
            for (int i = 0; i < _simController.elmList.Count; i++)
                _simController.getElm(i).reset();
            for (int i = 0; i < _simController.scopeCount; i++)
                _simController.scopes[i].resetGraph();
            _simController.analyzeFlag = true;
            _simController.t = 0;
            _ucSimulationParameters.IsStopped = false;
            pbCircuit.Invalidate();
        }

        private void tsmiImport_Click(object sender, EventArgs e)
        {

        }

        private void tsmiExport_Click(object sender, EventArgs e)
        {

        }

        private void tsmiExportLink_Click(object sender, EventArgs e)
        {

        }
    }
}
