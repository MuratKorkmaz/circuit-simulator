using System;

class Scope
{
	virtual internal bool LockScale
	{
		set
		{
			lockScale = value;
		}
		
	}
	virtual internal System.Drawing.Rectangle Rect
	{
		set
		{
			rect = value;
			resetGraph();
		}
		
	}
	virtual internal int Width
	{
		get
		{
			return rect.Width;
		}
		
	}
	virtual internal CircuitElm Elm
	{
		set
		{
			elm = value;
			reset();
		}
		
	}
	virtual internal System.Windows.Forms.ContextMenu Menu
	{
		get
		{
			if (elm == null)
				return null;
			if (elm is TransistorElm)
			{
				sim.scopeIbMenuItem.Checked = value_Renamed == VAL_IB;
				sim.scopeIcMenuItem.Checked = value_Renamed == VAL_IC;
				sim.scopeIeMenuItem.Checked = value_Renamed == VAL_IE;
				sim.scopeVbeMenuItem.Checked = value_Renamed == VAL_VBE;
				sim.scopeVbcMenuItem.Checked = value_Renamed == VAL_VBC;
				sim.scopeVceMenuItem.Checked = value_Renamed == VAL_VCE && ivalue != VAL_IC;
				sim.scopeVceIcMenuItem.Checked = value_Renamed == VAL_VCE && ivalue == VAL_IC;
				return sim.transScopeMenu;
			}
			else
			{
				sim.scopeVMenuItem.Checked = showV && value_Renamed == 0;
				sim.scopeIMenuItem.Checked = showI && value_Renamed == 0;
				sim.scopeMaxMenuItem.Checked = showMax_Renamed_Field;
				sim.scopeMinMenuItem.Checked = showMin_Renamed_Field;
				sim.scopeFreqMenuItem.Checked = showFreq_Renamed_Field;
				sim.scopePowerMenuItem.Checked = value_Renamed == VAL_POWER;
				sim.scopeVIMenuItem.Checked = plot2d && !plotXY;
				sim.scopeXYMenuItem.Checked = plotXY;
				sim.scopeSelectYMenuItem.Enabled = plotXY;
				sim.scopeResistMenuItem.Checked = value_Renamed == VAL_R;
				sim.scopeResistMenuItem.Enabled = elm is MemristorElm;
				return sim.scopeMenu;
			}
		}
		
	}
	virtual internal int Value
	{
		set
		{
			reset(); value_Renamed = value;
		}
		
	}
	//UPGRADE_NOTE: Final was removed from the declaration of 'FLAG_YELM '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
	internal int FLAG_YELM = 32;
	internal const int VAL_POWER = 1;
	internal const int VAL_IB = 1;
	internal const int VAL_IC = 2;
	internal const int VAL_IE = 3;
	internal const int VAL_VBE = 4;
	internal const int VAL_VBC = 5;
	internal const int VAL_VCE = 6;
	internal const int VAL_R = 2;
	internal double[] minV, maxV;
	internal double minMaxV;
	internal double[] minI, maxI;
	internal double minMaxI;
	internal int scopePointCount = 128;
	internal int ptr, ctr, speed, position;
	internal int value_Renamed, ivalue;
	internal System.String text;
	internal System.Drawing.Rectangle rect;
	internal bool showI, showV, showMax_Renamed_Field, showMin_Renamed_Field, showFreq_Renamed_Field, lockScale, plot2d, plotXY;
	internal CircuitElm elm, xElm, yElm;
	//UPGRADE_ISSUE: Class 'java.awt.image.MemoryImageSource' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtimageMemoryImageSource'"
	internal MemoryImageSource imageSource;
	internal System.Drawing.Image image;
	internal int[] pixels;
	internal int draw_ox, draw_oy;
	internal float[] dpixels;
	internal CirSim sim;
	internal Scope(CirSim s)
	{
		rect = new System.Drawing.Rectangle();
		reset();
		sim = s;
	}
	internal virtual void  showCurrent(bool b)
	{
		showI = b; value_Renamed = ivalue = 0;
	}
	internal virtual void  showVoltage(bool b)
	{
		showV = b; value_Renamed = ivalue = 0;
	}
	internal virtual void  showMax(bool b)
	{
		showMax_Renamed_Field = b;
	}
	internal virtual void  showMin(bool b)
	{
		showMin_Renamed_Field = b;
	}
	internal virtual void  showFreq(bool b)
	{
		showFreq_Renamed_Field = b;
	}
	internal virtual void  resetGraph()
	{
		scopePointCount = 1;
		while (scopePointCount <= rect.Width)
			scopePointCount *= 2;
		minV = new double[scopePointCount];
		maxV = new double[scopePointCount];
		minI = new double[scopePointCount];
		maxI = new double[scopePointCount];
		ptr = ctr = 0;
		allocImage();
	}
	internal virtual bool active()
	{
		return elm != null;
	}
	internal virtual void  reset()
	{
		resetGraph();
		minMaxV = 5;
		minMaxI = .1;
		speed = 64;
		showI = showV = showMax_Renamed_Field = true;
		showFreq_Renamed_Field = lockScale = showMin_Renamed_Field = false;
		plot2d = false;
		// no showI for Output
		if (elm != null && (elm is OutputElm || elm is LogicOutputElm || elm is ProbeElm))
			showI = false;
		value_Renamed = ivalue = 0;
		if (elm is TransistorElm)
			value_Renamed = VAL_VCE;
	}
	internal virtual int rightEdge()
	{
		return rect.X + rect.Width;
	}
	
