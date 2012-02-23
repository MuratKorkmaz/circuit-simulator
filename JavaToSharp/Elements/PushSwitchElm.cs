using System;

namespace JavaToSharp
{
    internal class PushSwitchElm : SwitchElm
    {
        public PushSwitchElm(int xx, int yy) : base(xx, yy, true)
        {
        }
        internal override Type DumpClass
        {
            get
            {
                return typeof(SwitchElm);
            }
        }
    }
}
