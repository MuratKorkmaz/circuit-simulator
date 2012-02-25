	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using JavaToSharp;

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
	    string sInductance = st.nextToken();
	    bool isParsedInductance = double.TryParse(sInductance, out inductance);
	    if (!isParsedInductance)
	    {
	        throw new Exception("Не удалось привести к типу double");
	    }
	    string sCurrent = st.nextToken();
	    bool isParsedCurrent = double.TryParse(sCurrent, out current);
	    if (!isParsedCurrent)
	    {
	        throw new Exception("Не удалось привести к типу double");
	    }
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
		drawCoil(g, 8, lead1, lead2, v1, v2);
		
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

    internal override void calculateCurrent()
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
		ei.checkbox = new CheckBox();
		    ei.checkbox.Text = "Трапецив. апроксимация";
		    if (ind.isTrapezoidal)
		    {
		        return ei;
		    }
		
		}
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0)
		inductance = ei.value;
		if (n == 1)
		{
		if (ei.checkbox.Checked)
			flags &= ~Inductor.FLAG_BACK_EULER;
		else
			flags |= Inductor.FLAG_BACK_EULER;
		}
		ind.setup(inductance, current, flags);
	}
	}
