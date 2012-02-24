using System;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain : Form, ISimulationView
    {
        private readonly CirSim _simController;

        public FormMain()
        {
            InitializeComponent();
            _simController = new CirSim();
            _simController.init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Image Image
        {
            get { throw new NotImplementedException(); }
        }

        public void ResetSimulation()
        {
            dbimage = main.createImage(winSize.width, winSize.height);

            for (i = 0; i < elmList.Count; i++)
                getElm(i).reset();
            for (i = 0; i < scopeCount; i++)
                scopes[i].resetGraph();
            analyzeFlag = true;
            t = 0;
            stoppedCheck.State = false;
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
