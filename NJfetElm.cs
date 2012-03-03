using System;

namespace circuit_emulator
{
    class NJfetElm:JfetElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(JfetElm);
            }
		
        }
        public NJfetElm(int xx, int yy):base(xx, yy, false)
        {
        }
    }

    class PJfetElm:JfetElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(JfetElm);
            }
		
        }
        public PJfetElm(int xx, int yy):base(xx, yy, true)
        {
        }
    }
}