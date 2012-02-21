using System.Windows.Forms;

namespace JavaToSharp
{
    public class EditInfo
    {
        internal EditInfo(string n, double val, double mn, double mx)
        {
            name = n;
            value = val;
            if (mn == 0 && mx == 0 && val > 0)
            {
                minval = 1e10;
                while (minval > val/100)
                    minval /= 10.;
                maxval = minval * 1000;
            }
            else
            {
                minval = mn;
                maxval = mx;
            }
            forceLargeM = name.IndexOf("(Ом)") > 0 || name.IndexOf("(Гц)") > 0;
            dimensionless = false;
        }
        internal virtual EditInfo setDimensionless()
        {
            dimensionless = true;
            return this;
        }
        internal string name, text;
        internal double value, minval, maxval;
        internal TextField textf;
        internal ScrollBar bar;
        internal Choice choice;
        internal CheckBox checkbox;
        internal bool newDialog;
        internal bool forceLargeM;
        internal bool dimensionless;
    }
}

