using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class FormMain : Form, ISimulationView
    {
        private CirSim _simController;

        public FormMain()
        {
            InitializeComponent();
            _simController = new CirSim();
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
    }
}
