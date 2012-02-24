using System.Drawing;

namespace JavaToSharp
{
    interface ISimulationView
    {
        Image Image { get; }
        void ResetSimulation();
    }
}