	internal virtual void  timeStep()
	{
		if (elm == null)
			return ;
		double v = elm.getScopeValue(value_Renamed);
		if (v < minV[ptr])
			minV[ptr] = v;
		if (v > maxV[ptr])
			maxV[ptr] = v;
		double i = 0;
		if (value_Renamed == 0 || ivalue != 0)
		{
			i = (ivalue == 0)?elm.getCurrent():elm.getScopeValue(ivalue);
			if (i < minI[ptr])
				minI[ptr] = i;
			if (i > maxI[ptr])
				maxI[ptr] = i;
		}
		
		if (plot2d && dpixels != null)
		{
			bool newscale = false;
			while (v > minMaxV || v < - minMaxV)
			{
				minMaxV *= 2;
				newscale = true;
			}
			double yval = i;
			if (plotXY)
				yval = (yElm == null)?0:yElm.VoltageDiff;
			while (yval > minMaxI || yval < - minMaxI)
			{
				minMaxI *= 2;
				newscale = true;
			}
			if (newscale)
				clear2dView();
			double xa = v / minMaxV;
			double ya = yval / minMaxI;
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int x = (int) (rect.Width * (1 + xa) * .499);
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int y = (int) (rect.Height * (1 - ya) * .499);
			drawTo(x, y);
		}
		else
		{
			ctr++;
			if (ctr >= speed)
			{
				ptr = (ptr + 1) & (scopePointCount - 1);
				minV[ptr] = maxV[ptr] = v;
				minI[ptr] = maxI[ptr] = i;
				ctr = 0;
			}
		}
	}
	
	internal virtual void  drawTo(int x2, int y2)
	{
		if (draw_ox == - 1)
		{
			draw_ox = x2;
			draw_oy = y2;
		}
		// need to draw a line from x1,y1 to x2,y2
		if (draw_ox == x2 && draw_oy == y2)
		{
			dpixels[x2 + rect.Width * y2] = 1;
		}
		else if (CircuitElm.abs(y2 - draw_oy) > CircuitElm.abs(x2 - draw_ox))
		{
			// y difference is greater, so we step along y's
			// from min to max y and calculate x for each step
			double sgn = CircuitElm.sign(y2 - draw_oy);
			int x, y;
			for (y = draw_oy; y != y2 + sgn; y = (int) (y + sgn))
			{
				x = draw_ox + (x2 - draw_ox) * (y - draw_oy) / (y2 - draw_oy);
				dpixels[x + rect.Width * y] = 1;
			}
		}
		else
		{
			// x difference is greater, so we step along x's
			// from min to max x and calculate y for each step
			double sgn = CircuitElm.sign(x2 - draw_ox);
			int x, y;
			for (x = draw_ox; x != x2 + sgn; x = (int) (x + sgn))
			{
				y = draw_oy + (y2 - draw_oy) * (x - draw_ox) / (x2 - draw_ox);
				dpixels[x + rect.Width * y] = 1;
			}
		}
		draw_ox = x2;
		draw_oy = y2;
	}
	
