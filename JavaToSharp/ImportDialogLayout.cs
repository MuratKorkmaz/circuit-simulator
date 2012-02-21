internal class ImportDialogLayout : LayoutManager
{
	public ImportDialogLayout()
	{
	}
	public virtual void addLayoutComponent(string name, Component c)
	{
	}
	public virtual void removeLayoutComponent(Component c)
	{
	}
	public virtual Dimension preferredLayoutSize(Container target)
	{
	return new Dimension(500, 500);
	}
	public virtual Dimension minimumLayoutSize(Container target)
	{
	return new Dimension(100,100);
	}
	public virtual void layoutContainer(Container target)
	{
	Insets insets = target.insets();
	int targetw = target.size().width - insets.left - insets.right;
	int targeth = target.size().height - (insets.top+insets.bottom);
	int i;
	int pw = 300;
	if (target.ComponentCount == 0)
		return;
	Component cl = target.getComponent(target.ComponentCount-1);
	Dimension dl = cl.PreferredSize;
	target.getComponent(0).move(insets.left, insets.top);
	int cw = target.size().width - insets.left - insets.right;
	int ch = target.size().height - insets.top - insets.bottom - dl.height;
	target.getComponent(0).resize(cw, ch);
	int h = ch + insets.top;
	int x = 0;
	for (i = 1; i < target.ComponentCount; i++)
	{
		Component m = target.getComponent(i);
		if (m.Visible)
		{
		Dimension d = m.PreferredSize;
		m.move(insets.left+x, h);
		m.resize(d.width, d.height);
		x += d.width;
		}
	}
	}
}

