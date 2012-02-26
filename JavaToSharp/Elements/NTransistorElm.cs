using System;

namespace JavaToSharp
{
    internal class NTransistorElm : TransistorElm
    {
        public NTransistorElm(int xx, int yy) : base(xx, yy, false)
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
