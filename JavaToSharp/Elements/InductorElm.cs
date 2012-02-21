	internal class InductorElm : CircuitElm
	{
	internal Inductor ind;
	internal double inductance;
	public InductorElm(int xx, int yy) : base(xx, yy)
	{
		ind = new Inductor(sim);
		inductance = 1;
		ind.setup(inductance, current, flags);
	}
	public InductorElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
		ind = new Inductor(sim);
		inductance = new (double)double?(st.nextToken());
		current = new (double)double?(st.nextToken());
		ind.setup(inductance, current, flags);
	}
	internal override int DumpType
	{
		get
		{
			return 'l';
		}
	}
	internal override string dump()
	{
		return base.dump() + " " + inductance + " " + current;
	}
	internal override void setPoints()
	{
		base.setPoints();
		calcLeads(32);
	}
	internal override void draw(Graphics g)
	{
		double v1 = volts[0];
		double v2 = volts[1];
		int i;
		int hs = 8;
		setBbox(point1, point2, hs);
		draw2Leads(g);
		setPowerColor(g, false);
		drawCoil(g, 8, lead1, lead2, v1, v2);
		if (sim.showValuesCheckItem.State)
		{
		string s = getShortUnitText(inductance, "Гн");
		drawValues(g, s, hs);
		}
		doDots(g);
		drawPosts(g);
	}
	internal override void reset()
	{
		current = volts[0] = volts[1] = curcount = 0;
		ind.reset();
	}
	internal override void stamp()
	{
		ind.stamp(nodes[0], nodes[1]);
	}
	internal override void startIteration()
	{
		ind.startIteration(volts[0]-volts[1]);
	}
	internal override bool nonLinear()
	{
		return ind.nonLinear();
	}

	    protected override void calculateCurrent()
	{
		double voltdiff = volts[0]-volts[1];
		current = ind.calculateCurrent(voltdiff);
	}
	internal override void doStep()
	{
		double voltdiff = volts[0]-volts[1];
		ind.doStep(voltdiff);
	}
	internal override void getInfo(string[] arr)
	{
		arr[0] = "индуктивность";
		getBasicInfo(arr);
		arr[3] = "L = " + getUnitText(inductance, "Гн");
		arr[4] = "P = " + getUnitText(Power, "Вт");
	}
	public override EditInfo getEditInfo(int n)
	{
		if (n == 0)
		return new EditInfo("Индутивность (Гн)", inductance, 0, 0);
		if (n == 1)
		{
		EditInfo ei = new EditInfo("", 0, -1, -1);
		ei.checkbox = new Checkbox("Трапецив. апроксимация", ind.Trapezoidal);
		return ei;
		}
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		inductance = ei.value;
		if (n == 1)
		{
		if (ei.checkbox.State)
			flags &= ~Inductor.FLAG_BACK_EULER;
		else
			flags |= Inductor.FLAG_BACK_EULER;
		}
		ind.setup(inductance, current, flags);
	}
	}
