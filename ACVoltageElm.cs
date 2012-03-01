namespace circuit_emulator
{
    class ACVoltageElm:VoltageElm
    {
        override internal System.Type DumpClass
        {
            get
            {
                return typeof(VoltageElm);
            }
		
        }
        public ACVoltageElm(int xx, int yy):base(xx, yy, WF_AC)
        {
        }
    }
}