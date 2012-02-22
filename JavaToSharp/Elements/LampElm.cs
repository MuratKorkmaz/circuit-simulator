using System;
using System.Drawing;
using JavaToSharp;


internal class LampElm : CircuitElm
	{
	internal double resistance;
	internal readonly double roomTemp = 300;
	internal double temp, nom_pow, nom_v, warmTime, coolTime;
	public LampElm(int xx, int yy) : base(xx, yy)
	{
		temp = roomTemp;
		nom_pow = 100;
		nom_v = 120;
		warmTime =.4;
		coolTime =.4;
	}
	public LampElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f)
	{
	    string sTemp = st.nextToken();
	    bool isParsedTemp = double.TryParse(sTemp, out temp);
	    if (!isParsedTemp)
	    {
	        throw new Exception("Не удалось привести к типу double");
	    }
	    string sNom_pow = st.nextToken();
	    bool isParsedNom_pow = double.TryParse(sNom_pow, out nom_pow);
	    if (!isParsedNom_pow)
	    {
	         throw new Exception("Не удалось привести к типу double");
	    }
	    string sNom_v = st.nextToken();
	    bool isParsedNom_v = double.TryParse(sNom_v, out nom_v);
	    if (!isParsedNom_v)
	    {
	        throw new Exception("Не удалось привести к типу double");
	    }
	    string sWarmTime = st.nextToken();
	    bool isParsedWarmTime = double.TryParse(sWarmTime, out warmTime);
	    if (!isParsedWarmTime)
	    {
	         throw new Exception("Не удалось привести к типу double");
	    }
	    string sCoolTime = st.nextToken();
	    bool isParsedCoolTime = double.TryParse(sCoolTime, out coolTime);
	    if (!isParsedCoolTime)
	    {
	         throw new Exception("Не удалось привести к типу double");
	    }
	}
	internal override string dump()
	{
		return base.dump() + " " + temp + " " + nom_pow + " " + nom_v + " " + warmTime + " " + coolTime;
	}
	internal override int DumpType
	{
		get
		{
			return 181;
		}
	}

	internal Point[] bulbLead; internal Point[] filament; internal Point bulb;
	internal int bulbR;

	internal override void reset()
	{
		base.reset();
		temp = roomTemp;
	}
	internal readonly int filament_len = 24;
	internal override void setPoints()
	{
		base.setPoints();
		int llen = 16;
		calcLeads(llen);
		bulbLead = newPointArray(2);
		filament = newPointArray(2);
		bulbR = 20;
		filament[0] = interpPoint(lead1, lead2, 0, filament_len);
		filament[1] = interpPoint(lead1, lead2, 1, filament_len);
		double br = filament_len-Math.Sqrt(bulbR*bulbR-llen*llen);
		bulbLead[0] = interpPoint(lead1, lead2, 0, br);
		bulbLead[1] = interpPoint(lead1, lead2, 1, br);
		bulb = interpPoint(filament[0], filament[1],.5);
	}

	internal virtual Color TempColor
	{
		get
		{
			if (temp < 1200)
			{
			int x = (int)(255*(temp-800)/400);
			if (x < 0)
				x = 0;
			return Color.FromArgb(x, 0, 0);  
			}
			if (temp < 1700)
			{
			int x = (int)(255*(temp-1200)/500);
			if (x < 0)
				x = 0;
			return  Color.FromArgb(255, x, 0);
			}
			if (temp < 2400)
			{
			int x = (int)(255*(temp-1700)/700);
			if (x < 0)
				x = 0;
			return  Color.FromArgb(255, 255, x);
			}
			return Color.White;
		}
	}

	internal override void draw(Graphics g)
	{
		double v1 = volts[0];
		double v2 = volts[1];
		setBbox(point1, point2, 4);
		adjustBbox(bulb.X-bulbR, bulb.Y-bulbR, bulb.X+bulbR, bulb.Y+bulbR);
		// adjustbbox
		draw2Leads(g);
		setPowerColor(g, true);
		g.Color = TempColor;
		g.fillOval(bulb.X-bulbR, bulb.Y-bulbR, bulbR*2, bulbR*2);
		g.Color = Color.White;
		drawThickCircle(g, bulb.X, bulb.Y, bulbR);
		setVoltageColor(g, v1);
		drawThickLine(g, lead1, filament[0]);
		setVoltageColor(g, v2);
		drawThickLine(g, lead2, filament[1]);
		setVoltageColor(g, (v1+v2)*.5);
		drawThickLine(g, filament[0], filament[1]);
		updateDotCount();
		if (sim.dragElm != this)
		{
		drawDots(g, point1, lead1, curcount);
		double cc = curcount+(dn-16)/2;
		drawDots(g, lead1, filament[0], cc);
		cc += filament_len;
		drawDots(g, filament[0], filament[1], cc);
		cc += 16;
		drawDots(g, filament[1], lead2, cc);
		cc += filament_len;
		drawDots(g, lead2, point2, curcount);
		}
		drawPosts(g);
	}

	    protected override void calculateCurrent()
	{
		current = (volts[0]-volts[1])/resistance;
		//System.out.print(this + " res current set to " + current + "\n");
	}
	internal override void stamp()
	{
		sim.stampNonLinear(nodes[0]);
		sim.stampNonLinear(nodes[1]);
	}
	internal override bool nonLinear()
	{
		return true;
	}
	internal override void startIteration()
	{
		// based on http://www.intusoft.com/nlpdf/nl11.pdf
		double nom_r = nom_v*nom_v/nom_pow;
		// this formula doesn't work for values over 5390
		double tp = (temp > 5390) ? 5390 : temp;
		resistance = nom_r*(1.26104 - 4.90662*Math.Sqrt(17.1839/tp - 0.00318794) - 7.8569/(tp - 187.56));
		double cap = 1.57e-4*nom_pow;
		double capw = cap * warmTime/.4;
		double capc = cap * coolTime/.4;
		//System.out.println(nom_r + " " + (resistance/nom_r));
		temp += Power*sim.timeStep/capw;
		double cr = 2600/nom_pow;
		temp -= sim.timeStep*(temp-roomTemp)/(capc*cr);
		//System.out.println(capw + " " + capc + " " + temp + " " +resistance);
	}
	internal override void doStep()
	{
		sim.stampResistor(nodes[0], nodes[1], resistance);
	}
	internal override void getInfo(string[] arr)
	{
		arr[0] = "лампа";
		getBasicInfo(arr);
		arr[3] = "R = " + getUnitText(resistance, sim.ohmString);
		arr[4] = "P = " + getUnitText(Power, "Вт");
		arr[5] = "T = " + ((int) temp) + " K";
	}
	public override EditInfo getEditInfo(int n)
	{
		// ohmString doesn't work here on linux
		if (n == 0)
		return new EditInfo("Номинальная мощность", nom_pow, 0, 0);
		if (n == 1)
		return new EditInfo("Номинальное напряжение", nom_v, 0, 0);
		if (n == 2)
		return new EditInfo("Время разогрева (с)", warmTime, 0, 0);
		if (n == 3)
		return new EditInfo("Время остывания (с)", coolTime, 0, 0);
		return null;
	}
	public override void setEditValue(int n, EditInfo ei)
	{
		if (n == 0 && ei.value > 0)
		nom_pow = ei.value;
		if (n == 1 && ei.value > 0)
		nom_v = ei.value;
		if (n == 2 && ei.value > 0)
		warmTime = ei.value;
		if (n == 3 && ei.value > 0)
		coolTime = ei.value;
	}
	}
