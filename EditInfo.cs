using System.Windows.Forms;
namespace circuit_emulator
{
    public class EditInfo
    {
        internal EditInfo(System.String n, double val, double mn, double mx)
        {
             bar = new HScrollBar();
            name = n;
            value_Renamed = val;
            if (mn == 0 && mx == 0 && val > 0)
            {
                minval = 1e10;
                while (minval > val / 100)
                    minval /= 10.0;
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
            dimensionless = true; return this;
        }
        internal System.String name, text;
        internal double value_Renamed, minval, maxval;
        internal System.Windows.Forms.TextBox textf;
        internal System.Windows.Forms.ScrollBar bar;
        internal System.Windows.Forms.ComboBox choice;
        internal System.Windows.Forms.CheckBox checkbox;
        internal bool newDialog;
        internal bool forceLargeM;
        internal bool dimensionless;
    }
}