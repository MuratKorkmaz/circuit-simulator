using JavaToSharp;

internal class Inductor
{
	public const int FLAG_BACK_EULER = 2;
	internal int[] nodes;
	internal int flags;
	internal CirSim sim;

	internal double inductance;
	internal double compResistance, current;
	internal double curSourceValue;
	internal Inductor(CirSim s)
	{
	sim = s;
	nodes = new int[2];
	}
	internal virtual void setup(double ic, double cr, int f)
	{
	inductance = ic;
	current = cr;
	flags = f;
	}
	internal virtual bool isTrapezoidal
	{
		get
		{
			return (flags & FLAG_BACK_EULER) == 0;
		}
	}
	internal virtual void reset()
	{
	current = 0;
	}
	internal virtual void stamp(int n0, int n1)
	{
	// inductor companion model using trapezoidal or backward euler
	// approximations (Norton equivalent) consists of a current
	// source in parallel with a resistor.  Trapezoidal is more
	// accurate than backward euler but can cause oscillatory behavior.
	// The oscillation is a real problem in circuits with switches.
	nodes[0] = n0;
	nodes[1] = n1;
	if (isTrapezoidal)
		compResistance = 2*inductance/sim.timeStep;
	else // backward euler
		compResistance = inductance/sim.timeStep;
	sim.stampResistor(nodes[0], nodes[1], compResistance);
	sim.stampRightSide(nodes[0]);
	sim.stampRightSide(nodes[1]);
	}
	internal virtual bool nonLinear()
	{
		return false;
	}

	internal virtual void startIteration(double voltdiff)
	{
	if (isTrapezoidal)
		curSourceValue = voltdiff/compResistance+current;
	else // backward euler
		curSourceValue = current;
	}

	internal virtual double calculateCurrent(double voltdiff)
	{
	// we check compResistance because this might get called
	// before stamp(), which sets compResistance, causing
	// infinite current
	if (compResistance > 0)
		current = voltdiff/compResistance + curSourceValue;
	return current;
	}
	internal virtual void doStep(double voltdiff)
	{
	sim.stampCurrentSource(nodes[0], nodes[1], curSourceValue);
	}
}
