using System;

namespace JavaToSharp
{
    internal class SquareRailElm : RailElm
    {
        public SquareRailElm(int xx, int yy) : base(xx, yy, WF_SQUARE)
        {
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(RailElm);
            }
        }
    }
}
