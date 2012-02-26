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
            _ucSimulationParameters.Initialize(this);
            SerializeMenu(схемыToolStripMenuItem);
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

        private void stackAll()
        {
            int count = _simController.scopeCount;
            for (int i = 0; i < count; i++)
            {
                _simController.scopes[i].position = 0;
                _simController.scopes[i].showMax(false);
                _simController.scopes[i].showMin(false);
            }
        }

        private void unstackAll()
        {
            int count = _simController.scopeCount;
            for (int i = 0; i < count; i++)
            {
                _simController.scopes[i].position = i;
                _simController.scopes[i].showMax(true);
            }
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

        private void tsmiUnionCurves_Click(object sender, EventArgs e)
        {
            try
            {
                stackAll();
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
            }
        }

        private void tsmiBreakCurves_Click(object sender, EventArgs e)
        {
            try
            {
                unstackAll();
            }
            catch (Exception ex)
            {
               UserMessageView.Instance.ShowError(ex.StackTrace);
            }
        }

        private void tsmiScheme_Click(object sender, EventArgs e)
        {
            try
            {
                var menuItem = (ToolStripMenuItem) sender;
                string filePath = (string) menuItem.Tag;
                string title = menuItem.Text;
                SerializeScheme(filePath);
                _ucSimulationParameters.SetSchemeName(title);
            }
            catch (Exception ex)
            {
                UserMessageView.Instance.ShowError(ex.StackTrace);
            }
        }
    }
}
