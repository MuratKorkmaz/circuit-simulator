using System.Drawing;

namespace JavaToSharp
{
    internal class SevenSegElm : ChipElm
    {
        public SevenSegElm(int xx, int yy) : base(xx, yy)
        {
        }
        public SevenSegElm(int xa, int ya, int xb, int yb, int f, StringTokenizer st) : base(xa, ya, xb, yb, f, st)
        {
        }
        internal override string ChipName
        {
            get
            {
                return "7ми сегментный индикатор";
            }
        }

        private Color darkred;
        internal override void setupPins()
        {
            darkred = Color.FromArgb(30, 0, 0);
            sizeX = 4;
            sizeY = 4;
            pins = new Pin[7];
            pins[0] = new Pin(0, SIDE_W, "a");
            pins[1] = new Pin(1, SIDE_W, "b");
            pins[2] = new Pin(2, SIDE_W, "c");
            pins[3] = new Pin(3, SIDE_W, "d");
            pins[4] = new Pin(1, SIDE_S, "e");
            pins[5] = new Pin(2, SIDE_S, "f");
            pins[6] = new Pin(3, SIDE_S, "g");
        }
       

     
        internal virtual int PostCount
        {
            get
            {
                return 7;
            }
        }
        internal override int VoltageSourceCount
        {
            get
            {
                return 0;
            }
        }
        internal virtual int DumpType
        {
            get
            {
                return 157;
            }
        }
    }
}
