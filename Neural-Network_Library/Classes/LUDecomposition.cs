using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class LUDecomposition
    {
        /// <summary>
        /// Array for internal storage of decomposition.
        /// </summary>
        private readonly double[][] LU;

        /// <summary>
        /// column dimension.
        /// </summary>
        private readonly int m;

        /// <summary>
        /// row dimension.
        /// </summary>
        private readonly int n;

        /// <summary>
        /// Internal storage of pivot vector.
        /// </summary>
        private readonly int[] piv;

        /// <summary>
        /// pivot sign.
        /// </summary>
        private readonly int pivsign;

        /// <summary>
        /// LU Decomposition
        /// </summary>
        /// <param name="A">Rectangular matrix</param>
        public LUDecomposition(Matrix A)
        {
            // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

            LU = A.GetArrayCopy();
            m = A.Rows;
            n = A.Cols;
            piv = new int[m];
            for (int i = 0; i < m; i++)
            {
                piv[i] = i;
            }
            pivsign = 1;
            double[] LUrowi;
            var LUcolj = new double[m];

            // Outer loop.

            for (int j = 0; j < n; j++)
            {
                // Make a copy of the j-th column to localize references.

                for (int i = 0; i < m; i++)
                {
                    LUcolj[i] = LU[i][j];
                }

                // Apply previous transformations.

                for (int i = 0; i < m; i++)
                {
                    LUrowi = LU[i];

                    // Most of the time is spent in the following dot product.

                    int kmax = Math.Min(i, j);
                    double s = 0.0;
                    for (int k = 0; k < kmax; k++)
                    {
                        s += LUrowi[k] * LUcolj[k];
                    }

                    LUrowi[j] = LUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.

                int p = j;
                for (int i = j + 1; i < m; i++)
                {
                    if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
                    {
                        p = i;
                    }
                }
                if (p != j)
                {
                    for (int k = 0; k < n; k++)
                    {
                        double t = LU[p][k];
                        LU[p][k] = LU[j][k];
                        LU[j][k] = t;
                    }
                    int temp = piv[p];
                    piv[p] = piv[j];
                    piv[j] = temp;
                    pivsign = -pivsign;
                }

                // Compute multipliers.

                if (j < m & LU[j][j] != 0.0)
                {
                    for (int i = j + 1; i < m; i++)
                    {
                        LU[i][j] /= LU[j][j];
                    }
                }
            }
        }



        public bool IsNonsingular
        {
            get
            {
                for (int j = 0; j < n; j++)
                {
                    if (LU[j][j] == 0)
                        return false;
                }
                return true;
            }
        }


        public Matrix L
        {
            get
            {
                var x = new Matrix(m, n);
                double[][] l = x.Data;
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i > j)
                        {
                            l[i][j] = LU[i][j];
                        }
                        else if (i == j)
                        {
                            l[i][j] = 1.0;
                        }
                        else
                        {
                            l[i][j] = 0.0;
                        }
                    }
                }
                return x;
            }
        }



        public Matrix U
        {
            get
            {
                var x = new Matrix(n, n);
                double[][] u = x.Data;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i <= j)
                        {
                            u[i][j] = LU[i][j];
                        }
                        else
                        {
                            u[i][j] = 0.0;
                        }
                    }
                }
                return x;
            }
        }


        public int[] Pivot
        {
            get
            {
                var p = new int[m];
                for (int i = 0; i < m; i++)
                {
                    p[i] = piv[i];
                }
                return p;
            }
        }


        public double[] DoublePivot
        {
            get
            {
                var vals = new double[m];
                for (int i = 0; i < m; i++)
                {
                    vals[i] = piv[i];
                }
                return vals;
            }
        }


        public double Det()
        {
            if (m != n)
            {

            }
            double d = pivsign;
            for (int j = 0; j < n; j++)
            {
                d *= LU[j][j];
            }
            return d;
        }


        /// <summary>
        /// Solve A*X = B
        /// </summary>
        /// <param name="B">A Matrix with as many rows as A and any number of columns.</param>
        /// <returns>so that L*U*X = B(piv,:)</returns>
        public Matrix Solve(Matrix B)
        {
            if (B.Rows != m)
            {
                throw new MatrixError(
                    "Matrix row dimensions must agree.");
            }
            if (!IsNonsingular)
            {
                throw new MatrixError("Matrix is singular.");
            }

            // Copy right hand side with pivoting
            int nx = B.Cols;
            Matrix Xmat = B.GetMatrix(piv, 0, nx - 1);
            double[][] X = Xmat.Data;

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < n; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j] * LU[i][k];
                    }
                }
            }
            // Solve U*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < nx; j++)
                {
                    X[k][j] /= LU[k][k];
                }
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j] * LU[i][k];
                    }
                }
            }
            return Xmat;
        }


        public double[] Solve(double[] value_ren)
        {
            if (value_ren == null)
            {
                throw new MatrixError("value");
            }

            if (value_ren.Length != LU.Length)
            {
                throw new MatrixError("Invalid matrix dimensions.");
            }

            if (!IsNonsingular)
            {
                throw new MatrixError("Matrix is singular");
            }

            // Copy right hand side with pivoting
            int count = value_ren.Length;
            var b = new double[count];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = value_ren[piv[i]];
            }

            int rows = LU[0].Length;
            int columns = LU[0].Length;
            double[][] lu = LU;


            // Solve L*Y = B
            var X = new double[count];
            for (int i = 0; i < rows; i++)
            {
                X[i] = b[i];
                for (int j = 0; j < i; j++)
                {
                    X[i] -= lu[i][j] * X[j];
                }
            }

            // Solve U*X = Y;
            for (int i = rows - 1; i >= 0; i--)
            {
                // double sum = 0.0;
                for (int j = columns - 1; j > i; j--)
                {
                    X[i] -= lu[i][j] * X[j];
                }
                X[i] /= lu[i][i];
            }
            return X;
        }



        public double[][] Inverse()
        {
            if (!IsNonsingular)
            {
                throw new MatrixError("Matrix is singular");
            }

            int rows = LU.Length;
            int columns = LU[0].Length;
            int count = rows;
            double[][] lu = LU;

            double[][] X = EngineArray.AllocateDouble2D(rows, columns);
            for (int i = 0; i < rows; i++)
            {
                int k = piv[i];
                X[i][k] = 1.0;
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < columns; k++)
            {
                for (int i = k + 1; i < columns; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        X[i][j] -= X[k][j] * lu[i][k];
                    }
                }
            }

            // Solve U*X = Y;
            for (int k = columns - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                {
                    X[k][j] /= lu[k][k];
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        X[i][j] -= X[k][j] * lu[i][k];
                    }
                }
            }

            return X;
        }
    }
}
