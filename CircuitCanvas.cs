using System;

namespace circuit_emulator
{
    [Serializable]
    class CircuitCanvas:System.Windows.Forms.Control
    {
        internal CirSim pg;
        internal CircuitCanvas(CirSim p)
        {
            pg = p;
        }
        //UPGRADE_NOTE: The equivalent of method 'java.awt.Component.getPreferredSize' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        public System.Drawing.Size getPreferredSize()
        {
            return new System.Drawing.Size(300, 400);
        }
        //UPGRADE_NOTE: The equivalent of method 'java.awt.Component.update' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
        public void  Update(System.Drawing.Graphics g)
        {
            pg.updateCircuit(g);
        }
        protected override void  OnPaint(System.Windows.Forms.PaintEventArgs g_EventArg)
        {
            System.Drawing.Graphics g = g_EventArg.Graphics;
            pg.updateCircuit(g);
        }
    }
}
