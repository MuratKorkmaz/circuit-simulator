using System;

namespace JavaToSharp
{
    internal class ACVoltageElm : VoltageElm
    {
        public ACVoltageElm(int xx, int yy) : base(xx, yy, WF_AC)
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