	internal virtual void  clear2dView()
	{
		int i;
		for (i = 0; i != dpixels.Length; i++)
			dpixels[i] = 0;
		draw_ox = draw_oy = - 1;
	}
	
	internal virtual void  adjustScale(double x)
	{
		minMaxV *= x;
		minMaxI *= x;
	}
	
	internal virtual void  draw2d(System.Drawing.Graphics g)
	{
		int i;
		if (pixels == null || dpixels == null)
			return ;
		int col = (int)((sim.printableCheckItem.Checked)?0xFFFFFFFF:0);
		for (i = 0; i != pixels.Length; i++)
			pixels[i] = col;
		for (i = 0; i != rect.Width; i++)
			pixels[i + rect.Width * (rect.Height / 2)] = unchecked((int) 0xFF00FF00);
		int ycol = (int)((plotXY)?0xFF00FF00:0xFFFFFF00);
		for (i = 0; i != rect.Height; i++)
			pixels[rect.Width / 2 + rect.Width * i] = ycol;
		for (i = 0; i != pixels.Length; i++)
		{
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int q = (int) (255 * dpixels[i]);
			if (q > 0)
				pixels[i] = unchecked((int) 0xFF000000) | (0x10101 * q);
			dpixels[i] = (float) (dpixels[i] * .997);
		}
		//UPGRADE_WARNING: Method 'java.awt.Graphics.drawImage' was converted to 'System.Drawing.Graphics.drawImage' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
		g.DrawImage(image, rect.X, rect.Y);
		SupportClass.GraphicsManager.manager.SetColor(g, CircuitElm.whiteColor);
		g.FillEllipse(SupportClass.GraphicsManager.manager.GetPaint(g), rect.X + draw_ox - 2, rect.Y + draw_oy - 2, 5, 5);
		int yt = rect.Y + 10;
		int x = rect.X;
		if (text != null && rect.Y + rect.Height > yt + 5)
		{
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(text, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			yt += 15;
		}
	}
	
	internal virtual void  draw(System.Drawing.Graphics g)
	{
		if (elm == null)
			return ;
		if (plot2d)
		{
			draw2d(g);
			return ;
		}
		if (pixels == null)
			return ;
		int i;
		int col = (sim.printableCheckItem.Checked)?0xFFFFFFFF:0;
		for (i = 0; i != pixels.Length; i++)
			pixels[i] = col;
		int x = 0;
		int maxy = (rect.Height - 1) / 2;
		int y = maxy;
		
		bool gotI = false;
		bool gotV = false;
		int minRange = 4;
		double realMaxV = - 1e8;
		double realMaxI = - 1e8;
		double realMinV = 1e8;
		double realMinI = 1e8;
		int curColor = unchecked((int) 0xFFFFFF00);
		int voltColor = (value_Renamed > 0)?0xFFFFFFFF:0xFF00FF00;
		if (sim.scopeSelected == - 1 && elm == sim.mouseElm)
			curColor = voltColor = unchecked((int) 0xFF00FFFF);
		int ipa = ptr + scopePointCount - rect.Width;
		for (i = 0; i != rect.Width; i++)
		{
			int ip = (i + ipa) & (scopePointCount - 1);
			while (maxV[ip] > minMaxV)
				minMaxV *= 2;
			while (minV[ip] < - minMaxV)
				minMaxV *= 2;
			while (maxI[ip] > minMaxI)
				minMaxI *= 2;
			while (minI[ip] < - minMaxI)
				minMaxI *= 2;
		}
		
		double gridStep = 1e-8;
		double gridMax = (showI?minMaxI:minMaxV);
		while (gridStep * 100 < gridMax)
			gridStep *= 10;
		if (maxy * gridStep / gridMax < .3)
			gridStep = 0;
		
		int ll;
		bool sublines = (maxy * gridStep / gridMax > 3);
		for (ll = - 100; ll <= 100; ll++)
		{
			// don't show gridlines if plotting multiple values,
			// or if lines are too close together (except for center line)
			if (ll != 0 && ((showI && showV) || gridStep == 0))
				continue;
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int yl = maxy - (int) (maxy * ll * gridStep / gridMax);
			if (yl < 0 || yl >= rect.Height - 1)
				continue;
			col = ll == 0?0xFF909090:0xFF404040;
			if (ll % 10 != 0)
			{
				col = unchecked((int) 0xFF101010);
				if (!sublines)
					continue;
			}
			for (i = 0; i != rect.Width; i++)
				pixels[i + yl * rect.Width] = col;
		}
		
		gridStep = 1e-15;
		double ts = sim.timeStep * speed;
		while (gridStep < ts * 5)
			gridStep *= 10;
		double tstart = sim.t - sim.timeStep * speed * rect.Width;
		double tx = sim.t - (sim.t % gridStep);
		int first = 1;
		for (ll = 0; ; ll++)
		{
			double tl = tx - gridStep * ll;
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int gx = (int) ((tl - tstart) / ts);
			if (gx < 0)
				break;
			if (gx >= rect.Width)
				continue;
			if (tl < 0)
				continue;
			col = unchecked((int) 0xFF202020);
			first = 0;
			if (((tl + gridStep / 4) % (gridStep * 10)) < gridStep)
			{
				col = unchecked((int) 0xFF909090);
				if (((tl + gridStep / 4) % (gridStep * 100)) < gridStep)
					col = unchecked((int) 0xFF4040D0);
			}
			for (i = 0; i < pixels.Length; i += rect.Width)
				pixels[i + gx] = col;
		}
		
		// these two loops are pretty much the same, and should be
		// combined!
		if (value_Renamed == 0 && showI)
		{
			int ox = - 1, oy = - 1;
			int j;
			for (i = 0; i != rect.Width; i++)
			{
				int ip = (i + ipa) & (scopePointCount - 1);
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				int miniy = (int) ((maxy / minMaxI) * minI[ip]);
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				int maxiy = (int) ((maxy / minMaxI) * maxI[ip]);
				if (maxI[ip] > realMaxI)
					realMaxI = maxI[ip];
				if (minI[ip] < realMinI)
					realMinI = minI[ip];
				if (miniy <= maxy)
				{
					if (miniy < - minRange || maxiy > minRange)
						gotI = true;
					if (ox != - 1)
					{
						if (miniy == oy && maxiy == oy)
							continue;
						for (j = ox; j != x + i; j++)
							pixels[j + rect.Width * (y - oy)] = curColor;
						ox = oy = - 1;
					}
					if (miniy == maxiy)
					{
						ox = x + i;
						oy = miniy;
						continue;
					}
					for (j = miniy; j <= maxiy; j++)
						pixels[x + i + rect.Width * (y - j)] = curColor;
				}
			}
			if (ox != - 1)
				for (j = ox; j != x + i; j++)
					pixels[j + rect.Width * (y - oy)] = curColor;
		}
		if (value_Renamed != 0 || showV)
		{
			int ox = - 1, oy = - 1, j;
			for (i = 0; i != rect.Width; i++)
			{
				int ip = (i + ipa) & (scopePointCount - 1);
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				int minvy = (int) ((maxy / minMaxV) * minV[ip]);
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				int maxvy = (int) ((maxy / minMaxV) * maxV[ip]);
				if (maxV[ip] > realMaxV)
					realMaxV = maxV[ip];
				if (minV[ip] < realMinV)
					realMinV = minV[ip];
				if ((value_Renamed != 0 || showV) && minvy <= maxy)
				{
					if (minvy < - minRange || maxvy > minRange)
						gotV = true;
					if (ox != - 1)
					{
						if (minvy == oy && maxvy == oy)
							continue;
						for (j = ox; j != x + i; j++)
							pixels[j + rect.Width * (y - oy)] = voltColor;
						ox = oy = - 1;
					}
					if (minvy == maxvy)
					{
						ox = x + i;
						oy = minvy;
						continue;
					}
					for (j = minvy; j <= maxvy; j++)
						pixels[x + i + rect.Width * (y - j)] = voltColor;
				}
			}
			if (ox != - 1)
				for (j = ox; j != x + i; j++)
					pixels[j + rect.Width * (y - oy)] = voltColor;
		}
		double freq = 0;
		if (showFreq_Renamed_Field)
		{
			// try to get frequency
			// get average
			double avg = 0;
			for (i = 0; i != rect.Width; i++)
			{
				int ip = (i + ipa) & (scopePointCount - 1);
				avg += minV[ip] + maxV[ip];
			}
			avg /= i * 2;
			int state = 0;
			double thresh = avg * .05;
			int oi = 0;
			double avperiod = 0;
			int periodct = - 1;
			double avperiod2 = 0;
			// count period lengths
			for (i = 0; i != rect.Width; i++)
			{
				int ip = (i + ipa) & (scopePointCount - 1);
				double q = maxV[ip] - avg;
				int os = state;
				if (q < thresh)
					state = 1;
				else if (q > - thresh)
					state = 2;
				if (state == 2 && os == 1)
				{
					int pd = i - oi;
					oi = i;
					// short periods can't be counted properly
					if (pd < 12)
						continue;
					// skip first period, it might be too short
					if (periodct >= 0)
					{
						avperiod += pd;
						avperiod2 += pd * pd;
					}
					periodct++;
				}
			}
			avperiod /= periodct;
			avperiod2 /= periodct;
			double periodstd = System.Math.Sqrt(avperiod2 - avperiod * avperiod);
			freq = 1 / (avperiod * sim.timeStep * speed);
			// don't show freq if standard deviation is too great
			if (periodct < 1 || periodstd > 2)
				freq = 0;
			// System.out.println(freq + " " + periodstd + " " + periodct);
		}
		//UPGRADE_WARNING: Method 'java.awt.Graphics.drawImage' was converted to 'System.Drawing.Graphics.drawImage' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
		g.DrawImage(image, rect.X, rect.Y);
		SupportClass.GraphicsManager.manager.SetColor(g, CircuitElm.whiteColor);
		int yt = rect.Y + 10;
		x += rect.X;
		if (showMax_Renamed_Field)
		{
			if (value_Renamed != 0)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getUnitText(realMaxV, elm.getScopeUnits(value_Renamed)), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
			else if (showV)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getVoltageText(realMaxV), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
			else if (showI)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getCurrentText(realMaxI), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
			yt += 15;
		}
		if (showMin_Renamed_Field)
		{
			int ym = rect.Y + rect.Height - 5;
			if (value_Renamed != 0)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getUnitText(realMinV, elm.getScopeUnits(value_Renamed)), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, ym - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
			else if (showV)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getVoltageText(realMinV), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, ym - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
			else if (showI)
			{
				//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
				g.DrawString(CircuitElm.getCurrentText(realMinI), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, ym - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			}
		}
		if (text != null && rect.Y + rect.Height > yt + 5)
		{
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(text, SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
			yt += 15;
		}
		if (showFreq_Renamed_Field && freq != 0 && rect.Y + rect.Height > yt + 5)
		{
			//UPGRADE_TODO: Method 'java.awt.Graphics.drawString' was converted to 'System.Drawing.Graphics.DrawString' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtGraphicsdrawString_javalangString_int_int'"
			g.DrawString(CircuitElm.getUnitText(freq, "Гц"), SupportClass.GraphicsManager.manager.GetFont(g), SupportClass.GraphicsManager.manager.GetBrush(g), x, yt - SupportClass.GraphicsManager.manager.GetFont(g).GetHeight());
		}
		if (ptr > 5 && !lockScale)
		{
			if (!gotI && minMaxI > 1e-4)
				minMaxI /= 2;
			if (!gotV && minMaxV > 1e-4)
				minMaxV /= 2;
		}
	}
	
	internal virtual void  speedUp()
	{
		if (speed > 1)
		{
			speed /= 2;
			resetGraph();
		}
	}
	internal virtual void  slowDown()
	{
		speed *= 2;
		resetGraph();
	}
	internal virtual System.String dump()
	{
		if (elm == null)
			return null;
		int flags = (showI?1:0) | (showV?2:0) | (showMax_Renamed_Field?0:4) | (showFreq_Renamed_Field?8:0) | (lockScale?16:0) | (plot2d?64:0) | (plotXY?128:0) | (showMin_Renamed_Field?256:0);
		flags |= FLAG_YELM; // yelm present
		int eno = sim.locateElm(elm);
		if (eno < 0)
			return null;
		int yno = yElm == null?- 1:sim.locateElm(yElm);
		System.String x = "o " + eno + " " + speed + " " + value_Renamed + " " + flags + " " + minMaxV + " " + minMaxI + " " + position + " " + yno;
		if (text != null)
			x += (" " + text);
		return x;
	}
	internal virtual void  undump(SupportClass.Tokenizer st)
	{
		reset();
		int e = System.Int32.Parse(st.NextToken());
		if (e == - 1)
			return ;
		elm = sim.getElm(e);
		speed = System.Int32.Parse(st.NextToken());
		value_Renamed = System.Int32.Parse(st.NextToken());
		int flags = System.Int32.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		minMaxV = System.Double.Parse(st.NextToken());
		//UPGRADE_TODO: The differences in the format  of parameters for constructor 'java.lang.Double.Double'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
		minMaxI = System.Double.Parse(st.NextToken());
		if (minMaxV == 0)
			minMaxV = .5;
		if (minMaxI == 0)
			minMaxI = 1;
		text = null;
		yElm = null;
		try
		{
			position = System.Int32.Parse(st.NextToken());
			int ye = - 1;
			if ((flags & FLAG_YELM) != 0)
			{
				ye = System.Int32.Parse(st.NextToken());
				if (ye != - 1)
					yElm = sim.getElm(ye);
			}
			while (st.HasMoreTokens())
			{
				if (text == null)
					text = st.NextToken();
				else
					text += (" " + st.NextToken());
			}
		}
		catch (System.Exception ee)
		{
		}
		showI = (flags & 1) != 0;
		showV = (flags & 2) != 0;
		showMax_Renamed_Field = (flags & 4) == 0;
		showFreq_Renamed_Field = (flags & 8) != 0;
		lockScale = (flags & 16) != 0;
		plot2d = (flags & 64) != 0;
		plotXY = (flags & 128) != 0;
		showMin_Renamed_Field = (flags & 256) != 0;
	}
	internal virtual void  allocImage()
	{
		pixels = null;
		int w = rect.Width;
		int h = rect.Height;
		if (w == 0 || h == 0)
			return ;
		if (sim.useBufferedImage)
		{
			try
			{
				/* simulate the following code using reflection:
				dbimage = new BufferedImage(d.width, d.height,
				BufferedImage.TYPE_INT_RGB);
				DataBuffer db = (DataBuffer)(((BufferedImage)dbimage).
				getRaster().getDataBuffer());
				DataBufferInt dbi = (DataBufferInt) db;
				pixels = dbi.getData();
				*/
				//UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				System.Type biclass = System.Type.GetType("java.awt.image.BufferedImage");
				//UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				System.Type dbiclass = System.Type.GetType("java.awt.image.DataBufferInt");
				//UPGRADE_TODO: The differences in the format  of parameters for method 'java.lang.Class.forName'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				System.Type rasclass = System.Type.GetType("java.awt.image.Raster");
				System.Reflection.ConstructorInfo cstr = biclass.GetConstructor(new System.Type[]{typeof(int), typeof(int), typeof(int)});
				image = (System.Drawing.Image) cstr.Invoke(new System.Object[]{(System.Int32) w, (System.Int32) h, (System.Int32) System.Drawing.Imaging.PixelFormat.Format32bppRgb});
				System.Reflection.MethodInfo m = biclass.getMethod("getRaster");
				System.Object ras = m.invoke(image);
				System.Object db = rasclass.getMethod("getDataBuffer").invoke(ras);
				pixels = (int[]) dbiclass.getMethod("getData").invoke(db);
			}
			catch (System.Exception ee)
			{
				// ee.printStackTrace();
				System.Console.Out.WriteLine("BufferedImage failed");
			}
		}
		if (pixels == null)
		{
			pixels = new int[w * h];
			int i;
			for (i = 0; i != w * h; i++)
				pixels[i] = unchecked((int) 0xFF000000);
			//UPGRADE_ISSUE: Constructor 'java.awt.image.MemoryImageSource.MemoryImageSource' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtimageMemoryImageSource'"
			imageSource = new MemoryImageSource(w, h, pixels, 0, w);
			//UPGRADE_ISSUE: Method 'java.awt.image.MemoryImageSource.setAnimated' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtimageMemoryImageSource'"
			imageSource.setAnimated(true);
			//UPGRADE_ISSUE: Method 'java.awt.image.MemoryImageSource.setFullBufferUpdates' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtimageMemoryImageSource'"
			imageSource.setFullBufferUpdates(true);
			//UPGRADE_ISSUE: Method 'java.awt.Component.createImage' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaawtComponentcreateImage_javaawtimageImageProducer'"
			image = sim.cv.createImage(imageSource);
		}
		dpixels = new float[w * h];
		draw_ox = draw_oy = - 1;
	}
	
	internal virtual void  handleMenu(System.Object event_sender, System.EventArgs e, System.Object mi)
	{
		if (mi == sim.scopeVMenuItem)
			showVoltage(sim.scopeVMenuItem.Checked);
		if (mi == sim.scopeIMenuItem)
			showCurrent(sim.scopeIMenuItem.Checked);
		if (mi == sim.scopeMaxMenuItem)
			showMax(sim.scopeMaxMenuItem.Checked);
		if (mi == sim.scopeMinMenuItem)
			showMin(sim.scopeMinMenuItem.Checked);
		if (mi == sim.scopeFreqMenuItem)
			showFreq(sim.scopeFreqMenuItem.Checked);
		if (mi == sim.scopePowerMenuItem)
			Value = VAL_POWER;
		if (mi == sim.scopeIbMenuItem)
			Value = VAL_IB;
		if (mi == sim.scopeIcMenuItem)
			Value = VAL_IC;
		if (mi == sim.scopeIeMenuItem)
			Value = VAL_IE;
		if (mi == sim.scopeVbeMenuItem)
			Value = VAL_VBE;
		if (mi == sim.scopeVbcMenuItem)
			Value = VAL_VBC;
		if (mi == sim.scopeVceMenuItem)
			Value = VAL_VCE;
		if (mi == sim.scopeVceIcMenuItem)
		{
			plot2d = true;
			plotXY = false;
			value_Renamed = VAL_VCE;
			ivalue = VAL_IC;
			resetGraph();
		}
		
		if (mi == sim.scopeVIMenuItem)
		{
			plot2d = sim.scopeVIMenuItem.Checked;
			plotXY = false;
			resetGraph();
		}
		if (mi == sim.scopeXYMenuItem)
		{
			plotXY = plot2d = sim.scopeXYMenuItem.Checked;
			if (yElm == null)
				selectY();
			resetGraph();
		}
		if (mi == sim.scopeResistMenuItem)
			Value = VAL_R;
	}
	
	internal virtual void  select()
	{
		sim.mouseElm = elm;
		if (plotXY)
		{
			sim.plotXElm = elm;
			sim.plotYElm = yElm;
		}
	}
	
	internal virtual void  selectY()
	{
		int e = yElm == null?- 1:sim.locateElm(yElm);
		int firstE = e;
		while (true)
		{
			for (e++; e < sim.elmList.Count; e++)
			{
				CircuitElm ce = sim.getElm(e);
				if ((ce is OutputElm || ce is ProbeElm) && ce != elm)
				{
					yElm = ce;
					return ;
				}
			}
			if (firstE == - 1)
				return ;
			e = firstE = - 1;
		}
	}
}