namespace JavaToSharp
{
    public interface IParametersView
    {
        int CurrentSpeed { get; }
        bool IsStopped { get; set; }
        int PowerLight { get; }
        int SimulationSpeed { get; }
    }
}
