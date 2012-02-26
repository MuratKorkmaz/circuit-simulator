	namespace JavaToSharp
	{
	    internal class XorGateElm : OrGateElm
	    {
	        public XorGateElm(int xx, int yy) : base(xx, yy)
	        {
	        }
	        public XorGateElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
	        {
	        }
	        internal override string GateName
	        {
	            get
	            {
	                return "Исключающее ИЛИ";
	            }
	        }
	        internal override bool calcFunction()
	        {
	            int i;
	            bool f = false;
	            for (i = 0; i != inputCount; i++)
	                f ^= getInput(i);
	            return f;
	        }
	        internal override int DumpType
	        {
	            get
	            {
	                return 154;
	            }
	        }
	    }
	}
