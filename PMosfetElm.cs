namespace circuit_emulator
{
    class PMosfetElm:MosfetElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(MosfetElm);
            }
		
        }
        public PMosfetElm(int xx, int yy):base(xx, yy, true)
        {
        }
    }
}