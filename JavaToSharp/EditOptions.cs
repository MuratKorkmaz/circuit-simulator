internal class EditOptions : Editable
{
	internal CirSim sim;
	public EditOptions(CirSim s)
	{
		sim = s;
	}
	public virtual EditInfo getEditInfo(int n)
	{
	if (n == 0)
		return new EditInfo("Длительность шага (с)", sim.timeStep, 0, 0);
	if (n == 1)
		return new EditInfo("Диапазон раскрашивания напряжения (В)", CircuitElm.voltageRange, 0, 0);

	return null;
	}
	public virtual void setEditValue(int n, EditInfo ei)
	{
	if (n == 0 && ei.value > 0)
		sim.timeStep = ei.value;
	if (n == 1 && ei.value > 0)
		CircuitElm.voltageRange = ei.value;
	}
}
