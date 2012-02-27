using System.Drawing;

namespace JavaToSharp
{
    class RectangularArrays
    {
        internal static double[][] ReturnRectangularDoubleArray(int Size1, int Size2)
        {
            double[][] Array = new double[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new double[Size2];
            }
            return Array;
        }

        internal static Point[][] ReturnRectangularPointArray(int Size1, int Size2)
        {
            Point[][] Array = new Point[Size1][];
            for (int Array1 = 0; Array1 < Size1; Array1++)
            {
                Array[Array1] = new Point[Size2];
            }
            return Array;
        }
    }
}
