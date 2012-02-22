using System.Drawing;

namespace JavaToSharp
{
    internal class CircuitCanvas : Canvas
    {
        private readonly CirSim pg;
        internal CircuitCanvas(CirSim p)
        {
            pg = p;
        }
        public virtual Dimension PreferredSize
        {
            get
            {
                return new Dimension(300,400);
            }
        }
        public virtual void update(Graphics g)
        {
            pg.updateCircuit(g);
        }
        public virtual void paint(Graphics g)
        {
            pg.updateCircuit(g);
        }
    }
}
