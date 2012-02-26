using System;

namespace JavaToSharp
{
    internal class ACRailElm : RailElm
    {
        public ACRailElm(int xx, int yy) : base(xx, yy, WF_AC)
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
