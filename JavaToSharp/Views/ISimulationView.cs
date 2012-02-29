using System.Drawing;

namespace JavaToSharp
{
    public interface ISimulationView
    {
        Image Image { get; }
        IParametersView Parameters { get; }
        void ResetSimulation();
        void UpdateCanvas();
    }
}
