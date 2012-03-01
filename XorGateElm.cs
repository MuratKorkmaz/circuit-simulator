namespace circuit_emulator
{
    class XorGateElm:OrGateElm
    {
        override internal System.String GateName
        {
            get
            {
                return "Исключающее ИЛИ";
            }
		
        }
        override internal int DumpType
        {
            get
            {
                return 154;
            }
		
        }
        public XorGateElm(int xx, int yy):base(xx, yy)
        {
        }
        public XorGateElm(int xa, int ya, int xb, int yb, int f, SupportClass.Tokenizer st):base(xa, ya, xb, yb, f, st)
        {
        }
        internal override bool calcFunction()
        {
            int i;
            bool f = false;
            for (i = 0; i != inputCount; i++)
                f ^= getInput(i);
            return f;
        }
    }
}