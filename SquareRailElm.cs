namespace circuit_emulator
{
    class SquareRailElm:RailElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(RailElm);
            }
		
        }
        public SquareRailElm(int xx, int yy):base(xx, yy, WF_SQUARE)
        {
        }
    }
}