namespace circuit_emulator
{
    class NMosfetElm:MosfetElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(MosfetElm);
            }
		
        }
        public NMosfetElm(int xx, int yy):base(xx, yy, false)
        {
        }
    }
}