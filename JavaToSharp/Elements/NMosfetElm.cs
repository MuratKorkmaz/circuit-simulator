using System;

namespace JavaToSharp
{
    internal class NMosfetElm : MosfetElm
    {
        public NMosfetElm(int xx, int yy) : base(xx, yy, false)
        {
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(MosfetElm);
            }
        }
    }
}
