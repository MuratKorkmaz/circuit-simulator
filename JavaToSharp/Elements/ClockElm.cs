using System;

namespace JavaToSharp
{
    internal class ClockElm : RailElm
    {
        public ClockElm(int xx, int yy) : base(xx, yy, WF_SQUARE)
        {
            maxVoltage = 2.5;
            bias = 2.5;
            frequency = 100;
            flags |= FLAG_CLOCK;
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