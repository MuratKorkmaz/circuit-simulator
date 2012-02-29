using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain : Form, ISimulationView
    {
        public IParametersView Parameters
        {
            get { return _ucSimulationParameters; }
        }

        public Image Image
        {
            get { return pbCircuit.Image; }
        }

        private readonly CirSim _simController;
        private readonly BackgroundWorker _backgroundWorker;

        public FormMain()
        {
            InitializeComponent();
            InitializeGraphics();
            _simController = new CirSim(this);
            _simController.init();
            _ucSimulationParameters.Initialize(this);
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            InitializeCircuitElms();
            InitializeSchemes();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var image = (Image) e.Argument;
            _simController.updateCircuit(Graphics.FromImage(image));
        }

        private void InitializeGraphics()
        {
            var canvas = new Bitmap(pbCircuit.Width, pbCircuit.Height);
            pbCircuit.Image = canvas;
        }

        private void InitializeSchemes()
        {
            _startSircuitPath = string.Empty;
            SerializeMenu(схемыToolStripMenuItem);
            if (!string.IsNullOrEmpty(_startSircuitPath))
            {
                SerializeScheme(_startSircuitPath);
            }
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

        public void UpdateCanvas()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateCanvas));
            }
            else
            {
                pbCircuit.Refresh();
                pbCircuit.Update(); 
            }
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

        private void FormMain_Load(object sender, EventArgs e)
        {
            _backgroundWorker.RunWorkerAsync(pbCircuit.Image);
        }
    }
}
