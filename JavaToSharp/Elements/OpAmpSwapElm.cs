using System;

namespace JavaToSharp
{
    internal class OpAmpSwapElm : OpAmpElm
    {
        public OpAmpSwapElm(int xx, int yy) : base(xx, yy)
        {
            flags |= FLAG_SWAP;
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(OpAmpElm);
            }
        }
    }
}
