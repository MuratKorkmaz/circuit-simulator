using System.Drawing;
using System.Windows.Forms;

namespace JavaToSharp
{
    internal class CircuitCanvas : Control
    {
        private readonly CirSim pg;
        internal CircuitCanvas(CirSim p)
        {
            pg = p;
        }
        public virtual Size PreferredSize
        {
            get
            {
                return new Size(300,400);
            }
        }
        public virtual void Update(Graphics g)
        {
            //pg.updateCircuit(g);
        }
        public virtual void Paint(Graphics g)
        {
            //pg.updateCircuit(g);
        }
    }
}
