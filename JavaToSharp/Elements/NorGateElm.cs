	namespace JavaToSharp
	{
	    internal class NorGateElm : OrGateElm
	    {
	        public NorGateElm(int xx, int yy) : base(xx, yy)
	        {
	        }
	        public NorGateElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
	        {
	        }
	        internal override string GateName
	        {
	            get
	            {
	                return "элемент ИЛИ-НЕ";
	            }
	        }
	        internal override bool isInverting
	        {
	            get
	            {
	                return true;
	            }
	        }
	        internal override int DumpType
	        {
	            get
	            {
	                return 153;
	            }
	        }
	    }
	}
