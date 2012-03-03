namespace circuit_emulator
{
    class ClockElm:RailElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(RailElm);
            }
		
        }
        public ClockElm(int xx, int yy):base(xx, yy, WF_SQUARE)
        {
            maxVoltage = 2.5;
            bias = 2.5;
            frequency = 100;
            flags |= FLAG_CLOCK;
        }
    }
}