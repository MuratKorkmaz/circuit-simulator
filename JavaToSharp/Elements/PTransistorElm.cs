using System;

namespace JavaToSharp
{
    internal class PTransistorElm : TransistorElm
    {
        public PTransistorElm(int xx, int yy) : base(xx, yy, true)
        {
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(TransistorElm);
            }
        }
    }
}
