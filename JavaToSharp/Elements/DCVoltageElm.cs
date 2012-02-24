using System;

namespace JavaToSharp
{
    internal class DCVoltageElm : VoltageElm
    {
        public DCVoltageElm(int xx, int yy) : base(xx, yy, WF_DC)
        {
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(VoltageElm);
            }
        }
    }
}
