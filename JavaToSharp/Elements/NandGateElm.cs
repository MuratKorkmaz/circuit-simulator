	namespace JavaToSharp
	{
	    internal class NandGateElm : AndGateElm
	    {
	        public NandGateElm(int xx, int yy) : base(xx, yy)
	        {
	        }
	        public NandGateElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
	        {
	        }
	        internal override bool isInverting
	        {
	            get
	            {
	                return true;
	            }
	        }
	        internal override string GateName
	        {
	            get
	            {
	                return "элемент И-НЕ";
	            }
	        }
	        internal override int DumpType
	        {
	            get
	            {
	                return 151;
	            }
	        }
	    }
	}
