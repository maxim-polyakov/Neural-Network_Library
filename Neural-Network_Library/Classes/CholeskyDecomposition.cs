using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class CholeskyDecomposition
    {

        private readonly bool isspd;


        private readonly double[][] l;


        private readonly int n;


        public CholeskyDecomposition(Matrix matrix)
        {
            // Initialize.
            double[][] a = matrix.Data;
            n = matrix.Rows;
            l = EngineArray.AllocateDouble2D(n, n);
            isspd = (matrix.Cols == n);
            // Main loop.
            for (int j = 0; j < n; j++)
            {
                double[] lrowj = l[j];
                double d = 0.0;
                for (int k = 0; k < j; k++)
                {
                    double[] lrowk = l[k];
                    double s = 0.0;
                    for (int i = 0; i < k; i++)
                    {
                        s += lrowk[i] * lrowj[i];
                    }
                    s = (a[j][k] - s) / l[k][k];
                    lrowj[k] = s;
                    d = d + s * s;
                    isspd = isspd & (a[k][j] == a[j][k]);
                }
                d = a[j][j] - d;
                isspd = isspd & (d > 0.0);
                l[j][j] = Math.Sqrt(Math.Max(d, 0.0));
                for (int k = j + 1; k < n; k++)
                {
                    l[j][k] = 0.0;
                }
            }
        }


        public bool IsSPD
        {
            get { return isspd; }
        }


        public Matrix L
        {
            get { return new Matrix(l); }
        }


        /// <returns>X so that L*L'*X = b.</returns>
        public Matrix Solve(Matrix b)
        {
            if (b.Rows != n)
            {
                throw new MatrixError(
                    "Matrix row dimensions must agree.");
            }
            if (!isspd)
            {
                throw new MatrixError(
                    "Matrix is not symmetric positive definite.");
            }

            // Copy right hand side.
            double[][] x = b.GetArrayCopy();
            int nx = b.Cols;

            // Solve L*Y = B;
            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < nx; j++)
                {
                    for (int i = 0; i < k; i++)
                    {
                        x[k][j] -= x[i][j] * l[k][i];
                    }
                    x[k][j] /= l[k][k];
                }
            }

            // Solve L'*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    for (int i = k + 1; i < n; i++)
                    {
                        x[k][j] -= x[i][j] * l[i][k];
                    }
                    x[k][j] /= l[k][k];
                }
            }

            return new Matrix(x);
        }

        public Matrix InverseCholesky()
        {
            double[][] li = LowerTriangularInverse(l);
            double[][] ic = EngineArray.AllocateDouble2D(n, n);

            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    for (int i = 0; i < n; i++)
                        ic[r][c] += li[i][r] * li[i][c];

            return new Matrix(ic);
        }


        private double[][] LowerTriangularInverse(double[][] m)
        {

            double[][] lti = EngineArray.AllocateDouble2D(m.Length, m.Length);

            for (int j = 0; j < m.Length; j++)
            {
                if (m[j][j] == 0)
                    throw new SyntError("Error, the matrix is not full rank");

                lti[j][j] = 1.0 / m[j][j];

                for (int i = j + 1; i < m.Length; i++)
                {
                    double sum = 0.0;

                    for (int k = j; k < i; k++)
                        sum -= m[i][k] * lti[k][j];

                    lti[i][j] = sum / m[i][i];
                }
            }

            return lti;

        }

        public double GetDeterminant()
        {
            double result = 1;

            for (int i = 0; i < n; i++)
                result *= l[i][i];

            return result * result;
        }
    }
}
