namespace circuit_emulator
{
    class NandGateElm:AndGateElm
    {
        override internal bool Inverting
        {
            get
            {
                return true;
            }
		
        }
        override internal System.String GateName
        {
            get
            {
                return "элемент И-НЕ";
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 151;
            }
		
        }
        public NandGateElm(int xx, int yy):base(xx, yy)
        {
        }
        public NandGateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
        }
    }
}