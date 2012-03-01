//UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
namespace circuit_emulator
{
    public class EditInfo
    {
        internal EditInfo(System.String n, double val, double mn, double mx)
        {
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
        //UPGRADE_TODO: The equivalent of class 'java.awt.Scrollbar' may be 'System.Windows.Forms.HScrollBar or System.Windows.Forms.VScrollBar' depending on constructor parameters. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1146'"
        internal System.Windows.Forms.ScrollBar bar;
        //UPGRADE_TODO: Class 'java.awt.Choice' was converted to 'System.Windows.Forms.ComboBox' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaawtChoice'"
        internal System.Windows.Forms.ComboBox choice;
        internal System.Windows.Forms.CheckBox checkbox;
        internal bool newDialog;
        internal bool forceLargeM;
        internal bool dimensionless;
    }
}