using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JavaToSharp
{
    public partial class ucSimulationParameters : UserControl
    {
        private Control _canvas;

        public ucSimulationParameters()
        {
            InitializeComponent();
            _canvas = null;
        }

        public void SetCanvas(Control canvas)
        {
            _canvas = canvas;
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            
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
