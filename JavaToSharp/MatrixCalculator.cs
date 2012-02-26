using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaToSharp
{
    class MatrixCalculator
    {
        // factors a matrix into upper and lower triangular matrices by
        // gaussian elimination.  On entry, a[0..n-1][0..n-1] is the
        // matrix to be factored.  ipvt[] returns an integer vector of pivot
        // indices, used in the lu_solve() routine.
        internal bool lu_factor(double[][] a, int n, int[] ipvt)
        {
            var scaleFactors = new double[n];

            // divide each row by its largest element, keeping track of the
            // scaling factors
            for (int i = 0; i != n; i++)
            {
                double largest = 0;
                for (int j = 0; j != n; j++)
                {
                    double x = Math.Abs(a[i][j]);
                    if (x > largest)
                        largest = x;
                }
                // if all zeros, it's a singular matrix
                if (Math.Abs(largest - 0) < double.Epsilon)
                    return false;
                scaleFactors[i] = 1.0 / largest;
            }

            // use Crout's method; loop through the columns
            for (int j = 0; j != n; j++)
            {
                // calculate upper triangular elements for this column
                int k;
                for (int i = 0; i != j; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != i; k++)
                        q -= a[i][k] * a[k][j];
                    a[i][j] = q;
                }

                // calculate lower triangular elements for this column
                double largest = 0;
                int largestRow = -1;
                for (int i = j; i != n; i++)
                {
                    double q = a[i][j];
                    for (k = 0; k != j; k++)
                        q -= a[i][k] * a[k][j];
                    a[i][j] = q;
                    double x = Math.Abs(q);
                    if (x >= largest)
                    {
                        largest = x;
                        largestRow = i;
                    }
                }

                // pivoting
                if (j != largestRow)
                {
                    for (k = 0; k != n; k++)
                    {
                        double x = a[largestRow][k];
                        a[largestRow][k] = a[j][k];
                        a[j][k] = x;
                    }
                    scaleFactors[largestRow] = scaleFactors[j];
                }

                // keep track of row interchanges
                ipvt[j] = largestRow;

                // avoid zeros
                if (Math.Abs(a[j][j] - 0.0) < double.Epsilon)
                {
                    Console.WriteLine("avoided zero");
                    a[j][j] = 1e-18;
                }

                if (j != n - 1)
                {
                    double mult = 1.0 / a[j][j];
                    for (int i = j + 1; i != n; i++)
                        a[i][j] *= mult;
                }
            }
            return true;
        }
        
        // Solves the set of n linear equations using a LU factorization
        // previously performed by lu_factor.  On input, b[0..n-1] is the right
        // hand side of the equations, and on output, contains the solution.
        internal void lu_solve(double[][] a, int n, int[] ipvt, double[] b)
        {
            int bi = 0;
            // find first nonzero b element
            for (int i = 0; i != n; i++)
            {
                int row = ipvt[i];
                double swap = b[row];
                b[row] = b[i];
                b[i] = swap;
                if (Math.Abs(swap - 0) > double.Epsilon)
                {
                    bi = i + 1;
                    break;
                }
            }

            for (int i = bi; i < n; i++)
            {
                int row = ipvt[i];
                double tot = b[row];
                b[row] = b[i];
                // forward substitution using the lower triangular matrix
                for (int j = bi; j < i; j++)
                    tot -= a[i][j] * b[j];
                b[i] = tot;
            }
            for (int i = n - 1; i >= 0; i--)
            {
                double tot = b[i];
                // back-substitution using the upper triangular matrix
                for (int j = i + 1; j != n; j++)
                    tot -= a[i][j] * b[j];
                b[i] = tot / a[i][i];
            }
        }
    }
}
