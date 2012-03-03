namespace circuit_emulator
{
    class OpAmpSwapElm:OpAmpElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(OpAmpElm);
            }
		
        }
        public OpAmpSwapElm(int xx, int yy):base(xx, yy)
        {
            flags |= FLAG_SWAP;
        }
    }
}